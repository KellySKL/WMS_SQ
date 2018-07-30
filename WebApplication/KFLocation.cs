using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using makelanlan;

namespace WebApplication
{
    public class KFLocation
    {
        /// <summary>
        /// 获取客户定位
        /// </summary>
        /// <param name="name"> 客户名称</param>
        /// <returns></returns>
        public static string Get(string name)
        {
            string position = "";
            ClientService_kfku k = new ClientService_kfku();
            List<ClientService_kfku> ts = k.Select(" and name='" + name + "' ");
            kfku m = new kfku();
            List<kfku> ms = m.Select(" and name='" + name + "' ");
            if (ts.Count > 0)
            {
                if (ts[0].ZF29 != null || ts[0].ZF29.Trim() != "")
                {
                    position = ts[0].ZF29;
                }
            }
            else
            {
                if (ms.Count > 0)
                {
                    if (ms[0].ZF29 != null || ms[0].ZF29.Trim() != "")
                    {
                        position = ts[0].ZF29;
                    }

                }   
            }
            return position;
        }
    }
}