using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using makelanlan;
namespace 维修单同步
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Skin.GetExeUname();
            //--马克岚岚----
            //--数据植入代码----
            string[] Getinfo = MLogin.GetExeUname();
            if (Getinfo[1] == "null")
            {
                Msg Err = new Msg(3, "请勿非法启动源程序！\r\n若有疑问，请联系系统管理员", "非法启动", 1);
            }
            else
            {
                Application.Run(new Form1());
            }
        }
    }
}
