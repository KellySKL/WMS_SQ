﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using makelanlan;
using System.Data;
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
        [WebMethod(EnableSession = true)]
        //int client_id, 
        public string GetKFLoc(string client)
        {
            //获取客户位置
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return "NoCookies";
            }
            MLogin.GetExeUname();
            return KFLocation.Get(client);
        }

        /// <summary>
        /// 新建客户拜访
        /// </summary>
        [WebMethod(EnableSession = true)]
        public int NewVisit(int  bfid, string userId, string client,string zf2, string billtype,string date, string hisname,
            string hisposition,string hisphone,string content,string userName,double lng1,double lat1,
            string nextTime,string nextMethod,string nextNotice,string saleContent,string IfWX,
            string list_id,string list_reply)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return 10;//用户验证失败
            }
            MLogin.GetExeUname();
            int flag = 0;//默认为0
            try
            {
                //事务处理开始,劫持链接通道的sql语句
                DateTime now = SysTime.GetTime;
                TransactionSql.Start(MyGlobal.DataBase);  //===>开始
                if (list_id.Trim() != "" && list_reply.Trim() != "")
                {
                    list_id = list_id.Substring(0, list_id.Length - 1);
                    list_reply = list_reply.Substring(0, list_reply.Length - 1);
                    string[] ids = list_id.Split(',');
                    string[] replies = list_reply.Split(',');
                    int count;
                    for (count = 0; count < ids.Length; count++)
                    {
                        L_ContactSheetMsg msg = new L_ContactSheetMsg();
                        msg.MTITLE = replies[count];
                        msg.INSERTER = userName;
                        msg.FID = Convert.ToInt32(ids[count]);
                        msg.INSERTDATE = now;
                        msg.STATE = "已提交";
                        msg.Insert();
                    }
                }

                string position = KFLocation.Get(client);
                double lng, lat;
                if (MyGlobal.AERA_CHECK)  //是否范围校验
                {
                    if (position == "")//该客户没有定位，将现在这个位置赋值给当前客户
                    {
                        flag = -1;
                        return flag;
                    }
                    else if (position == "none")//不存在该用户
                    {
                        flag = -2;
                        return flag;
                    }
                    string[] strs_kf = position.Split(',');
                    lng = Convert.ToDouble(strs_kf[0]);
                    lat = Convert.ToDouble(strs_kf[1]);

                    #region 数据库获取定位
                    //string userpoi = UserLocation.Get(userId);
                    //
                    //if (userpoi == "")
                    //{
                    //    flag = -2;
                    //    return flag;//用户无定位信息
                    //}
                    //string[] poi = userpoi.Split(':');
                    //lng1 = Convert.ToDouble(poi[0]);
                    //lat1 = Convert.ToDouble(poi[1]);
                    #endregion

                    if (ScueFun.LngLatDis.GetDistance(lng, lat, lng1, lat1) > MyGlobal.VISITAERA)
                    {
                        flag = 5;
                        return flag;//超出范围不能提交
                    }
                }
                else
                {
                    lng = lng1;
                    lat = lat1;
                }

                clientservice_VisitBill bill = new clientservice_VisitBill();
                //kfku m = new kfku();
                string lev = string.Empty;
                int day = 0;
                //List<kfku> ms = m.Select(" and NAME='" + client + "'  order by id desc  ");
                //if (ms.Count > 0)
                //{
                //    if(ms[0].TRADETYPENAME!=null&& ms[0].LEV!=null)
                //    {
                //        bill.ZF1 = ms[0].TRADETYPENAME.ToString();
                //        lev = ms[0].LEV;
                //    }
                //}
                //else
                //{

                ClientService_kfku k = new ClientService_kfku();
                List<ClientService_kfku> ks = k.Select(" and NAME='" + client + "'   order by id desc   ");
                if (ks.Count > 0)
                {
                    if (ks[0].TRADETYPENAME != null && ks[0].LEV != null)
                    {
                        bill.ZF1 = ks[0].TRADETYPENAME.ToString();
                        lev = ks[0].LEV;
                        if (ks[0].TRADETYPENAME == "新建客户" && ks[0].CONTACTOR == null && ks[0].PHONE == null && ks[0].ZF34 == "PHONE")
                        {
                            k.CONTACTOR = hisname;
                            k.PHONE = hisphone;
                            k.ZF8 = hisposition;
                            k.Updata(" and id=" + ks[0].ID);
                        }
                    }
                }
                //}
                if (lev.Contains("1级"))
                {
                    day = 150;
                }
                else if (lev.Contains("2级"))
                {
                    day = 100;
                }
                else if (lev.Contains("3级"))
                {
                    day = 60;
                }
                else if (lev.Contains("4级"))
                {
                    day = 30;
                }
                else if (lev.Contains("5级"))
                {
                    day = 7;
                }
                else
                {
                    day = 0;
                }
                bill.CODE = ScueFun.Code.BasicCode();
                bill.CLIENT = client;
                bill.BILLDATE = Convert.ToDateTime(date);
                bill.BILLMAN = userName;
                bill.NOTEPRE = content;
                bill.ZF2 = zf2;
                bill.BILLTYPE = billtype;
                bill.INTRODUCERDATE = now;
                bill.FINISHDATE = now;
                bill.ZF3 = hisname;//存放对方信息
                bill.ZF4 = hisposition;
                bill.ZF5 = hisphone;

                //增加天
                DateTime dt1 = now.AddDays(day);
                if (nextTime == "默认")
                {
                    bill.ZF6 = dt1.ToString("yyyy-MM-dd");
                }
                else
                {
                    bill.ZF6 = nextTime;
                }
                bill.ZF7 = IfWX;//是否有其他需求
                bill.ZF8 = nextNotice;
                bill.ZF9 = nextMethod;
                bill.ZF10 = saleContent;
                string[] strs = Function.StayTime(userId, lng1, lat1, client);
                if (strs != null)
                {
                    bill.ZF11 = strs[0] == null ? "" : strs[0];//开始时间
                    bill.ZF12 = strs[1] == null ? "" : strs[1];//结束时间
                    bill.ZF13 = strs[2] == null ? "" : strs[2];//时间间隔
                }
                bill.INTRODUCER = userName;

                L_VisitSend s = new L_VisitSend();
                string today_1 = now.ToString("yyyy-MM-dd 00:00:00");
                string today = now.ToString("yyyy-MM-dd 23:59:59");
                string refer = string.Empty;
                if (bfid == -1) //若不是链接操作,主动查询,校验是否存在派单信息，取最早的
                {
                    List<L_VisitSend> sl = s.Select("  and VISITCLIENT='" + client + "'  and state = 7  and GETUSER='" + userName + "'  order by id ");
                    if (sl.Count > 0)
                    {
                        bfid = sl[0].ID;
                    }
                }
                if (bfid != -1)
                {
                    List<L_VisitSend> list = s.Select("  and id= " + bfid + "  and state = 7 ");
                    foreach (L_VisitSend i in list)
                    {
                        L_VisitSend vs = new L_VisitSend();
                        vs.STATE = 10;
                        vs.RQ2 = now;//完成时间
                        if (IfWX.Contains("有业务需求"))
                        {
                            vs.ZF1 = "有意向";
                        }
                        vs.ZF5 = content;//拜访内容
                        vs.Updata(" and id=" + i.ID);
                        refer = i.CODE;
                    }
                }
                if (IfWX.Contains("有业务需求"))
                {
                    bill.ZF14 = "有意向";
                }
                ClientService_kfku km = new ClientService_kfku();
                km.ZF33 = bill.CODE;//关联单据号
                km.RQ1 = Convert.ToDateTime(bill.ZF6 + " 00:00:00");//string格式有要求，必须是yyyy-MM-dd hh: mm: ss
                km.Updata("  and NAME='" + client + "'  ");
                bill.REFERCODE = refer;
                bill.Insert();

                //更新关联单据号，最新的，可作为最后打卡的目标公司,名称、地址  20180904 skl
                puku_user u = new puku_user();
                u.REFERCODE = bill.CODE;
                u.ZF1 = client;
                u.ZF2 = lng.ToString();
                u.ZF3 = lat.ToString();
                u.ZF4 = now.ToString("yyyy-MM-dd HH:mm:ss");
                u.Updata(" and  USERPU='" + userId + "'   ");

                //提交事务到sql服务器处理
                if (!TransactionSql.EndSql())
                {
                    flag = -3;//回滚触发
                }                     //===>结束
            }
            catch (Exception ex)
            {
                flag = -3;//回滚触发
            }
            finally
            {
               
            }
            return flag;
        }
        [WebMethod(EnableSession = true)]
        public List<VisitBill> HisVisit(string userName)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;
            }
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
        [WebMethod(EnableSession = true)]
        public void EditVisit(string code,string content)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return;
            }
            MLogin.GetExeUname();
            clientservice_VisitBill bills = new clientservice_VisitBill();
            List<clientservice_VisitBill> list = bills.Select(" and  code='"+ code + "'");
            if (list.Count > 0)
            {
                clientservice_VisitBill bill = list[0];
                bill.Reset_clientservice_VisitBill();
                bill.NOTEPRE = content;
               // bill.FINISHDATE= SysTime.GetTime;
                bill.Updata(" and code='"+ code + "' ");
                
            }
        }

        [WebMethod(EnableSession = true)]
        public List<VisitBill> P_HisVisit(string userName, int curPage,int pageSize)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;
            }
            MLogin.GetExeUname();
            int num = curPage * pageSize;
            string sql = "select top "+ pageSize.ToString()+ " o.* from (  "+
            "  select row_number() over(order by id desc) as rownumber,a.* from(  "+
            "  select id, client, billdate, notepre, code from clientservice_VisitBill where billman = '"+ userName + "') as a ) as o where rownumber> "+ num.ToString();
            DataTable dt = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            List<VisitBill> history = new List<VisitBill>();
            foreach (DataRow c in dt.Rows)
            {
                VisitBill b = new VisitBill();
                b.code = c["CODE"].ToString();
                b.client = c["CLIENT"].ToString();
                b.date = Convert.ToDateTime(c["BILLDATE"].ToString()).ToString("yyyy-MM-dd");
                b.content = c["NOTEPRE"].ToString();
                history.Add(b);
            }
            return history;
        }

        [WebMethod(EnableSession = true)]
        public int PushPoi(string userName , string name, string position)//修改为name  20180728 skl
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return -10;//身份验证失败
            }
            MLogin.GetExeUname();
            ClientService_kfku k = new ClientService_kfku();
            List<ClientService_kfku> ts = k.Select(" and name='" + name.Trim() + "' ");
            if (ts.Count > 0)
            {
                //注释 2018 10 12 原始定位需提交审核单。
                if (ts[0].ZF29 != null && ts[0].ZF29.Trim() != "")//当前客户有定位
                {
                    return -1;
                }
                else
                {
                    try
                    {
                        //事务处理开始,劫持链接通道的sql语句
                        TransactionSql.Start(MyGlobal.DataBase);  //===>开始
                        ClientService_kfku ke = new ClientService_kfku();
                        string[] po = position.Split(',');
                        ke.ZF29 = po[0];//经度
                        ke.ZF30 = po[1];//纬度
                        ke.ZF28 = userName;
                        ke.Updata(" and name='" + name + "' ");

                       // string sql_测试库 = " update makelanaln.dbo.clientservice_kfku set zf28 ='" + userName + "', " +
                       //" zf29 = '" + po[0] + "', zf30 = '" + po[1] + "'  where makelanaln.dbo.clientservice_kfku.NAME ='" + name + "' ";
                       // DBLL.ExecuteNonQuery(MyGlobal.DataBase, sql_测试库);

                        string sql_正式库 = " update sysanqi.dbo.clientservice_kfku set zf28 ='" + userName + "', " +
                        " zf29 = '" + po[0] + "', zf30 = '" + po[1] + "'  where sysanqi.dbo.clientservice_kfku.NAME ='" + name + "' ";
                        DBLL.ExecuteNonQuery(MyGlobal.DataBase, sql_正式库);
                        //提交事务到sql服务器处理
                        if (!TransactionSql.EndSql())
                        {
                            return -3;//回滚触发
                        }                   //===>结束
                        else
                        {
                            return 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        return -4;//程序出现错误
                    }
                }
            }
            else
            {
                return -2;
            }
        }

        [WebMethod(EnableSession = true)]
        public string PoiApply(string userName, string name, string position)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return "身份验证失败,请重新登录系统！";//身份验证失败
            }
            MLogin.GetExeUname();
            ClientService_kfku k = new ClientService_kfku();
            List<ClientService_kfku> ts = k.Select(" and name='" + name.Trim() + "' and zf29<>'' and zf30<>'' ");
            if (ts.Count > 0)
            {
                L_PushPoi p = new L_PushPoi();
                string sql = " select * from L_PushPoi a where not exists( " +
                 "  select 1 from L_PushPoi b where b.client = a.client and b.id > a.id) " +
                  "  and  client='" + name.Trim() + "' ";
                DataTable dt = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string 当前单据状态 = dt.Rows[0]["OPERATE_TYPE"].ToString();
                    string 当前审核人 = dt.Rows[0]["ZF1"].ToString();
                    if (当前单据状态 != "已提交")//最新申请已被查看
                    {
                        p.ZF1 = "总经办";
                        p.BILLMAN = userName;
                        p.OPERATE_TIME = SysTime.GetTime.ToString();
                        p.OPERATE_TYPE = "已提交";
                        p.CLIENT = name;
                        p.BEFORE_LNG = ts[0].ZF29;//原始定位
                        p.BEFORE_LAT = ts[0].ZF30;
                        string[] po = position.Split(',');
                        p.AFTER_LNG = po[0];//经度//现在请求定位
                        p.AFTER_LAT = po[1];//纬度
                        p.Insert();
                        return "已提交审核至总经办！";//已提交总经办审核
                    }
                    else//最新记录还在审核状态
                    {
                        return "定位还在审核，请勿重复提交！";//
                    }

                }
                else
                {
                    p.ZF1 = "销售内勤";
                    p.BILLMAN = userName;
                    p.OPERATE_TIME = SysTime.GetTime.ToString();
                    p.OPERATE_TYPE = "已提交";
                    p.CLIENT = name;
                    p.BEFORE_LNG = ts[0].ZF29;//原始定位
                    p.BEFORE_LAT = ts[0].ZF30;
                    string[] po = position.Split(',');
                    p.AFTER_LNG = po[0];//经度//现在请求定位
                    p.AFTER_LAT = po[1];//纬度
                    p.Insert();
                    return "已提交审核至销售内勤！";
                }
            }
            else
            {
                return "该客户尚未定位，无需申请修正！";//没有该用户
            }  
        }

        [WebMethod(EnableSession = true)]
        public List<LLBill> GetDBBill(string client, string userName, string dept)//获取待办事项
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            MLogin.GetExeUname();
            List<LLBill> result = new List<LLBill>();
            string where = "";
            if (dept.Contains("维修"))
            {
                where = " and (depar = '" + userName + "'  or  depar = '维修部' or depar = '智能化' )";
            }
            if (dept.Contains("业务")|| dept.Contains("销售") || dept.Contains("文员"))
            {
                where = " and (depar = '" + userName + "'  or  depar = '销售部' or depar = '智能化' )";
            }
            string sql = " select top 15 * from " +
                 "   (select * from L_ContactSheet as aa " +
                 "   left join " +
                 "   (select name, ZF29, ZF30,CONTACTOR,PHONE from clientservice_kfku ) as bb on aa.Client = bb.NAME " +
                 "   ) as cc" +
                 "   where  type=0  and del =0  and  Client='"+ client + "' "+where+"  order by finshdate   ";
            DataTable tb = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            foreach (DataRow row in tb.Rows)
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
                //n.distance = row["DISTANCE"].ToString()+"km"; //距离
                n.contactor = row["CONTACTOR"].ToString(); //联系人
                n.phone = row["PHONE"].ToString(); //电话
                n.lng = row["ZF29"].ToString(); //经度
                n.lat = row["ZF30"].ToString(); //纬度
                result.Add(n);
            }
            return result;
        }

        [WebMethod(EnableSession = true)]
        public kfContact GetContact(string client)
        {
            kfContact contact = new kfContact();
            List<Contact> result = new List<Contact>();
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;
            }
            MLogin.GetExeUname();
            string kf_code =string.Empty;
            string tablename = string.Empty;
            #region
            //string kf_tb = string.Empty;
            //kfku m = new kfku();
            //List<kfku> ms = m.Select(" and NAME='" + client + "'  order by id desc  ");
            //if (ms.Count > 0)
            //{
            //    kf_code = ms[0].CODE;
            //    //kf_tb = "kfku";
            //    tablename = "kfku_contacter";
            //}
            //else
            //{
            #endregion
            ClientService_kfku k = new ClientService_kfku();
            List<ClientService_kfku> ks = k.Select(" and NAME='" + client + "'   order by id desc   ");
            if (ks.Count > 0)
            {
                kf_code = ks[0].CODE;
                tablename = "clientservice_kfku_contacter";
            }
            if (kf_code == "" || tablename=="")//
            {
                return null;
            }
            else
            {
                string sql = " select * from  "+tablename +"  where 单位代码="+ kf_code.ToString();
                DataTable dt = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
                foreach (DataRow row in dt.Rows)
                {
                    Contact con = new Contact();
                    con.id= row["ID"].ToString();
                    con.name = row["姓名"].ToString();
                    con.phone = row["手机"].ToString();
                    con.position =row["职务"].ToString();
                    result.Add(con);
                }
                contact.list = result;
                contact.tablename = tablename;
                contact.kfcode = kf_code;
            }
            return contact;
        }

        [WebMethod(EnableSession = true)]
        public bool NewContact(string kfcode,string tablename,string name,string position,string phone)
        {
            bool flag = true;
            kfContact contact = new kfContact();
            List<Contact> result = new List<Contact>();
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                flag = false;
                return flag;
            }
            MLogin.GetExeUname();
            try
            {
                //if (tablename == "kfku_contacter")
                //{
                //    kfku_contacter k = new kfku_contacter();
                //    k.姓名 = name;
                //    k.单位代码 = kfcode;
                //    k.手机 = phone;
                //    k.职务 = position;
                //    k.Insert();
                //}
                //else
                //{
                clientservice_kfku_contacter m = new clientservice_kfku_contacter();
                    m.姓名 = name;
                    m.单位代码 = kfcode;
                    m.手机 = phone;
                    m.职务 = position;
                    m.Insert();
                //}
            }
            catch
            {
                //除非try里面执行代码发生了异常，否则这里的代码不会执行
                flag = false;
            }
            return flag;    
        }

    }
}
