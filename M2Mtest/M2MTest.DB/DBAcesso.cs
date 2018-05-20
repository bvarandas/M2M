using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M2MTest.Entities;
using log4net;
using MongoDB.Driver;
using System.Configuration;
using System.Globalization;

namespace M2MTest.DB
{
    /// <summary>
    /// Classe de acesso ao banco de dados 
    /// </summary>
    public class DBAcesso: IDisposable
    {
        private CultureInfo lCulture = new CultureInfo("pt-br");

        /// <summary>
        /// Atributo de conexão com o Mongodb
        /// </summary>
        private string _Conexao;

        /// <summary>
        /// Atributo do nome do banco de dados
        /// </summary>
        private string _NomeBanco;

        /// <summary>
        /// Atributo responsável pela log da classe
        /// </summary>
        public static readonly log4net.ILog _Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DBAcesso()
        {
            _Conexao = ConfigurationManager.AppSettings["conexao"];
            _NomeBanco = ConfigurationManager.AppSettings["nomeBanco"];
        }

        /// <summary>
        /// Busca a lista de compras no banco de dados
        /// </summary>
        /// <returns>Retorna uma lista de compras</returns>
        public List<Compra> ListarCompras()
        {
            var lRetorno = new List<Compra>();

            try
            {
                var lConexao = new MongoClient(_Conexao);
                
                var lDataBase = lConexao.GetDatabase(_NomeBanco);

                IMongoCollection<Compra> lCollection = lDataBase.GetCollection<Compra>("Compras");

                var lFiltro = Builders<Compra>.Filter.Empty;

                var lLista = lCollection.Find<Compra>(lFiltro).ToList();

                lRetorno.AddRange(lLista);

                //lRetorno = lCollection.s

            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);
            }

            return lRetorno;
        }

        /// <summary>
        /// Lista o extrato com o total a ser pago por mês...
        /// </summary>
        /// <param name="pData"></param>
        /// <returns>Retorna </returns>
        public List<ExtratoMensal> ListaExtrato(DateTime pData)
        {
            var lRetorno = new List<ExtratoMensal>();

            try
            {
                var lConexao = new MongoClient(_Conexao);

                var lDataBase = lConexao.GetDatabase(_NomeBanco);

                IMongoCollection<Compra> lCollection = lDataBase.GetCollection<Compra>("Compras");

                //lCollection.Aggregate([$month: { }])

                //var lFiltro = Builders<Compra>.Filter.Where(xd => Convert.ToDateTime( xd.DataCompra).Month == pData.Month && Convert.ToDateTime( xd.DataCompra).Year == pData.Year);

                var lFiltro = Builders<Compra>.Filter.Empty;

                var lLista = lCollection.Find<Compra>(lFiltro).ToList();

                lLista.ForEach(xd => 
                {
                    if (xd.DataCompra.Month == pData.Month && xd.DataCompra.Year == pData.Year)
                        lRetorno.Add( this.CalculoJurosParcelado(xd));
                });
                
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);
            }

            return lRetorno;
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
                    decimal lJuros = Convert.ToDecimal(ConfigurationManager.AppSettings["ValorJuros"], lCulture);

                    lRetorno.ValorJuros = (pCompra.ValorPagto * lJuros);
                    
                    lRetorno.ValorTotal = pCompra.ValorPagto + lRetorno.ValorJuros;

                    lRetorno.ValorParcela = (lRetorno.ValorTotal / 3);
                }else
                {
                    lRetorno.ValorJuros = 0;

                    lRetorno.ValorTotal = pCompra.ValorPagto;

                    lRetorno.ValorParcela = 0;
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);
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
                var lConexao = new MongoClient(_Conexao);

                var lDataBase = lConexao.GetDatabase(_NomeBanco);

                IMongoCollection<Compra> lCollection = lDataBase.GetCollection<Compra>("Compras");

                var lFiltro = Builders<Compra>.Filter.Eq(xd => xd.IdCompra ,pListaCompra[0].IdCompra);

                lCollection.DeleteMany(lFiltro);

                _Logger.InfoFormat("As seguintes comprars foram canceladas {0}", Newtonsoft.Json.JsonConvert.SerializeObject( pListaCompra ));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);
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
                _Logger.InfoFormat("Inserindo o objeto (lista) no mongodb: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(pListaCompra));

                var lConexao = new MongoClient(_Conexao);

                var lDataBase = lConexao.GetDatabase(_NomeBanco);

                IMongoCollection<Compra> lCollection = lDataBase.GetCollection<Compra>("Compras");

                lCollection.InsertMany(pListaCompra);

                _Logger.Info("Inserção de Compras inserida no banco de dados");

            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);
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
                var lConexao = new MongoClient(_Conexao);

                var lDataBase = lConexao.GetDatabase(_NomeBanco);

                lDataBase.DropCollection("Compras");
                
            }
            catch (Exception ex)
            {
                _Logger.Error(ex.Message, ex);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
