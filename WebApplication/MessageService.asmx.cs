using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using makelanlan;
using ScueFun;
using System.Data;
namespace WebApplication
{
    /// <summary>
    /// MessageService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class MessageService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        
        [WebMethod(EnableSession = true)]
        public string WxGet(string userName)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            string where = " and  BILLMAN='" + userName + "'  and  zf3 <> ''   and left(code,1) <> '*'  ";
            where += " and  ((zf2=''  and  sz10=0 ) or (zf2<>'" + userName + "'))  ";//未接收
            clientservice_AfterServiceBill bill = new clientservice_AfterServiceBill();
            List<clientservice_AfterServiceBill> list = bill.Select(where);
            if (list.Count <= 0)
            {
                return null;//没有未接收消息
            }
            string msg = "您有" + list.Count.ToString() + "条维修单未接收，请尽快处理！";
            return msg;
        }
        [WebMethod(EnableSession = true)]
        public string  MsgGet(string userName)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            L_VisitSend s = new L_VisitSend();
            string today_1 = System.DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            string today = System.DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            //未接收消息
            List<L_VisitSend> list = s.Select(" and SENDUSER='" + userName + "'  and  LEFT(CODE,1)<>'*'   and  VISITDATE < '" + today + "'  and state=0 and visittype='上门拜访' ");
            if (list.Count <= 0)
            {
                return null;//没有未接收消息
            }
            string msg = "您有" + list.Count.ToString()+ "条任务单未接收，请尽快处理！";
            return msg;
        }
        [WebMethod(EnableSession = true)]
        public List<VSBill> YWBillGet(string userName,string where)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            L_VisitSend s = new L_VisitSend();
            List<VSBill> returnlist = new List<VSBill>();
            string today_1 = System.DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            string today = System.DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            //未接收消息
            List<L_VisitSend> list = s.Select(" and SENDUSER='" + userName + "'  and  LEFT(CODE,1)<>'*'   and  VISITDATE < '" + today + "'  "+ where ," top 20 * ");
            if (list.Count <= 0)
            {
                return null;//没有消息
            }
            else
            {
                foreach (L_VisitSend l in list)
                {
                    VSBill vs = new VSBill();
                    vs.id = l.ID;
                    vs.client = l.VISITCLIENT;
                    vs.hisname = l.VISITNAME;
                    vs.hisphone = l.VISITTEL;
                    vs.content = l.VISITCONTENT;
                    vs.notice = l.DOTHING;
                    vs.visittype = l.VISITTYPE;
                    vs.date = l.VISITDATE.ToShortDateString();
                    vs.code = l.CODE;
                    vs.state = l.STATE;
                    vs.rq1 = l.RQ1.ToString();
                    vs.rq2 = l.RQ2.ToString();
                    vs.zf5 = l.ZF5;
                    returnlist.Add(vs);
                }
            }
            return returnlist;
        }
        [WebMethod(EnableSession = true)]
        public List<VSBill> YWBillGet_P(string userName, int index, int curPage,int pageSize)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            int num = curPage * pageSize;
            List<VSBill> returnlist = new List<VSBill>();
            //未接收消息
            string ORDER= " order by id ";
            string WHERE = string.Empty;
            if (index == 0)//未接收
            {
                WHERE = " and state=0 and visittype='上门拜访' ";
            }
            else if (index == 1)//处理中
            {
                WHERE = " and state=7 and visittype='上门拜访' ";
            }
            else//完成
            {
                WHERE = " and state=10 and visittype='上门拜访' ";
                ORDER = " order by id desc ";
            }
            string today = System.DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            string sql = " select top " + pageSize.ToString() + " o.* from (  " +
                    " select row_number() over("+ORDER+") as rownumber, N_A.* from( " +
                    " select * from L_VisitSend " +
                    " where 1 = 1  and SENDUSER='" + userName + "' and  LEFT(CODE,1)<>'*'  and  VISITDATE < '" + today + "'  " + WHERE +
                    " ) as N_A) as o where rownumber> " + num.ToString();
            DataTable dt = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            // List<L_VisitSend> list = s.Select(" and SENDUSER='" + userName + "'  and  LEFT(CODE,1)<>'*'   and  VISITDATE < '" + today + "'  " + where, " top 20 * ");
            if (dt==null|| dt.Rows.Count <= 0)
            {
                return null;//没有消息
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    VSBill vs = new VSBill();
                    vs.id = Convert.ToInt32(row["ID"].ToString());
                    vs.client = row["VISITCLIENT"].ToString();
                    vs.hisname = row["VISITNAME"].ToString();
                    vs.hisphone = row["VISITTEL"].ToString();
                    vs.content = row["VISITCONTENT"].ToString();
                    vs.notice = row["DOTHING"].ToString();
                    vs.visittype = row["VISITTYPE"].ToString();
                    vs.date = row["VISITDATE"].ToString();
                    vs.code = row["CODE"].ToString();
                    vs.state = Convert.ToInt32(row["STATE"].ToString());
                    vs.rq1 = row["RQ1"].ToString();
                    vs.rq2 = row["RQ2"].ToString();
                    vs.zf5 = row["ZF5"].ToString();
                    returnlist.Add(vs);
                }
            }
            return returnlist;
        }
        [WebMethod(EnableSession = true)]
        public VSBill YW_Detail_Bill(string code)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            L_VisitSend s = new L_VisitSend();
            VSBill vs = new VSBill();
            //未接收消息
            List<L_VisitSend> list = s.Select(" and CODE='" + code + "' ");
            if (list.Count <= 0)
            {
                return null;//没有未接收消息
            }
            else
            {
                L_VisitSend l = list[0];
                    vs.id = l.ID;
                    vs.client = l.VISITCLIENT;
                    vs.hisname = l.VISITNAME;
                    vs.hisphone = l.VISITTEL;
                    vs.content = l.VISITCONTENT;
                    vs.notice = l.DOTHING;
                    vs.visittype = l.VISITTYPE;
                    vs.date = l.VISITDATE.ToShortDateString();
                    vs.state = l.STATE;
                    vs.code = l.CODE;
            }
            return vs;
        }

        [WebMethod(EnableSession = true)]
        public bool Accept_YWBill(string id,string userName)
        {
            bool flag = false;
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return false;//身份验证失败
            }
            MLogin.GetExeUname();
            L_VisitSend s = new L_VisitSend();
            s.GETUSER = userName;
            s.RQ1 = System.DateTime.Now;//接收时间
            s.STATE = 7;//接受派单
            s.Updata(" and id=" + id);
            flag = true;
            return flag;
        }

        [WebMethod(EnableSession = true)]
        public List<LLBill> NeighborBill(string id,string userName ,double lng,double lat, int curPage, int pageSize)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            DataTable dt=Function.NeighbourBill(curPage,pageSize,lng, lat);
            List<LLBill> result = new List<LLBill>();
            foreach (DataRow row in dt.Rows)
            {
                LLBill n = new LLBill();
                n.id = row["ID"].ToString(); //客户
                n.client = row["CLIENT"].ToString(); //客户
                n.code = row["CODE"].ToString(); //单号
                n.depar = row["DEPAR"].ToString(); //部门
                n.finshdate = Convert.ToDateTime(row["FINSHDATE"].ToString()).ToString("yyyy-MM-dd"); //要求完成时间
                n.id = row["ID"].ToString(); 
                n.inserter = row["INSERTER"].ToString(); //提出人
                n.machinecn = row["MACHINECN"].ToString(); //机床编号
                n.mtitle = row["MTITLE"].ToString(); //内容
                n.type = row["TYPE"].ToString(); //状态
                n.distance = row["DISTANCE"].ToString()+"km"; //距离
                n.contactor = row["CONTACTOR"].ToString(); //联系人
                n.phone = row["PHONE"].ToString(); //电话
                n.lng = row["ZF29"].ToString(); //经度
                n.lat = row["ZF30"].ToString(); //纬度
                result.Add(n);
            }
            return result;
        }

        [WebMethod(EnableSession = true)]
        public List<LLBill> LLBill_Mine(string id, string userName, double lng, double lat, int curPage, int pageSize)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            string where = "";
            where = " and depar='" + userName + "' ";
            if (userName == "邵凯丽" || userName == "郑昌仁")
            {
                where = "";
            }
            string sql = " select top 15 * from " +
                 "   (select * from L_ContactSheet as aa " +
                 "   left join " +
                 "   (select name, ZF29, ZF30,CONTACTOR,PHONE from clientservice_kfku ) as bb on aa.Client = bb.NAME " +
                 "   ) as cc" +
                 "   where  type=0  and del =0 " + where + "  order by finshdate   ";
            List<LLBill> result = new List<LLBill>();
            DataTable dt = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            foreach (DataRow row in dt.Rows)
            {
                LLBill n = new LLBill();
                n.id = row["ID"].ToString(); //客户
                n.client = row["CLIENT"].ToString(); //客户
                n.code = row["CODE"].ToString(); //单号
                n.depar = row["DEPAR"].ToString(); //部门
                n.finshdate = Convert.ToDateTime(row["FINSHDATE"].ToString()).ToString("yyyy-MM-dd"); //要求完成时间
                n.id = row["ID"].ToString();
                n.inserter = row["INSERTER"].ToString(); //提出人
                n.machinecn = row["MACHINECN"].ToString(); //机床编号
                n.mtitle = row["MTITLE"].ToString(); //内容
                n.type = row["TYPE"].ToString(); //状态
               // n.distance = row["DISTANCE"].ToString() + "km"; //距离
                n.contactor = row["CONTACTOR"].ToString(); //联系人
                n.phone = row["PHONE"].ToString(); //电话
                n.lng = row["ZF29"].ToString(); //经度
                n.lat = row["ZF30"].ToString(); //纬度
                result.Add(n);
            }
            return result;
        }
        

        [WebMethod(EnableSession = true)]
        public List<RpBill> GetReply(string fid)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            List<RpBill> result = new List<RpBill>();
            L_ContactSheetMsg msg = new L_ContactSheetMsg();
            List<L_ContactSheetMsg> list =  msg.Select("  and  fid='" + fid + "' order by id desc "," top 30 * ");
            foreach (L_ContactSheetMsg r in list)
            {
                RpBill n = new RpBill();
                n.createtime = r.INSERTDATE.ToString("yyyy-MM-dd HH:mm:ss");
                n.state = r.STATE;
                n.inserter = r.INSERTER;
                n.content = r.MTITLE;
                result.Add(n);
            }
            return result;
        }

        [WebMethod(EnableSession = true)]
        public string Reply(int id, string reply,string userName,double lng, double lat, double kflng, double kflat)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            if (MyGlobal.AERA_CHECK)//是否检核范围
            {
                if (kflng== -1 || kflat == -1)
                {
                    return "None";
                }
                if (ScueFun.LngLatDis.GetDistance(lng, lat, kflng, kflat) > MyGlobal.VISITAERA)//超出范围
                {
                    return "Toofar";
                }
            }

            //事务处理开始,劫持链接通道的sql语句
            TransactionSql.Start(MyGlobal.DataBase);  //===>开始
            DateTime now = SysTime.GetTime;
            List<RpBill> result = new List<RpBill>();
            L_ContactSheetMsg msg = new L_ContactSheetMsg();
            msg.MTITLE = reply;
            msg.INSERTER = userName;
            msg.FID = id;
            msg.INSERTDATE = now;
            msg.STATE = "已提交";
            int get_id = msg.Insert();
            List<L_ContactSheetMsg> msg_list =  msg.Select(" and INSERTDATE='" + now.ToString() + "'  ");
            L_ContactSheet l_Contact = new L_ContactSheet();
            List<L_ContactSheet>  list =  l_Contact.Select(" and id = " + id.ToString());
            if (list.Count > 0)
            {
                //更新关联单据号，最新的，可作为最后打卡的目标公司,名称、地址  20180904 skl
                puku_user u = new puku_user();
                u.REFERCODE = msg_list.Count>0 ? msg_list[0].ID.ToString():"";
                u.ZF1 = list[0].CLIENT;
                u.ZF2 = kflng.ToString();
                u.ZF3 = kflat.ToString();
                u.ZF4 = now.ToString("yyyy-MM-dd HH:mm:ss");
                u.Updata(" and  TURENAME='" + userName + "'  ");
            }
            else
            {
                return "没有该联络单！";
            }
            //提交事务到sql服务器处理//===>结束
            if (!TransactionSql.EndSql())//判断是否成功
            {
                return "回复失败！";
            }
            return msg.INSERTDATE.ToString();
        }
    }
}
