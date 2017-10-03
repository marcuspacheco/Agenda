using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Configuration.ConfigurationManager;

namespace Agenda
{
    public class AcessoDb
    {
        private readonly string _stringConexao;

        public AcessoDb()
        {
            _stringConexao = ConnectionStrings["Agenda"].ConnectionString;
        }

        public DataTable Buscar(string instrucaoSql, List<KeyValuePair<string, object>> parametros)
        {

            var conexaoSql = new SqlConnection(_stringConexao);
            var comandoSql = new SqlCommand(instrucaoSql, conexaoSql);
            var tabela = new DataTable();

            foreach (KeyValuePair<string, object> t in parametros)
            {
                comandoSql.Parameters.AddWithValue(t.Key, t.Value);
            }

            try
            {
                conexaoSql.Open();
                using (SqlDataReader dr = comandoSql.ExecuteReader())
                {
                    tabela.Load(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            comandoSql.Dispose();
            conexaoSql.Close();
            conexaoSql.Dispose();
            return tabela;
        }

        public bool Inserir(string instrucaoSql, List<KeyValuePair<string, object>> parametros, ref int codigo)
        {
            instrucaoSql += "; SELECT @ID = SCOPE_IDENTITY();";

            var conexaoSql = new SqlConnection(_stringConexao);
            var comandoSql = new SqlCommand(instrucaoSql, conexaoSql);

            foreach (KeyValuePair<string, object> t in parametros)
            {
                comandoSql.Parameters.AddWithValue(t.Key, t.Value ?? DBNull.Value);
            }

            var outputIdParam = new SqlParameter("@ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            comandoSql.Parameters.Add(outputIdParam);

            try
            {
                conexaoSql.Open();

                if (comandoSql.ExecuteNonQuery() > 0)
                {
                    codigo = outputIdParam.Value as int? ?? default(int);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comandoSql.Dispose();
                conexaoSql.Close();
                conexaoSql.Dispose();
                return false;
            }

            comandoSql.Dispose();
            conexaoSql.Close();
            conexaoSql.Dispose();

            return true;
        }

        public bool AtualizarApagar(string instrucaoSql, List<KeyValuePair<string, object>> parametros)
        {
            var conexaoSql = new SqlConnection(_stringConexao);
            var comandoSql = new SqlCommand(instrucaoSql, conexaoSql);

            foreach (KeyValuePair<string, object> t in parametros)
            {
                comandoSql.Parameters.AddWithValue(t.Key, t.Value ?? DBNull.Value);
            }

            try
            {
                conexaoSql.Open();
                comandoSql.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comandoSql.Dispose();
                conexaoSql.Close();
                conexaoSql.Dispose();
                return false;
            }

            comandoSql.Dispose();
            conexaoSql.Close();
            conexaoSql.Dispose();

            return true;
        }

    }
}