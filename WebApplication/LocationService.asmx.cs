using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using makelanlan;
using ScueFun;
using System.Data;
namespace WebApplication
{
    /// <summary>
    /// LocationService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class LocationService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        public string Uname()
        {
            puku p = new puku();
            List<puku> mp = p.Select(" and id=8");
            return mp[0].TURENAME.ToString();
        }


        /// <summary>
        /// 存取用户定位
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string GetLocation(string userId,double x, double y)
        {
            scue_user u = new scue_user();
            DateTime datetime = SysTime.GetTime;
            string code = GetCode.SetCode(datetime);//日期段
            string time_now = datetime.ToString();
            string poi_time = datetime.ToLongTimeString().ToString();//该点时间 ，20:16:15
            string today = datetime.ToString("yyyyMMdd");

            List<puku_user> pus = GetPUser.PUser(" and USERPU='" + userId + "'  ");
            string username;
            if (pus.Count > 0)
            {
                username=pus[0].TURENAME;
                if (code != "")//工作时间段
                {
                    List<scue_user> list = u.Select(" and USERPU='" + userId + "' and CODE='" + code + "' and  DATE='" + today + "' ");

                    if (list.Count <= 0)
                    {
                        u.CREATEDATE = time_now;//创建时间
                        u.OPERATETIME = time_now;//
                        u.DATE = today;//日期
                        u.CODE = code;//时间段
                        u.USERPU = userId;//
                        u.TURENAME = username;
                        u.POINTS = x.ToString() + ":" + y.ToString() +":"+ poi_time+ ",";
                        u.Insert();
                    }
                    else
                    {
                        u.Reset_scue_user();
                        u.OPERATETIME = time_now;
                        u.POINTS = list[0].POINTS + x.ToString() + ":" + y.ToString() + ":" + poi_time + ",";
                        u.Updata(" and USERPU='" + userId + "'  and CODE='" + code + "' and  DATE='" + today + "' ");
                    }
                }
            }
            
            return u.POINTS;
        }


        /// <summary>
        /// 读取用户某天某时间段的定位点
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public List<Point> ReadPoints(string findId,string userName,string date,string start,string end)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;//身份验证失败
            }
            //List<Point>
            MLogin.GetExeUname();
            string str_date = System.Text.RegularExpressions.Regex.Replace(date, "-", "");
            scue_user u = new scue_user();
            List<scue_user> list = u.Select(" and USERPU='" + findId + "' and code between '"+ start + "' and  '" + end + "' and date='" + str_date + "'   order by id ");
            List<Point> points = new List<Point>();
            if (list.Count > 0)
            {
                foreach (scue_user user in list)
                {
                    string str = user.POINTS;
                    string[] strs = str.Split(',');
                    for (int i = 0; i < strs.Length - 1; i++)
                    {
                        string[] p = strs[i].Split(':');
                        string strx = p[0];
                        string stry = p[1];
                        Point point = new Point
                        {
                            lng = Convert.ToDouble(strx),
                            lat = Convert.ToDouble(stry),
                            time = p[2] + ":" + p[3] + ":" + p[4]
                        };
                        points.Add(point);
                    }
                }
                return points;

            }
            else
            {
                return null;
            }
           // return points;
        }

        [WebMethod(EnableSession = true)]
        public List<Kfkf> Neighbour(string type,double lng, double lat,string userName)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return null;
            }
            MLogin.GetExeUname();
            puku_user p = new puku_user();
            List<puku_user> list = GetPUser.PUser(" and TURENAME='" + userName + "'  ");
            string dept = "";
            if (list.Count <= 0)
            {
                dept = "业务员";
            }
            else
            {
                dept = list[0].DEPT;
            }
            DataTable dt = Function.NeighbourPosition(dept,lng, lat, type,userName);
            List<Kfkf> ks = new List<Kfkf>();
            foreach (DataRow r in dt.Rows)
            {
                Kfkf k = new Kfkf();
                k.name= r["NAME"].ToString();
                k.type= r["TRADETYPENAME"].ToString(); 
                k.contactor= r["CONTACTOR"].ToString();
                k.phone = r["PHONE"].ToString();
                k.lng = Convert.ToDouble(r["ZF29"].ToString());
                k.lat = Convert.ToDouble(r["ZF30"].ToString());
                k.lastVisit = r["INTRODUCERDATE"].ToString();
                k.billman= r["BILLMAN"].ToString();
                k.remark = r["NotePre"].ToString();
                ks.Add(k);
            }
            return ks;
        }

        /// <summary>
        /// 计算时间间隔
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        private string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            //dateDiff = ts.Days.ToString() + "天" +
            //      ts.Hours.ToString() + "小时" +
            //      ts.Minutes.ToString() + "分钟" +
            //      ts.Seconds.ToString() + "秒";
            dateDiff = ts.Hours.ToString() + "小时" +
                 ts.Minutes.ToString() + "分钟" +
                 ts.Seconds.ToString() + "秒";

            return dateDiff;

        }

        public string[] StayTime(string userId,double lng,double lat,string client)
        {
            string  kf_poi = KFLocation.Get(client);

            if (kf_poi != "" && kf_poi != "none")
            {
                string[] strs = kf_poi.Split(',');
                double lng1 = Convert.ToDouble(strs[0]);
                double lat1 = Convert.ToDouble(strs[1]);
                string start = string.Empty;
                string end = string.Empty;
                scue_user su = new scue_user();
                List<scue_user> list = su.Select("  and USERPU='" + userId + "' and date ='" + DateTime.Now.ToString("yyyyMMdd") + "'  order by id desc ", " top 3 * ");
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)//从最新记录开始
                    {
                        List<string> time = Function.SearchST(list[i].POINTS.Split(','), lng1, lat1, MyGlobal.STAY_AREA);
                        if (time == null)//这一组没有 不必继续，break'
                        {
                            break;
                        }
                        else
                        {
                            if (time[0] != "start")//开始值有效  开始值取小，需要不断更新
                            {
                                start = time[0];
                            }
                            if (time[1] != "end" && end == "")//结束值有效 结束值取大，第一次即为最大
                            {
                                end = time[1];
                            }
                        }
                    }
                    //计算时间差
                    if (start != "" && end != "")
                    {
                        string date = DateTime.Now.ToString("yyyy-MM-dd ");
                        DateTime a = Convert.ToDateTime(date + start);
                        DateTime b = Convert.ToDateTime(date + end);
                        string stay = DateDiff(a, b);
                        string[] vs = {a.ToString(),b.ToString(),stay };
                        return vs;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [WebMethod(EnableSession = true)]
        public int KFNew(string userName, string client, string type, string lev, double lng, double lat)
        {
            string cookies = HttpContext.Current.Request.Cookies["guid"] == null ? "" : HttpContext.Current.Request.Cookies["guid"].Value as string;
            string session = Session.SessionID.ToString();
            if (!cookies.Equals(session))
            {
                return -5;//身份验证失败
            }
            MLogin.GetExeUname();
            string position = lng.ToString() + ',' + lat.ToString();
            ClientService_kfku newkf = new ClientService_kfku();
            if (KFLocation.Get(client) == "none")
            {
                newkf.NAME = client;
                newkf.TRADETYPENAME = type;
                newkf.创建人 = userName;
                // newkf.CONTACTOR = hisname;//联系人对方姓名
                // newkf.PHONE = hisphone;//联系人对方手机
                newkf.ZF29 = lng.ToString();
                newkf.ZF30 = lat.ToString();
                newkf.ZF34 = "PHONE";//手机新建客户标识
                if (lev.Contains("1级"))
                {
                    lev = "1级";
                    newkf.回访间隔 = "150";
                }
                else if (lev.Contains("2级"))
                {
                    lev = "2级";
                    newkf.回访间隔 = "100";
                }
                else if (lev.Contains("3级"))
                {
                    lev = "3级";
                    newkf.回访间隔 = "60";
                }
                else if (lev.Contains("4级"))
                {
                    lev = "4级";
                    newkf.回访间隔 = "30";
                }
                else if (lev.Contains("5级"))
                {
                    lev = "5级";
                    newkf.回访间隔 = "7";
                }
                else { }
               // newkf.RQ1 = System.DateTime.Now;//创建时间
                newkf.CODE = Function.CalKFcode();
                newkf.LEV = lev;
                newkf.Insert();
                return 0;//成功
            }
            else {
                return -1;//数据库内有该客户
            }
            
        }

    }
}
