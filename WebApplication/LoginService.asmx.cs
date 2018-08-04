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
    /// LoginService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class LoginService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "hello world!";
           // return "M:"+UserInfo.UName+"S:"+ MyGlobal.UserLogin.ToString();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = false)]
        public string WS1()
        {
            return "POST无参数";
        }

        [WebMethod]
        public void MsgOk()
        {
            Msg m = new Msg(3, UserInfo.UName);
        }
        [WebMethod]
        public string Uname()
        {
            puku p = new puku();
            List<puku> mp = p.Select(" and id=8");
            return mp[0].TURENAME.ToString();
        }
        [WebMethod]
        public List<KfkfCode> KFKF(string userId,string searchName)
        {
            MLogin.GetExeUname();
            List<KfkfCode> ls = new List<KfkfCode>();
            string sql = "  select top 30 NAME,CONTACTOR,PHONE from ( " +
                "   select NAME,CONTACTOR,PHONE from  kfku where id in (select max(id) from kfku group by name)  " +
                "   union all " +
                "   select NAME,CONTACTOR,PHONE from  ClientService_kfku where id in (select max(id) from ClientService_kfku group by name)   " +
                ")  as a  " +
                "  group by  NAME,CONTACTOR,PHONE  " +
                " HAVING NAME like '%" + searchName + "%' or CONTACTOR like '%" + searchName + "%' or  PHONE like '%" + searchName + "%' ";
            DataTable table = (DataTable)BLL.SqltoView(BLL.数据库.默认数据库, BLL.数据类型.Table, sql);
                        
            foreach (DataRow r in  table.Rows)
            {
                KfkfCode C = new KfkfCode();
                C.NAME = r["NAME"].ToString();
                ls.Add(C);
            }
            return ls;
            //ClientService_kfku f = new ClientService_kfku();
            //List<ClientService_kfku> list_f=  f.Select(" and NAME like '%" + searchName+"%' or CONTACTOR like '%"+searchName+"%' or  PHONE like '%"+searchName+"%' ",
            //    " top 30 NAME,id");
            //foreach (ClientService_kfku k in list_f)
            //{
            //    KfkfCode C = new KfkfCode();
            //    C.ID = k.ID;
            //    C.NAME = k.NAME;
            //    ls.Add(C);
            //}
            //kfku m = new kfku();
            //List<kfku> list_m = m.Select(" and NAME like '%" + searchName + "%' or CONTACTOR like '%" + searchName + "%' or  PHONE like '%" + searchName + "%' ",
            //    " top 30 NAME,id");
            //foreach (kfku mk in list_m)
            //{
            //    KfkfCode C = new KfkfCode();
            //    C.ID = mk.ID;
            //    C.NAME = mk.NAME;
            //    ls.Add(C);
            //}
            //string sql = "select NAME,CONTACTOR,PHONE from ClientService_kfku where 1=1  and name like '%" + searchName + "%' or CONTACTOR like '%" + searchName + "%' or  PHONE like '%" + searchName + "%' ";

        }

        [WebMethod]
        public User WS2(string userId, string password)
        {
            //----------------------
            //lxdlxd
            MLogin.GetExeUname();
            User u = new User();
            u.status = -1;
            u.userId = userId.Trim();
            List<puku_user> mp = GetPUser.PUser(" and USERPU='" + userId.Trim() + "' ");
            if (mp.Count > 0)
            {
                if (BitLock.RealseLock_L(mp[0].MM) == password)
                {
                    //UserInfo.UName = mp[0].TURENAME;
                    UserInfo.Uzw = "管理员";
                    MyGlobal.UserLogin.Add(mp[0].TURENAME);
                    u.name = mp[0].TURENAME;
                    u.authorised = mp[0].DEPT;
                    u.status = 10;
                    u.mname = UserInfo.UName;
                }
                else
                {
                    u.status = 5;
                }
            }
            else
            {
                u.status = 0;
            }
            return u;
        }

        [WebMethod]
       // [ScriptMethod(UseHttpGet = true)]
        public string WS3(string name)
        {
          //  return JsonConvert.SerializeObject("cdshi");
            return name;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string WS4(string s)
        {
            return s;
        }

        [WebMethod]
        public string HelloName(string name)
        {
            return "Hello," + name;
        }

        [WebMethod]
        public string CheckLogin(User user)
        {
            return "Hello," + user.userId;
        }

        [WebMethod]
        public string GetPersonList()
        {
            //List<Person>
            List<Person> lst = new List<Person>();
            lst.Add(new Person { id = 1, name = "小明", age = 34 });
            lst.Add(new Person { id = 1, name = "小明", age = 34 });
            lst.Add(new Person { id = 1, name = "小明", age = 34 });
            lst.Add(new Person { id = 1, name = "小明", age = 34 });

            //return lst;
            return JsonConvert.SerializeObject(lst);
        }
    }







    //---------------------------------------------
    public class Person
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
    }

    public class Kfkf
    {
        private string _CONTACTOR;
        public string CONTACTOR { get { return _CONTACTOR; } set { _CONTACTOR = value; } }

        private string _NAME;
        public string NAME { get { return _NAME; } set { _NAME = value; } }

        private string _PHONE;
        public string PHONE { get { return _PHONE; } set { _PHONE = value; } }

        private int _status;
        public int status { get { return _status; } set { _status = value; } }

        private string _authorised;
        public string authorised { get { return _authorised; } set { _authorised = value; } }

    }

    public class KfkfCode
    {
        private int _ID;
        public int ID { get { return _ID; } set { _ID = value; } }

        private string _NAME;
        public string NAME { get { return _NAME; } set { _NAME = value; } }
    }

    public class User
    {
        private string _userId;
        public string userId { get { return _userId; } set { _userId = value; } }

        private string _name;
        public string name { get { return _name; } set { _name = value; } }

        private string _password;
        public string password { get { return _password; } set { _password = value; } }

        private int _status;
        public int status { get { return _status; } set { _status = value; } }

        private string _authorised;
        public string authorised { get { return _authorised; } set { _authorised = value; } }

        private string _mname;
        public string mname { get { return _mname; } set { _mname = value; } }

    }
}
