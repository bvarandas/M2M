using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M2MTest.DB;
using M2MTest.Entities;
using log4net;
using Newtonsoft.Json;
using System.Configuration;
using System.Globalization;
using M2MTest.Bus.Mock;

namespace M2MTest.Bus
{
    /// <summary>
    /// Classe de controle para a 
    /// </summary>
    public class CompraBus : IDisposable
    {
        /// <summary>
        /// PAra Conversão de moeda
        /// </summary>
        private CultureInfo _culture = new CultureInfo("pt-br");

        /// <summary>
        /// Atributo de acesso a camada de base de dados
        /// </summary>
        public DBAcesso _dbAcesso;

        /// <summary>
        /// Atributo responsável pela log da classe
        /// </summary>
        public static readonly log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Busca a lista de compras no banco de dados
        /// </summary>
        /// <returns>Retorna uma lista de compras</returns>
        public List<Compra> ListarCompras()
        {
            var lRetorno = new List<Compra>();

            try
            {
                using (_dbAcesso = new DBAcesso())
                {
                    lRetorno = _dbAcesso.ListarCompras();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return lRetorno;
        }

        /// <summary>
        /// Lista o extrato com o total a ser pago por mês...
        /// </summary>
        /// <param name="pData">Data para filtro de mês e Ano</param>
        /// <returns>Retorna uma lista do objeto Extrato</returns>
        public List<ExtratoMensal> ListaExtrato(DateTime pData)
        {
            var lRetorno = new List<ExtratoMensal>();

            try
            {
                using (_dbAcesso = new DBAcesso())
                {
                    var lLista = _dbAcesso.ListaExtrato(pData);

                    lLista.ForEach(item => 
                    {
                        lRetorno.Add(this.CalculoJurosParcelado(item));
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return lRetorno;
        }

        /// <summary>
        /// Deleta registro de compra do mongodb
        /// </summary>
        /// <param name="pListaCompra">Lista do registro de compras a ser deletada do banco</param>
        public void CancelarCompra(List<Compra> pListaCompra)
        {
            try
            {
                _logger.Info("Inicio do  cancelamento de items");

                using (_dbAcesso = new DBAcesso())
                {
                    _dbAcesso.CancelarCompra(pListaCompra);
                }

                _logger.InfoFormat("Cancelamento efetuado com sucesso dos objetos: {0}",Newtonsoft.Json.JsonConvert.SerializeObject(pListaCompra));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Insere a lista de compras no banco de dados mongodb
        /// </summary>
        /// <param name="pListaCompra">Lista de compras a ser inserida</param>
        /// <returns>Retorna a lista de compras inserida no banco de dados mongodb</returns>
        public List<Compra> InserirCompra(List<Compra> pListaCompra)
        {
            var lRetorno = new List<Compra>();

            try
            {
                using (_dbAcesso = new DBAcesso())
                {
                    lRetorno = _dbAcesso.InserirCompra(pListaCompra);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return lRetorno;
        }


        /// <summary>
        /// Limpa a collection de compra para poder inserir novamente.
        /// </summary>
        public void LimparTabela()
        {
            try
            {
                using (_dbAcesso = new DBAcesso())
                {
                    _dbAcesso.LimparTabela();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Calcula o juros da compra parcelada
        /// </summary>
        /// <param name="pCompra">Objeto com os dados da compra parcelada</param>
        /// <returns>Retorna o valor do objeto de extratoMensal com o Juros calculado</returns>
        public ExtratoMensal CalculoJurosParcelado(Compra pCompra)
        {
            var lRetorno = new ExtratoMensal();

            try
            {
                lRetorno.FormaPagamento = pCompra.FormaPagamento;

                lRetorno.ValorPagto = pCompra.ValorPagto;

                if (pCompra.FormaPagamento != "1")
                {
                    decimal lJuros = Convert.ToDecimal(ConfigurationManager.AppSettings["ValorJuros"], _culture);

                    lRetorno.ValorJuros = (pCompra.ValorPagto * lJuros);

                    lRetorno.ValorTotal = pCompra.ValorPagto + lRetorno.ValorJuros;

                    lRetorno.ValorParcela = (lRetorno.ValorTotal / 3);
                }
                else
                {
                    lRetorno.ValorJuros = 0;

                    lRetorno.ValorTotal = pCompra.ValorPagto;

                    lRetorno.ValorParcela = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return lRetorno;
        }


        /// <summary>
        /// Carrega uma listagem de objetos de compra fake para carregar o banco de dados mongo db
        /// </summary>
        /// <returns>Retorna uma lista de objeto de compra fake</returns>
        public List<Compra> CarregarDadosMock()
        {
            var lRetorno = new List<Compra>();

            try
            {
                using (var lMock = new DadosMockBus())
                {
                    lRetorno = lMock.CarregarDadosMock();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return lRetorno;
        }

        public void Dispose()
        {

        }
    }
}
