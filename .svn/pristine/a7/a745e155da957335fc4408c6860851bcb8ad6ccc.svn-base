using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using makelanlan;
namespace WebApplication
{
    /// <summary>
    /// WeixiuService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class WeixiuService : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /// <summary>
        /// 售后维修
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [WebMethod]
        public List<WXOBJ1> AfterSRepair(string userName)
        {
            MLogin.GetExeUname();
            List<WXOBJ1> ms = new List<WXOBJ1>();
            clientservice_AfterServiceBill bill = new clientservice_AfterServiceBill();
            List<clientservice_AfterServiceBill> list = bill.Select(" and  BILLMAN='" + userName + "'   and  sz10=0 ");
            foreach (clientservice_AfterServiceBill c in list)
            {
                WXOBJ1 o = new WXOBJ1();
                o.CODE = c.CODE;//维修单号
                o.BILLMAN = c.BILLMAN;
                o.CLIENT = c.CLIENT;
                o.BILLTYPE = c.BILLTYPE;
                o.ZF1 = c.ZF1;//设备编号
                ms.Add(o);
            }
            return ms;
        }
        /// <summary>
        /// 内部维修
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [WebMethod]
        public List<WXOBJ1> NeibuWx(string userName)
        {
            MLogin.GetExeUname();
            List<WXOBJ1> ms = new List<WXOBJ1>();
            BILLnouse2 JC = new BILLnouse2();
            string where = "and  BILLTYPE ='售后返厂维修' and (isnull(Paytype,'')<>'')and left(code,1)<>'*' and  (freenum5 is null or freenum5 < 1)";
            List<BILLnouse2> bs = JC.Select(where, " PAYTYPE ");
            string str = string.Empty;
            foreach (BILLnouse2 h in bs)
            {
                str += "'" + h.PAYTYPE + "',";
            }
            str = str.Substring(0, str.Length - 1);
            clientservice_AfterServiceBill bill = new clientservice_AfterServiceBill();
            List<clientservice_AfterServiceBill> list = bill.Select(" and  BILLMAN='" + userName + "'   and code in (" + str + ")");
            foreach (clientservice_AfterServiceBill c in list)
            {
                WXOBJ1 o = new WXOBJ1();
                o.BILLMAN = c.BILLMAN;
                o.CLIENT = c.CLIENT;
                o.BILLTYPE = c.BILLTYPE;
                o.ZF1 = c.ZF1;
                ms.Add(o);
            }
            return ms;
        }
        /// <summary>
        /// 维修单接收实体
        /// </summary>
        public class WXOBJ1
        {
            private string _CLIENT;
            public string CLIENT { get { return _CLIENT; } set { _CLIENT = value; } }

            private string _CODE;
            public string CODE { get { return _CODE; } set { _CODE = value; } }

            private string _ZF1;
            public string ZF1 { get { return _ZF1; } set { _ZF1 = value; } }

            private string _BILLMAN;
            public string BILLMAN { get { return _BILLMAN; } set { _BILLMAN = value; } }

            private string _BILLTYPE;
            public string BILLTYPE { get { return _BILLTYPE; } set { _BILLTYPE = value; } }

            private int _ID;
            public int ID { get { return _ID; } set { _ID = value; } }

        }
    }
}
