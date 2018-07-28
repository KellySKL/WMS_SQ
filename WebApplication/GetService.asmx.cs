using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using makelanlan;
namespace WebApplication
{
    /// <summary>
    /// GetService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class GetService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public List<ClientService_kfku> KFKF(string userId, string searchName)
        {
            ClientService_kfku f = new ClientService_kfku();
            return f.Select(" and name like '%" + searchName + "%' or CONTACTOR like '%" + searchName + "%' or  PHONE like '%" + searchName + "%' ",
                "NAME,CONTACTOR,PHONE");
            //string sql = "select NAME,CONTACTOR,PHONE from ClientService_kfku where 1=1  and name like '%" + searchName + "%' or CONTACTOR like '%" + searchName + "%' or  PHONE like '%" + searchName + "%' ";

        }
    }
}
