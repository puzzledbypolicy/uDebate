using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ATC;
using DotNetNuke.Services.Localization;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace DotNetNuke.Modules.uDebate
{
    public partial class ThreadsPostsPrintTreeView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string ThreadID = ATC.Tools.URLParam("ThreadId");            
          
            lbThreadTitle.Text = getThreadTitleById();

            string sSQLFirstLevel = string.Empty;
            string sSQL = string.Empty;
            string sSQLFilter = string.Empty;
            sSQLFirstLevel = "SELECT [ID] FROM [uDebate_Forum_Posts] WHERE ThreadID=" + ThreadID + " and IsPublished=1 and ParentID=0";
            sSQL = "SELECT [ID],[ParentID],[Subject],[PostDate],[Message],[PostType],[IsPublished],[UserID] FROM [uDebate_Forum_Posts] WHERE IsPublished=1 and ThreadID=" + ThreadID;
            sSQLFilter = "SELECT [ID] FROM [uDebate_Forum_Posts] WHERE IsPublished=1 and ThreadID=" + ThreadID;
            System.Data.DataSet dsTreeFirstLevel = ATC.Database.sqlExecuteDataSet(sSQLFirstLevel);
            System.Data.DataSet dsTree = ATC.Database.sqlExecuteDataSet(sSQL);
            if (dsTree.Tables[0].Rows.Count > 0)
            {
                string sKeyValue = ATC.Database.sqlGetFirst(sSQLFilter);

                HierarchicalXMLDataBuilder XmlData = new HierarchicalXMLDataBuilder();
                foreach (DataRow rFirst in dsTreeFirstLevel.Tables[0].Rows)
                {
                    XmlData.BuildMap(TreeView1.Nodes, dsTree, "ID", "ParentID", rFirst["ID"].ToString(), true, Request);
                }
                //XmlData.BuildMap(TreeView1.Nodes, dsTree, "ID", "ParentID", sKeyValue, ThreadViewPermission);
            }
            //printBtn.Text = Localization.GetString("Print");
            //closeBtn.Text = Localization.GetString("Close");
        }
        public string getUserIdByThread()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Threads] where [ID]=" + ATC.Tools.URLParam("ThreadId"));
            return sUserId;
        }
        public string getUserIdByTopic()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Topics] where [ID]=" + ATC.Tools.URLParam("ThreadId"));
            return sUserId;
        }
        public string getThreadTitleById()
        {
            string sTitle = ATC.Database.sqlGetFirst("SELECT [Description] FROM [uDebate_Forum_Threads] where [ID]=" + ATC.Tools.URLParam("ThreadId"));
            return sTitle;
        }

        public class HierarchicalXMLDataBuilder
        {
            private string ParentColumn;
            private string ChildColumn;
            private int NoOfColumns;
            private DataSet Data;
            private DataView vuData;
            private HttpRequest RequestUrl;

            public void BuildMap(TreeNodeCollection nodes, DataSet data, string IdentityColumn, string ReferenceColumn,
                                string KeyValue, bool ThreadViewPermission, HttpRequest RequestUrl)
            {
                this.ParentColumn = IdentityColumn;
                this.ChildColumn = ReferenceColumn;
                this.Data = data;
                this.RequestUrl = RequestUrl;
                vuData = new DataView(data.Tables[0]);

                BuildChildNodes(nodes, vuData, ParentColumn + " = " + KeyValue, ThreadViewPermission);
            }

            private void BuildChildNodes(TreeNodeCollection nodes, DataView DtView, string filter, bool ThreadViewPermission)
            {
                //string href = ATC.Tools.GetParam("RootURL") + "?page=" + ATC.Tools.URLParam("page") + "&Thread=" + ATC.Tools.URLParam("Thread");

                string rURL = RequestUrl.Url.OriginalString;
                //string href = "#";//rURL.Substring(0, RequestUrl.Url.AbsoluteUri.IndexOf("Thread") + 10) + ATC.Tools.URLParam("Thread");

                if (DtView.Count > 0)
                {
                    DtView.RowFilter = filter;
                    int RowCounter = 0;

                    string sFieldName = string.Empty;
                    string sId = string.Empty;
                    string sParentId = string.Empty;
                    string sMessage = string.Empty;
                    string sImageUrl = string.Empty;
                    string state = string.Empty;

                    for (RowCounter = 0; RowCounter <= DtView.Count - 1; RowCounter++)
                    {
                        TreeNode tn = new TreeNode();
                        sFieldName = DtView[RowCounter]["Subject"].ToString() + " " + state;
                        sId = DtView[RowCounter]["ID"].ToString();
                        sParentId = DtView[RowCounter]["ParentID"].ToString();
                        sMessage = DtView[RowCounter]["Message"].ToString();
                        sImageUrl = DtView[RowCounter]["PostType"].ToString();
                        sImageUrl = getImageIconUrl(sImageUrl);

                        tn.Text = sFieldName;
                        tn.Value = sId;
                        tn.ToolTip = sMessage;
                        tn.ImageUrl = sImageUrl;
                        tn.SelectAction = TreeNodeSelectAction.None;

                        nodes.Add(tn);
                        tn.ToggleExpandState();

                        DataView vuData2 = new DataView(Data.Tables[0]);
                        if (DtView.Count > RowCounter)
                        {
                            BuildChildNodes(tn.ChildNodes, vuData2, ChildColumn + " = " + DtView[RowCounter][ParentColumn], ThreadViewPermission);
                        }
                    }
                }
            }
            public string getImageIconUrl(string sID)
            {
                string sOut = string.Empty;
                switch (sID)
                {
                    case "1":
                        sOut = "issue_icon.gif";
                        break;
                    case "2":
                        sOut = "alter_icon.gif";
                        break;
                    case "3":
                        sOut = "pro_icon.gif";
                        break;
                    case "4":
                        sOut = "con_icon.gif";
                        break;
                    case "5":
                        sOut = "question_icon.gif";
                        break;
                    case "6":
                        sOut = "answer_icon.gif";
                        break;
                    case "7":
                        sOut = "comments_icon.gif";
                        break;
                    case "8":
                        sOut = "comments_icon.gif";
                        break;
                    default:
                        sOut = "issue_icon.gif";
                        break;
                }
                return "DesktopModules/uDebate/images/" + sOut;
            }
        }
    }
}
