using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace WebApplication
{
    #region 配置登录标头
    /// <summary>
    /// Code CreateBy BanLao
    /// </summary>
    public class MySoapHeader : SoapHeader
    {
        private string strUserName = string.Empty;
        private string strPassWord = string.Empty;

        public MySoapHeader() { }

        public MySoapHeader(string username, string password)
        {
            this.strUserName = username;
            this.strPassWord = password;
        }

        #region 构造 用户名|密码
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return strUserName; }
            set { strUserName = value; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get { return strPassWord; }
            set { strPassWord = value; }
        }

        #endregion

        #region 检测是否正确登录
        /// <summary>
        /// 检测是否正确登录
        /// </summary>
        /// <returns></returns>
        public bool CheckLogin()
        {
            if (strUserName == "lxd")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
    #endregion
}