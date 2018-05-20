using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M2MTest.DB;
using M2MTest.Entities;
using log4net;
using System.Web;
using System.Globalization;

namespace M2MTest.WebAPI.Controllers
{
    public class ComprasController : ApiController
    {
        /// <summary>
        /// Atributo responsável pela log da classe
        /// </summary>
        public static readonly log4net.ILog _Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// PAra conversão de moeda
        /// </summary>
        private CultureInfo lCulture = new CultureInfo("pt-BR");

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
                
                Compra lCompra = new Compra();
                
                lCompra.Estabelecimento     = lRequest.Unvalidated["Estabelecimento"];
                lCompra.Descricao           = lRequest.Unvalidated["Descricao"];
                lCompra.FormaPagamento      = lRequest.Unvalidated["FormaPagamento"];
                lCompra.DataCompra          = Convert.ToDateTime( lRequest.Unvalidated["DataCompra"]);
                lCompra.ValorPagto          = Convert.ToDecimal(lRequest.Unvalidated["ValorPagto"], lCulture);
                
                DB.DBAcesso ldb = new DB.DBAcesso();

                var listTemp = ldb.ListarCompras();

                lCompra.IdCompra = listTemp.Count+1;

                var listaCompra = new List<Compra>();

                listaCompra.Add(lCompra);

                

                ldb.InserirCompra(listaCompra);

                return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);

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
            try
            {
                var lRequest = HttpContext.Current.Request;

                DB.DBAcesso ldb = new DB.DBAcesso();
                
                var listaCompra = new List<Compra>();

                listaCompra = ldb.ListarCompras();

                return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao Lista compras: " + ex.Message);
            }
        }


        [HttpPost]
        [ActionName("ListarExtratoCompras")]
        public HttpResponseMessage ListarExtratoCompras()
        {
            try
            {
                var lRequest = HttpContext.Current.Request;

                DateTime lDate = Convert.ToDateTime(lRequest.Unvalidated["DataCompra"]);

                var listaExtrato = new List<ExtratoMensal>();

                DB.DBAcesso ldb = new DB.DBAcesso();

                listaExtrato = ldb.ListaExtrato(lDate);

                return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaExtrato));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao Listar extrato compras: " + ex.Message);
            }
        }

        /// <summary>
        /// Manda a chamada para o banco de dados para cancelar uma compra específica
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CancelarCompra")]
        public HttpResponseMessage CancelarCompra()
        {
            try
            {
                var lRequest = HttpContext.Current.Request;

                int IdCompra = Convert.ToInt32( lRequest.Unvalidated["idCompra"]);

                var listaCompra = new List<Compra>();

                listaCompra.Add(new Compra() { IdCompra = IdCompra });

                DB.DBAcesso ldb = new DB.DBAcesso();

                ldb.CancelarCompra(listaCompra);

                return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao cancelar compra: " + ex.Message);
            }
        }

        [HttpPost]
        [ActionName("CarregaDadosMock")]
        public HttpResponseMessage CarregaDadosMock()
        {
            try
            {
                //var httpRequest = HttpContext.Current.Request;
                Mock.DadosMock lMock = new Mock.DadosMock();

                var listaCompra = lMock.CarregarDadosMock();

                DB.DBAcesso ldb = new DB.DBAcesso();

                ldb.LimparTabela();

                ldb.InserirCompra(listaCompra);

                return Request.CreateResponse(HttpStatusCode.OK, Newtonsoft.Json.JsonConvert.SerializeObject(listaCompra));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Erro ao cancelar compra: " + ex.Message);
            }
        }

    }
}
