using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M2MTest.Entities;
using log4net;
using System.Web;
using System.Globalization;
using M2MTest.Bus;

namespace M2MTest.WebAPI.Controllers
{
    public class ComprasController : ApiController
    {
        /// <summary>
        /// Atributo responsável pela log da classe
        /// </summary>
        public static readonly log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Para conversão de moeda
        /// </summary>
        private CultureInfo _culture = new CultureInfo("pt-BR");

        /// <summary>
        /// Classe de negócio de compras
        /// </summary>
        public CompraBus _compraBus;

        /// <summary>
        /// Inserção no mongo DB
        /// </summary>
        /// <returns>Retorna o item inserido</returns>
        [HttpPost]
        [ActionName("InserirNovaCompra")]
        public HttpResponseMessage InserirNovaCompraServer()
        {
            try
            {
                var lRequest = HttpContext.Current.Request;

                using (_compraBus = new CompraBus())
                {

                    Compra lCompra = new Compra();

                    lCompra.Estabelecimento = lRequest.Unvalidated["Estabelecimento"];
                    lCompra.Descricao       = lRequest.Unvalidated["Descricao"];
                    lCompra.FormaPagamento  = lRequest.Unvalidated["FormaPagamento"];
                    lCompra.DataCompra      = Convert.ToDateTime(lRequest.Unvalidated["DataCompra"]);
                    lCompra.ValorPagto      = Convert.ToDecimal(lRequest.Unvalidated["ValorPagto"], _culture);
                    
                    var listTemp = _compraBus.ListarCompras();

                    lCompra.IdCompra = listTemp.Count + 1;

                    var listaCompra = new List<Compra>();

                    listaCompra.Add(lCompra);

                    _compraBus.InserirCompra(listaCompra);

                    return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao inserir uma Nova Compra: " + ex.Message);
            }
        }

        /// <summary>
        /// Lista de compras 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ListarCompras")]
        public HttpResponseMessage ListarCompras()
        {
            var lRetorno = new HttpResponseMessage();

            try
            {
                var lRequest = HttpContext.Current.Request;

                using (_compraBus = new CompraBus())
                {
                    var listaCompra = new List<Compra>();

                    listaCompra = _compraBus.ListarCompras();

                    lRetorno =  Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao Listar compras: " + ex.Message);
            }

            return lRetorno;
        }


        [HttpPost]
        [ActionName("ListarExtratoCompras")]
        public HttpResponseMessage ListarExtratoCompras()
        {
            try
            {
                var lRequest = HttpContext.Current.Request;

                using (_compraBus = new CompraBus())
                {
                    DateTime lDate = Convert.ToDateTime(lRequest.Unvalidated["DataCompra"]);

                    var listaExtrato = new List<ExtratoMensal>();
                    
                    listaExtrato = _compraBus.ListaExtrato(lDate);

                    return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaExtrato));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao Listar extrato compras: " + ex.Message);
            }
        }

        /// <summary>
        /// Manda a chamada para o banco de dados para cancelar uma compra específica
        /// </summary>
        /// <returns>Retorna uma lista de objeto do tipo compra</returns>
        [HttpPost]
        [ActionName("CancelarCompra")]
        public HttpResponseMessage CancelarCompra()
        {
            try
            {
                var lRequest = HttpContext.Current.Request;

                using (_compraBus = new CompraBus())
                {

                    int IdCompra = Convert.ToInt32(lRequest.Unvalidated["idCompra"]);

                    var listaCompra = new List<Compra>();

                    listaCompra.Add(new Compra() { IdCompra = IdCompra });

                    _compraBus.CancelarCompra(listaCompra);

                    return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao cancelar compra: " + ex.Message);
            }
        }

        /// <summary>
        /// Método para carregar dados falsos mock, só para carregar dados no banco e gerar a collection necessária
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CarregaDadosMock")]
        public HttpResponseMessage CarregaDadosMock()
        {
            try
            {
                using (Mock.DadosMock lMock = new Mock.DadosMock())
                {
                    var listaCompra = lMock.CarregarDadosMock();

                    using (_compraBus = new CompraBus())
                    {
                        _compraBus.LimparTabela();

                        _compraBus.InserirCompra(listaCompra);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao carregar dados mock: " + ex.Message);
            }
        }

    }
}
