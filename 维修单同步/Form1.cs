using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using makelanlan;
namespace 维修单同步
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //事务处理开始,劫持链接通道的sql语句
            TransactionSql.Start(BLL.数据库.默认数据库);  //===>开始
            string sql = "insert into makelanaln.dbo.clientservice_AfterServiceBill "+
             "  select CODE,BILLTYPE,BILLDATE,Client,BillMan,Auditing,introducer,introducerDate,BillNote,Note " +
             "  ,NotePre,referCode,finishDATE,ZF1,ZF2,ZF3,ZF4,ZF5,ZF6, " +
             "  ZF7,ZF8,ZF9,ZF10,ZF11,ZF12,ZF13,ZF14,ZF15,ZF16, " +
             "  ZF17,ZF18,ZF19,ZF20,ZF21,ZF22,ZF23,ZF24,ZF25,ZF26, " +
             "  ZF27,ZF28,ZF29,ZF30,SZ1,SZ2,SZ3,SZ4,SZ5,SZ6,SZ7,SZ8,SZ9,SZ10,RQ1,RQ2,RQ3,RQ4,RQ5 " +
             "  from sysanqi.dbo.clientservice_AfterServiceBill " +
             "  where sysanqi.dbo.clientservice_AfterServiceBill.code not in  " +
             "  (select code from makelanaln.dbo.clientservice_AfterServiceBill)  " +
             "  and left(sysanqi.dbo.clientservice_AfterServiceBill.code,10)= 'SH' + CONVERT(varchar(12), getdate(), 112) " +
             "  order by id";
            BLL.SetToSql(BLL.数据库.默认数据库, sql);
            //提交事务到sql服务器处理
            if (!TransactionSql.EndSql())
            {
                DialogResult dr = MessageBox.Show("同步出错！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else
            {
                DialogResult dr = MessageBox.Show("同步成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
