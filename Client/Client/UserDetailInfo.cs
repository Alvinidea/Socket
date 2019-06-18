using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class UserDetailInfo : Form
    {
        public UserDetailInfo()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void UserDetailInfo_Load(object sender, EventArgs e)
        {
            List<object> listinfo = this.Tag as List<object>;
            textBox1.Text = listinfo[0].ToString();
            textBox2.Text = listinfo[1].ToString();
            textBox3.Text = listinfo[2].ToString();
            textBox4.Text = listinfo[3].ToString();           
            textBox5.Text = (listinfo[4].ToString() == "0") ? "女" : "男";
            textBox6.Text = listinfo[5].ToString();
            textBox7.Text = listinfo[6].ToString();
            textBox8.Text = listinfo[7].ToString();
            IsNotEnabled(false);
        }
        void IsNotEnabled(bool boo)
        {
            textBox1.Enabled = boo;
            textBox2.Enabled = boo;
            textBox3.Enabled = boo;
            textBox4.Enabled = boo;
            textBox5.Enabled = boo;
            textBox6.Enabled = boo;
            textBox7.Enabled = boo;
            textBox8.Enabled = boo;
        }
    }
}
