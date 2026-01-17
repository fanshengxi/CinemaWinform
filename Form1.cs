using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo0425
{
    public partial class Form1 : Form
    {
        DataClasses1DataContext dataContext = null;
        MainForm form = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userTb.Text = "";
            passwdTb.Text = "";
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {

            if (form == null) form = new MainForm();
            form.Show();
            this.Hide();

            /**if (String.IsNullOrEmpty(userTb.Text)|| String.IsNullOrEmpty(passwdTb.Text))
            {
                MessageBox.Show("账号或密码为空");
                return;
            }
            //查询数据库
            var operList = dataContext.t_oper.Where(oper => oper.oper_no.Equals(userTb.Text) && oper.password.Equals(passwdTb.Text));
            foreach (var o in operList)
            {
                Console.WriteLine("登陆成功,欢迎" + o.oper_name + "！！！");
                if (form == null) form = new MainForm();
                form.Show();
                this.Hide();
            }*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataContext = new DataClasses1DataContext();
        }
    }
}
