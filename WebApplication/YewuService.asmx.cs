﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using makelanlan;
namespace WebApplication
{
    /// <summary>
    /// YewuService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class YewuService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public Boolean NewVisit(int client_id, string client,string zf2, string billtype,string date, string hisname,
            string hisposition,string hisphone,string content,string userName)
        {
            MLogin.GetExeUname();
            clientservice_VisitBill bill = new clientservice_VisitBill();
            ClientService_kfku k = new ClientService_kfku();
            List<ClientService_kfku> ks = k.Select(" and ID=" + client_id, " TRADETYPENAME  ");
            if (ks.Count > 0)
            {
                bill.ZF1 = ks[0].TRADETYPENAME;
            }
            bill.CODE = ScueFun.Code.BasicCode();
            bill.CLIENT = client;
            bill.BILLDATE = Convert.ToDateTime(date);
            bill.BILLMAN = userName;
            bill.NOTEPRE = content;
            bill.ZF2 = zf2;
            bill.BILLTYPE = billtype;
            bill.INTRODUCERDATE = SysTime.GetTime;
            bill.FINISHDATE = SysTime.GetTime;
            bill.ZF3 = hisname;
            bill.ZF4 = hisposition;
            bill.ZF5 = hisphone;
            bill.INTRODUCER = userName;
            bill.Insert();
            return true;
        }
        [WebMethod]
        public List<VisitBill> HisVisit(string userName)
        {
            MLogin.GetExeUname();
            clientservice_VisitBill bills = new clientservice_VisitBill();
            List<clientservice_VisitBill> list= bills.Select(" and billman='"+ userName + "' order by id desc ", " top 20 client,billdate,notepre,code");
            List<VisitBill> history = new List<VisitBill>();
            foreach (clientservice_VisitBill c in list)
            {
                VisitBill b = new VisitBill();
                b.code = c.CODE;
                b.client = c.CLIENT;
                b.date = c.BILLDATE.ToString("yyyy-MM-dd");
                b.content = c.NOTEPRE;
                history.Add(b);
            }
            return history;
        }
        [WebMethod]
        public void EditVisit(string code,string content)
        {
            MLogin.GetExeUname();
            clientservice_VisitBill bills = new clientservice_VisitBill();
            List<clientservice_VisitBill> list = bills.Select(" and  code='"+ code + "'");
            if (list.Count > 0)
            {
                clientservice_VisitBill bill = list[0];
                bill.Reset_clientservice_VisitBill();
                bill.NOTEPRE = content;
                bill.FINISHDATE= SysTime.GetTime;
                bill.Updata(" and code='"+ code + "' ");
                
            }
        }


        public class VisitBill
        {
            private string _code;
            public string code { get { return _code; } set { _code = value; } }
            private string _client;
            public string client { get { return _client; } set { _client = value; } }

            private string _date;
            public string date { get { return _date; } set { _date = value; } }

            private string _content;
            public string content { get { return _content; } set { _content = value; } }

            //private int _status;
            //public int status { get { return _status; } set { _status = value; } }

            //private string _authorised;
            //public string authorised { get { return _authorised; } set { _authorised = value; } }

        }
    }
}