using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M2MTest.Entities;
using log4net;
using MongoDB.Driver;
using System.Configuration;
using MongoDB.Bson;
using System.Globalization;

namespace M2MTest.DB
{
    /// <summary>
    /// Classe de acesso ao banco de dados 
    /// </summary>
    public class DBAcesso: IDisposable
    {
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

        /// <summary>
        /// Para conversão de moeda
        /// </summary>
        public CultureInfo _culture = new CultureInfo("en-US");

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
        /// <param name="pData">Data usada para filtro no MOngo db</param>
        /// <returns>Retorna uma lista de objetos Compra para classe de negócio</returns>
        public List<Compra> ListaExtrato(DateTime pData)
        {
            var lRetorno = new List<Compra>();

            try
            {
                var lConexao = new MongoClient(_Conexao);

                var lDataBase = lConexao.GetDatabase(_NomeBanco);

                IMongoCollection<Compra> lCollection = lDataBase.GetCollection<Compra>("Compras");

                var lProject = BsonDocument.Parse("{_id:1, IdCompra:1, Descricao:1, Estabelecimento:1, DataCompra:1, FormaPagamento:1,ValorPagto:1, year:{ $year: '$DataCompra'}, month: {$month: '$DataCompra'}}");

                var aggDoc = lCollection
                    .Aggregate()
                    //.Unwind("C")
                    .Project(lProject)
                    .Match(BsonDocument.Parse("{year: {$eq:" + pData.Year + "}, month: {$eq:" + pData.Month + "}}")).ToList() ;


                //lCollection.Aggregate([$month: { }])

                //var lFiltro = Builders<Compra>.Filter.Where(xd => Convert.ToDateTime( xd.DataCompra).Month == pData.Month && Convert.ToDateTime( xd.DataCompra).Year == pData.Year);

                //var lFiltro = Builders<Compra>.Filter.Empty;

                //var lLista = lCollection.Find<Compra>(lFiltro).ToList();

                
                aggDoc.ForEach(item => 
                {
                    var compra = new Compra();

                    compra.IdCompra         = Convert.ToInt32(item["IdCompra"]);
                    compra.Descricao        = item["Descricao"].ToString();
                    compra.Estabelecimento  = item["Estabelecimento"].ToString();
                    compra.DataCompra       = Convert.ToDateTime(item["DataCompra"].ToString());
                    compra.FormaPagamento   = item["FormaPagamento"].ToString();
                    compra.ValorPagto       = Convert.ToDecimal(item["ValorPagto"], _culture);


                    lRetorno.Add(compra);
                });
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
            //this.Dispose();
        }
    }
}
