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

namespace AddressList
{
    public partial class Form1 : Form
    {
        DataSet DataSet1;
        string starpath = @"..\..\";    //相对路径
        public static int allPersonNum;     //通讯录中的总人数
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)    //读取所有记通讯录记录
        {
            DataSet1 = new DataSet();    //实例化DataSet1
            DataSet1.ReadXml(starpath + @"\AddressList.xml", XmlReadMode.Auto);    //读取所有联络人信息
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = DataSet1;    //设置数据源
            dataGridView1.DataMember = "PersonList";
            allPersonNum = dataGridView1.RowCount;    //根据行数获取总的联络人数
            toolStripStatusLabel2.Text = "共有" + allPersonNum.ToString() + "条记录";
        }

        private void Form1_Load(object sender, EventArgs e)   //加载窗体
        {
            toolStripStatusLabel1.Text = "登录时间" + DateTime.Now.ToString();   //显示时间
            toolStripButton4_Click(null, null);    //读取所有联系人信息，即一共有多少条联系人记录
            //读取组并显示在treeView1上
            treeView1.Nodes.Clear();     //这里是删除我们在安装控件时输入的一些树节点吧？
            TreeXML TreeXml = new TreeXML();    //实例化对象
            TreeXml.XMLToTree(starpath + @"\List.xml", treeView1);    //调用方法
        }

        private void toolStripButton7_Click(object sender, EventArgs e)    //删除所选择的联系人
        {
            //获取选中的行的集合
            DataGridViewSelectedRowCollection selectedRows = dataGridView1.SelectedRows;
            if (selectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("你确定要删除选定的数据吗？", "删除数据", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach(DataGridViewRow row in selectedRows)
                    {
                        int i = Convert.ToInt32(row.Cells["ID"].Value);    //读取ID并转换为int类型
                        DataSet myDataSet = new DataSet();
                        myDataSet.ReadXml(starpath + @"\AddressList.xml");    //读取联系人到数据集中
                        myDataSet.Tables["PersonList"].Rows[i - 1].Delete();     //在数据集中删除
                        myDataSet.WriteXml(starpath + @"\AddressList.xml");     //保存删除后的XML文件
                        MessageBox.Show("删除成功！");
                        toolStripButton4_Click(null, null);    //重新读取联系人
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)   //显示AddressList中的信息到dataGridView中
        {
            TreeXML TreeXml = new TreeXML();    //实例化一个对象
            //调用TreeXml中的GetPersonInfo方法，根据treeView中的分组将相关信息显示到dataGridView1中
            dataGridView1.DataSource = TreeXml.GetPersonInfo(starpath + @"\AddressList.xml", treeView1.SelectedNode.Text);    //调用方法
        }
           
        private void toolStripButton8_Click(object sender, EventArgs e)    //搜索,从AddressList中搜索
        {
            DataSet1 = new DataSet();
            DataSet1.ReadXml(starpath + @"\AddressList.xml", XmlReadMode.Auto);   //将数据读入DataSet
            DataView dv = DataSet1.Tables["PersonList"].DefaultView;
            dv.RowFilter = "姓名='" + toolStripTextBox1.Text.Trim() + "'";     //设置筛选条件
            //注意这里dataGridView中的显示是一直在变化的，所有不能有固定的DataSourse
            dataGridView1.DataSource = dv.ToTable();    //显示搜索结果
        }

        private void 退出系统EToolStripMenuItem_Click(object sender, EventArgs e)   //退出系统
        {
            Application.Exit();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)   //删除分组
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择一个节点", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //添加一个人性化的判断，使得删除操作显得不那么草率
                DialogResult dResult = MessageBox.Show("确定要删除此节点的所有内容吗？", "删除节点", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dResult)
                {
                    case DialogResult.Yes:
                        TreeXML TreXml = new TreeXML();
                        TreXml.DeleXml(starpath + @"\List.xml", treeView1.SelectedNode.Text);    //节点都是在List.xml中删除
                        treeView1.SelectedNode.Remove();    //删除节点
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        break;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)    //添加同级分组
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();    //弹出添加修改分组窗体
            if (form2.groupName != "")     //如果输入的组名不为空
            {
                TreeNode tn = new TreeNode();     //新建一个树节点
                tn.Text = form2.groupName;    //为该树节点赋值
                treeView1.Nodes.Add(tn);    //将新创建的树节点添加到树节点集合的末尾

                /*写入XML文件*/
                if (treeView1.SelectedNode.Parent == null)    //判断是否是父节点
                {
                    TreeXML TreXML = new TreeXML();
                    TreXML.AddXmlNode(starpath + @"\List.xml", form2.groupName);     //starpath为文件路径，form2.groupName为所添加分组的组名
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)   //修改组
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();    //弹出添加修改分组窗体
            if (form2.groupName != "")
            {
                if (treeView1.SelectedNode == null)
                {
                    MessageBox.Show("请选择一个节点", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    TreeXML TreeXml = new TreeXML();
                    TreeXml.AlterXml(starpath + @"\List.xml", treeView1.SelectedNode.Text, form2.groupName);
                    treeView1.SelectedNode.Text = form2.groupName;
                }
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)     //新建联系人
        {
            Form3 form3 = new Form3(true);
            form3.ShowDialog();
            toolStripButton4_Click(null,null);   //重新读取,新建联系人后总记录数要发生变化
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                //获取所选择人的ID
                string PersonId = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value.ToString();
                Form3 form3 = new Form3(PersonId);
                form3.ShowDialog();    //显示联系人详细信息
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void 添加下级分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();    //弹出添加下级分组窗体
            if (form2.groupName != "" )     //如果信息输入完整
            {
                TreeNode tn = new TreeNode();
                tn.Text = form2.groupName;
                TreeNode tn1 = treeView1.SelectedNode;    //找到根节点
                tn1.Nodes.Add(tn);    //添加子节点tn
                try
                {
                    if (treeView1.SelectedNode.Text == "")
                    {
                        MessageBox.Show("没有选择父组名！");
                    }
                    TreeXML TreXML = new TreeXML();
                    TreXML.AddXmlNextNode(starpath + @"\List.xml", form2.groupName, treeView1.SelectedNode.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }  
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)   //查找
        {
            Form5 form5 = new Form5();
            form5.ShowDialog();   //弹出窗口 

            DataSet1 = new DataSet();
            DataSet1.ReadXml(starpath + @"\AddressList.xml", XmlReadMode.Auto);   //将数据读入DataSet
            DataView dv = DataSet1.Tables["PersonList"].DefaultView;
            dv.RowFilter = "所在分组='" + form5.groupName + "'";     //设置筛选条件
            dv.RowFilter = "姓名='" + form5.name + "'";     //设置筛选条件
            dv.RowFilter = "QQ='" + form5.qq + "'";     //设置筛选条件
            dv.RowFilter = "Email='" + form5.email + "'";     //设置筛选条件
            dv.RowFilter = "电话='" + form5.mobile + "'";     //设置筛选条件
            //注意这里dataGridView中的显示是一直在变化的，所有不能有固定的DataSourse
            dataGridView1.DataSource = dv.ToTable();    //显示搜索结果
        }
    }
}
