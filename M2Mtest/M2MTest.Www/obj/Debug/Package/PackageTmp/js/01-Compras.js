/// <reference path="00-Auxiliares.js" />

var gFirstClicktblListaCompras = false;

var gGridListaCompras;
var gGridExtratoCompras;

var gDados_ExtratoCompras   = {};
var gDados_ListaCompras     = {};

function Load_Grid_Compras(pParametros) {
    
    gGridListaCompras = 
    {
         datatype: "jsonstring"
        , datastr: gDados_ListaCompras.rows
        //, mtype: "POST"
        , hoverrows: true
        //, postData: pParametros
        , autowidth: false
        , shrinkToFit: false
        , loadonce: false
        , colModel: [
              { label: "idCompra",          name: "idCompra",           jsonmap: "idCompra",        index: "idCompra",          width:100,align: "left", sortable: true }
            , { label: "Descrição",         name: "Descricao",          jsonmap: "Descricao",       index: "Descricao",         width:100,align: "center", sortable: true }
            , { label: "Estabelecimento",   name: "Estabelecimento",    jsonmap: "Estabelecimento", index: "Estabelecimento",   width:100,align: "center", sortable: true }
            , { label: "Data Compra",       name: "DataCompra",         jsonmap: "DataCompra",      index: "DataCompra",        width:100,align: "center", sortable: true,  formatter: 'date', formatoptions: { srcformat: 'Y-m-d', newformat: 'd/m/Y', } ,datefmt:'d-M-Y' }
            , { label: "Forma Pagamento",   name: "FormaPagamento",     jsonmap: "FormaPagamento",  index: "FormaPagamento",    width:100,align: "center", sortable: true }
            , { label: "Cancelar",          name: "Cancelamento",       jsonmap: "Cancelamento",    index: "Cancelamento",      width: 100, align: "center", sortable: true }
        ]
        , height: 'auto'
        //, width: '700'
        //, rowNum: 0
        , sortname: "invid"
        , sortorder: "asc"
        , sortable: true
        , viewrecords: true
        , gridview: false   //flag importante para velocidade, mas exclui evento afterRowInsert; Ver documentação em http://www.trirand.com/jqgridwiki/doku.php?id=wiki:options
        , subGrid: false
        , caption: 'Lista de compras'
        //, pager: '#ComprasGridPager'
        , rowNum: 15
        , rowList: [10, 20, 30]
        , afterInsertRow: Load_Grid_Compras_ItemDataBound
        }

    $("#tblListaCompras").jqGrid(gGridListaCompras).jqGrid('hideCol', "idCompra");

    $("#lui_tblListaCompras").removeClass('ui-widget-overlay')
    
}

function Load_Grid_Compras_Callback(pResposta) {

    var lTemp = JSON.parse(pResposta);

    var lData = JSON.parse(lTemp);

    if (lData.length) {
        for (i = 0; i < lData.length; i++) {
            var lObjeto =
                {
                    idCompra        : lData[i].IdCompra,
                    Descricao       : lData[i].Descricao,
                    Estabelecimento: lData[i].Estabelecimento,
                    FormaPagamento  : lData[i].FormaPagamento,
                    Cancelamento    : 'S'
                };

            var lTemp = new Date(lData[i].DataCompra);

            var lDateString = lTemp.getDate() + "/" + (lTemp.getMonth() + 1) + "/" + lTemp.getFullYear();

            lObjeto.DataCompra = lDateString,

            gDados_ListaCompras.rows.push(lObjeto);
            gDados_ListaCompras.total++;
            gDados_ListaCompras.records++;
        }
    }

    $("#txtExtratoDescricao").val(''),
    $("#txtExtratoEstabelecimento").val(''),
    $("#txtExtratoDataCompra").val(''),
    $("#cboFormaPagamento").val('1'),
    $("#txtValorPagto").val('')

    $("#tblListaCompras")
        .jqGrid('setGridParam', { datatype: 'local', data: gDados_ListaCompras.rows })
        .trigger("reloadGrid");
}

function Load_Grid_ExtratoMensal()
{
    gGridExtratoCompras =
    {
        datatype: "jsonstring"
        , datastr: gDados_ExtratoCompras.rows
        //, mtype: "POST"
        , hoverrows: true
        //, postData: pParametros
        , autowidth: false
        , shrinkToFit: false
        , loadonce: false
        , colModel: [
              { label: "Forma Pagamento",   name: "FormaPagamento",     jsonmap: "FormaPagamento", index: "FormaPagamento", width: 185, align: "center", sortable: true }
            , { label: "Valor Pagamento",   name: "ValorPagto",         jsonmap: "ValorPagto",      index: "ValorPagto",        width: 85,  align: "center", sortable: true, formatter: 'number', formatoptions: {  thousandsSeparator: '.', decimalSeparator: ',', decimalPlaces: 2  } }
            , { label: "Valor Juros",       name: "ValorJuros",         jsonmap: "ValorJuros",      index: "ValorJuros",        width: 85,  align: "center", sortable: true, formatter: 'number', formatoptions: {  thousandsSeparator: '.', decimalSeparator: ',', decimalPlaces: 2  } }
            , { label: "Valor Parcela",     name: "ValorParcela",       jsonmap: "ValorParcela",    index: "ValorParcela",      width: 85,  align: "center", sortable: true, formatter: 'number', formatoptions: {  thousandsSeparator: '.', decimalSeparator: ',', decimalPlaces: 2 } }
            , { label: "Valor Total",       name: "ValorTotal",         jsonmap: "ValorTotal",      index: "ValorTotal",        width: 85,  align: "center", sortable: true, formatter: 'number', formatoptions: {  thousandsSeparator: '.', decimalSeparator: ',', decimalPlaces: 2 } }
        ]
        , height: 'auto'
        , width: 'auto'
        , sortname: "invid"
        , sortorder: "asc"
        , sortable: true
        , viewrecords: true
        , gridview: false   //flag importante para velocidade, mas exclui evento afterRowInsert; Ver documentação em http://www.trirand.com/jqgridwiki/doku.php?id=wiki:options
        , subGrid: false
        , caption: 'Extrato Mensal'
        //, pager: '#ExtratoGridPager'
        , rowNum: 15
        , rowList: [10, 20, 30]
        , afterInsertRow: Load_Grid_Extrato_ItemDataBound
    }

    $("#tblListaExtratoMensal").jqGrid(gGridExtratoCompras);//.jqGrid('hideCol', "Id");

    $("#lui_tblListaExtratoMensal").removeClass('ui-widget-overlay');
}

function Load_Grid_Extrato_Callback(pResposta) {

    var lTemp = JSON.parse(pResposta);

    var lData = JSON.parse(lTemp);

    if (lData.length) {
        for (i = 0; i < lData.length; i++) {
            var lObjeto =
                {
                    FormaPagamento  : lData[i].FormaPagamento,
                    ValorTotal      : lData[i].ValorTotal,
                    ValorParcela    : lData[i].ValorParcela,
                    ValorJuros      : lData[i].ValorJuros,
                    ValorPagto      : lData[i].ValorPagto
                };

            gDados_ExtratoCompras.rows.push(lObjeto);
            gDados_ExtratoCompras.total++;
            gDados_ExtratoCompras.records++;
        }
    }

    $("#tblListaExtratoMensal")
        .jqGrid('setGridParam', { datatype: 'local', data: gDados_ExtratoCompras.rows })
        .trigger("reloadGrid");
}

function Load_Grid_Compras_ItemDataBound(rowid, pData) {

    let lGrid = $("#tblListaCompras");

    if (pData.Cancelamento == 'S')
    {
        lGrid.jqGrid('setCell', rowid, 'Cancelamento', '<div><a href="javascript:void(0);" onclick="javascript:Load_Grid_Compra_Cancelar(\'' + pData.idCompra + '\')" ><span class="ui-button-icon ui-icon ui-icon-closethick" style="padding-left:5px"></span></a></div>', '');
    }
    var lDate =  pData.DataCompra;

    //let lDataStirng = lDate.getDate() + "/"+ (lDate.getMonth()+1) + "/" + lDate.getFullYear();

    //lGrid.jqGrid('setCell', rowid, 'DataCompra', lDataStirng);

    if (pData.FormaPagamento == "1")
    {
        lGrid.jqGrid('setCell', rowid, 'FormaPagamento', 'a Vista')
    }else
    {
        lGrid.jqGrid('setCell', rowid, 'FormaPagamento', 'Parc. c/ Juros')
    }
}

function Load_Grid_Extrato_ItemDataBound(rowid, pData)
{
    let lGrid = $("#tblListaExtratoMensal");

    if (pData.FormaPagamento == "1")
    {
        lGrid.jqGrid('setCell', rowid, 'FormaPagamento', 'a Vista')
    } else {
        lGrid.jqGrid('setCell', rowid, 'FormaPagamento', 'Parc. c/ Juros')
    }
}

function Load_Grid_Compra_Cancelar(pIdCompra)
{
    var lUrl = Aux_UrlComRaiz('api/compras/CancelarCompra');

    var lDados =
        {
            acao: "CancelarCompra",
            idCompra: pIdCompra
        };

    Aux_CarregarHtmlVerificandoErro(lUrl, lDados, Load_Grid_Compra_Cancelar_Callback);
}

function Load_Grid_Compra_Cancelar_Callback(pReposta)
{
    var rowIds = $("#tblListaCompras").jqGrid('getDataIDs');

    var lTemp = JSON.parse(JSON.parse(pReposta));

    for (j = 0; j < lTemp.length; j++)
    {
        var idSearchValue = lTemp[j].IdCompra;

        for (i = 1; i <= rowIds.length; i++)
        {
            rowData = $("#tblListaCompras").jqGrid('getRowData', i);

            if (rowData['idCompra'] == idSearchValue)
            {
                $('#tblListaCompras').jqGrid('delRowData', i);
            } 
        }
    }

    alert('Compra removida com sucesso');

}

function btnFiltroExtrato_Click()
{
    gDados_ExtratoCompras = { page: 1, total: 0, records: 0, rows: [] }

    var lUrl = Aux_UrlComRaiz('api/compras/ListarExtratoCompras');

    var lDados =
        {
            acao:               "CarregarHtmlComDados",
            DataCompra:  "01/" + $("#txtDataFiltro").val(),
        };

    Load_Grid_ExtratoMensal();

    Aux_CarregarHtmlVerificandoErro(lUrl, lDados, Load_Grid_Extrato_Callback);

    return false;
}

function btnAbreModalCompra_Click() {
    ///TODO - Abre modal com o formulário....

    $("#dialog").dialog();
    
}

function btnNovaCompra_Click() {

    gDados_ListaCompras = { page: 1, total: 0, records: 0, rows: [] }

    var lUrl = Aux_UrlComRaiz('api/compras/InserirNovaCompra');

    var lDados =
        {
            acao            :   "CarregarHtmlComDados",
            Descricao       :   $("#txtExtratoDescricao").val(),
            Estabelecimento :   $("#txtExtratoEstabelecimento").val(),
            DataCompra      :   $("#txtExtratoDataCompra").val(),
            FormaPagamento  :   $("#cboFormaPagamento").val(),
            ValorPagto      :   $("#txtValorPagto").val().replace("R$:", '')
        };

    Load_Grid_Compras(null);

    Aux_CarregarHtmlVerificandoErro(lUrl, lDados, Load_Grid_Compras_Callback);
    
    $("#dialog").dialog('close');

    alert('Compra inserida com sucesso!!!');

    return false;
}

function btnCarregaMock_Click()
{
    gDados_ListaCompras = { page: 1, total: 0, records: 0, rows: [] }

    var lUrl = Aux_UrlComRaiz('api/compras/CarregaDadosMock');

    var lDados = "";

    //$('#tblListaCompras').jqGrid('clearGridData');

    Load_Grid_Compras(null);

    Aux_CarregarHtmlVerificandoErro(lUrl, lDados, Load_Grid_Compras_Callback);

    return false;
}

function LoadInicialGridCompras()
{
    gDados_ListaCompras = { page: 1, total: 0, records: 0, rows: [] }

    var lUrl = Aux_UrlComRaiz('api/compras/ListarCompras');

    var lDados = "";

    //$('#tblListaCompras').jqGrid('clearGridData');

    Load_Grid_Compras(null);

    Aux_CarregarHtmlVerificandoErro(lUrl, lDados, Load_Grid_Compras_Callback);

    return false;
}