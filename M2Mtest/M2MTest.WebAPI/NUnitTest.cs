using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using M2MTest.Bus;

namespace M2MTest.WebAPI
{
    [TestFixture]
    public class NUnitTest
    {
        [TestCase]
        public void CarregarMock()
        {
            CompraBus lBus = new CompraBus();

            int lCount = lBus.CarregarDadosMock().Count;

            Assert.AreEqual(5, lCount);
        }

        [TestCase]
        public void ListarCompras()
        {
            CompraBus lBus = new CompraBus();

            lBus.CarregarDadosMock();

            int lCount = lBus.ListarCompras().Count;

            Assert.AreEqual(5, lCount);
        }

        [TestCase]
        public void ListaComprasAnaliseDetalhe()
        {
            CompraBus lBus = new CompraBus();

            var lista = lBus.ListarCompras();

            lista.ForEach(item => 
            {
                Assert.IsNotNull(item, "teste" );
            });
        }

        [TestCase]
        public void CancelarCompra()
        {
            CompraBus lBus = new CompraBus();

            var lLista = lBus.ListarCompras();

            var lCount = lLista.Count;

            lBus.CancelarCompra(lLista);

            var lListaAfter = lBus.ListarCompras();

            var lCountAfter = lListaAfter;

            Assert.AreNotEqual(lCount, lCountAfter);

            lBus.CarregarDadosMock();
        }

        [TestCase]
        public void ListarExtrato()
        {
            CompraBus lBus = new CompraBus();

            lBus.CarregarDadosMock();

            var lData1 = new DateTime(2018, 1, 1);
            var lData2 = new DateTime(2018, 2, 1);
            var lData3 = new DateTime(2018, 3, 1);
            var lData4 = new DateTime(2018, 4, 1);
            var lData5 = new DateTime(2018, 5, 1);

            /// há um registro de extrato no mÊs de Janeiro
            var lLista1 = lBus.ListaExtrato(lData1);

            Assert.AreEqual(1, lLista1.Count);

            /// há um registro de extrato no mÊs de Fevereiro
            var lLista2 = lBus.ListaExtrato(lData2);

            Assert.AreEqual(1, lLista2.Count);

            /// há um registro de extrato no mÊs de Março
            var lLista3 = lBus.ListaExtrato(lData3);

            Assert.AreEqual(1, lLista3.Count);

            /// há um registro de extrato no mÊs de Abril
            var lLista4 = lBus.ListaExtrato(lData4);

            Assert.AreEqual(1, lLista4.Count);

            /// há um registro de extrato no mÊs de Maio
            var lLista5 = lBus.ListaExtrato(lData5);

            Assert.AreEqual(1, lLista5.Count);
        }


    }
}