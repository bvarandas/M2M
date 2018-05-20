using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M2MTest.Entities
{
    public class ExtratoMensal : Compra
    {
        public decimal ValorJuros { get; set; }
        
        public decimal ValorTotal { get; set; }
        
        public decimal ValorParcela { get; set; }

        public ExtratoMensal() { }
    }
}
