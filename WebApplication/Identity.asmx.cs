using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using makelanlan;
using ScueFun;
using System.Data;
using System.Web.SessionState;
namespace WebApplication
{
    /// <summary>
    /// Identity 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class Identity : System.Web.Services.WebService
    {
        [System.Web.Services.Protocols.SoapHeader("myHeader")]
        [WebMethod(Description = "判断用户是否开通", EnableSession = true)]
        public static bool _GetValue()
        {
            return MyGlobal.myHeader.CheckLogin();
        }
    }
}
