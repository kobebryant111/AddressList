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
    public partial class Form5 : Form
    {
        public string groupName = "";    //组名
        public string name = "";
        public string qq = "";
        public string email = "";
        public string mobile = "";
        public Form5()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupName = textBox1.Text.Trim();
            name = textBox2.Text.Trim();
            qq = textBox3.Text.Trim();
            email = textBox4.Text.Trim();
            mobile = textBox5.Text.Trim();
            this.Close();
        }
    }
}
