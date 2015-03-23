<%@ WebService Language="C#" Class="AdpWebService" %>
using ApacsAdapter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://schemas.datacontract.org/2004/07/ApacsAdapter")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class AdpWebService  : WebService {

    [WebMethod]
    public bool fillGRCardHolders()
    {
        AdpCfgXml cfg = new AdpCfgXml();
        ApacsServer apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
        AdpGRAdapter gr = new AdpGRAdapter(cfg.GRhost, cfg.GRuser, cfg.GRpassword);
        return gr.copyCHfromApacs(apacsInstance); 
    }
}