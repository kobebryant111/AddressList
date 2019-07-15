using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddressList
{
    public partial class Form4 : Form
    {
        public string groupName1 = "";    //上级组名
        public string groupName2 = "";    //下级分组组名
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupName1 = textBox1.Text.Trim();
            groupName2 = textBox2.Text.Trim();
            this.Close();     //关闭窗体
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();     //关闭窗体
        }
    }
}
