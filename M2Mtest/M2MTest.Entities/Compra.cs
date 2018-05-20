using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2MTest.Entities
{
    public class Compra
    {
        public ObjectId _id { get; set; }

        public int IdCompra { get; set; }

        public string Descricao { get; set; }

        public string Estabelecimento { get; set; }

        public DateTime DataCompra { get; set; }

        public string FormaPagamento { get; set; }

        public decimal ValorPagto { get; set; }
        
        public Compra()
        {
            
        }
    }
}
