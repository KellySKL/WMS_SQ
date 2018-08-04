using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using makelanlan;
namespace WebApplication
{
    public class UserLocation
    {
        /// <summary>
        /// 获取用户定位
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public static string Get(string userId)
        {
            string position="";
            string datetime = SysTime.GetTime.ToString("yyyyMMdd");
            //只取当天人的最新记录
            string sql = "select * from scue_user a where not exists(select 1 from scue_user b where b.userpu=a.userpu and b.id>a.id) and a.date ='" + datetime + "'  and userpu='" + userId + "'  ";
            List<scue_user> scue_s = BLL.SqlToList<scue_user>(new BLL.数据库(), sql);
            
            if (scue_s.Count>0)
            {
                string[] strs = scue_s[0].POINTS.Split(',');
                int count = strs.Length - 2;
                position = strs[count];
            }
            return position;
        }
    }
}