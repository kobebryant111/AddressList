﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Data;
using System.Windows.Forms;

namespace AddressList
{
    class TreeXML
    {
        TreeView thetreeview;    //定义TreeView成员变量
        XmlDocument xmldocument;    //定义XmlDocument成员变量
        public TreeXML()     //构造函数
        {
            xmldocument = new XmlDocument();
        }
        ~TreeXML()
        {
            //析构函数
        }

        //添加组名
        public void AddXmlNode(string XMLFilePath, string NodeName)
        {
            xmldocument.Load(XMLFilePath);
            XmlNode root = xmldocument.SelectSingleNode("List");    //查找List
            XmlElement xe1 = xmldocument.CreateElement("组名");   //创建一个<组名>节点
            xe1.InnerText = NodeName;    //设置节点的串联值
            root.AppendChild(xe1);    //添加到<List>节点中
            xmldocument.Save(XMLFilePath);   //将XML文档保存在指定的文件中
        }

        //添加下级分组组名
        public void AddXmlNextNode(string XMLFilePath,string NodeName1,string NodeName2)
        {
            xmldocument.Load(XMLFilePath);
            XmlNode root = xmldocument.SelectSingleNode("List");   //查找到下级分组的根节点
            XmlElement xe1 = xmldocument.CreateElement("子组");   //创建一个下级节点
            xe1.InnerText = NodeName1;
            xe1.SetAttribute("父组", NodeName2);
            root.AppendChild(xe1);
            xmldocument.Save(XMLFilePath);   //将XML文档保存在指定的文件中
        }

        //读取分组的XML文件并显示在TreeView控件上
        public void XMLToTree(string XMLFilePath, TreeView TheTreeView)
        {
            thetreeview = TheTreeView;
            xmldocument.Load(XMLFilePath);    //读取XML文件
            XmlNode root = xmldocument.SelectSingleNode("List");   //选择匹配List的第一个节点
            foreach (XmlNode subXmlnod in root.ChildNodes)    //遍历所有的子节点
            {
                if (subXmlnod.Name == "组名")    //子节点的限定名为“组名”
                {
                    TreeNode trerotnod = new TreeNode();   //实例化一个树节点
                    trerotnod.Text = subXmlnod.InnerText;    //将子节点串联值作为树节点名称
                    thetreeview.Nodes.Add(trerotnod);    //添加此树节点
                }
                if (subXmlnod.Name == "子组")
                {
                    TreeNode treeSubNode = new TreeNode();   //实例化一个树节点
                    treeSubNode.Text = subXmlnod.InnerText;    //将子节点串联值作为树节点名称
                    foreach (TreeNode tn in thetreeview.Nodes)
                    {
                        if (tn.Text == subXmlnod.Attributes["父组"].Value)
                        {
                            tn.Nodes.Add(treeSubNode);
                        }
                    }
                }
            }
        }

        //删除分组
        public void DeleXml(string XMLFilePath,string NodeName)
        {
            xmldocument.Load(XMLFilePath);
            XmlNodeList xnl = xmldocument.SelectSingleNode("List").ChildNodes;   //获取List节点下的所有子节点
            foreach(XmlNode xd in xnl)     //遍历
            {
                XmlElement xe = (XmlElement)xd;    //转换为XmlElement
                if (xe.InnerText == NodeName)
                {
                    xe.ParentNode.RemoveChild(xe);    //删除此节点以及此节点下的所有节点
                    xmldocument.Save(XMLFilePath);    //保存
                }
            }
        }

        //更改分组名称
        public void AlterXml(string XMLFilePath,string OldNodeName,string NewNodeName)
        {
            xmldocument.Load(XMLFilePath);
            XmlNodeList xnl = xmldocument.SelectSingleNode("List").ChildNodes;   //获取List节点下的所有子节点
            foreach (XmlNode xd in xnl)     //遍历
            {
                XmlElement xe = (XmlElement)xd;    //转换为XmlElement
                if (xe.InnerText == OldNodeName)    //如果为要修改的节点
                {
                    xe.InnerText=NewNodeName;    //则修改
                    xmldocument.Save(XMLFilePath);    //保存
                }
            }
        }

        //获得所选中的分组中的所有联系人信息表
        public DataTable GetPersonInfo(string XMLFilePath,string NodeName)
        {
            xmldocument.Load(XMLFilePath);
            XmlNodeList xnl = xmldocument.SelectSingleNode("CONTENTS").ChildNodes;
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));    //添加列名为ID
            dt.Columns.Add("姓名", typeof(string));
            dt.Columns.Add("出生时间", typeof(string));
            dt.Columns.Add("电话", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("QQ", typeof(string));
            dt.Columns.Add("性别", typeof(string));
            dt.Columns.Add("地址", typeof(string));
            dt.Columns.Add("备注", typeof(string));
            foreach(XmlNode xd in xnl)
            {
                XmlElement xe = (XmlElement)xd;
                if (xe.GetAttribute("所在分组") == NodeName)
                {
                    DataRow myRow = dt.NewRow();    //新建一行
                    myRow["ID"] = xe.ChildNodes.Item(0).InnerText;
                    myRow["姓名"] = xe.ChildNodes.Item(1).InnerText;
                    myRow["出生时间"] = xe.ChildNodes.Item(2).InnerText;
                    myRow["电话"] = xe.ChildNodes.Item(3).InnerText;
                    myRow["Email"] = xe.ChildNodes.Item(4).InnerText;
                    myRow["QQ"] = xe.ChildNodes.Item(5).InnerText;
                    myRow["性别"] = xe.ChildNodes.Item(6).InnerText;
                    myRow["地址"] = xe.ChildNodes.Item(7).InnerText;
                    myRow["备注"] = xe.ChildNodes.Item(8).InnerText;
                    dt.Rows.Add(myRow);
                }
            }
            return dt;     //返回表dt
        }
    }
}
