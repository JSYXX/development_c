using System;
using System.Collections;
using System.Xml;               //使用xml读写

public class XmlFileHelper
{    
    #region 公共变量    
    XmlDocument xmldoc;    
    XmlNode xmlnode;    
    XmlElement xmlelem;    
    #endregion 

    #region 创建Xml文档    
        /// <summary>    
        /// 创建一个带有根节点的Xml文件    
        /// </summary>    
        /// <paramname="FileName">Xml文件名称</param>    
        /// <paramname="rootName">根节点名称</param>    
        /// <paramname="Encode">编码方式:gb2312，UTF-8等常见的</param>    
        /// <paramname="DirPath">保存的目录路径</param>    
        ///<returns></returns>    
        public bool CreateXmlDocument(string FileName, string RootName,string Encode)    
    {    
        try    
        {    
            xmldoc = new XmlDocument();        
            XmlDeclaration xmldecl;        
            xmldecl =xmldoc.CreateXmlDeclaration("1.0", Encode,null);        
            xmldoc.AppendChild(xmldecl);        
            xmlelem =xmldoc.CreateElement("", RootName, "");        
            xmldoc.AppendChild(xmlelem);        
            xmldoc.Save(FileName);        
            return true;        
        }
        catch (Exception e)
        {
            return false;
        throw new Exception(e.Message);
        }
    } 
        #endregion 

    #region XML文档节点查询和读取
    /**/
    /// <summary>    
    /// 选择匹配XPath表达式的第一个节点XmlNode.    
    /// </summary>    
    /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>    
    /// <paramname="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名")</param>    
    /// <returns>返回XmlNode</returns>    
    public XmlNode GetXmlNodeByXpath(string xmlFileName, string xpath)
    {
        xmldoc = new XmlDocument();
        try
        {
            xmldoc.Load(xmlFileName); //加载XML文档    
            XmlNode xmlNode = xmldoc.SelectSingleNode(xpath);
            return xmlNode;
        }
        catch (Exception ex)
        {
            return null;
            //throw ex; //这里可以定义你自己的异常处理    
        }
    }

    /**/
    /// <summary>    
    /// 选择匹配XPath表达式的节点列表XmlNodeList.    
    /// </summary>    
    /// <paramname="xmlFileName">XML文档完全文件名(包含物理路径)</param>    
    /// <paramname="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名")</param>    
    /// <returns>返回XmlNodeList</returns>    
    public XmlNodeList GetXmlNodeListByXpath(string xmlFileName, string xpath)
    {
        xmldoc = new XmlDocument();
        try
        {
            xmldoc.Load(xmlFileName); //加载XML文档    
            XmlNodeList xmlNodeList = xmldoc.SelectNodes(xpath);
            return xmlNodeList;
        }
        catch (Exception ex)
        {
            return null;
            //throw ex; //这里可以定义你自己的异常处理    
        }
    }

    /**/
    /// <summary>    
    /// 选择匹配XPath表达式的第一个节点的匹配xmlAttributeName的属性XmlAttribute. /// </summary>    
    /// <paramname="xmlFileName">XML文档完全文件名(包含物理路径)</param>    
    /// <paramname="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>    
    /// <paramname="xmlAttributeName">要匹配xmlAttributeName的属性名称</param>    
    /// <returns>返回xmlAttributeName</returns>    
    public XmlAttribute GetXmlAttribute(string xmlFileName, string xpath, string xmlAttributeName)
    {
        string content = string.Empty;
        xmldoc = new XmlDocument();
        XmlAttribute xmlAttribute = null;
        try
        {
            xmldoc.Load(xmlFileName); //加载XML文档    
            XmlNode xmlNode = xmldoc.SelectSingleNode(xpath);
            if (xmlNode != null)
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    xmlAttribute = xmlNode.Attributes[xmlAttributeName];
                }
            }
        }
        catch (Exception ex)
        {
            throw ex; //这里可以定义你自己的异常处理    
        }
        return xmlAttribute;
    }
    #endregion

    #region 常用操作方法(增删改)
        /// <summary>
        /// 插入一个节点和它的若干子节点
        /// </summary>
        /// <paramname="XmlFile">Xml文件路径</param>
        /// <paramname="NewNodeName">插入的节点名称</param>
        /// <paramname="HasAttributes">此节点是否具有属性，True为有，False为无</param>
        /// <paramname="fatherNode">此插入节点的父节点,要匹配的XPath表达式(例如:"//节点名//子节点名)</param>
        /// <paramname="htAtt">此节点的属性，Key为属性名，Value为属性值</param>
        /// <paramname="htSubNode">子节点的属性，Key为Name,Value为InnerText</param>
        /// <returns>返回真为更新成功，否则失败</returns>
        public bool InsertNode(string XmlFile, string NewNodeName, bool HasAttributes, string fatherNode, Hashtable htAtt, Hashtable htSubNode)
    {
        try        
        {        
            xmldoc = new XmlDocument();        
            xmldoc.Load(XmlFile);        
            XmlNode root =xmldoc.SelectSingleNode(fatherNode);        
            xmlelem =xmldoc.CreateElement(NewNodeName);        
            if (htAtt != null && HasAttributes)//若此节点有属性，则先添加属性        
            {        
                SetAttributes(xmlelem,htAtt);        
                SetNodes(xmlelem.Name,xmldoc, xmlelem, htSubNode);//添加完此节点属性后，再添加它的子节点和它们的InnerText        
            }        
            else        
            {        
                SetNodes(xmlelem.Name,xmldoc, xmlelem, htSubNode);//若此节点无属性，那么直接添加它的子节点        
            }         
            root.AppendChild(xmlelem);        
            xmldoc.Save(XmlFile);        
             return true;        
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

        /// <summary>
    /// 更新节点
    /// </summary>
    /// <paramname="XmlFile">Xml文件路径</param>
    /// <paramname="fatherNode">需要更新节点的上级节点,要匹配的XPath表达式(例如:"//节点名//子节点名)</param>
    /// <paramname="htAtt">需要更新的属性表，Key代表需要更新的属性，Value代表更新后的值</param>
    /// <param name="htSubNode">需要更新的子节点的属性表，Key代表需要更新的子节点名字Name,Value代表更新后的值InnerText</param>
    /// <returns>返回真为更新成功，否则失败</returns>
        public bool UpdateNode(string XmlFile, string fatherNode,Hashtable htAtt, Hashtable htSubNode)
    {
        try
        {
            xmldoc = new XmlDocument();
            xmldoc.Load(XmlFile);
            XmlNodeList root =xmldoc.SelectSingleNode(fatherNode).ChildNodes;
            UpdateNodes(root, htAtt,htSubNode);
            xmldoc.Save(XmlFile);
            return true;
        }    
        catch (Exception e)    
        {    
            throw new Exception(e.Message);    
        }    
    }
        /// <summary>    
        /// 更新节点属性和子节点InnerText值。    
        /// </summary>    
        /// <paramname="root">根节点名字</param>    
        /// <paramname="htAtt">需要更改的属性名称和值</param>    
        /// <paramname="htSubNode">需要更改InnerText的子节点名字和值</param>    
        private void UpdateNodes(XmlNodeList root, Hashtable htAtt, Hashtable htSubNode)
        {
            foreach (XmlNode xn in root)
            {
                if (xn.NodeType != System.Xml.XmlNodeType.Element) continue;    //20181115添加，因为xml可能有注释性节点以及其他节点，这里只能更新XmlNodeType.Element类的节点
                xmlelem = (XmlElement)xn;
                if (xmlelem.HasAttributes)//如果节点有属性，则先更改它的属性    
                {
                    foreach (DictionaryEntry de in htAtt)//遍历属性哈希表    
                    {
                        if (xmlelem.HasAttribute(de.Key.ToString()))//如果节点有需要更改的属性    
                        {
                            xmlelem.SetAttribute(de.Key.ToString(), de.Value.ToString());//则把哈希表中相应的值Value赋给此属性Key    
                        }
                    }
                }
                if (xmlelem.HasChildNodes)//如果有子节点，则修改其子节点的InnerText    
                {
                    XmlNodeList xnl = xmlelem.ChildNodes;
                    foreach (XmlNode xn1 in xnl)
                    {
                        XmlElement xe = (XmlElement)xn1;
                        foreach (DictionaryEntry de in htSubNode)
                        {
                            if (xe.Name == de.Key.ToString())//htSubNode中的key存储了需要更改的节点名称，    
                            {
                                xe.InnerText = de.Value.ToString();//htSubNode中的Value存储了Key节点更新后的数据    
                            }
                        }
                    }
                }
            }
        }    
 
        /// <summary>
    /// 删除指定节点下的子节点
    /// </summary>
    /// <paramname="XmlFile">Xml文件路径</param>
    /// <paramname="fatherNode">制定节点,要匹配的XPath表达式(例如:"//节点名//子节点名)</param>
    /// <returns>返回真为更新成功，否则失败</returns>
        public bool DeleteNodes(string XmlFile, string fatherNode)
    {    
        try        
        {        
            xmldoc = new XmlDocument();        
            xmldoc.Load(XmlFile);        
            xmlnode =xmldoc.SelectSingleNode(fatherNode);        
            xmlnode.RemoveAll();        
            xmldoc.Save(XmlFile);        
            return true;        
        }        
        catch (XmlException xe)        
        {        
            throw new XmlException(xe.Message);    
        }
    }   

        /// <summary>
    /// 删除匹配XPath表达式的第一个节点(节点中的子元素同时会被删除)
    /// </summary>
    /// <paramname="xmlFileName">XML文档完全文件名(包含物理路径)</param>
    /// <paramname="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
    /// <returns>成功返回true,失败返回false</returns>
        public bool DeleteXmlNodeByXPath(string xmlFileName, string xpath)
    {    
        bool isSuccess = false;        
        xmldoc = new XmlDocument();        
        try        
        {
            xmldoc.Load(xmlFileName); //加载XML文档            
            XmlNode xmlNode =xmldoc.SelectSingleNode(xpath);            
            if (xmlNode != null)            
            {            
            //删除节点            
            xmldoc.ParentNode.RemoveChild(xmlNode);            
            }            
            xmldoc.Save(xmlFileName); //保存到XML文档            
            isSuccess = true;        
        }        
        catch (Exception ex)        
        {
            throw ex; //这里可以定义你自己的异常处理        
        }    
    return isSuccess;    
    } 

        /// <summary>
    /// 删除匹配XPath表达式的第一个节点中的匹配参数xmlAttributeName的属性
    /// </summary>
    /// <paramname="xmlFileName">XML文档完全文件名(包含物理路径)</param>
    /// <paramname="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
    /// <paramname="xmlAttributeName">要删除的xmlAttributeName的属性名称</param>
    /// <returns>成功返回true,失败返回false</returns>
        public bool DeleteXmlAttributeByXPath(string xmlFileName, string xpath, string xmlAttributeName)
    {    
        bool isSuccess = false;    
        bool isExistsAttribute =false;    
        xmldoc = new XmlDocument();    
        try    
        {    
            xmldoc.Load(xmlFileName); //加载XML文档    
            XmlNode xmlNode =xmldoc.SelectSingleNode(xpath);    
            XmlAttribute xmlAttribute =null;    
            if (xmlNode != null)    
            {    
                //遍历xpath节点中的所有属性    
                foreach (XmlAttribute attribute in xmlNode.Attributes)    
                {    
                    if (attribute.Name.ToLower()== xmlAttributeName.ToLower())    
                    {    
                        //节点中存在此属性    
                        xmlAttribute = attribute;    
                        isExistsAttribute = true;    
                        break;    
                    }    
                }    
                if (isExistsAttribute)    
                {    
                    //删除节点中的属性    
                    xmlNode.Attributes.Remove(xmlAttribute);    
                }    
            }    
            xmldoc.Save(xmlFileName); //保存到XML文档    
            isSuccess = true;    
        }    
        catch (Exception ex)    
        {    
            throw ex; //这里可以定义你自己的异常处理    
        }    
        return isSuccess;    
    }

        /// <summary>
    /// 删除匹配XPath表达式的第一个节点中的所有属性
    /// </summary>
    /// <paramname="xmlFileName">XML文档完全文件名(包含物理路径)</param>
    /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
    /// <returns>成功返回true,失败返回false</returns>
        public bool DeleteAllXmlAttributeByXPath(string xmlFileName,string xpath)    
    {    
        bool isSuccess = false;    
        xmldoc = new XmlDocument();    
        try    
        {    
            xmldoc.Load(xmlFileName); //加载XML文档    
            XmlNode xmlNode =xmldoc.SelectSingleNode(xpath);    
            if (xmlNode != null)    
            {    
                //遍历xpath节点中的所有属性    
                xmlNode.Attributes.RemoveAll();    
            }    
            xmldoc.Save(xmlFileName); //保存到XML文档    
            isSuccess = true;    
        }    
        catch (Exception ex)    
        {    
            throw ex; //这里可以定义你自己的异常处理    
        }    
        return isSuccess;    
    }
        #endregion
            
    #region 私有方法
    /// <summary>
    /// 设置节点属性
    /// </summary>
    /// <paramname="xe">节点所处的Element</param>
    /// <paramname="htAttribute">节点属性，Key代表属性名称，Value代表属性值</param>
    private void SetAttributes(XmlElement xe, Hashtable htAttribute)
    {
        foreach (DictionaryEntry de in htAttribute)
        {
            xe.SetAttribute(de.Key.ToString(),de.Value.ToString());
        }
    }
    
    /// <summary>
    /// 增加子节点到根节点下
    /// </summary>
    /// <paramname="rootNode">上级节点名称</param>
    /// <paramname="XmlDoc">Xml文档</param>
    /// <paramname="rootXe">父根节点所属的Element</param>
    /// <paramname="SubNodes">子节点属性，Key为Name值，Value为InnerText值</param>
    private void SetNodes(string rootNode, XmlDocument XmlDoc,XmlElement rootXe, Hashtable SubNodes)
    {
        if (SubNodes == null)
        return;
        foreach (DictionaryEntry de in SubNodes)
        {
            xmlnode =XmlDoc.SelectSingleNode(rootNode);
            XmlElement subNode =XmlDoc.CreateElement(de.Key.ToString());
            subNode.InnerText =de.Value.ToString();
            rootXe.AppendChild(subNode);
        }
    }
    
    
    #endregion

   
}//end class
