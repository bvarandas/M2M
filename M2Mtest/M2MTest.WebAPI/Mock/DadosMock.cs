using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using M2MTest.Entities;

namespace M2MTest.WebAPI.Mock
{
    public class DadosMock: IDisposable
    {
        public List<Compra> CarregarDadosMock()
        {
            var lRetorno = new List<Compra>();

            try
            {
                lRetorno.Add(new Compra() { Descricao = "Par de meias adidas",  DataCompra = new DateTime(2018, 1, 26), Estabelecimento = "Deas artigos esportivos", IdCompra = 1, FormaPagamento = "1", ValorPagto = 15.60M });
                lRetorno.Add(new Compra() { Descricao = "Churrasqueira de aco", DataCompra = new DateTime(2018, 2, 18), Estabelecimento = "Boa vista supermercado", IdCompra = 2, FormaPagamento = "2", ValorPagto = 500.85M });
                lRetorno.Add(new Compra() { Descricao = "Tenys pe Garuel",      DataCompra = new DateTime(2018, 3, 9), Estabelecimento = "Droga Arraia", IdCompra = 3, FormaPagamento = "1", ValorPagto = 90.99M });
                lRetorno.Add(new Compra() { Descricao = "Carne de Porco",       DataCompra = new DateTime(2018, 4, 19), Estabelecimento = "Boa vista supermercado", IdCompra = 4, FormaPagamento = "2", ValorPagto = 29.99M });
                lRetorno.Add(new Compra() { Descricao = "Pacote de fralda",     DataCompra = new DateTime(2018, 5, 15), Estabelecimento = "Droga Arraia", IdCompra = 5, FormaPagamento = "1", ValorPagto = 50.99M });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lRetorno;
        }

        public void Dispose()
        {
            
        }
    }
}