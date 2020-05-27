using System;
using System.Collections.Generic;
using System.Text;

namespace DevIO.Business.Models
{
    public class Cliente : Entity
    {
        public string Nome { get; set; }
        public string Documento { get; set; } //cpf
        public DateTime DtNascimento { get; set; }      
        public Endereco Endereco { get; set; }
        public bool Ativo { get; set; }
 
    }
}
