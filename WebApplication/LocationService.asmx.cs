﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using makelanlan;
using ScueFun;
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

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string Uname()
        {
            puku p = new puku();
            List<puku> mp = p.Select(" and id=8");
            return mp[0].TURENAME.ToString();
        }

        [WebMethod]
        public string GetLocation(string userId,double x,double y)
        {
            MLogin.GetExeUname();
            scue_user u = new scue_user();
            DateTime datetime = SysTime.GetTime;
            string code = GetCode.SetCode(datetime);//日期段
            string time_now = datetime.ToString();
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
                        u.POINTS = x.ToString() + ":" + y.ToString() + ",";
                        u.Insert();
                    }
                    else
                    {
                        u.Reset_scue_user();
                        u.OPERATETIME = time_now;
                        u.POINTS = list[0].POINTS + x.ToString() + ":" + y.ToString() + ",";
                        u.Updata(" and USERPU='" + userId + "'  and CODE='" + code + "' and  DATE='" + today + "' ");
                    }
                }
            }
            
            return u.POINTS;
        }
       
        
        [WebMethod]
        public List<Point> ReadPoints(string userId,string code,string date)
        {
            //List<Point>
            MLogin.GetExeUname();
            scue_user u = new scue_user();
            List<scue_user> list = u.Select(" and USERPU='" + userId + "' and code='"+ code+"' and date='"+date+"'  ");
            List<Point> points = new List<Point>();
            if (list.Count > 0)
            {
                string str = list[0].POINTS;
                string[] strs = str.Split(',');
                for (int i = 0; i < strs.Length-1 ; i++)
                {
                    string strx = strs[i].Split(':')[0];
                    string stry = strs[i].Split(':')[1];
                    Point point = new Point
                    {
                        x = strx,
                        y = stry
                    };
                    points.Add(point);
                }
                return points;
                //foreach (string s in strs)
                //{

                //    
                //}
                //
            }
            return null;
           // return points;
        }
    }
}
