
function Aux_UrlComRaiz(pURL) {
    var lURL = $("#hidRaizDoSite").val();

    if ((lURL.charAt(lURL.length - 1) == "/") && (pURL.charAt(0) == "/"))
        lURL = lURL.substr(0, lURL.length - 1);

    lURL = lURL + pURL;

    return lURL;
}

function Aux_CarregarHtmlVerificandoErro(pUrl, pDadosDoRequest, pCallBackDeSucesso) {

    $.ajax({
        url: pUrl
        , type: "post"
        , cache: false
        , data: pDadosDoRequest
        , success: pCallBackDeSucesso
        //, error: Aux_TratarRespostaComErro
        ,beforeSend: function( xhr ) {
            xhr.overrideMimeType( "text/plain; charset=x-user-defined" );
        }
    });
}

function Aux_CarregarVerificandoErro_CallBack(pResposta, pCallBack) {
    if (pResposta != null) {
        if (pResposta.Mensagem) {
            // resposta jSON

            if (pResposta.TemErro) {
                //erro em chamada json
                //GradIntra_TratarRespostaComErro(pResposta, pPainelParaAtualizar);
                alert(pResposta.Mensagem);
            }
            else {
                //sucesso em chamada JSON
                if (pCallBack && pCallBack != null)
                    pCallBack(pResposta);
            }
        }
        else {   // resposta html
            pCallBack(pResposta);
            //            if (pResposta.indexOf('"TemErro":true,') != -1)
            //            {   //erro, porém retorno json em chamada html (ocorre com timeout por exemplo)
            //                //GradIntra_TratarRespostaComErro($.evalJSON(pResposta));
            //            }
        }
    }
}

function Page_Load() {
    //
    Page_Load_CodeBehind();
}