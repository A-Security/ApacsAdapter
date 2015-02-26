<%@ WebService Language="C#" Class="AdpWebService" %>
using ApacsAdapter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://schemas.datacontract.org/2004/07/ApacsAdapterService")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class AdpWebService  : System.Web.Services.WebService {

    [WebMethod]
    public List<string> fillCardHoldersToGR()
    {
        AdpConfigXml cfg = new AdpConfigXml();
        ApacsServer apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
        AdpGRAdapter gr = new AdpGRAdapter(cfg);
        gr.fillGRfromApacs();
        return gr.getResourceFromRegistry();
    }
}