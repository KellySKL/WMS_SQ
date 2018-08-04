﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using makelanlan;

namespace WebApplication
{
    /// <summary>
    /// WebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        

        [WebMethod]
        public string Hell()
        {
            return UserInfo.UName;
        }
        [WebMethod]
        public void PushPoi(string name, string position)//修改为name  20180728 skl
        {
            ClientService_kfku k = new ClientService_kfku();
            k.ZF29 = position;
            k.Updata(" and name='" + name+"' ");
            kfku p = new kfku();
            p.ZF29 = position;
            p.Updata(" and name='" + name + "' ");
        }
        [WebMethod]
        public FindLoc GetPep(string name, string type)//修改为name   20180728 skl
        {
            MLogin.GetExeUname();
            string position = "";
            string lng;//经度
            string lat;//纬度
            string datetime = SysTime.GetTime.ToString("yyyyMMdd");
            FindLoc findLoc = new FindLoc();
            findLoc.status = 0;//标记状态 0：未获取到客户位置  10：获取到客户位置
            List<PersonLOC> locs = new List<PersonLOC>();
            if (KFLocation.Get(name)!="")
            {
                findLoc.status = 10;
                position = KFLocation.Get(name);
                string[] strs_kf = position.Split(',');
                lng = strs_kf[0];
                lat = strs_kf[1];
                //只取当天人的最新记录
                string sql = "select * from scue_user a where not exists(select 1 from scue_user b where b.userpu=a.userpu and b.id>a.id) and a.date ='" + datetime + "' ";
                List<scue_user> scue_s = BLL.SqlToList<scue_user>(new BLL.数据库(), sql);
                 
                foreach (scue_user s in scue_s)
                {
                    string[] strs = s.POINTS.Split(',');
                    int count = strs.Length - 2;
                    string newpoi = strs[count];
                    string[] poi = newpoi.Split(':');
                    PersonLOC o = new PersonLOC();
                    o.userId = s.USERPU;
                    o.userName = s.TURENAME;
                    o.lng = Convert.ToDouble(poi[0]);
                    o.lat = Convert.ToDouble(poi[1]);
                    o.lastTime = s.OPERATETIME.ToString();
                    locs.Add(o);
                }
                findLoc.kflng = Convert.ToDouble(lng);
                findLoc.kflat = Convert.ToDouble(lat);
                findLoc.list = locs;
            }
            return findLoc;
            //if (type.Trim() == "YW")
            //{

            //}
            //else if (type.Trim() == "YW")
            //{

            //}
            //else
            //{

            //}
        }

        public class FindLoc
        {
            private double _kflng;
            private double _kflat;
            private int _status;
            private List<PersonLOC> _list;
            public List<PersonLOC> list { get { return _list; } set { _list = value; } }
            public double kflng { get { return _kflng; } set { _kflng = value; } }
            public double kflat { get { return _kflat; } set { _kflat = value; } }
            public int status { get { return _status; } set { _status = value; } }
        }

        public class PersonLOC
        {
            private string _userId;
            private string _userName;
            private string _lastTime;
            private double _lng;
            private double _lat;
            public string userId { get { return _userId; } set { _userId = value; } }
            public string userName { get { return _userName; } set { _userName = value; } }
            public string lastTime { get { return _lastTime; } set { _lastTime = value; } }
            public double lng { get { return _lng; } set { _lng = value; } }
            public double lat { get { return _lat; } set { _lat = value; } }
        }
    }
}
