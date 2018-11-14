using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using makelanlan;
using System.Data;
namespace WebApplication
{
    public class KFLocation
    {
        /// <summary>
        /// 获取客户定位
        /// </summary>
        /// <param name="name"> 客户名称</param>
        /// <returns>  返回 ""  说明未标记，返回"none" 说明无客户需新建</returns>
        public static string Get(string name)
        {
            string position = "";
            ClientService_kfku k = new ClientService_kfku();
            List<ClientService_kfku> ts = k.Select(" and name='" + name.Trim() + "' ");
         //   kfku m = new kfku();
           // List<kfku> ms = m.Select(" and name='" + name.Trim() + "' ");
            if (ts.Count > 0)
            {
                if (ts[0].ZF29 != null && ts[0].ZF29.Trim()!= ""&& ts[0].ZF29!="0" && ts[0].ZF29 != "00")
                {
                    position = ts[0].ZF29+','+ ts[0].ZF30;
                }
            }
            else
            {
                position = "none";//返回没有该客户
            }
            return position;
        }
        public static void Set(string userName,string name, string position)
        {
            //ClientService_kfku k = new ClientService_kfku();
            string[] po = position.Split(',');
            //k.ZF29 = po[0];//经度
            //k.ZF30 = po[1];//纬度
            //k.ZF28 = userName;
            //k.Updata(" and name='" + name + "' ");

            string sql_测试库 = " update makelanaln.dbo.clientservice_kfku set zf28 ='" + userName + "', " +
           " zf29 = '" + po[0] + "', zf30 = '" + po[1] + "'  where makelanaln.dbo.clientservice_kfku.NAME ='" + name + "' ";
            DBLL.ExecuteNonQuery(MyGlobal.DataBase, sql_测试库);
            string sql = " update sysanqi.dbo.clientservice_kfku set zf28 ='" + userName + "', " +
            " zf29 = '" + po[0] + "', zf30 = '" + po[1] + "'  where sysanqi.dbo.clientservice_kfku.NAME ='" + name + "' ";
            DBLL.ExecuteNonQuery(MyGlobal.DataBase, sql);
        }
    }
}