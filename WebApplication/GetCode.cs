using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using makelanlan;
namespace WebApplication
{

    public class GetCode
    {
        public static string SetCode(DateTime dateTime)
        {
            string datenow = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string date = datenow.Split(' ')[0];
            string time = datenow.Split(' ')[1];
            string hour = time.Split(':')[0];
            switch (hour)
            {
                case "08":
                    return hour;
                case "09":
                    return hour;
                case "10":
                    return hour;
                case "11":
                    return hour;
                case "12":
                    return "";
                case "13":
                    return hour;
                case "14":
                    return hour;
                case "15":
                    return hour;
                case "16":
                    return hour;
                default:
                    return "";
            }
        }
    }
}