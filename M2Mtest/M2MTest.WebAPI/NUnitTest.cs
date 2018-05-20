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
    }
}