﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using makelanlan;
using System.Xml;
namespace WebApplication
{
    public class Function
    {

        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
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
        public static List<double> Which_Sign(DateTime time)
        {
            List<double> list = new List<double>();
            DateTime now = System.DateTime.Now;
            DateTime time1 = Convert.ToDateTime(now.ToString("yyyy-MM-dd 08:00:00"));
            DateTime time2 = Convert.ToDateTime(now.ToString("yyyy-MM-dd 12:00:00"));
            DateTime time3 = Convert.ToDateTime(now.ToString("yyyy-MM-dd 13:00:00"));
            DateTime time4 = Convert.ToDateTime(now.ToString("yyyy-MM-dd 17:00:00"));
            TimeSpan timeSpan1 = time1 - time;
            TimeSpan timeSpan2 = time2 - time;
            TimeSpan timeSpan3 = time3 - time;
            TimeSpan timeSpan4 = time4 - time;
            list.Add(timeSpan1.TotalMinutes);
            list.Add(timeSpan2.TotalMinutes);
            list.Add(timeSpan3.TotalMinutes);
            list.Add(timeSpan4.TotalMinutes);
            return list;
        }

        public static double DateDiff_Min(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan timeSpan = DateTime2 - DateTime1;
            return timeSpan.TotalMinutes;
        }

        public static string[] StayTime(string userId, double lng, double lat, string client)
        {
            string kf_poi = KFLocation.Get(client);

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
                        string[] vs = { a.ToString(), b.ToString(), stay };
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

        /// <summary>
        /// 二分法查找
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="key">要查找的对象</param>
        public static int BinarySearchArr(int[] arr, int value)
        {
            int low = 0;
            int high = arr.Length - 1;
            while (low <= high)
            {
                int middle = (low + high) / 2;
                if (value == arr[middle])
                {
                    return middle;//如果找到了就直接返回这个元素的索引
                }
                else if (value > arr[middle])
                {
                    low = middle + 1;
                }
                else
                {
                    high = middle - 1;
                }
            }
            return -1;//如果找不到就返回-1；
        }
        /// <summary>
        /// 获得停留时间
        /// </summary>
        /// <param name="pois">位置串</param>
        /// <param name="lng">客户经度</param>
        /// <param name="lat">客户纬度</param>
        /// <param name="area">合理范围 ： 米</param>
        /// <returns>返回start end  null  </returns>
        public static List<string> SearchST(string[] pois,double lng,double lat,double area)
        {
            List<string> times=new List<string> ();
            times.Add("start");
            times.Add("end");
            int flag = -1;
            for (int i = 0; i < pois.Length - 1; i++)
            {
                string[] p = pois[i].Split(':');
                double p_lng = Convert.ToDouble(p[0]);
                double p_lat = Convert.ToDouble(p[1]);
                double dis = ScueFun.LngLatDis.GetDistance(lng, lat, p_lng, p_lat);
                if (dis <= area)
                {
                    times[0]=p[2]+":"+ p[3] + ":"+p[4];//开始
                    flag = i;
                    break;
                }
            }
            if (flag == -1)
            {
                return null;
            }
            else if (flag == pois.Length - 2)
            {
                return times;
            }
            else
            {
                for (int i = pois.Length - 2; i >=0 ; i--)
                {
                    string[] p = pois[i].Split(':');
                    double p_lng = Convert.ToDouble(p[0]);
                    double p_lat = Convert.ToDouble(p[1]);
                    double dis = ScueFun.LngLatDis.GetDistance(lng, lat, p_lng, p_lat);
                    if (dis <= area)
                    {
                        times[1]= p[2] + ":" + p[3] + ":" + p[4];//结束
                        break;
                    }
                }
                return times;
            }   
        }

        /// <summary>
        /// 获取客户code，Code= CalKFcode()
        /// </summary>
        /// <returns></returns>
        public static string CalKFcode()
        {
            string maxV = string.Empty;
            string sql = "select code from ClientService_kfku where id = " +
                " (select max(id) from ClientService_kfku where code <> '' ) ";
            DataTable tb = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            if (tb.Rows.Count <= 0)
            {
                maxV = "1";
            }
            else
            {
                maxV = (Convert.ToInt32(tb.Rows[0]["code"].ToString()) + 1).ToString();
            }
            return maxV;
        }
        /// <summary>
        ///
        /// 附近客户(全局变量范围内可见) | 最近客户（radius半径缩小即可）
        /// type  1:访问时间越久远的越靠前 搜索范围由全局变量DIS而定
        ///       2:距离越近越靠前  默认搜索范围由全局变量RADIUS而定，可调整radius
        ///       默认为 1
        /// </summary>
        /// <param name="longitude">当前位置 经度</param>
        /// <param name="latitude">当前位置 纬度</param>
        /// <param name="where">选择类型</param>
        /// <param name="type">type 排序类型默认是第一种</param>
        /// <param name="radius">搜索范围 单位：千米（大于0），默认就是全局变量,</param>
        /// <returns></returns>
        public static DataTable NeighbourPosition(string dept , double longitude, double latitude, string where,string userName,int type=1,double radius=-1)
        {
            //先计算查询点的经纬度范围
            double r = 6371;//地球半径千米
            double dis;
            if (type==1)
            {
                dis = MyGlobal.DIS;
            }
            else
            {
                if (radius<=0)
                {
                    dis = MyGlobal.RADIUS;
                }
                else
                {
                    dis = radius;
                }
            }
            double dlng = 2 * Math.Asin(Math.Sin(dis / (2 * r)) / Math.Cos(latitude * Math.PI / 180));
            dlng = dlng * 180 / Math.PI;//角度转为弧度
            double dlat = dis / r;
            dlat = dlat * 180 / Math.PI;
            double minlat = latitude - dlat;
            double maxlat = latitude + dlat;
            double minlng = longitude - dlng;
            double maxlng = longitude + dlng;
            string sql = string.Empty;
            string typename = string.Empty;
            if (where == "in")//公司客户
            {
                typename = "  and TRADETYPENAME='公司客户' ";
            }
            else if (where == "out")//潜在客户-不在kf表
            {
                typename = "  and TRADETYPENAME <> '公司客户' ";
            }
            else
            {
                typename = " ";
            }
            sql = " select top " + MyGlobal.TOPCOUNT + " name,contactor,phone,zf29,zf30,TRADETYPENAME,introducerDate,BillMan,NotePre from ( " +
                " select cc.*,dd.introducerDate,dd.BillMan,dd.NotePre,LEV-DATEDIFF(DAY, dd.introducerDate,GETDATE()) AS REDUCE from( " +
                "  select name,contactor,phone,zf29,zf30,TRADETYPENAME, " +
                "  CASE LEV  WHEN '1级' THEN 150  WHEN '2级' THEN 100  WHEN '3级' THEN 60 WHEN '4级' THEN 30  WHEN '5级' THEN 7  ELSE '' END LEV  " +
                "  from ClientService_kfku where zf29 between '" + minlng.ToString() + "'  and '" + maxlng.ToString() + "'  " +
                "  and  zf30 between '" + minlat.ToString() + "'  and '" + maxlat.ToString() + "' " +
                "  and  LEV <> ''  " + typename+
                "  group by  name,contactor,phone,zf29,zf30,TRADETYPENAME,LEV  " +
                " ) as cc " +
                " left join" +
                " (select * from clientservice_VisitBill a where not exists(select 1 from clientservice_VisitBill b where b.Client = a.Client and b.id > a.id) )" +
                " as dd" +
                " on dd.Client = cc.NAME " +
                " )as ff where REDUCE<0 order by REDUCE,lev asc ";
            //if (type == 1)
            //{
            //    sql += " order by introducerDate ";//相隔时间久的排名靠前
            //}
            DataTable tb = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            string task_Sql = "";
            if (dept == "维修员")
            {
                task_Sql = "  select  client from clientservice_AfterServiceBill  " +
                "  where BILLMAN = '" + userName + "' and zf3<> ''   and left(code,1) <> '*'   " +
                "  and((zf2 = ''  and  sz10 = 0) or(zf2 <> '" + userName + "')) or   " +
                "  (zf2 = '" + userName + "' and  id  in (select zf1 from L_AfterRepair a   " +
                "  where not exists(select 1 from L_AfterRepair b where b.refercode = a.refercode and b.id > a.id)   " +
                "  and OPERATE_TYPE = '接受' and BILLMAN = '" + userName + "' ))   ";
            }
            else
            {
                task_Sql = " select VisitClient from L_VisitSend WHERE State != 10 AND SendUser = '" + userName + "' ";
            }

            string sql_send = " select top 15 cc.*,dd.introducerDate,dd.BillMan,dd.NotePre from(    " +
                "  select name, contactor, phone, zf29, zf30, TRADETYPENAME" +
                "  from ClientService_kfku where NAME in ( "+ task_Sql + " )  " +
                "  and  zf29 between '" + minlng.ToString() + "'  and '" + maxlng.ToString() + "'  " +
                "  and  zf30 between '" + minlat.ToString() + "'  and '" + maxlat.ToString() + "'  " + typename +
                "  group by  name,contactor,phone,zf29,zf30,TRADETYPENAME" +
                " ) as cc" +
                " left join" +
                " (select * from clientservice_VisitBill a where not exists(select 1 from clientservice_VisitBill b where b.Client = a.Client and b.id > a.id) )   " +
                " as dd" +
                " on dd.Client = cc.NAME  order by RQ1 asc  ";

            DataTable tb2 = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql_send);
            tb.Merge(tb2);
            ////多个表合并可能产生重复数据，过滤掉重复数据
            DataView dv = new DataView(tb);
            tb = dv.ToTable(true, new[] { "NAME", "CONTACTOR", "PHONE", "ZF29", "ZF30", "TRADETYPENAME", "INTRODUCERDATE", "BILLMAN", "NOTEPRE" });

            if (type == 2)
            {
                int len = tb.Rows.Count;
                int i = 0, j = 0;
                for (i = 0; i < len; i++)
                {
                    for (j = 0; j < len - i - 1; j++)
                    {
                        double disj = ScueFun.LngLatDis.GetDistance(longitude, latitude, Convert.ToDouble(tb.Rows[j]["ZF29"].ToString()), Convert.ToDouble(tb.Rows[j]["ZF30"].ToString()));
                        double disj_1 = ScueFun.LngLatDis.GetDistance(longitude, latitude, Convert.ToDouble(tb.Rows[j + 1]["ZF29"].ToString()), Convert.ToDouble(tb.Rows[j + 1]["ZF30"].ToString()));
                        if (disj > disj_1)
                        {
                            DataRow dr = tb.NewRow();
                            dr.ItemArray = tb.Rows[j].ItemArray;//暂存
                            tb.Rows[j].ItemArray = tb.Rows[j + 1].ItemArray;
                            tb.Rows[j + 1].ItemArray = dr.ItemArray;
                        }
                    }
                }
            }    
            return tb;
        }
        /// <summary>
        /// 附近的打卡点
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="where"></param>
        /// <param name="userName"></param>
        /// <param name="type"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static DataTable NeighbourCpn(double longitude, double latitude)
        {
            //先计算查询点的经纬度范围
            double r = 6371;//地球半径千米
            double dis= MyGlobal.AREA_COMPANY;
            double dlng = 2 * Math.Asin(Math.Sin(dis / (2 * r)) / Math.Cos(latitude * Math.PI / 180));
            dlng = dlng * 180 / Math.PI;//角度转为弧度
            double dlat = dis / r;
            dlat = dlat * 180 / Math.PI;
            double minlat = latitude - dlat;
            double maxlat = latitude + dlat;
            double minlng = longitude - dlng;
            double maxlng = longitude + dlng;
            string sql = string.Empty;
            string typename = string.Empty;
            sql ="  select * "+
                "  from sanqi_kfku where zf29 between '" + minlng.ToString() + "'  and '" + maxlng.ToString() + "'  " +
                "  and  zf30 between '" + minlat.ToString() + "'  and '" + maxlat.ToString() + "' ";
            DataTable tb = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            int len = tb.Rows.Count;
            tb.Columns.Add("DISTANCE", typeof(Double));
            for (int m = 0; m < len; m++)
            {
                double distance = ScueFun.LngLatDis.GetDistance(longitude, latitude, Convert.ToDouble(tb.Rows[m]["ZF29"].ToString()), Convert.ToDouble(tb.Rows[m]["ZF30"].ToString()));
                tb.Rows[m]["DISTANCE"] = distance;
            }
            int i = 0, j = 0;
            for (i = 0; i < len; i++)
            {
                for (j = 0; j < len - i - 1; j++)
                {
                    double disj = Convert.ToDouble(tb.Rows[j]["DISTANCE"].ToString());
                    double disj_1 = Convert.ToDouble(tb.Rows[j + 1]["DISTANCE"].ToString());
                    if (disj > disj_1)// 前 > 后 则交换，即从小到大排序
                    {
                        DataRow dr = tb.NewRow();
                        dr.ItemArray = tb.Rows[j].ItemArray;//暂存
                        tb.Rows[j].ItemArray = tb.Rows[j + 1].ItemArray;
                        tb.Rows[j + 1].ItemArray = dr.ItemArray;
                    }
                }
            }
            return tb;
        }

        public static void setUserName()
        {
            //<? xml version = "1.0" encoding = "gb2312" ?>
            //< AppConfig >
            //< config nameSeq="CheckVersionURL" values ="http://121.43.103.228/android-debug.apk" />
            //< config nameSeq = "2" values = "username2" />
            //</ AppConfig >
            XmlDocument xmlDoc = new XmlDocument();//声明读取xml的对象
            string XML_FILE_DIR = "E:/WebService/SPlat.xml";
            xmlDoc.Load(XML_FILE_DIR);//获取xml文件，XML_FILE_DIR为xml文件的存放路径，我在这里定义的是常量。
            XmlNode root = xmlDoc.SelectSingleNode("AppConfig");
            XmlNode xn = xmlDoc.SelectSingleNode("config");
            XmlElement xe1 = xmlDoc.CreateElement("config");//创建一个<userID>节点
            xe1.SetAttribute("CheckVersionURL", "CheckVersionURL"); //给节点的nameSeq赋值
            xe1.SetAttribute("values", "http://121.43.103.228/android-debug.apk");                                //给节点的values赋值
            root.AppendChild(xe1);//添加节点
            xmlDoc.Save(XML_FILE_DIR);//保存
        }

        /// <summary>
        /// 附近的待办事项，距离为：米
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="where"></param>
        /// <param name="userName"></param>
        /// <param name="type"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static DataTable NeighbourBill(int curPage,int pageSize, double longitude, double latitude)
        {
            //先计算查询点的经纬度范围
            double r = 6371;//地球半径千米
            double dis = MyGlobal.DIS;
            double dlng = 2 * Math.Asin(Math.Sin(dis / (2 * r)) / Math.Cos(latitude * Math.PI / 180));
            dlng = dlng * 180 / Math.PI;//角度转为弧度
            double dlat = dis / r;
            dlat = dlat * 180 / Math.PI;
            double minlat = latitude - dlat;
            double maxlat = latitude + dlat;
            double minlng = longitude - dlng;
            double maxlng = longitude + dlng;
            string sql = string.Empty;
            string typename = string.Empty;
            int num = curPage * pageSize;
            sql = "  select top " + pageSize .ToString()+ "  * from (  " +
                    " select row_number() over(order by finshdate) as rownumber, a.* from( " +
                    " select * from " +
                    " (select * from L_ContactSheet as aa " +
                    " left join " +
                    " (select name, ZF29, ZF30, CONTACTOR, PHONE from clientservice_kfku ) as bb on aa.Client = bb.NAME " +
                    " ) as cc " +
                    "   where  type=0 and  del =0  " +
                    "  and  zf29 between '" + minlng.ToString() + "'  and '" + maxlng.ToString() + "'  " +
                    "  and  zf30 between '" + minlat.ToString() + "'  and '" + maxlat.ToString() + "'   " +
                    " ) as a) as o where rownumber > "+num.ToString();
            //sql = " select * from " +
            //     "   (select * from L_ContactSheet as aa " +
            //     "   left join " +
            //     "   (select name, ZF29, ZF30,CONTACTOR,PHONE from clientservice_kfku ) as bb on aa.Client = bb.NAME " +
            //     "   ) as cc" +
            //     "   where  type=0 and  zf29 between '" + minlng.ToString() + "'  and '" + maxlng.ToString() + "'  " +
            //     "  and  zf30 between '" + minlat.ToString() + "'  and '" + maxlat.ToString() + "'  order by finshdate   ";

            DataTable tb = DBLL.ExecuteDataTable(MyGlobal.DataBase, sql);
            int len = tb.Rows.Count;
            tb.Columns.Add("DISTANCE", typeof(Double));
            for (int m = 0; m < len; m++)
            {
                double distance = ScueFun.LngLatDis.GetDistance(longitude, latitude, Convert.ToDouble(tb.Rows[m]["ZF29"].ToString()), Convert.ToDouble(tb.Rows[m]["ZF30"].ToString()));
                tb.Rows[m]["DISTANCE"] = Math.Round(distance / 1000, 2);

            }
            int i = 0, j = 0;
            for (i = 0; i < len; i++)
            {
                for (j = 0; j < len - i - 1; j++)
                {
                    double disj = Convert.ToDouble(tb.Rows[j]["DISTANCE"].ToString());
                    double disj_1 = Convert.ToDouble(tb.Rows[j + 1]["DISTANCE"].ToString());
                    if (disj > disj_1)// 前 > 后 则交换，即从小到大排序
                    {
                        DataRow dr = tb.NewRow();
                        dr.ItemArray = tb.Rows[j].ItemArray;//暂存
                        tb.Rows[j].ItemArray = tb.Rows[j + 1].ItemArray;
                        tb.Rows[j + 1].ItemArray = dr.ItemArray;
                    }
                }
            }
            return tb;
        }
    }
}