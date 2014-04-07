using System;
using System.Data;
using System.Collections.Generic;
using System.Web;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ATC;
using System.IO;
using DotNetNuke.Modules.uDebate.Components;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Installer.Log;
using DotNetNuke.Services.Mail;
using System.Collections;

namespace DotNetNuke.Modules.uDebate
{
    public partial class ThreadsPostsTreeView : uDebateModuleBase
    {

        private bool HasReferences = false;
        private bool HasEmbendedPollReference = false;
        private bool HasEmbendedNewsASSETReference = false;
        private bool HasEmbendedDocASSETReference = false;
        private bool DisplayFullHeadText = true;
        private Logger log = new Logger();
        public string SelectedPostType = String.Empty; /* Used to color background in the post header*/

        protected void Page_Load(object sender, EventArgs e)
        {
            log.AddInfo("loading discussion tree " + ATC.Tools.URLParam("Thread"));

            Page.MaintainScrollPositionOnPostBack = true;

            if (!Page.IsClientScriptBlockRegistered("csTreeViewRefresh"))
            {
                // Form the script that is to be registered at client side.
                String scriptString = "<script type=\"text/javascript\"> function TreeViewGotFocus() {";
                scriptString += "try{ ";
                scriptString += "var myTextField = document.getElementById('" + TreeView1.ClientID.ToString() + "_SelectedNode');";
                scriptString += "if(myTextField.value != '')";
                scriptString += "var node = document.getElementById(myTextField.value);";
                scriptString += "if(node != null) ";
                scriptString += "{ ";
                scriptString += "node.scrollIntoView(true); ";
                scriptString += "} ";
                scriptString += "}";
                scriptString += "catch(err)";
                scriptString += "{";
                scriptString += "}}";
                scriptString += "<";
                scriptString += "/";
                scriptString += "script>";
                Page.RegisterClientScriptBlock("csTreeViewRefresh", scriptString);
            }

            string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            LocalResourceFile = Localization.GetResourceFile(this, "ThreadsPostsTreeView.ascx." + culture + ".resx");

            ibxStartNewPost.ImageUrl = "images/buttons/new_discussion_" + culture + ".png";
            ibxStartNewPost.ImageOverUrl = "images/buttons/new_discussion_" + culture + "_sel.png";
            ibxOpenTreePrint.ImageUrl = "images/buttons/print_" + culture + ".png";
            ibxOpenTreePrint.ImageOverUrl = "images/buttons/print_" + culture + "_sel.png";
            ibxRemoveStartFiles.ImageUrl = "images/buttons/cancel_" + culture + ".png";
            ibxRemoveStartFiles.ImageOverUrl = "images/buttons/cancel_" + culture + "_sel.png";
            ibxCancelStartFiles.ImageUrl = "images/buttons/cancel_" + culture + ".png";
            ibxCancelStartFiles.ImageOverUrl = "images/buttons/cancel_" + culture + "_sel.png";
            ibxSave.ImageUrl = "images/buttons/sendmessage_" + culture + ".png";
            ibxSave.ImageOverUrl = "images/buttons/sendmessage_" + culture + "_sel.png";
            ibxStartingSave.ImageUrl = "images/buttons/sendmessage_" + culture + ".png";
            ibxStartingSave.ImageOverUrl = "images/buttons/sendmessage_" + culture + "_sel.png";
            ibxPrinterPost.ImageUrl = "images/buttons/print_" + culture + ".png";
            ibxPrinterPost.ImageOverUrl = "images/buttons/print_" + culture + "_sel.png";
                       
            legendIssueImg.ImageUrl = "images/issue_icon.gif";
            legendIssueLbl.Text = Localization.GetString("Issue", LocalResourceFile);
            legendAltImg.ImageUrl = "images/alter_icon.gif";
            legendAltLbl.Text = Localization.GetString("Alternative", LocalResourceFile);
            legendProImg.ImageUrl = "images/pro_icon.gif";
            legendProLbl.Text = Localization.GetString("ProArgument", LocalResourceFile);
            legendConImg.ImageUrl = "images/con_icon.gif";
            legendConLbl.Text = Localization.GetString("ConArgument", LocalResourceFile);
            legendCommentImg.ImageUrl = "images/comments_icon.gif";
            legendCommentLbl.Text = Localization.GetString("Comment", LocalResourceFile);

            TreeView1.CollapseImageToolTip = Localization.GetString("Collapse", LocalResourceFile);
            TreeView1.ExpandImageToolTip = Localization.GetString("Expand", LocalResourceFile);
            //btnReply.Text = Localization.GetString("Post", LocalResourceFile);
            ibxRemoveStartFiles.ToolTip = Localization.GetString("Cancel", LocalResourceFile);
            ibxSave.ToolTip = Localization.GetString("Save", LocalResourceFile);

            lbBody.Text = "<div class='dnnFormMessage dnnFormInfo'>" +
                           Localization.GetString("MsgChoose", LocalResourceFile) + "</div>";
            lbSubjectD.Text = Localization.GetString("Subject", LocalResourceFile);
            lbFiles.Text = Localization.GetString("AttachedFiles", LocalResourceFile);
            lbPostType.Text = Localization.GetString("ResponseType", LocalResourceFile);
            lbReply.Text = Localization.GetString("Description", LocalResourceFile);
            lbStartFiles.Text = Localization.GetString("AttachedFiles", LocalResourceFile);
            lbStartingPostType.Text = Localization.GetString("ResponseType", LocalResourceFile);
            lbStartingSubject.Text = Localization.GetString("Subject", LocalResourceFile);
            lbMessagePostHead.Text = Localization.GetString("PostTitle", LocalResourceFile);

            hdThreadId.Value = ATC.Tools.URLParam("Thread");
            ibxOpenTreePrint.Visible = false;
            LoadData();
            
            AlignTreeView();
            log.AddInfo("finished loading discussion tree " + ATC.Tools.URLParam("Thread"));  

            if (!Request.IsAuthenticated)
            {
                notifyCheck.Visible = false;
                notifyLabel.Visible = false;
            }
            if (!IsPostBack)
            {
                notifyCheck.Checked = checkUsedNotified(UserId.ToString(), ATC.Tools.URLParam("Thread"));
                IncViewCounter();
            }

        }
        private void AlignTreeView()
        {
            TreeNode tn = TreeView1.SelectedNode;
            string s = string.Empty;
        }

        private void LoadData()
        {
            string ThreadLinkCode = "";// ReadData("THREAD_POST_VIEW_THREAD_LINK");

            string PostLinkCode = "home"; // ReadData("THREAD_POST_VIEW_POST_LINK");
            // if (PostLinkCode == String.Empty) PostLinkCode = PageCode;

            try
            {
                string postAction = ATC.Tools.URLParam("PostAction").ToString();
                if (postAction != string.Empty)
                    ChangePostStatus(postAction);

                //btnReply.Visible = true;
                ibxOpenTreePrint.Visible = true;

                if (!Request.IsAuthenticated)
                {
                    ibxStartNewPost.Enabled = false;
                    ibxStartNewPost.Visible = false;
                    lbStartPosting.Visible = true;
                    lbStartPosting.Text = "<div class='dnnFormMessage dnnFormWarning'>" +
                        Localization.GetString("LoggedIn", LocalResourceFile) +
                     "  <a href='/" + System.Threading.Thread.CurrentThread.CurrentCulture.Name +
                     "/loginpage.aspx?returnurl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery) + "'>" +
                     Localization.GetString("Login", LocalResourceFile) + "</div>";
                }

                string msg = Server.UrlDecode(ATC.Tools.URLParam("msg"));
                string msgText = "<div class='dnnFormMessage dnnFormWarning'>" +
                    ATC.Translate.String("WARNING: The Forum is pre-moderated. Your post has been successfully submitted but will appear within a few hours, after approval by the Forum Moderators.", "", "EN") +
                    "</div>";
                //lblConfirm.Text = msg;
                if (msg == "alert")
                {
                    lbStartPosting.Text = msgText;
                    lbStartPosting.Visible = false;
                }
                string action = ATC.Tools.URLParam("Action").ToString();
                if (action == "newpost") PostReply();

                PrintHeadHeadDetails(true);
                #region Security
                string sThread = ATC.Tools.URLParam("Thread");
                string sSQLFirstLevel = string.Empty;
                string sSQL = string.Empty;
                string sSQLFilter = string.Empty;

                string ThreadID = ATC.Tools.URLParam("Thread");
                bool ThreadViewPermission = false;
                if (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == getUserIdByThread()
                    ||
                    (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == getUserIdByTopic())
                    ||
                    (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                   )
                {
                    ThreadViewPermission = true;
                }
                #endregion
                #region DrawPostDetails

                if (!Request.IsAuthenticated || ThreadViewPermission == false)
                {
                    sSQLFirstLevel = "SELECT [ID] FROM [uDebate_Forum_Posts] WHERE ThreadID=" + sThread + " and [IsPublished]=1 and Active=1 and ParentID=0";
                    sSQL = "SELECT [ID],[ParentID],[Subject],[PostDate],[Message],[PostType],[IsPublished],[UserID] FROM [uDebate_Forum_Posts] WHERE IsPublished=1  and Active=1 AND ThreadID=" + sThread;
                    sSQLFilter = "SELECT [ID] FROM [uDebate_Forum_Posts] WHERE IsPublished=1 and Active=1 AND ThreadID=" + sThread;
                }
                else
                {
                    sSQLFirstLevel = "SELECT [ID] FROM [uDebate_Forum_Posts] WHERE ThreadID=" + sThread + "  and ParentID=0";
                    sSQL = "SELECT [ID],[ParentID],[Subject],[PostDate],[Message],[PostType],[IsPublished],[UserID] FROM [uDebate_Forum_Posts] WHERE ThreadID=" + sThread;
                    sSQLFilter = "SELECT [ID] FROM [uDebate_Forum_Posts] WHERE ThreadID=" + sThread;
                }

                System.Data.DataSet dsTreeFirstLevel = ATC.Database.sqlExecuteDataSet(sSQLFirstLevel);
                System.Data.DataSet dsTree = ATC.Database.sqlExecuteDataSet(sSQL);
                if (dsTree.Tables[0].Rows.Count > 0)
                {
                    string sKeyValue = ATC.Database.sqlGetFirst(sSQLFilter);
                    HierarchicalXMLDataBuilder XmlData = new HierarchicalXMLDataBuilder();

                    foreach (DataRow rFirst in dsTreeFirstLevel.Tables[0].Rows)
                    {
                        XmlData.BuildMap(TreeView1.Nodes, dsTree, "ID", "ParentID", rFirst["ID"].ToString(), ThreadViewPermission, PanelStartPosting, tblForums, Request);
                    }
                    //XmlData.BuildMap(TreeView1.Nodes, dsTree, "ID", "ParentID", "62", ThreadViewPermission, PanelStartPosting, tblForums);
                }
                else //Code when starting a thread
                {
                    if (Request.IsAuthenticated)
                    {
                        PanelHolder.Visible = true;
                    }

                    tblForums.Visible = false;
                    if (!Request.IsAuthenticated)
                    {
                        PanelStartPosting.Visible = false;
                        PanelStartPostingButtons.Visible = false;
                    }
                    else
                    {
                        string sThread_Status = ATC.Database.sqlGetFirst("SELECT [STATUS] FROM [uDebate_Forum_Threads] where ID=" + hdThreadId.Value.ToString());
                        if (sThread_Status == "1")//Einai anoixto to thread
                        {
                            PanelStartPosting.Visible = true;
                            PanelStartPostingButtons.Visible = true;
                            postMessageView.Visible = false;
                            lbStartingPostType.Visible = true;
                            fillStartingPostTypesButtons();
                        }
                        else
                        {
                            PanelHolder.Visible = false;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + ex.Source);
            }
        }

        public void PrintHeadHeadDetails(bool FullDetail)
        {
            HyperLink editLink = new HyperLink();
            string ShowReferencesLink = "";
            string PreviousThread = "";
            bool DisplayThread = true; // (ReadData("THREAD_POST_VIEW_DISPLAY_THREAD") == "1");
            bool DisplayPost = true; // (ReadData("THREAD_POST_VIEW_DISPLAY_POST") == "1");
            bool userValid = false;
            string SQL = CreateSQLStatement(DisplayThread, DisplayPost);

            DataSet ds = ATC.Database.sqlExecuteDataSet(SQL);

            Forums.Controls.Clear();

            if (ds.Tables[0].Rows.Count == 0)
            {
                Forums.Controls.Add(new LiteralControl("<table cellspacing='0' cellpadding='0' width='100%'><tr><td>" +
                    Localization.GetString("NoData", LocalResourceFile) + "</td><tr></table>"));
                return;
            }

            Forums.Controls.Add(new LiteralControl("<table cellspacing='0' cellpadding='0' width='100%'>"));

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (
                        (DisplayThread && HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == row["Thread_User_ID"].ToString())
                        ||
                        (DisplayThread && HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == ATC.Database.sqlGetFirst("SELECT UserID FROM uDebate_Forum_Topics WHERE ID = " + row["Thread_TopicID"].ToString()))
                        ||
                        (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                    )
                {
                    userValid = true;

                    editLink.NavigateUrl = "/tabid/" + PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                           "/TopicID/" + row["Thread_TopicID"].ToString() + "/ThreadID/" + row["Thread_ID"].ToString() +
                           "/language/" + System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/default.aspx";
                    editLink.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";
                    editLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(editLink.NavigateUrl, this, PortalSettings, true, false));

                    /* Summary Button */
                    threadSummaryBtn.Visible = true;
                    threadSummaryBtn.NavigateUrl = "/tabid/" + PortalSettings.ActiveTab.TabID + "/ctl/TreeViewSummary/mid/" + ModuleId +
                            "/ThreadID/" + row["Thread_ID"].ToString() +
                            "/language/" + System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/uDebate.aspx";
                }
                else
                {
                    /* HIde summary Button if not pilot leader*/
                    threadSummaryBtn.Visible = false;
                }

                if (row["Thread_Status"].ToString() == "2")
                {
                    //btnReply.Visible = false;
                    lbBody.Text = "This Thread is Closed.";
                    lbBodyFiles.Visible = false;
                }
                if (DisplayThread && PreviousThread != row["Thread_ID"].ToString())
                {
                    PreviousThread = row["Thread_ID"].ToString();

                    string contact = "";
                    //if (Request.IsAuthenticated)
                    //    contact = "<input type='button' onclick=\"popup('SendMail.aspx?mode=contact&thread=" + row["Thread_ID"].ToString() + "','SendEmail')\" class='forum_button' value='" + ATC.Translate.String("Contact Email", "", CurrentLanguageCode) + "' />&nbsp;";

                    //ATC.WebControls.ImageButtonExt ibContact = new ATC.WebControls.ImageButtonExt();
                    //ibContact.ID = "ibVote";
                    //if (CurrentLanguageCode == "LANG1")
                    //{
                    //    ibContact.ImageUrl = "images/buttons/email_norm_gr.gif";
                    //    ibContact.ImageOverUrl = "images/buttons/email_roll_gr.gif";
                    //}
                    //else
                    //{
                    //    ibContact.ImageUrl = "images/buttons_en/email_norm_en.gif";
                    //    ibContact.ImageOverUrl = "images/buttons_en/email_roll_en.gif";
                    //}
                    //ibContact.OnClientClick = "popup('SendMail.aspx?mode=contact&thread=" + row["Thread_ID"].ToString() + "','SendEmail');";


                    string HeadDisplayMoreDetails = " - <A HREF=http://" + Request.Url.Host.ToString() + "/tabid/" + PortalSettings.ActiveTab.TabID
                        + "/ctl/TreeView/mid/" + ModuleId + "/Thread/" + ATC.Tools.URLParam("Thread") + "/language/" +
                        System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/uDebate.aspx" + " class='TopMenuLink'>Details</A>";
                    /*Forums.Controls.Add(new LiteralControl(@" <tr>
                                                                <td class='topImage tImg_" + System.Threading.Thread.CurrentThread.CurrentCulture.Name));
                    */
                    Forums.Controls.Add(new LiteralControl(@" 
                                                                <tr>
                                                                    <td class='threads_posts_thread_outer_td'>
                                                                        <table width='100%' cellpadding='0' cellspacing='0' border='0' style='float:left' class='threads_posts_thread_table'>
                                                                            <tr>
                                                                                <td colspan='2' class='thread_posts_thread_desc'>
                                                                                    "));
                    Forums.Controls.Add(editLink);
                    Forums.Controls.Add(new LiteralControl(@"" + row["Thread_Desc"].ToString()));
                    if (FullDetail == false)
                    {
                        Forums.Controls.Add(new LiteralControl(HeadDisplayMoreDetails));
                        Forums.Controls.Add(new LiteralControl(@"<tr>
                                                                    <td colspan='2' class='thread_posts_thread_contact'></td></tr>"));
                    }

                    Forums.Controls.Add(new LiteralControl(@"</td> </tr>"));
                    if (FullDetail == true)
                    {
                        Forums.Controls.Add(new LiteralControl(@"
                                                                            <tr>
                                                                                <td colspan='2' class='thread_posts_thread_contact'>"));

                        //if (Request.IsAuthenticated)
                        //{
                        //    Forums.Controls.Add(ibContact);
                        //}
                        Forums.Controls.Add(new LiteralControl(@"</td>
                                                               </tr><tr>
                                                                   <td class='thread_posts_thread_summary'>" + Localization.GetString("Summary", LocalResourceFile) + ": " + row["Thread_Sum"].ToString() + @"</td>
                                                                   <td class='thread_posts_thread_coordinator' align='right'>" /*+ Localization.GetString("Coordinator", LocalResourceFile) + ": "*/
                                                                        + getUserId(row["Post_ID"].ToString()) +
                                                                    @"</td>
                                                               </tr>"));
                        Forums.Controls.Add(new LiteralControl(@"
                                                             <tr>
                                                                 <td colspan='2' class='thread_posts_thread_text'>" + Server.HtmlDecode(row["Thread_Text"].ToString()) + @"</td>
                                                             </tr>"));

                        Forums.Controls.Add(new LiteralControl(@"                                         
                                                   <td colspan='2' class='thread_posts_thread_date'>" +
                                                   Localization.GetString("Posts", LocalResourceFile) + ": " +
                                                   ATC.Database.sqlGetFirst("SELECT Count(ID) FROM uDebate_Forum_Posts WHERE Active = 1 AND IsPublished = 1 AND ThreadID = " +
                                                   ATC.Tools.IntURLParam("Thread").ToString()) +
                                                   @", " + Localization.GetString("Views", LocalResourceFile) + ": " +
                                                   ATC.Database.sqlGetFirst("SELECT View_Count FROM uDebate_Forum_Threads WHERE ID = " +
                                                   ATC.Tools.IntURLParam("Thread").ToString()) + @", "
                                                   + Localization.GetString("OpenedDate", LocalResourceFile) + ": " + DateToDayMonthYear(row["Opened_Date"].ToString()) + (row["Closed_Date"].ToString() != string.Empty && row["Thread_Status"].ToString() == "2" ? ", " + ATC.Translate.String("Closed date: ", "", "EN") + DateToDayMonthYear(row["Closed_Date"].ToString()) : "") +
                                               @"</td>
                                               </tr>"));
                    }
                    Forums.Controls.Add(new LiteralControl(@"</table>              
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td height='7'></td>
                                                                </tr>
                                                                "));
                }
                string delete = "";
                string href = ATC.Tools.GetParam("RootURL") + "?page=" + ATC.Tools.URLParam("page") + "&Thread=" + ATC.Tools.URLParam("Thread");
                if (!(
                        (DisplayThread && Request.IsAuthenticated && (Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == row["Thread_User_ID"].ToString()))
                        ||
                        (DisplayThread && Request.IsAuthenticated && (Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == ATC.Database.sqlGetFirst("SELECT UserID FROM uDebate_Forum_Topics WHERE ID = " + row["Thread_TopicID"].ToString())))
                        ||
                        (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                    ))
                {
                    if (row["Post_IsPublished"].ToString() == "0") continue;
                }
                else
                {
                    delete = "<input type='button' onclick=\"DeletePost('" + href + "&PostAction=Delete&Post=" +
                        row["Post_ID"].ToString() + "')\" class='forum_button' value='Svisimo" /*+ ATC.Translate.String("Delete", "", CurrentLanguageCode)*/ + "' />";
                }

                string state = (row["Post_IsPublished"].ToString() == "0" ? "<span style='color:Red;'>NOT PUBLISHED</span>&nbsp;-&nbsp;<span class='forum_link'><a href='"
                    + href + "&PostAction=Accept&Post=" + row["Post_ID"].ToString() +
                    "' class='forum_link'>Accept</a></span>&nbsp;|&nbsp;<span class='forum_link'><a href='" + href +
                    "&PostAction=Reject&Post=" + row["Post_ID"].ToString() + "' class='forum_link'>Reject</a></span>" : "");

            }
            Forums.Controls.Add(new LiteralControl("</table>"));

        }

        public string getThreadGroupById()
        {
            string sThreadGroupTitle = ATC.Database.sqlGetFirst(@"SELECT [uDebate_Forum_ThreadGroup].[Title] FROM [uDebate_Forum_Threads], [uDebate_Forum_ThreadGroup]
                                                    where [uDebate_Forum_Threads].id=" + ATC.Tools.URLParam("Thread") +
                                                   " AND [uDebate_Forum_ThreadGroup].[ID]=[uDebate_Forum_Threads].[GroupID]");
            return sThreadGroupTitle;
        }

        public string getThreadStatusById()
        {
            string sThreadGroupTitle = ATC.Database.sqlGetFirst(@"SELECT [uDebate_Forum_ThreadStatus].[Title] FROM [uDebate_Forum_Threads], [uDebate_Forum_ThreadStatus]
                                                    where [uDebate_Forum_Threads].id=" + ATC.Tools.URLParam("Thread") +
                                                    " AND [uDebate_Forum_ThreadStatus].[ID]=[uDebate_Forum_Threads].[StatusID]");
            return sThreadGroupTitle;
        }

        public class HierarchicalXMLDataBuilder
        {
            private string ParentColumn;
            private string ChildColumn;
            private int NoOfColumns;
            private DataSet Data;
            private DataView vuData;
            private HttpRequest RequestUrl;

            //XmlData.BuildMap(TreeView1.Nodes, dsTree, "ID", "ParentID", sKeyValue, ThreadViewPermission, PanelStartPosting, tblForums);
            public void BuildMap(TreeNodeCollection nodes, DataSet data, string IdentityColumn, string ReferenceColumn,
                                string KeyValue, bool ThreadViewPermission, Panel StartingPanel, HtmlTable ForumsTable, HttpRequest RequestUrl)
            {
                this.ParentColumn = IdentityColumn;
                this.ChildColumn = ReferenceColumn;
                this.Data = data;
                this.RequestUrl = RequestUrl;
                vuData = new DataView(data.Tables[0]);

                BuildChildNodes(nodes, vuData, ParentColumn + " = " + KeyValue, ThreadViewPermission, StartingPanel, ForumsTable);
            }

            private void BuildChildNodes(TreeNodeCollection nodes, DataView DtView, string filter, bool ThreadViewPermission, Panel StartingPanel, HtmlTable ForumsTable)
            {
                //string href = ATC.Tools.GetParam("RootURL") + "?page=" + ATC.Tools.URLParam("page") + "&Thread=" + ATC.Tools.URLParam("Thread");

                string rURL = RequestUrl.Url.OriginalString;
                string href = rURL.Substring(0, RequestUrl.Url.AbsoluteUri.IndexOf("Thread") + 10) + ATC.Tools.URLParam("Thread");

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

                    int iCount = 0;

                    for (RowCounter = 0; RowCounter <= DtView.Count - 1; RowCounter++)
                    {
                        TreeNode tn = new TreeNode();

                        if (ThreadViewPermission)
                        {
                            /* if post is published admins can unpublish or delete */
                            if (DtView[RowCounter]["IsPublished"].ToString() == "1")
                            {
                                state = "<span class='forum_link'><a href='" 
                                      + href + "&PostAction=Reject&Post=" + DtView[RowCounter]["ID"].ToString()
                                      + "' class='admin_link'>Unpublish</a></span>&nbsp;-&nbsp;<span class='forum_link'><a href='" 
                                      + href + "&PostAction=Delete&Post="
                                      + DtView[RowCounter]["ID"].ToString() + "' class='admin_link'>Delete</a></span>&nbsp;";
                            }
                            else // Post unpublished
                            {
                                state = "<span class='forum_link'><a href='" 
                                      + href + "&PostAction=Accept&Post=" + DtView[RowCounter]["ID"].ToString() 
                                      +  "' class='admin_link'>Re-publish</a></span>&nbsp;|&nbsp;<span class='forum_link'><a href='" 
                                      + href + "&PostAction=Delete&Post="
                                      + DtView[RowCounter]["ID"].ToString() + "' class='admin_link'>Delete</a></span>&nbsp;";
                            }
                        }
                        sFieldName = DtView[RowCounter]["Subject"].ToString() + " " + state;
                        sId = DtView[RowCounter]["ID"].ToString();
                        sParentId = DtView[RowCounter]["ParentID"].ToString();
                        sMessage = DtView[RowCounter]["Message"].ToString();
                        sImageUrl = DtView[RowCounter]["PostType"].ToString();
                        sImageUrl = getImageIconUrl(sImageUrl);

                        tn.Text = sFieldName;
                        tn.Value = sId;

                        //tn.ToolTip = sMessage;
                        tn.ImageUrl = sImageUrl;

                        nodes.Add(tn);
                        tn.ToggleExpandState();

                        DataView vuData2 = new DataView(Data.Tables[0]);
                        if (DtView.Count > RowCounter)
                        {
                            BuildChildNodes(tn.ChildNodes, vuData2, ChildColumn + " = " +
                                DtView[RowCounter][ParentColumn], ThreadViewPermission, StartingPanel, ForumsTable);
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
                return "images/" + sOut;
            }
        }

        private void IncViewCounter()
        {
            string ThreadID = ATC.Tools.IntURLParam("Thread", "-1").ToString();
            if (ThreadID == "-1" || DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders")
                || DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                return;

            try
            {
                ATC.Database.sqlExecuteCommand("UPDATE uDebate_Forum_Threads SET View_Count = View_Count + 1 WHERE ID=" +
                    ThreadID + " AND View_Count is not null");
            }
            catch
            {
            }
        }

        public string getPostTypeIdByThread()
        {
            string sThread = ATC.Tools.URLParam("Thread");
            string sKeyValue = ATC.Database.sqlGetFirst(@"SELECT uDebate_Forum_PostTypes.ID 
                                            FROM uDebate_Forum_Threads, uDebate_Forum_PostTypes
                                            WHERE uDebate_Forum_Threads.ID=" + sThread + @"
                                            AND uDebate_Forum_Threads.GroupID=uDebate_Forum_PostTypes.ThreadGroupID
                                            AND uDebate_Forum_PostTypes.StartingType=1");
            return sKeyValue;
        }

        private string CreateSQLStatement(bool DisplayThread, bool DisplayPost)
        {
            string SQL = "";
            try
            {
                string SelectStatement = "";

                //Select statement
                SelectStatement = @"SELECT ";
                SelectStatement += " [Th].ID AS Thread_ID, [Th].Description AS Thread_Desc, [Th].Summary AS Thread_Sum, [Th].Text AS Thread_Text, User_Thread.Username AS Thread_User, [Th].UserID AS Thread_User_ID, [Th].Opened_Date AS Opened_Date, [Th].Closed_Date AS Closed_Date, [Th].Complain_Email AS Complaint_Email, [Th].Contact_Email AS Contact_Email, User_Thread.Lastname AS Surname, User_Thread.Firstname AS Name, [Th].Status AS Thread_Status, [Th].TopicID as Thread_TopicID,";

                if (DisplayPost)
                    SelectStatement += " P.ID AS Post_ID, P.Subject AS Post_Sub, P.Message AS Post_Msg, P.PostDate AS Post_Date, User_Post.Username AS Post_User, P.UserID AS Post_User_ID,[P].IsPublished AS Post_IsPublished ";
                else
                    SelectStatement += " NULL AS Post_ID, NULL AS Post_Sub, NULL AS Post_Msg, NULL AS Post_Date, NULL AS Post_User, NULL AS Post_User_ID, NULL AS Post_IsPublished ";
                SelectStatement = SelectStatement.TrimEnd(',');

                //From statement
                SelectStatement += " FROM ";
                SelectStatement += " uDebate_Forum_Threads AS Th ";
                SelectStatement += " INNER JOIN Users AS User_Thread ON Th.UserID = User_Thread.UserID ";
                if (DisplayPost) SelectStatement += " LEFT OUTER JOIN uDebate_Forum_Posts AS P ON Th.ID = P.ThreadID AND [P].Active = 1 ";
                if (DisplayPost)
                    SelectStatement += " LEFT OUTER JOIN Users AS User_Post ON P.UserID = User_Post.UserID ";


                string WhereStatement = "";
                //Forum ID
                string ThreadID = "-1";// ReadData("THREAD_POST_VIEW_ID");
                if (ThreadID == "-1") ThreadID = ATC.Tools.IntURLParam("Thread"); //Current Forum
                if (ThreadID != String.Empty)
                {
                    WhereStatement += " AND [Th].ID = " + ThreadID;
                }

                //SelectStatement += " AND [P].Active = 1 ";

                //Status (Open Closed)
                string Status = "3";// ReadData("THREAD_POST_VIEW_STATUS");
                if (Status != "3" && Status != String.Empty)
                {
                    if (DisplayThread && DisplayPost) SelectStatement += " AND P.Status = " + Status;
                    else if (DisplayThread) WhereStatement += " AND [Th].Status = " + Status;
                    else if (DisplayPost) SelectStatement += " AND [P].Status = " + Status;
                }
                if (WhereStatement != String.Empty) WhereStatement = " WHERE " + WhereStatement.Substring(4);

                string OrderByStatement = "";
                //Order By 
                string OrderBy = "OPENED_DATE";// ReadData("THREAD_POST_VIEW_ORDER_FIELD");
                if (OrderBy != String.Empty)
                {
                    OrderByStatement += " ORDER BY ";
                    if (DisplayThread) OrderByStatement += " [Th]." + OrderBy + ",";
                    if (DisplayPost) OrderByStatement += " [P].PostDate,";
                    OrderByStatement = OrderByStatement.TrimEnd(',');
                }

                SQL = SelectStatement + " " + WhereStatement + " " + OrderByStatement;
                //Response.Write(SQL);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

            return SQL;
        }

        public void PostReply()
        {
            if (!Request.IsAuthenticated)
                return;

            PanelReply.Visible = true;
            PanelReplyButtons.Visible = true;
            lbReply.Visible = false;
            txtReply.Focus();
            lblConfirm.Text = "";
        }

        public void ClearStartingReply()
        {
            PanelReply.Visible = false;
            PanelReplyButtons.Visible = false;
            txtStartingReply.Text = "";
            txtStartingSubject.Text = "";
            lblStartingError.Text = "";
        }

        public void ClearReply()
        {
            txtReply.Text = "";
            lblError.Text = "";
            PanelHolder.Visible = false;
            ibxStartNewPost.Visible = true;
        }

        protected void lnkSave_Click(object sender, EventArgs e)
        {
            SaveReply();
        }

        public void PostNewSub()
        {
            if (!Request.IsAuthenticated)
                return;

            PanelReply.Visible = true;
            PanelReplyButtons.Visible = true;
            lbReply.Visible = true;
            txtReply.Focus();
        }

        public void SaveReply()
        {
            if (Request.IsAuthenticated)
            {
                PanelHolder.Visible = true;
            }
            PrintHeadHeadDetails(false);

            if (!Request.IsAuthenticated) return;

            /*Basic Form Fields Validation
             * For con and pro arguments we don't need subject and text
             * For issue,alternative,comment everything is obligatory
             */
            bool fieldChecker = true;
            string sChecker = string.Empty;

            if (txtSubjectPost.Text == "" && (txtPostTypeId.Value.Trim() == "1" ||
                txtPostTypeId.Value.Trim() == "2" || txtPostTypeId.Value.Trim() == "8"))
            {
                sChecker = sChecker + Localization.GetString("SubjectHelp", LocalResourceFile);
            }

            if (txtReply.Text.Trim() == "" && (txtPostTypeId.Value.Trim() == "1" ||
                txtPostTypeId.Value.Trim() == "2" || txtPostTypeId.Value.Trim() == "8"))
            {
                sChecker = sChecker + "<BR/>" + Localization.GetString("DescHelp", LocalResourceFile);
            }

            if (txtPostTypeId.Value.Trim() == "")
            {
                sChecker = sChecker + "<BR/>" + Localization.GetString("TypeHelp", LocalResourceFile);
            }

            if (txtSubjectPost.Text == "" && fieldChecker == true && (txtPostTypeId.Value.Trim() == "1" ||
                txtPostTypeId.Value.Trim() == "2" || txtPostTypeId.Value.Trim() == "8"))
            {
                fieldChecker = false;
            }

            if (txtReply.Text.Trim() == "" && fieldChecker == true && (txtPostTypeId.Value.Trim() == "1" ||
                txtPostTypeId.Value.Trim() == "2" || txtPostTypeId.Value.Trim() == "8"))
            {
                fieldChecker = false;
            }

            if (txtPostTypeId.Value.Trim() == "")
            {
                fieldChecker = false;
            }
            lblError.Text = "<font color=Red size=2>" + sChecker + "</font>";

            if (fieldChecker == false) return;
            //Basic Form Fields Validation/////////////////

            string SQL = "";
            string UserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString(); //((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString();
            string ThreadID = ATC.Tools.URLParam("Thread");
            string ParentID = hdParentId.Value;
            // 1: Posts are published immediately after posting
            // 0: Pre-moderated forum. Posts must be approved before being published
            string published = "1";// (ProjectSettings.OPT_Publish_ForumPosts == true ? "0" : "1");

            txtSubjectPost.Text = txtSubjectPost.Text.Replace("'", "\"");
            txtReply.Text = txtReply.Text.Replace("'", "\"");
            string sSubject = txtSubjectPost.Text;
            string PostType = txtPostTypeId.Value;

            #region Adding in DB
            //Writing Post and Files in db
            IDbConnection Connection = Database.GenerateConnection();
            Connection.Open();
            IDbTransaction Transaction = Connection.BeginTransaction();

            SQL = string.Format(@"
                    INSERT INTO uDebate_Forum_Posts (ThreadID,ParentID,UserID,PostLevel,SortOrder,Subject,Message,PostDate
                        ,IsPublished,PostType,Active,Published_Date,Complaint_Count,ModuleID)
                    VALUES (" + ThreadID + @"," + ParentID + @"," + UserID + @", 1, 1, {0}Subject, {0}Message
                        , getDate()," + published + @"," + PostType + @", 1, getDate(), 0," + ModuleId + @")
                   ", ATC.Database.Sql_VariableIdent);

            try
            {
                ATC.Database.sqlExecuteCommandParams(SQL, Connection,
                    new string[] { ATC.Database.Sql_VariableIdent + "Subject", ATC.Database.Sql_VariableIdent + "Message" },
                    new object[] { txtSubjectPost.Text.Trim(), txtReply.Text.Trim() }, Transaction);

                string sPostId = ATC.Database.sqlGetFirst("SELECT TOP 1 [ID] FROM [uDebate_Forum_Posts] order by ID DESC", Connection, Transaction);

                //saveUserFilesInPost(Connection, Transaction, sPostId, false);

                Transaction.Commit();

                ClearReply();

                if (true/*ProjectSettings.OPT_Publish_ForumPosts*/)
                {

                    string fromAddress = "info@puzzledbypolicy.eu";
                    string subject = "PuzzledByPolicy - New Post";
                    string body = "New post added at http://join.puzzledbypolicy.eu/tabid/" +
                                PortalSettings.ActiveTab.TabID + "/ctl/TreeView/mid/" + ModuleId +
                                "/Thread/" + ATC.Tools.IntURLParam("Thread") + "/language/" +
                                System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/uDebate.aspx";


                    switch (getPostLanguageByThread(ThreadID).ToLower())
                    {
                        case "el-gr": Mail.SendEmail(fromAddress, "i.giannakoudaki@daem.gr", subject, body);
                            Mail.SendEmail(fromAddress, "l.kallipolitis@atc.gr", subject, body);
                            break;
                        case "es-es": Mail.SendEmail(fromAddress, "enielsen@ull.es", subject, body);
                            Mail.SendEmail(fromAddress, "cmartinv@ull.es", subject, body);
                            Mail.SendEmail(fromAddress, "vzapata@ull.es", subject, body);
                            Mail.SendEmail(fromAddress, "l.kallipolitis@atc.gr", subject, body);
                            break;
                        case "it-it": Mail.SendEmail(fromAddress, "stefano.moro@csi.it", subject, body);
                            Mail.SendEmail(fromAddress, "l.kallipolitis@atc.gr", subject, body);
                            break;
                        case "hu-hu": Mail.SendEmail(fromAddress, "Takacs.GyulaPeter@nisz.hu", subject, body);
                            Mail.SendEmail(fromAddress, "l.kallipolitis@atc.gr", subject, body);
                            break;
                        case "en-gb": Mail.SendEmail(fromAddress, "matej.delakorda@inepa.si", subject, body);
                            Mail.SendEmail(fromAddress, "mateja.delakorda@inepa.si", subject, body);
                            Mail.SendEmail(fromAddress, "l.kallipolitis@atc.gr", subject, body);
                            break;
                        default: Mail.SendEmail(fromAddress, "l.kallipolitis@atc.gr", subject, body);
                            break;
                    }

                    /* Send an email to all the subscribed users of this thread*/ 
                    string subjectNotify = "PuzzledByPolicy - There is a new post in the thread you are following";
                    string bodyNotify = "Hi, <br /><br />A new post was added at http://join.puzzledbypolicy.eu/tabid/" +
                                PortalSettings.ActiveTab.TabID + "/ctl/TreeView/mid/" + ModuleId +
                                "/Thread/" + ATC.Tools.IntURLParam("Thread") + "/language/" +
                                System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/uDebate.aspx";

                    string SQL_notified = "SELECT userID,userEmail FROM uDebate_Forum_Notifications where threadID=" + ATC.Tools.URLParam("Thread");
                    try
                    {
                        DataSet ds = ATC.Database.sqlExecuteDataSet(SQL_notified);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                // Only send email to users different than the current one (the post writer)
                                if(UserId != Int32.Parse(row["userID"].ToString()))
                                    Mail.SendEmail(fromAddress, row["userEmail"].ToString(), subjectNotify, bodyNotify);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.Message);
                    }


                    /* Notify moderators of the new post*/


                    Mail.SendEmail("info@puzzledbypolicy.eu", "l.kallipolitis@atc.gr", subject, body);


                    Response.Redirect("/tabid/" + PortalSettings.ActiveTab.TabID +
                                                     "/ctl/TreeView/mid/" + ModuleId +
                                                     "/Thread/" + ATC.Tools.IntURLParam("Thread") + "/language/" +
                                                     System.Threading.Thread.CurrentThread.CurrentCulture.Name +
                                                     "/uDebate.aspx?msg=alert#post");
                }
                else
                {
                    Response.Redirect("?page=" + ATC.Tools.URLParam("page") + "&Thread=" + ATC.Tools.IntURLParam("Thread") + "#post");
                }
            }
            catch (InvalidOperationException ex)
            {
                Response.Write(ex.Message);
            }
            catch (Exception ex)
            {
                // Transaction.Rollback();  TODO: check if this can come back
                Response.Write(ex.Message);
            }

            finally
            {
                Connection.Close();
            }
            #endregion
        }


        // Take all attachments from the attachment control and change their associated post-id to be the id of 
        // the new post that was just created
        public void saveUserFilesInPost(IDbConnection Connection, IDbTransaction Transaction, string PostId, bool isNewPost)
        {
            string lstAttachmentIDs = string.Empty;

            // Check if it is a New Discussion or a new Post and take the respective Attachmetn control
            if (!isNewPost)
                lstAttachmentIDs = ctlAddAttachment.lstAttachmentIDs; // comma separated list of ids
            else
                lstAttachmentIDs = ctlNewAttachment.lstAttachmentIDs;

            if (lstAttachmentIDs != string.Empty)
            {
                string[] stringSeparators = new string[] { ";" };
                string[] attachIDs = lstAttachmentIDs.Split(stringSeparators, System.StringSplitOptions.None);

                foreach (string attachID in attachIDs)
                {
                    if (attachID != string.Empty)
                    {
                        String cmd = string.Empty;
                        cmd = "UPDATE uDebate_Attachments SET PostId = " + PostId + "WHERE AttachmentID = " + attachID;
                        ATC.Database.sqlExecuteCommand(cmd, Connection, Transaction);
                    }
                }
            }
        }


        private string DateToDayMonthYear(string inDate)
        {
            string sOut = string.Empty;
            try
            {
                DateTime myDt = Convert.ToDateTime(inDate);
                int iDay = 0;
                int iMonth = 0;
                int iYear = 0;

                iDay = myDt.Day;
                iMonth = myDt.Month;
                iYear = myDt.Year;
                sOut = iDay.ToString() + "/" + iMonth.ToString() + "/" + iYear.ToString();
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getFileTypeByExt(string path)
        {
            string sExt = string.Empty;
            FileInfo myfile = new FileInfo(path);
            sExt = myfile.Extension;
            switch (sExt)
            {
                case ".doc":
                    return "msword";
                    break;
                case ".DOC":
                    return "msword";
                    break;
                case ".docx":
                    return "msword";
                    break;
                case ".DOCX":
                    return "msword";
                    break;

                case ".XLSX":
                    return "excel";
                    break;
                case ".xlsx":
                    return "excel";
                    break;

                case ".XLS":
                    return "excel";
                    break;
                case ".xls":
                    return "excel";
                    break;

                case ".txt":
                    return "text/plain";
                    break;
                case ".TXT":
                    return "text/plain";
                    break;

                case ".pdf":
                    return "pdf";
                    break;
                case ".PDF":
                    return "pdf";
                    break;
                default:
                    return "text/plain";
                    break;
            }
        }

        private void SaveUserFileToDB(IDbConnection myConn, IDbTransaction Transaction,
                                        string userTitle, string imgName, byte[] fileBinary, string fileType, string PostId)
        {
            IDbCommand cmd = myConn.CreateCommand();
            cmd.CommandText = "INSERT INTO uDebate_Forum_Post_Files (UserTitle,FileTitle,FileBinary,FileType,FileSize,PostId) VALUES (@UserTitle,@FileTitle,@FileBinary,@FileType,@FileSize,@PostId)";

            ATC.Database.sqlExecuteCommandParams(cmd.CommandText, myConn,
              new string[] { 
                  ATC.Database.Sql_VariableIdent + "UserTitle", 
                  ATC.Database.Sql_VariableIdent + "FileTitle", 
                  "FileBinary",
                  ATC.Database.Sql_VariableIdent + "FileType",
                  ATC.Database.Sql_VariableIdent + "FileSize",
                  ATC.Database.Sql_VariableIdent + "PostId" },

              new object[] { 
                  userTitle, 
                  imgName,
                  fileBinary,
                  fileType,
                  fileBinary.Length.ToString(),
                  PostId},
              Transaction);
        }

        public void SaveStartingReply()
        {
            if (!Request.IsAuthenticated)
                return;

            //Basic Form Fields Validation/////////////////
            bool fieldChecker = true;
            string sChecker = string.Empty;

            if (txtStartingSubject.Text == "")
            {
                sChecker = sChecker + Localization.GetString("SubjectHelp", LocalResourceFile);
            }

            if (txtStartingReply.Text.Trim() == "")
            {
                sChecker = sChecker + "<BR/>" + Localization.GetString("DescHelp", LocalResourceFile);
            }
            if (txtPostTypeId.Value.Trim() == "")
            {
                sChecker = sChecker + "<BR/>" + Localization.GetString("TypeHelp", LocalResourceFile);
            }

            if (txtStartingSubject.Text == "" && fieldChecker == true)
            {
                fieldChecker = false;
            }

            if (txtStartingReply.Text.Trim() == "" && fieldChecker == true)
            {
                fieldChecker = false;
            }
            if (txtPostTypeId.Value.Trim() == "" && fieldChecker == true)
            {
                fieldChecker = false;
            }

            lblStartingError.Text = "<font color=Red size=2>" + sChecker + "</font>";

            if (fieldChecker == false) return;
            //Basic Form Fields Validation/////////////////

            string SQL = "";
            string UserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();
            string ThreadID = ATC.Tools.URLParam("Thread");
            string ParentID = hdParentId.Value;
            // Post Moderated forum
            string published = "1";// (ProjectSettings.OPT_Publish_ForumPosts == true ? "0" : "1");
            if (ParentID == "") { ParentID = "0"; }
            txtStartingSubject.Text = txtStartingSubject.Text.Replace("'", "\"");
            txtStartingReply.Text = txtStartingReply.Text.Replace("'", "\"");
            string sSubject = txtStartingSubject.Text;
            string PostType = txtPostTypeId.Value;

            #region Adding in DB
            //Writing Post and Files in db
            IDbConnection Connection = Database.GenerateConnection();
            Connection.Open();
            IDbTransaction Transaction = Connection.BeginTransaction();

            SQL = string.Format(@"
                    INSERT INTO uDebate_Forum_Posts (ThreadID,ParentID,UserID,PostLevel,SortOrder,Subject,Message,PostDate
                        ,IsPublished,PostType,Active,Published_Date,Complaint_Count,ModuleID)
                    VALUES (" + ThreadID + @"," + ParentID + @"," + UserID + @", 1, 1, {0}Subject, {0}Message
                        , getDate()," + published + @"," + PostType + @", 1, getDate(), 0," + ModuleId + @")
                   ", ATC.Database.Sql_VariableIdent);
            try
            {
                ATC.Database.sqlExecuteCommandParams(SQL, Connection,
                    new string[] { ATC.Database.Sql_VariableIdent + "Subject", ATC.Database.Sql_VariableIdent + "Message" },
                    new object[] { txtStartingSubject.Text.Trim(), txtStartingReply.Text.Trim() }, Transaction);

                string sPostId = ATC.Database.sqlGetFirst("SELECT TOP 1 [ID] FROM [uDebate_Forum_Posts] order by ID DESC", Connection, Transaction);

                saveUserFilesInPost(Connection, Transaction, sPostId, true);

                Transaction.Commit();

                ClearReply();

                if (true/*ProjectSettings.OPT_Publish_ForumPosts*/)
                {
                    Response.Redirect("/tabid/" + PortalSettings.ActiveTab.TabID +
                                                     "/ctl/TreeView/mid/" + ModuleId +
                                                     "/Thread/" + ATC.Tools.IntURLParam("Thread") + "/language/" +
                                                     System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/uDebate.aspx?msg=alert#post");
                }
                else
                {
                    Response.Redirect("?page=" + ATC.Tools.URLParam("page") + "&Thread=" + ATC.Tools.IntURLParam("Thread") + "#post");
                }
            }
            catch (Exception ex)
            {
                // Transaction.Rollback();
                Response.Write(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            #endregion
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearReply();
        }

        protected void ibxSave_Click(object sender, EventArgs e)
        {
            SaveReply();
        }
        public void ChangePostStatus(string postAction)
        {
            string postID = ATC.Tools.IntURLParam("Post", "-1").ToString();
            if (postID != "-1")
            {
                string published = "";
                string active = "";

                if (postAction == "Accept")
                {
                    active = "1";
                    published = "1";
                    string SQL = "UPDATE uDebate_Forum_Posts SET Published_Date = GetDate(), IsPublished = " +
                                  published + ", Active = " + active + " WHERE ID = " + postID;
                    ATC.Database.sqlExecuteCommand(SQL);
                }
                else if (postAction == "Reject")
                {
                    active = "0";
                    published = "0";
                    string SQL = "UPDATE uDebate_Forum_Posts SET IsPublished = " + published + ", Active = " + active + " WHERE ID = " + postID;
                    ATC.Database.sqlExecuteCommand(SQL);


                    string notifySQL = @"SELECT Users.Email FROM Users 
                                         INNER JOIN uDebate_Forum_Posts on Users.UserID=uDebate_Forum_Posts.UserID
                                         WHERE uDebate_Forum_Posts.ID =" + postID;

                    string notifyEmail = ATC.Database.sqlGetFirst(notifySQL);

                    string notifyBody = String.Format(Localization.GetString("UnpublishedBody", LocalResourceFile),
                                        "http://join.puzzledbypolicy.eu/tabid/" + PortalSettings.ActiveTab.TabID 
                                        + "/ctl/TreeView/mid/" + ModuleId + "/Thread/" + ATC.Tools.IntURLParam("Thread") 
                                        + "/language/" + System.Threading.Thread.CurrentThread.CurrentCulture.Name + "/uDebate.aspx");

                    Mail.SendEmail("info@puzzledbypolicy", notifyEmail, Localization.GetString("UnpublishedSubject", LocalResourceFile),
                                    notifyBody);

                    return;
                }
                else if (postAction == "Delete")
                {
                    ATC.Database.sqlExecuteCommand("UPDATE uDebate_Forum_Threads SET Delete_Count = Delete_Count + 1 WHERE ID = (SELECT ThreadID FROM uDebate_Forum_Posts WHERE ID = " + postID + ")");
                    ATC.Database.sqlExecuteCommand("DELETE FROM uDebate_Forum_Posts WHERE ID = " + postID);
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            string sThread_Status = ATC.Database.sqlGetFirst("SELECT [STATUS] FROM [uDebate_Forum_Threads] where ID=" +
                                   hdThreadId.Value.ToString());

            // uDebate: Update the list of attachments for the attachment control
            ctlAddAttachment.PostId = Convert.ToInt32(this.TreeView1.SelectedNode.Value);

            if (Request.IsAuthenticated && sThread_Status == "1")
            {
                PanelHolder.Visible = true;
                postMessageView.Visible = true;
            }
            PrintHeadHeadDetails(false);

            string sSQL = string.Empty;
            string sReMessage = string.Empty;

            lbStartPosting.Visible = false;
            if (sThread_Status == "1")//Einai anoixto to thread
            {
                hdParentId.Value = this.TreeView1.SelectedNode.Value;
                sSQL = "SELECT [Subject] FROM [uDebate_Forum_Posts] where ID=" + this.TreeView1.SelectedNode.Value;
                try
                {
                    sReMessage = ATC.Database.sqlGetFirst(sSQL);
                }
                catch (Exception v) { }

                lbMessagePost.Text = sReMessage + getUserId(this.TreeView1.SelectedValue); // this.TreeView1.SelectedNode.Text; //
                lbBody.Text = Server.HtmlDecode(getPostBodyById(this.TreeView1.SelectedValue)); //this.TreeView1.SelectedNode.ToolTip;
                lbBodyFiles.Text = getAttachedFilesByPostId(hdParentId.Value.ToString());

                /* Check the post type in order to color the background */
                sSQL = "SELECT [PostType] FROM [uDebate_Forum_Posts] where ID=" + this.TreeView1.SelectedNode.Value;
                try
                {
                    String type = String.Empty;
                    type = ATC.Database.sqlGetFirst(sSQL);
                    if (type.Equals("3"))
                    {
                        lbMessagePost.CssClass = "forum_title_text3";
                    }
                    else if (type.Equals("4"))
                    {
                        lbMessagePost.CssClass = "forum_title_text4";
                    }

                }
                catch (Exception v) { }

                if (!Request.IsAuthenticated)
                {
                    lbPostType.Visible = false;
                    PanelReply.Visible = false;
                    PanelReplyButtons.Visible = false;
                    lbStartPosting.Visible = true;
                    lbStartPosting.Text = "<div class='dnnFormMessage dnnFormWarning'>" +
                        Localization.GetString("LoggedIn", LocalResourceFile) +
                     "  <a href='/" + System.Threading.Thread.CurrentThread.CurrentCulture.Name +
                     "/loginpage.aspx?returnurl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery) + "'>" +
                     Localization.GetString("Login", LocalResourceFile) + "</div>";
                }
                else
                {
                    //txtSubjectPost.Text = sReMessage; // this.TreeView1.SelectedNode.Text; //
                    txtSubjectPost.Text = "";
                    txtPostTypeId.Value = "";

                    /* Hide the starting panel and buttons
                     * in case the user clicked the post while
                     * in the new post form
                     */
                    PanelStartPosting.Visible = false;
                    PanelStartPostingButtons.Visible = false;
                    lbStartingReply.Visible = false;

                    /* Show the reply panel and buttons */
                    lbPostType.Visible = true;
                    PanelReply.Visible = true;
                    PanelReplyButtons.Visible = true;
                    lbSubjectD.Visible = true;
                    lbReply.Visible = true;
                    ibxStartNewPost.Visible = false; //new
                    fillPostTypesButtons();
                }
            }
            else
            {
                PanelReply.Visible = false;
                PanelHolder.Visible = false;
                ibxStartNewPost.Visible = false;

                hdParentId.Value = this.TreeView1.SelectedNode.Value;
                sSQL = "SELECT [Subject] FROM [uDebate_Forum_Posts] where ID=" + this.TreeView1.SelectedNode.Value;
                try
                {
                    sReMessage = ATC.Database.sqlGetFirst(sSQL);
                }
                catch (Exception v) { }

                lbMessagePost.Text = sReMessage + getUserId(this.TreeView1.SelectedValue); // this.TreeView1.SelectedNode.Text; //
                lbBody.Text = this.TreeView1.SelectedNode.ToolTip;
                lbBodyFiles.Text = getAttachedFilesByPostId(hdParentId.Value.ToString());
            }
        }

        public string getPostBodyById(string postID)
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT [Message]
                  FROM [uDebate_Forum_Posts]
                where [ID]=" + postID;
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getPostLanguageByThread(string threadID)
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT [Language]
                  FROM [uDebate_Forum_Threads]
                where [ID]=" + threadID;
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getUserId(string postID)
        {
            string sOut = string.Empty;
            string sUser = string.Empty;

            string sSQL = @"SELECT Users.UserID,Users.Username,Users.FirstName, uDebate_Forum_Posts.PostDate
                            FROM  uDebate_Forum_Posts INNER JOIN
                            Users ON uDebate_Forum_Posts.UserID = Users.UserID
                            WHERE (uDebate_Forum_Posts.ID = " + postID + ")";
            try
            {
                DataTable dt = ATC.Database.sqlExecuteDataTable(sSQL);

                if (dt.Rows.Count > 0)
                {
                    DataRow dtr = dt.Rows[0];
                    DateTime postDate = (DateTime)dtr["PostDate"];
                    if (dtr["FirstName"] != null && dtr["FirstName"].ToString() != "")
                        sUser = sUser + dtr["FirstName"].ToString();
                    else
                        sUser = sUser + dtr["Username"].ToString();
                    sOut = "<div class='postDate'> " + postDate.ToString("dd MMM yyyy")
                          + ", <span class='time'>" + postDate.ToString("HH:mm")
                          + " " + Localization.GetString("PostBy", LocalResourceFile)
                          + ":</span><a href='" + DotNetNuke.Common.Globals.UserProfileURL(Convert.ToInt32(dtr["UserID"]))
                          + "'>" + sUser + "</a></div>";
                }
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        /* lkallipolitis: ta replyBYId exoyn allaksei sti vasi gia na epitrepetai
         * to uper/kata kai sta issues,comments
         */
        public void fillPostTypesButtons()
        {
            string ParentId = hdParentId.Value;

            if (ParentId == "0") //An einai iso me 0 tote den yparxei akoma Post, opote pame sto 1o post type vasi tou Thread
            {
            }
            else //An yparxei pame sto uDebate_Forum_PostTypes kai tsekaroume to pedio ReplyByID
            {
                string SQL = @"SELECT  [uDebate_Forum_PostTypes].*, [uDebate_Forum_Posts].[PostType], [uDebate_Forum_Posts].[ID]
                                FROM [uDebate_Forum_PostTypes], [uDebate_Forum_Posts]
                                where [uDebate_Forum_Posts].[PostType]=[uDebate_Forum_PostTypes].[ID]
                                AND [uDebate_Forum_Posts].[ID]=" + ParentId;
                try
                {
                    DataRow row = ATC.Database.sqlExecuteDataRow(SQL);
                    if (row.Table.Rows.Count > 0)
                    {
                        string sReplyById = row["ReplyById"].ToString();
                        string sNotReplyById = row["NotReplyById"].ToString();
                        //Enabled Buttons
                        string[] postTypes = sReplyById.Split(',');
                        int count = 1;
                        foreach (string sID in postTypes)
                        {
                            updatePostTypeImageByID(Convert.ToInt32(sID), count);
                            count++;
                        }
                        //Disabled Buttons
                        string[] unPostTypes = sNotReplyById.Split(',');
                        int count2 = 1;
                        foreach (string sID in unPostTypes)
                        {
                            updateUnPostTypeImageByID(Convert.ToInt32(sID), count2);
                            count2++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message + ex.Source);
                }
            }

            postButtons.Visible = true;
        }

        public void fillStartingPostTypesButtons()
        {
            string sThread = ATC.Tools.URLParam("Thread");

            string SQL = @"SELECT [uDebate_Forum_Threads].[GroupID],[uDebate_Forum_Threads].[ID],[uDebate_Forum_Threads].[Description],
                            [uDebate_Forum_ThreadGroup].[Title]
                            FROM [uDebate_Forum_Threads],
                            [uDebate_Forum_ThreadGroup]
                            where [uDebate_Forum_Threads].[ID]=" + sThread + @"
                            and [uDebate_Forum_Threads].[GroupID] =[uDebate_Forum_ThreadGroup].[ID]";

            try
            {
                DataRow row = ATC.Database.sqlExecuteDataRow(SQL);
                if (row.Table.Rows.Count > 0)
                {
                    string sGroupId = row["GroupID"].ToString();
                    //Enabled Buttons
                    if (sGroupId == "1")
                    {
                        ImageButton11.ImageUrl = getImageButtonUrl("1");
                        ImageButton11.AlternateText = Localization.GetString("Issue", LocalResourceFile);
                        ImageButton11.Visible = true;
                        ImageButton11.Enabled = true;
                    }
                    else if (sGroupId == "2")
                    {
                        ImageButton12.ImageUrl = getImageButtonUrl("5");
                        ImageButton12.AlternateText = Localization.GetString("Question", LocalResourceFile);
                        ImageButton12.Visible = true;
                        ImageButton12.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + ex.Source);
            }

        }

        public void updateUnPostTypeImageByID(int ID, int count)
        {
            string SQL = @"SELECT * FROM [uDebate_Forum_PostTypes] where ID=" + ID;

            try
            {
                DataRow row = ATC.Database.sqlExecuteDataRow(SQL);
                if (row.Table.Rows.Count > 0)
                {
                    switch (ID)
                    {
                        case 1:
                            ImageButton1.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton1.AlternateText = row["Title"].ToString();
                            ImageButton1.Visible = true;
                            ImageButton1.Enabled = false;
                            break;
                        case 2:
                            ImageButton2.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton2.AlternateText = row["Title"].ToString();
                            ImageButton2.Visible = true;
                            ImageButton2.Enabled = false;
                            break;
                        case 3:
                            ImageButton3.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton3.AlternateText = row["Title"].ToString();
                            ImageButton3.Visible = true;
                            ImageButton3.Enabled = false;
                            break;
                        case 4:
                            ImageButton4.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton4.AlternateText = row["Title"].ToString();
                            ImageButton4.Visible = true;
                            ImageButton4.Enabled = false;
                            break;
                        case 5:
                            ImageButton5.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton5.AlternateText = row["Title"].ToString();
                            ImageButton5.Visible = true;
                            ImageButton5.Enabled = false;
                            break;
                        case 6:
                            ImageButton6.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton6.AlternateText = row["Title"].ToString();
                            ImageButton6.Visible = true;
                            ImageButton6.Enabled = false;
                            break;
                        case 7:
                            ImageButton7.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton7.AlternateText = row["Title"].ToString();
                            ImageButton7.Visible = true;
                            ImageButton7.Enabled = false;
                            break;
                        case 8:
                            ImageButton8.ImageUrl = getImageButtonOffUrl(ID.ToString());
                            ImageButton8.AlternateText = row["Title"].ToString();
                            ImageButton8.Visible = true;
                            ImageButton8.Enabled = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + ex.Source);
            }
            //return myImage;
        }

        public void updatePostTypeImageByID(int ID, int count)
        {
            //ImageButton myImage = new ImageButton();

            string SQL = @"SELECT * FROM [uDebate_Forum_PostTypes] where ID=" + ID;

            try
            {
                DataRow row = ATC.Database.sqlExecuteDataRow(SQL);
                if (row.Table.Rows.Count > 0)
                {
                    switch (ID)
                    {
                        case 1:
                            ImageButton1.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton1.AlternateText = row["Title"].ToString();
                            ImageButton1.Visible = true;
                            ImageButton1.Enabled = true;
                            break;
                        case 2:
                            ImageButton2.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton2.AlternateText = row["Title"].ToString();
                            ImageButton2.Visible = true;
                            ImageButton2.Enabled = true;
                            break;
                        case 3:
                            ImageButton3.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton3.AlternateText = row["Title"].ToString();
                            ImageButton3.Visible = true;
                            ImageButton3.Enabled = true;
                            break;
                        case 4:
                            ImageButton4.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton4.AlternateText = row["Title"].ToString();
                            ImageButton4.Visible = true;
                            ImageButton4.Enabled = true;
                            break;
                        case 5:
                            ImageButton5.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton5.AlternateText = row["Title"].ToString();
                            ImageButton5.Visible = true;
                            ImageButton5.Enabled = true;
                            break;
                        case 6:
                            ImageButton6.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton6.AlternateText = row["Title"].ToString();
                            ImageButton6.Visible = true;
                            ImageButton6.Enabled = true;
                            break;
                        case 7:
                            ImageButton7.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton7.AlternateText = row["Title"].ToString();
                            ImageButton7.Visible = true;
                            ImageButton7.Enabled = true;
                            break;
                        case 8:
                            ImageButton8.ImageUrl = getImageButtonUrl(ID.ToString());
                            ImageButton8.AlternateText = row["Title"].ToString();
                            ImageButton8.Visible = true;
                            ImageButton8.Enabled = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + ex.Source);
            }
            //return myImage;
        }

        // returns a string with image name depending on the ID of PostType
        public string getImageButtonUrl(string sID)
        {
            string sOut = string.Empty;
            switch (sID)
            {
                case "1":
                    sOut = "issue_icon_on.gif";
                    break;
                case "2":
                    sOut = "alter_icon_on.gif";
                    break;
                case "3":
                    sOut = "pro_icon_on.gif";
                    break;
                case "4":
                    sOut = "con_icon_on.gif";
                    break;
                case "5":
                    sOut = "question_icon_on.gif";
                    break;
                case "6":
                    sOut = "answer_icon_on.gif";
                    break;
                case "7":
                    sOut = "comments_icon_on.gif";
                    break;
                case "8":
                    sOut = "comments_icon_on.gif";
                    break;
                default:
                    sOut = "issue_icon_on.gif";
                    break;
            }
            return "images/" + sOut;
        }

        public string getImageButtonOffUrl(string sID)
        {
            string sOut = string.Empty;
            switch (sID)
            {
                case "1":
                    sOut = "issue_icon_off.gif";
                    break;
                case "2":
                    sOut = "alter_icon_off.gif";
                    break;
                case "3":
                    sOut = "pro_icon_off.gif";
                    break;
                case "4":
                    sOut = "con_icon_off.gif";
                    break;
                case "5":
                    sOut = "question_icon_off.gif";
                    break;
                case "6":
                    sOut = "answer_icon_off.gif";
                    break;
                case "7":
                    sOut = "comments_icon_off.gif";
                    break;
                case "8":
                    sOut = "comments_icon_off.gif";
                    break;
                default:
                    sOut = "issue_icon_off.gif";
                    break;
            }
            return "images/" + sOut;
        }

        public string getUserIdByThread()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Threads] where [ID]=" +
                            ATC.Tools.URLParam("Thread"));
            return sUserId;
        }

        public string getUserIdByTopic()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Topics] where [ID]=" +
                            ATC.Tools.URLParam("Thread"));
            return sUserId;
        }

        public string getAttachedFilesByPostId(string PostId)
        {
            string SQL = @"SELECT [ID],[UserTitle],[FileTitle],[FileSize],[FileType],[PostId]
                        FROM [uDebate_Forum_Post_Files] where PostId=" + PostId;

            string sHTML = string.Empty;


            AttachmentController cntAttachment = new AttachmentController();
            List<AttachmentInfo> lstFiles = null;

            if (Convert.ToInt32(PostId) > 0)
            {
                lstFiles = cntAttachment.GetAllByPostID(Convert.ToInt32(PostId));
            }

            if (lstFiles.Count > 0)
            {
                sHTML += "<table class='attachments' cellspacing='0' cellpadding='0' width='160px'>";
                sHTML += "<td style='border-top: 1px solid #FDFDFD; padding:5px 3px;width:24px;'> <img src='" + ATC.Tools.GetParam("RootURL") +
                                     @"DesktopModules/uDebate/images/s_attachment.png' align=middle'> </td> ";
                foreach (AttachmentInfo objFile in lstFiles)
                {
                    string strlink = Common.Globals.LinkClick("FileID=" + objFile.FileID, PortalSettings.ActiveTab.TabID, ModuleId, false, true);
                    string strFileName = objFile.LocalFileName;
                    sHTML += @" <td style='border-top: 1px solid #FDFDFD; padding: 5px 0;'><a href='" + strlink + "'>"
                            + strFileName.Substring(0, Math.Min(strFileName.Length, 15)).ToLower() + @"</a>&nbsp;</td>";
                }
                sHTML += "</table>";
            }
            return sHTML;
        }

        protected void ibxStartNewPost_Click(object sender, ImageClickEventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                PanelHolder.Visible = true;
            }

            PrintHeadHeadDetails(false);

            //Disable Main Button
            ibxStartNewPost.Visible = false;

            PanelStartPosting.Visible = true;
            PanelStartPostingButtons.Visible = true;
            lbStartingReply.Visible = true;
            //btnReply.Visible = false;
            postMessageView.Visible = false;
            PanelReply.Visible = false;
            PanelReplyButtons.Visible = false;
            lbStartingPostType.Visible = true;
            fillStartingPostTypesButtons();
        }

        //Cancel from the Reply form
        protected void ibxRemoveStartFiles_Click(object sender, ImageClickEventArgs e)
        {
            ClearReply();
            postMessageView.Visible = true;
        }
        protected void ibxSave_Click(object sender, ImageClickEventArgs e)
        {
            SaveReply();
        }

        //Cancel from the new post form
        protected void ibxCancelStartFiles_Click(object sender, ImageClickEventArgs e)
        {
            ClearStartingReply();
            postMessageView.Visible = true;
            PanelHolder.Visible = false;
            PanelStartPosting.Visible = false;
            PanelStartPostingButtons.Visible = false;
            ibxStartNewPost.Visible = true;


        }
        protected void ibxStartingSave_Click(object sender, ImageClickEventArgs e)
        {
            SaveStartingReply();
        }


        public class ButtonExtension : Button
        {
            string _Key = string.Empty;

            public ButtonExtension(string Key)
            {
                _Key = Key;
            }

            [UrlProperty]
            public virtual string KeyToRemove
            {
                get
                {
                    return KeyToRemove;
                }
                set { KeyToRemove = value; }
            }
            protected override void OnClick(EventArgs e)
            {
                Dictionary<string, string> userUpFiles = (Dictionary<string, string>)HttpContext.Current.Session["PortalUserFiles"];
                if (userUpFiles.Count > 0)
                {
                    userUpFiles.Remove(_Key);
                    HttpContext.Current.Session["PortalUserFiles"] = userUpFiles;
                }
                base.OnClick(e);
            }
        }

        protected void RenderLinkButton(HtmlTextWriter wr, string URL, string Text, string Css)
        {
            if (Css.Length > 0)
            {
                wr.AddAttribute(HtmlTextWriterAttribute.Class, Css);
            }
            wr.AddAttribute(HtmlTextWriterAttribute.Href, URL);
            wr.RenderBeginTag(HtmlTextWriterTag.A); // <a>
            wr.Write(Text);
            wr.RenderEndTag();  // </A>
        }

        protected void notifyCheck_CheckedChanged(object sender, EventArgs e)
        {

            if (Request.IsAuthenticated)//make sure user is logged in
            {                
                string userID = UserId.ToString();
                string userEmail = UserInfo.Email;
                string threadID = ATC.Tools.URLParam("Thread");

                if (notifyCheck.Checked)
                {
                    String SQL = "INSERT INTO uDebate_Forum_Notifications (userID,threadID,userEmail,insertedOn) VALUES(" +
                                userID + "," + threadID + ",'" + userEmail + "',getdate())";
                    ATC.Database.sqlExecuteCommand(SQL);
                    notifyCheck.Checked = true;
                }
                else
                {
                    String SQL = "DELETE FROM uDebate_Forum_Notifications WHERE userID=" + userID +
                                " AND threadID=" + threadID;
                    ATC.Database.sqlExecuteCommand(SQL);
                    notifyCheck.Checked = false;
                }
            }
        }

        public bool checkUsedNotified(string userID, string threadID)
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT [userID]
                  FROM [uDebate_Forum_Notifications]
                where [userID]=" + userID + " AND [threadID]=" + threadID;
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            if (sOut != "")
                return true;
            return false;
        }


    }
}