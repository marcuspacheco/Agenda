using System;

namespace Agenda.Modelo
{
    public class Contato
    {
        public Contato(string nome, string empresa, string cargo, string email, DateTime dataNascimento, string website, string residencial, string celular, int parentescoId)
        {
            Nome = nome;
            Empresa = empresa;
            Cargo = cargo;
            Email = email;
            DataNascimento = dataNascimento;
            Website = website;
            Residencial = residencial;
            Celular = celular;
            ParentescoId = parentescoId;
        }

        public Contato(int contatoId, string nome, string empresa, string cargo, string email, DateTime? dataNascimento, string website, string residencial, string celular, int parentescoId)
        {
            ContatoId = contatoId;
            Nome = nome;
            Empresa = empresa;
            Cargo = cargo;
            Email = email;
            DataNascimento = dataNascimento;
            Website = website;
            Residencial = residencial;
            Celular = celular;
            ParentescoId = parentescoId;
        }

        public int ContatoId { get; set; }
        public string Nome { get; set; }
        public string Empresa { get; set; }
        public string Cargo { get; set; }
        public string Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Website { get; set; }
        public string Residencial { get; set; }
        public string Celular { get; set; }
        public int ParentescoId { get; set; }
    }
}