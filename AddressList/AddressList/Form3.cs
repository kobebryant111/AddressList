using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Xml;
using System.IO;

namespace AddressList
{
    public partial class Form3 : Form
    {
        string personId;     //联系人的ID
        bool insertdate = false;    //用于判断是更新联系人还是新建联系人
        string starpath = @"..\..\";     //相对路径
        public Form3()
        {
            InitializeComponent();
        }

        //添加读取所有的分组方法。将所有的分组信息读取并添加到comboBox1中
        private void ReadList()     //读取所有分组
        {
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(starpath + @"List.xml");
            XmlNodeList xnl = xmldocument.SelectSingleNode("List").ChildNodes;
            foreach(XmlNode xd in xnl)
            {
                XmlElement xe = (XmlElement)xd;
                comboBox1.Items.Add(xe.InnerText);   //加载分组
            }
        }

        //添加读取所选择联系人的信息方法
        private void ReadPerson()
        {
            DataSet myDataSet = new DataSet();
            myDataSet.ReadXml(starpath + @"\AddressListxml");
            int ipersonid = Convert.ToInt32(personId);
            textBox1.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][1].ToString();    //读取名字
            dateTimePicker1.Text= myDataSet.Tables["PersonList"].Rows[ipersonid - 1][2].ToString();   //读取出生时间
            textBox2.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][3].ToString();   //电话
            textBox3.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][4].ToString();   //Email
            textBox4.Text = myDataSet.Tables["PersonList"].Rows[ipersonid - 1][5].ToString();   //QQ
            comboBox3.Text= myDataSet.Tables["PersonList"].Rows[ipersonid - 1][6].ToString();    //性别
            textBox5.Text= myDataSet.Tables["PersonList"].Rows[ipersonid - 1][7].ToString();    //地址
            textBox6.Text= myDataSet.Tables["PersonList"].Rows[ipersonid - 1][8].ToString();   //备注
            comboBox1.Text= myDataSet.Tables["PersonList"].Rows[ipersonid - 1][9].ToString();   //所在分组
        }

       //添加显示照片和照片说明的方法
       private void ShowPicture(string userId)
        {
            if (!Directory.Exists(starpath + @"\Picture")) ;    //创建存放图片的文件夹
            {
                Directory.CreateDirectory(starpath + @"\Picture");
            }
            string[] files = Directory.GetFiles(starpath + @"\Picture");   //读取文件夹中的所有文件
            foreach(string file in files)
            {
                string filename = Path.GetFileName(file);    //得到文件名+后缀名
                if (filename == userId + ".jpg")
                {
                    FileStream fs = new FileStream(starpath + @"\Picture\" + userId + ".jpg", FileMode.Open, FileAccess.Read);
                    pictureBox1.Image = Image.FromStream(fs);
                    fs.Close();
                }
                if (filename == userId + ".TXT")   //读取照片说明
                {
                    textBox7.Text = "";
                    using(StreamReader sr=new StreamReader(starpath + @"\Picture\" + filename, System.Text.Encoding.Default))
                    {
                        while (sr.Peek() > -1)
                        {
                            string strres = sr.ReadLine();     //读取一行
                            textBox7.AppendText(strres + "\r\n");
                        }
                    }
                }
            }
        }


        //添加显示照片和照片说明的方法。当Picture文件夹中有所要添加联系人照片和照片说明时则首先删除再保存
        private void SavePicture(string userId)
        {
            if (!Directory.Exists(starpath + @"\Picture"))     //如果没存放图片的文件夹则创建
            {
                Directory.CreateDirectory(starpath + @"\Picture");
            }
            if (File.Exists(starpath + @"\Picture\" + personId + ".jpg"))    //如果有就删除
            {
                File.Delete(starpath + @"\Picture\" + personId + ".jpg");
            }
            if (File.Exists(starpath + @"\Picture\" + personId + ".TXT"))    //如果图片有说明就删除
            {
                File.Delete(starpath + @"\Picture\" + personId + ".TXT");
            }
            try
            {
                pictureBox1.Image.Save(starpath + @"\Picture\" + personId + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);    //保存图片
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //创建一个文件流，用以写入或者创建一个StreamWriter
            FileStream fs = new FileStream(starpath + @"\Picture\" + personId + ".jpg", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Flush();
            //使用StreamWriter往文件中写入内容
            sw.BaseStream.Seek(0, SeekOrigin.Begin);
            sw.Write(textBox7.Text);
            //关闭此文件
            sw.Flush();
            sw.Close();
        }

        public Form3(string s)    //读取联系人
        {
            InitializeComponent();
            personId = s;    //所选取的联系人的ID
            ReadList();   //读取分组
            ReadPerson();    //读取联系人
            ShowPicture(personId);   //读取照片
        }

        public Form3(bool b)   //当b为TRUE时新建联系人
        {
            InitializeComponent();
            ReadList();     //读取分组
            insertdate = b;    //是否新建联系人
        }

        private void button1_Click(object sender, EventArgs e)     //保存
        {
            DataSet myDataSet = new DataSet();
            myDataSet.ReadXml(starpath + @"\AddressList.xml");
            if (insertdate)     //如果插入数据
            {
                DataRow myRow = myDataSet.Tables["PersonList"].NewRow();
                Form1.allPersonNum++;
                myRow["ID"] = Form1.allPersonNum.ToString();
                myRow["姓名"] = textBox1.Text.Trim();
                myRow["出生时间"] = dateTimePicker1.Text.Trim();
                myRow["电话"] = textBox2.Text.Trim();
                myRow["Email"] = textBox3.Text.Trim();
                myRow["QQ"] = textBox4.Text.Trim();
                myRow["性别"] = comboBox3.Text.Trim();
                myRow["地址"] = textBox5.Text.Trim();
                myRow["备注"] = textBox6.Text.Trim();
                myRow["所在分组"] = comboBox1.Text.Trim();
                myDataSet.Tables["PersonList"].Rows.Add(myRow);
            }
            else   //更新数据
            {
                int ipersonid = Convert.ToInt32(personId);
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][1] = textBox1.Text.Trim();   //更新姓名
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][2] = dateTimePicker1.Text.Trim();   //出生时间
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][3] = textBox2.Text.Trim();
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][4] = textBox3.Text.Trim();
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][5] = textBox4.Text.Trim();
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][6] = comboBox3.Text.Trim();
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][7] = textBox5.Text.Trim();
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][8] = textBox6.Text.Trim();
                myDataSet.Tables["PersonList"].Rows[ipersonid - 1][9] = comboBox1.Text.Trim();
            }
            myDataSet.WriteXml(starpath + @"\AddressList.xml");
            SavePicture(personId);
            MessageBox.Show("保存成功！");
            this.Close();      //关闭窗口
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();    //关闭此窗体
        }

        private void pictureBox1_Click(object sender, EventArgs e)     //添加选择照片
        {
            try
            {
                openFileDialog1.RestoreDirectory = true;    //用来指示对话框在关闭前还原当前目录
                openFileDialog1.DefaultExt = ".jpg";
                openFileDialog1.Filter = "Jpeg文件(*.jpg)|*.jpg|gif文件(*.gif)|*.gif|bmp文件(*.bmp)|*.bmp";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Image image = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = image;
                    pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            finally
            {
                openFileDialog1.Dispose();
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            textBox7.ReadOnly = false;     //允许编辑
        }
    }
}
