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
using System.Text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace DotNetNuke.Modules.uDebate
{
    public partial class ThreadsPostsTreeViewSummary : uDebateModuleBase
    {
        string ThreadID;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            ThreadID = ATC.Tools.URLParam("ThreadId");

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

            string SQLStats = "SELECT COUNT(ID) AS 'Posts', COUNT(Distinct(UserId)) as 'Users' FROM uDebate_Forum_Posts"
                              + " WHERE (ThreadID =" + ThreadID + ")";
            System.Data.DataSet statistics = ATC.Database.sqlExecuteDataSet(SQLStats);

            TotalPosts.Text = statistics.Tables[0].Rows[0]["Posts"].ToString();
            TotalUsers.Text = statistics.Tables[0].Rows[0]["Users"].ToString();

            FetchNotes(ThreadID);
        }

        public string getThreadTitleById()
        {
            string sTitle = ATC.Database.sqlGetFirst("SELECT [Description] FROM [uDebate_Forum_Threads] where [ID]=" + ATC.Tools.URLParam("ThreadId"));
            return sTitle;
        }
        
        public void HideEditorBtn_Click(object sender, EventArgs e)
        {
            txtNotesEditor.Visible = false;
            HideEditorBtn.Visible = false;
            SaveNotesBtn.Visible = false;
        }

        public void SaveNotesBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ATC.Database.sqlExecuteCommand("UPDATE uDebate_Forum_Threads SET FacilitatorNotes = N'" + txtNotesEditor.Text.Trim()
                + "' WHERE ID=" + ATC.Tools.URLParam("ThreadId") );

                FetchNotes(ThreadID);
            }
            catch
            {
            }
        }

        private void FetchNotes(string thread_ID)
        {
            string sSQLGetNotes = "Select FacilitatorNotes from uDebate_Forum_Threads where ID=" + thread_ID;

            string notes = ATC.Database.sqlGetFirst(sSQLGetNotes);
            NotesLabel.Text = Server.HtmlDecode(notes);
            txtNotesEditor.Text = notes;
        }

        //public void PDFbutton_Click(object sender, EventArgs e)
        //{


        //    //bind data to data bound controls and do other stuff 

        //    Response.Clear(); //this clears the Response of any headers or previous output
        //    Response.Buffer = true; //make sure that the entire output is rendered simultaneously

        //    ///
        //    ///Set content type to MS Excel sheet
        //    ///Use "application/msword" for MS Word doc files
        //    ///"application/pdf" for PDF files
        //    ///

        //    Response.ContentType = "application/pdf";
        //    StringWriter stringWriter = new StringWriter(); //System.IO namespace should be used

        //    HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter);

        //    ///
        //    ///Render the entire Page control in the HtmlTextWriter object
        //    ///We can render individual controls also, like a DataGrid to be
        //    ///exported in custom format (excel, word etc)
        //    ///
        //    this.RenderControl(htmlTextWriter);
        //    Response.Write(stringWriter.ToString());
        //    Response.End();

        //}


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
                return "~/DesktopModules/uDebate/images/" + sOut;
            }
        }
    }
}
