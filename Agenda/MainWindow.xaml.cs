using Agenda.Modelo;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MessageBox = System.Windows.MessageBox;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var acessoBd = new AcessoDb();
            var dtTipoAlocacao = acessoBd.Buscar("SELECT * FROM Parentesco ORDER BY Nome",
                new List<KeyValuePair<string, object>>());
            cboParentesco.ItemsSource = dtTipoAlocacao.DefaultView;
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            if (CamposObrigatorios())
            {
                Contato contato;
                if (String.IsNullOrEmpty(txtCodigo.Text))
                {
                    // Inserir
                    contato = new Contato(txtNome.Text, txtEmpresa.Text, txtCargo.Text, txtEmail.Text,
                       dtpData.DisplayDate, txtSite.Text, (int)cboParentesco.SelectedValue);
                }
                else
                {
                    // Alterar
                    contato = new Contato(int.Parse(txtCodigo.Text), txtNome.Text, txtEmpresa.Text, txtCargo.Text, txtEmail.Text,
                       dtpData.SelectedDate.Value.Date, txtSite.Text, int.Parse(cboParentesco.SelectedValue.ToString()));
                }

                Salvar(contato);
            }
        }

        public static string FormatarDataSQL(string dataOriginal)
        {
            return Convert.ToDateTime(dataOriginal).ToString("yyyy-MM-dd");
        }

        private void Salvar(Contato contato)
        {
            var acessoBd = new AcessoDb();
            var parametros = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("@Nome", contato.Nome),
                new KeyValuePair<string, object>("@Empresa", contato.Empresa),
                new KeyValuePair<string, object>("@Cargo", contato.Cargo),
                new KeyValuePair<string, object>("@Email", contato.Email),
                new KeyValuePair<string, object>("@Website", contato.Website),
                new KeyValuePair<string, object>("@ParentescoID", contato.ParentescoId),
                new KeyValuePair<string, object>("@DataNascimento", FormatarDataSQL(contato.DataNascimento.ToString()))
            };
            if (contato.ContatoId == 0)
            {
                var comandoSql =
                    "INSERT INTO Contato (Nome, Empresa, Cargo, Email, DataNascimento, Website, ParentescoID) VALUES (@Nome, @Empresa, @Cargo, @Email, @DataNascimento, @Website, @ParentescoID)";
                var codigo = 0;
                if (acessoBd.Inserir(comandoSql, parametros, ref codigo))
                {
                    contato.ContatoId = codigo;
                    txtCodigo.Text = codigo.ToString();
                    MessageBox.Show("Contato inserido com sucesso!", "Informação", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            else
            {
                var comandoSql =
                    "UPDATE Contato SET Nome = @Nome, Empresa = @Empresa, Cargo = @Cargo, Email = @Email, DataNascimento = @DataNascimento, Website = @Website, ParentescoID = @ParentescoID WHERE ContatoID = @ContatoID";
                parametros.Add(new KeyValuePair<string, object>("@ContatoID", contato.ContatoId));
                if (acessoBd.AtualizarApagar(comandoSql, parametros))
                {
                    MessageBox.Show("Contato atualizado!", "Informação", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            LimparDados();
        }

        private void LimparDados()
        {
            txtCodigo.Text = string.Empty;
            txtNome.Text = string.Empty;
            txtEmpresa.Text = string.Empty;
            txtCargo.Text = string.Empty;
            txtEmail.Text = string.Empty;
            dtpData.SelectedDate = DateTime.Now.Date;
            txtSite.Text = string.Empty;
            cboParentesco.SelectedIndex = -1;
        }

        private bool CamposObrigatorios()
        {
            if (String.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Preencha o campo nome, por favor.", "Erro", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && tbLista.IsSelected)
            {
                var acessoBd = new AcessoDb();
                var dtContato = acessoBd.Buscar(
                    "SELECT ContatoID, A.Nome, Empresa, Cargo, Email, CONVERT(NVARCHAR(50), A.DataNascimento, 103) AS DataNascimento, Website, A.ParentescoID, B.Nome AS Parentesco FROM Contato A LEFT JOIN Parentesco B ON A.ParentescoID = B.ParentescoID ORDER BY Nome",
                    new List<KeyValuePair<string, object>>());

                DataGrid.ItemsSource = dtContato.DefaultView;
                Dispatcher.BeginInvoke((Action)(() => DataGrid.Columns[7].Visibility = Visibility.Hidden));
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedCells.Count > 0)
            {
                DataGridCellInfo cellInfo = DataGrid.SelectedCells[0];
                DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
                FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
                BindingOperations.SetBinding(element, TagProperty, column.Binding);
                CarregaDetalhesContato(element.Tag.ToString());
            }
        }

        private void CarregaDetalhesContato(string codigo)
        {
            var acessoBd = new AcessoDb();
            var dtContato = acessoBd.Buscar(
                $"SELECT ContatoID, A.Nome, Empresa, Cargo, Email, CONVERT(NVARCHAR(50), A.DataNascimento, 103) AS DataNascimento, Website, A.ParentescoID, B.Nome AS Parentesco FROM Contato A LEFT JOIN Parentesco B ON A.ParentescoID = B.ParentescoID WHERE ContatoID = {codigo} ORDER BY Nome",
                new List<KeyValuePair<string, object>>());

            if (dtContato.DefaultView.Count > 0)
            {
                txtCodigo.Text = dtContato.DefaultView[0]["contatoId"].ToString();
                txtNome.Text = dtContato.DefaultView[0]["Nome"].ToString();
                txtEmpresa.Text = dtContato.DefaultView[0]["Empresa"].ToString();
                txtCargo.Text = dtContato.DefaultView[0]["Cargo"].ToString();
                txtEmail.Text = dtContato.DefaultView[0]["Email"].ToString();
                dtpData.Text = dtContato.DefaultView[0]["DataNascimento"].ToString();
                txtSite.Text = dtContato.DefaultView[0]["Website"].ToString();
                cboParentesco.SelectedValue = dtContato.DefaultView[0]["ParentescoId"].ToString();
                Dispatcher.BeginInvoke((Action)(() => tabControl1.SelectedIndex = 1));
            }
        }
    }
}
