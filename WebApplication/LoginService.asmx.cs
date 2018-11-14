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
    /// LoginService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class LoginService : Identity
    {
        [WebMethod(EnableSession = true)]
        public bool WS3(string userId, string password, string uuid)
        {
            //----------------------
            //lxdlxd
            MLogin.GetExeUname();
            User NowUser = new User();
            List<puku_user> mp = GetPUser.PUser(" and USERPU='" + userId.Trim() + "' ");
            if (mp.Count > 0)
            {
                if (BitLock.RealseLock_L(mp[0].MM) == password && uuid == mp[0].UUID)
                {

                    string my = string.Empty;
                    HttpContext.Current.Session["user"] = userId;
                    // my = "没有cookies";
                    my = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
                    if (my.Trim() != "")
                    {
                        string cookies = HttpContext.Current.Request.Cookies["guid"].Value as string;
                        string session = Session.SessionID.ToString();
                        if (!session.Equals(cookies))//有cookies 但是已过期
                        {
                            HttpCookie objCookie = new HttpCookie("guid", Session.SessionID.ToString());
                            objCookie.Expires = DateTime.MaxValue;
                            System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                        }
                        my = System.Web.HttpContext.Current.Request.Cookies["guid"].Value;
                    }
                    else//没有cookies 增加cookies
                    {
                        HttpCookie objCookie = new HttpCookie("guid", Session.SessionID.ToString());
                        objCookie.Expires = DateTime.MaxValue;
                        System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                    }
                    NowUser.name = mp[0].TURENAME;
                    NowUser.dept = mp[0].DEPT;
                    NowUser.authorised = mp[0].允许管理物料BY仓库名称;
                    NowUser.status = 10;
                    return true;
                    //var headers = Context.Request.Headers.GetValues("My-Header");
                }
                else
                {
                    NowUser.status = -1;
                    return false;
                }
            }
            else
            {
                NowUser.status = -1;
                return false;
            }
        }
        [WebMethod(EnableSession = true)]
        public User WS3_P(string userId, string password, string uuid)
        {
            //----------------------
            //lxdlxd
            MLogin.GetExeUname();
            User NowUser = new User();
            List<puku_user> mp = GetPUser.PUser(" and USERPU='" + userId.Trim() + "' ");
            if (mp.Count > 0)
            {
                if (BitLock.RealseLock_L(mp[0].MM) == password&& uuid== mp[0].UUID)
                {
                  
                    string my = string.Empty;
                    HttpContext.Current.Session["user"] = userId;
                   // my = "没有cookies";
                    my = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
                    if (my!="")
                    {
                        string cookies = HttpContext.Current.Request.Cookies["guid"].Value as string;
                        string session = Session.SessionID.ToString();
                        if (!session.Equals(cookies))//有cookies 但是已过期
                        {
                            HttpCookie objCookie = new HttpCookie("guid", Session.SessionID.ToString());
                            objCookie.Expires = DateTime.MaxValue;
                            System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                        }
                        my = System.Web.HttpContext.Current.Request.Cookies["guid"].Value;
                    }
                    else//没有cookies 增加cookies
                    {
                        HttpCookie objCookie = new HttpCookie("guid", Session.SessionID.ToString());
                        objCookie.Expires = DateTime.MaxValue;
                        System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                    }
                    NowUser.userId = userId;

                    NowUser.name = mp[0].TURENAME;
                    NowUser.dept = mp[0].DEPT;
                    NowUser.authorised = mp[0].允许管理物料BY仓库名称;
                    NowUser.status = 10;
                    return NowUser;
                    //var headers = Context.Request.Headers.GetValues("My-Header");
                }
                else
                {
                    NowUser.status = -1;
                    return NowUser;
                }
            }
            else
            {
                NowUser.status = -1;
                return NowUser;
            }
        }

        [WebMethod(EnableSession = true)]
        public List<KfkfCode> KFKF(string userId,string searchName)
        {
            //读取客户  用于客户搜索框
            List<KfkfCode> ls = new List<KfkfCode>();
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                KfkfCode C = new KfkfCode();
                C.NAME = "session:" + session;
                ls.Add(C);
                KfkfCode C2 = new KfkfCode();
                C2.NAME = "cookies:" + cookies;
                ls.Add(C2);
                return ls;
            }
            MLogin.GetExeUname();
            string sql = "  select top 30 NAME,CONTACTOR,PHONE from ClientService_kfku where id in (select max(id) from ClientService_kfku group by name)   " +
                "  group by  NAME,CONTACTOR,PHONE  " +
                " HAVING NAME like '%" + searchName + "%' or CONTACTOR like '%" + searchName + "%' or  PHONE like '%" + searchName + "%' ";
            DataTable table = (DataTable)BLL.SqltoView(MyGlobal.DataBase, BLL.数据类型.Table, sql);
            foreach (DataRow r in table.Rows)
            {
                KfkfCode C = new KfkfCode();
                C.NAME = r["NAME"].ToString();
                ls.Add(C);
            }
            return ls;
        }
        
        [WebMethod(EnableSession = true)]
        public User WS2(string userId, string password,string uuid)
        {
            MLogin.GetExeUname();
            User u = new User();
            u.status = -1;
            u.userId = userId.Trim();
            puku_user p = new puku_user();
            List<puku_user> mp = p.Select(" and USERPU='" + userId.Trim() + "'  and isstop <> '是' ");
            if (mp.Count > 0)
            {
                if (BitLock.RealseLock_L(mp[0].MM) == password)
                {
                    string sql_uuid = mp[0].UUID==null? "":mp[0].UUID;
                    if (uuid != "web" && sql_uuid == "")
                    {
                        puku_user _User = new puku_user();
                        _User.UUID = uuid;
                        _User.Updata(" and USERPU='" + userId.Trim() + "' and isstop <> '是' and (uuid is null or uuid ='' )");
                    }
                    //UserInfo.UName = mp[0].TURENAME;
                    UserInfo.Uzw = "管理员";
                    u.name = mp[0].TURENAME;
                    u.dept = mp[0].DEPT;
                    u.authorised = mp[0].允许管理物料BY仓库名称;
                    u.status = 10;
                    string my = string.Empty;
                    HttpContext.Current.Session["user"] = userId;
                    my = "没有cookies";
                    //my = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;

                    if (System.Web.HttpContext.Current.Request.Cookies["guid"] != null&& System.Web.HttpContext.Current.Request.Cookies["guid"].ToString().Trim()!="")
                    {
                        string cookies = HttpContext.Current.Request.Cookies["guid"].Value as string;
                        string session = Session.SessionID.ToString();
                        if (!session.Equals(cookies))
                        {
                            HttpCookie objCookie = new HttpCookie("guid", Session.SessionID.ToString());
                            objCookie.Expires = DateTime.MaxValue;
                            System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                        }
                        my = System.Web.HttpContext.Current.Request.Cookies["guid"].Value;
                    }
                    else
                    {
                        HttpCookie objCookie = new HttpCookie("guid", Session.SessionID.ToString());
                        objCookie.Expires = DateTime.MaxValue;
                        System.Web.HttpContext.Current.Response.Cookies.Add(objCookie);
                    }
                  
                    u.session = Session.SessionID.ToString();
                    u.cookies= my;
                    //var headers = Context.Request.Headers.GetValues("My-Header");
                }
                else
                {
                    u.status = 5;//密码错误
                }
            }
            else
            {
                u.status = 0;//不存在该用户
            }
            return u;
        }

        [WebMethod(EnableSession = true)]
        public bool AppVersion(string curVersion)
        {
            return  SFun.CheckUpdate(Server.MapPath("ScuePlat.xml"), curVersion);
        }
    }
}
