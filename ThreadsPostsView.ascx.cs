using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ATC;
using DotNetNuke.Modules.uDebate;

namespace DotNetNuke.Modules.uDebate
{
    public partial class ThreadsPostsView : uDebateModuleBase
    {
        private bool HasReferences = false;
        private bool HasEmbendedPollReference = false;
        private bool HasEmbendedNewsASSETReference = false;
        private bool HasEmbendedDocASSETReference = false;
        

        /* missing variables */
        string CurrentLanguageCode = "en-US";

        protected void Page_Load(object sender, EventArgs e)
        {
           // btnReply.Text = ATC.Translate.String("Post Now", "", CurrentLanguageCode);
            btnCancel.Text = ATC.Translate.String("Cancel", "", CurrentLanguageCode);
            btnSave.Text = ATC.Translate.String("Post Now", "", CurrentLanguageCode);

            HasReferences = true;/* (
                                 (ATC.Tools.GetParam("REFERENCE_ForumThread_Poll").ToUpper() == "ON" && ReadData("THREAD_POST_POLL_REF") == "1")
                                 ||
                                 (ATC.Tools.GetParam("REFERENCE_ForumThread_NewsASSET").ToUpper() == "ON" && ReadData("THREAD_POST_NEWSASSET_REF") == "1")
                                 ||
                                 (ATC.Tools.GetParam("REFERENCE_ForumThread_DocASSET").ToUpper() == "ON" && ReadData("THREAD_POST_DOCASSET_REF") == "1")
                             );*/

            HasEmbendedPollReference = false;// (ATC.Tools.GetParam("REFERENCE_ForumThread_Poll").ToUpper() == "ON" && ReadData("THREAD_POST_POLL_REF") == "1" && ReadData("THREAD_POST_POLL_REF_EMBENDED") == "1");
            HasEmbendedNewsASSETReference = false;// (ATC.Tools.GetParam("REFERENCE_ForumThread_NewsASSET").ToUpper() == "ON" && ReadData("THREAD_POST_NEWSASSET_REF") == "1" && ReadData("THREAD_POST_NEWSASSET_REF_EMBENDED") == "1");
            HasEmbendedDocASSETReference = false;// (ATC.Tools.GetParam("REFERENCE_ForumThread_DocASSET").ToUpper() == "ON" && ReadData("THREAD_POST_DOCASSET_REF") == "1" && ReadData("THREAD_POST_DOCASSET_REF_EMBENDED") == "1");

            btnReply.Visible = false;
            
            LoadData();
            IncViewCounter();

            loginRegDiv.Visible = (Session["Portal_User"] == null);
        }

        private void LoadData()
        {
            Forums.Controls.Clear();

            string PreviousThread = "";
            string editLink = "";
            string ShowReferencesLink = "";

            bool DisplayThread = true;// (ReadData("THREAD_POST_VIEW_DISPLAY_THREAD") == "1");
            string ThreadLinkCode = ""; // ReadData("THREAD_POST_VIEW_THREAD_LINK");

            bool DisplayPost = true;// (ReadData("THREAD_POST_VIEW_DISPLAY_POST") == "1");
            string PostLinkCode = "";// ReadData("THREAD_POST_VIEW_POST_LINK");
            /*if (PostLinkCode == String.Empty)
                PostLinkCode = PageCode;*/

            string SQL = CreateSQLStatement(DisplayThread, DisplayPost);

            try
            {
                string postAction = ATC.Tools.URLParam("PostAction").ToString();
                if (postAction != string.Empty)
                    ChangePostStatus(postAction);

                DataSet ds = ATC.Database.sqlExecuteDataSet(SQL);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    Forums.Controls.Add(new LiteralControl("<table cellspacing='0' cellpadding='0' width='100%'><tr><td>"+ ATC.Translate.String("Sorry. No data found!", "", CurrentLanguageCode) +"</td><tr></table>"));
                    loginRegDiv.Visible = false;
                    return;
                }

                btnReply.Visible = true; 
                if (Session["Portal_User"] == null)
                {
                    btnReply.Enabled = false;
                    btnReply.ToolTip = ATC.Translate.String("You must be logged in to post a reply to this thread!", "", CurrentLanguageCode);
                }
                else
                {
                    btnReply.Enabled = true;
                    btnReply.ToolTip = ATC.Translate.String("Click to reply to this thread.", "", CurrentLanguageCode);
                }

                Forums.Controls.Add(new LiteralControl("<table cellspacing='0' cellpadding='0' width='100%'>"));

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    bool Inactive = (row["Thread_Active"].ToString() == "0");

                    if (
                            (DisplayThread && HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == row["Thread_User_ID"].ToString()) 
                            ||
                            (DisplayThread && HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == ATC.Database.sqlGetFirst("SELECT UserID FROM Portal_Forum_Topics WHERE ID = " + row["Thread_TopicID"].ToString()))
                        ||
                        (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                        )
                    {
                        editLink = "<a href=javascript:OpenDialog('Satellites/Threads_Edit.aspx?TopicID=" + row["Thread_TopicID"].ToString() + "&ThreadID=" + row["Thread_ID"].ToString() + "','ThreadsEdit',800,730)><img alt='" + ATC.Translate.String("Edit this Thread", "", CurrentLanguageCode) + "' border=0 src='" + ATC.Tools.GetParam("RootURL") + "img/admin/edit.gif' align='middle' ></a>&nbsp;";
                        if (HasReferences)
                        {
                          //  ShowReferencesLink = "<input class='forum_button' type='button' onclick=\"javascript:OpenDialog('Satellites/Thread_References.aspx?PortletID=" + PortletID + "&ThreadID=" + row["Thread_ID"].ToString() + "','ThreadReferencess',800,730)\" value='" + ATC.Translate.String("References", "", CurrentLanguageCode) + "'>&nbsp;";
                        }
                    }
                    if (row["Thread_Status"].ToString() == "2")
                        btnReply.Visible = false;

                    if (DisplayThread && PreviousThread != row["Thread_ID"].ToString())
                    {
                        PreviousThread = row["Thread_ID"].ToString();

                        string contact = "";
                        if (Session["Portal_User"] != null)
                            contact = "<input type='button' onclick=\"popup('SendMail.aspx?mode=contact&thread=" + row["Thread_ID"].ToString() + "','SendEmail')\" class='forum_button' value='" + ATC.Translate.String("Contact Email", "", CurrentLanguageCode) + "' />&nbsp;";

                        Forums.Controls.Add(new LiteralControl(@"
                                                                <tr>
                                                                    <td class='threads_posts_thread_outer_td' colspan='5'>
                                                                        <table width='100%' cellpadding='0' cellspacing='0' class='threads_posts_thread_table'>
                                                                            <tr>
                                                                                <td colspan=2 class='thread_posts_thread_desc'>
                                                                                    " + editLink + row["Thread_Desc"].ToString() + (Inactive ? " - <span class='topic_threads_inactive_span'>" + ATC.Translate.String("INACTIVE", "", CurrentLanguageCode) : "") + @"</span>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan='2' class='thread_posts_thread_contact'>" + ShowReferencesLink + contact + @"</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan='2' class='thread_posts_thread_summary'>Summary: " + row["Thread_Sum"].ToString() + @"</td>
                                                                            </tr>"));
                        Forums.Controls.Add(new LiteralControl(@"
                                                                            <tr>
                                                                                <td colspan='2' class='thread_posts_thread_text'>" + row["Thread_Text"].ToString() + @"</td>
                                                                            </tr>"));
                       /* if (HasEmbendedPollReference)
                        {
                            string EmbendedPollPortletID = "12";//ReadData("THREAD_POST_POLL_REF_PORTLET");
                            string EmbendedPollSource = ATC.Database.sqlGetFirst("SELECT Source_" + CurrentLanguageCode + ", SupportsMultilang FROM Portal_Portlets WHERE ID = " + EmbendedPollPortletID);
                            string EmbemdedPollSupportsMultilang = ATC.Database.sqlGetFirst("SELECT SupportsMultilang FROM Portal_Portlets WHERE ID = " + EmbendedPollPortletID);
                            if (EmbendedPollSource != String.Empty)
                            {
                                Forums.Controls.Add(new LiteralControl(@"<tr><td colspan='2' class='thread_posts_thread_text'>"));
                                Control EmbendedPollControl = (Control)Page.LoadControl(EmbendedPollSource);
                                if (EmbendedPollControl is PortletControl)
                                {
                                    ((PortletControl)EmbendedPollControl).PortletID = EmbendedPollPortletID;
                                    ((PortletControl)EmbendedPollControl).PageID = this.PageID;
                                    ((PortletControl)EmbendedPollControl).PageCode = this.PageCode;
                                    ((PortletControl)EmbendedPollControl).Params = this.Params;
                                    ((PortletControl)EmbendedPollControl).UIPosition = this.UIPosition;
                                    ((PortletControl)EmbendedPollControl).SupportsMultilang = (EmbemdedPollSupportsMultilang == "1");
                                    ((PortletControl)EmbendedPollControl).AdminFlags = this.AdminFlags;
                                    ((PortletControl)EmbendedPollControl).IsInherited = this.IsInherited;

                                } 
                                Forums.Controls.Add(EmbendedPollControl);
                                Forums.Controls.Add(new LiteralControl(@"</td></tr>"));
                            }
                        }*/

                       /* if (HasEmbendedNewsASSETReference)
                        {
                            string EmbendedNewsASSETPortletID = ReadData("THREAD_POST_NEWSASSET_REF_PORTLET");
                            string EmbendedNewsASSETSource = ATC.Database.sqlGetFirst("SELECT Source_" + CurrentLanguageCode + ", SupportsMultilang FROM Portal_Portlets WHERE ID = " + EmbendedNewsASSETPortletID);
                            string EmbemdedNewsASSETSupportsMultilang = ATC.Database.sqlGetFirst("SELECT SupportsMultilang FROM Portal_Portlets WHERE ID = " + EmbendedNewsASSETPortletID);
                            if (EmbendedNewsASSETSource != String.Empty)
                            {
                                Forums.Controls.Add(new LiteralControl(@"<tr><td colspan='2' class='thread_posts_thread_text'>"));
                                Control EmbendedNewsASSETControl = (Control)Page.LoadControl(EmbendedNewsASSETSource);
                                if (EmbendedNewsASSETControl is PortletControl)
                                {
                                    ((PortletControl)EmbendedNewsASSETControl).PortletID = EmbendedNewsASSETPortletID;
                                    ((PortletControl)EmbendedNewsASSETControl).PageID = this.PageID;
                                    ((PortletControl)EmbendedNewsASSETControl).PageCode = this.PageCode;
                                    ((PortletControl)EmbendedNewsASSETControl).Params = this.Params;
                                    ((PortletControl)EmbendedNewsASSETControl).UIPosition = this.UIPosition;
                                    ((PortletControl)EmbendedNewsASSETControl).SupportsMultilang = (EmbemdedNewsASSETSupportsMultilang == "1"); 
                                    ((PortletControl)EmbendedNewsASSETControl).AdminFlags = this.AdminFlags;
                                    ((PortletControl)EmbendedNewsASSETControl).IsInherited = this.IsInherited;

                                }
                                Forums.Controls.Add(EmbendedNewsASSETControl);
                                Forums.Controls.Add(new LiteralControl(@"</td></tr>"));
                            }
                        }*/

                       /* if (HasEmbendedDocASSETReference)
                        {
                            string EmbendedDocASSETPortletID = ReadData("THREAD_POST_DOCASSET_REF_PORTLET");
                            string EmbendedDocASSETSource = ATC.Database.sqlGetFirst("SELECT Source_" + CurrentLanguageCode + " FROM Portal_Portlets WHERE ID = " + EmbendedDocASSETPortletID);
                            string EmbemdedDocASSETSupportsMultilang = ATC.Database.sqlGetFirst("SELECT SupportsMultilang FROM Portal_Portlets WHERE ID = " + EmbendedDocASSETPortletID);
                            if (EmbendedDocASSETSource != String.Empty)
                            {
                                Forums.Controls.Add(new LiteralControl(@"<tr><td colspan='2' class='thread_posts_thread_text'>"));
                                Control EmbendedDocASSETControl = (Control)Page.LoadControl(EmbendedDocASSETSource);
                                if (EmbendedDocASSETControl is PortletControl)
                                {
                                    ((PortletControl)EmbendedDocASSETControl).PortletID = EmbendedDocASSETPortletID;
                                    ((PortletControl)EmbendedDocASSETControl).PageID = this.PageID;
                                    ((PortletControl)EmbendedDocASSETControl).PageCode = this.PageCode;
                                    ((PortletControl)EmbendedDocASSETControl).Params = this.Params;
                                    ((PortletControl)EmbendedDocASSETControl).UIPosition = this.UIPosition;
                                    ((PortletControl)EmbendedDocASSETControl).SupportsMultilang = (EmbemdedDocASSETSupportsMultilang == "1");
                                    ((PortletControl)EmbendedDocASSETControl).AdminFlags = this.AdminFlags;
                                    ((PortletControl)EmbendedDocASSETControl).IsInherited = this.IsInherited;

                                } Forums.Controls.Add(EmbendedDocASSETControl);
                                Forums.Controls.Add(new LiteralControl(@"</td></tr>"));
                            }
                        }
                        */
                        Forums.Controls.Add(new LiteralControl(@"
                                                                            <tr>
                                                                                <td class='thread_posts_thread_coordinator'>"
                                                                                    + row["Surname"].ToString() + @"&nbsp;" + row["Name"].ToString() +
                                                                                @"</td>
                                                                                <td class='thread_posts_thread_date'>" +
                                                                                    ATC.Translate.String("Posts", "", CurrentLanguageCode) + ": " + ATC.Database.sqlGetFirst("SELECT Count(ID) FROM Portal_Forum_Posts WHERE Active = 1 AND IsPublished = 1 AND ThreadID = " + ATC.Tools.IntURLParam("Thread").ToString()) +
                                                                                    @", " + ATC.Translate.String("Views", "", CurrentLanguageCode) + ": " + ATC.Database.sqlGetFirst("SELECT View_Count FROM Portal_Forum_Threads WHERE ID = " + ATC.Tools.IntURLParam("Thread").ToString()) + @", "
                                                                                    + ATC.Translate.String("Opened date: ", "", CurrentLanguageCode) + Convert.ToDateTime(row["Opened_Date"].ToString()).ToShortDateString() + (row["Closed_Date"].ToString() != string.Empty && row["Thread_Status"].ToString() == "2" ? ATC.Translate.String(", Closed date:", "", CurrentLanguageCode) + Convert.ToDateTime(row["Closed_Date"].ToString()).ToShortDateString() : "") +
                                                                                @"</td>
                                                                            </tr>
                                                                        </table>
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
                            (DisplayThread && HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == row["Thread_User_ID"].ToString())
                            ||
                            (DisplayThread && HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == ATC.Database.sqlGetFirst("SELECT UserID FROM Portal_Forum_Topics WHERE ID = " + row["Thread_TopicID"].ToString()))
                            ||
                            (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                        ))
                    {
                        if (row["Post_IsPublished"].ToString() == "0") continue;
                    }
                    else
                    {
                        delete = "<input type='button' onclick=\"DeletePost('" + href + "&PostAction=Delete&Post=" + row["Post_ID"].ToString() + "')\" class='forum_button' value='" + ATC.Translate.String("Delete", "", CurrentLanguageCode) + "' />";
                    }

                    string state = (row["Post_IsPublished"].ToString() == "0" ? "<span style='color:Red;'>NOT PUBLISHED</span>&nbsp;-&nbsp;<span class='forum_link'><a href='" + href + "&PostAction=Accept&Post=" + row["Post_ID"].ToString() + "' class='forum_link'>Accept</a></span>&nbsp;|&nbsp;<span class='forum_link'><a href='" + href + "&PostAction=Reject&Post=" + row["Post_ID"].ToString() + "' class='forum_link'>Reject</a></span>" : "");

                    string complaint = "";
                    if (Session["Portal_User"] != null)
                        complaint = "<input type='button' onclick=\"popup('SendMail.aspx?mode=complaint&thread=" + row["Thread_ID"].ToString() + "&post=" + row["Post_ID"].ToString() + "','SendEmail')\" class='forum_button' value='" + ATC.Translate.String("Complain", "", CurrentLanguageCode) + "' />";

                    if (row["Post_ID"].ToString() != string.Empty)
                    {
                        Forums.Controls.Add(new LiteralControl(
                        @"  
                        <tr>
                            <td class=thread_post_post_outer_td'>
                                " + state + @"
                                <table width='100%' cellpadding='0' cellspacing='0' class='thread_post_post_table'>
                                    <tr>
                                        <td class='thread_post_post_left_blank'>&nbsp;</td>
                                        <td class='thread_post_post_text'>" + row["Post_Msg"].ToString() + @"</td>
                                        <td class='thread_post_post_details'>
                                            " + row["Post_User"].ToString() + @"<br />
                                            " + ATC.Translate.String("Total Posts: ", "", CurrentLanguageCode) + ATC.Database.sqlGetFirst("SELECT Count(ID) FROM Portal_Forum_Posts WHERE Active = 1 AND IsPublished = 1 AND UserID = " + row["Post_User_ID"].ToString()) + @"<br />
                                            " + row["Post_Date"].ToString() + @"<br />
                                            " + complaint + @"
                                            " + delete + @"
                                        </td>
                                    </tr>
                                "));

                        if (ATC.Database.sqlGetFirst("SELECT PublishPersonalInfo FROM Portal_Users WHERE ID = " + row["Post_User_ID"].ToString()) == "1")
                        {
                            DataRow uRow = ATC.Database.sqlExecuteDataRow("SELECT * FROM Portal_Users WHERE ID = " + row["Post_User_ID"].ToString());
                            if (uRow != null)
                                Forums.Controls.Add(new LiteralControl(
                                @"
                                <tr>
                                    <td class='thread_post_post_left_blank'>&nbsp;</td>
                                    <td colspan='2' class='thread_post_advanced_user_details'>
                                        " + uRow["FirstName"].ToString() + @" 
                                        " + uRow["SurName"].ToString() + @",
                                        " + uRow["Occupation"].ToString() + @",
                                        " + uRow["Organization"].ToString() + @",
                                        " + uRow["Position"].ToString() + @",
                                        " + uRow["Location"].ToString() + @"
                                    </td>
                                </tr>
                            "));
                        }

                        Forums.Controls.Add(new LiteralControl(
                        @"
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td height='5'></td>
                        </tr>
                        "));
                    }
                    editLink = "";
                }
                Forums.Controls.Add(new LiteralControl("</table>"));
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        private void IncViewCounter()
        {
            string ThreadID = ATC.Tools.IntURLParam("Thread", "-1").ToString();
            if (ThreadID == "-1" || (bool)HttpContext.Current.Session["AdminMode"] == true) return;

            try
            {
                if (ATC.Database.sqlGetFirst("SELECT View_Count FROM Portal_Forum_Threads WHERE ID = " + ThreadID) == "")
                    ATC.Database.sqlExecuteCommand("UPDATE Portal_Forum_Threads SET View_Count = 0 WHERE ID = " + ThreadID);

                ATC.Database.sqlExecuteCommand("UPDATE Portal_Forum_Threads SET View_Count = View_Count + 1 WHERE ID=" + ThreadID + " AND View_Count is not null");
            }
            catch { }
        }

        private string CreateSQLStatement(bool DisplayThread, bool DisplayPost)
        {
            string SQL = "";
            try
            {
                string SelectStatement = "";

                //Select statement
                SelectStatement = @"SELECT ";
                //SelectStatement += " [Th].ID AS Thread_ID, [Th].Description AS Thread_Desc, [Th].Summary AS Thread_Sum, [Th].Text AS Thread_Text, User_Thread.Username AS Thread_User, [Th].UserID AS Thread_User_ID, [Th].Opened_Date AS Opened_Date, [Th].Closed_Date AS Closed_Date, [Th].Complain_Email AS Complaint_Email, [Th].Contact_Email AS Contact_Email, User_Thread.Surname AS Surname, User_Thread.Firstname AS Name, [Th].Status AS Thread_Status, [Th].TopicID as Thread_TopicID, Th.Active as Thread_Active,";
                SelectStatement += " [Th].ID AS Thread_ID, [Th].Description AS Thread_Desc, [Th].Summary AS Thread_Sum, [Th].Text AS Thread_Text, User_Thread.Username AS Thread_User, [Th].UserID AS Thread_User_ID, [Th].Opened_Date AS Opened_Date, [Th].Closed_Date AS Closed_Date, [Th].Complain_Email AS Complaint_Email, [Th].Contact_Email AS Contact_Email, User_Thread.LastName AS Surname, User_Thread.Firstname AS Name, [Th].Status AS Thread_Status, [Th].TopicID as Thread_TopicID, Th.Active as Thread_Active,";

                if (DisplayPost)
                    SelectStatement += " P.ID AS Post_ID, P.Subject AS Post_Sub, P.Message AS Post_Msg, P.PostDate AS Post_Date, User_Post.Username AS Post_User, P.UserID AS Post_User_ID,[P].IsPublished AS Post_IsPublished ";
                else
                    SelectStatement += " NULL AS Post_ID, NULL AS Post_Sub, NULL AS Post_Msg, NULL AS Post_Date, NULL AS Post_User, NULL AS Post_User_ID, NULL AS Post_IsPublished ";
                SelectStatement = SelectStatement.TrimEnd(',');

                //From statement
                SelectStatement += " FROM ";
                SelectStatement += " Portal_Forum_Threads AS Th ";
                SelectStatement += " INNER JOIN Portal_Users AS User_Thread ON Th.UserID = User_Thread.ID ";
                if (DisplayPost) SelectStatement += " LEFT OUTER JOIN Portal_Forum_Posts AS P ON Th.ID = P.ThreadID AND [P].Active = 1 ";
                if (DisplayPost) SelectStatement += " LEFT OUTER JOIN Portal_Users AS User_Post ON P.UserID = User_Post.ID ";
                

                string WhereStatement = "";
                //Forum ID
                string ThreadID = "-1";// ReadData("THREAD_POST_VIEW_ID");
                if (ThreadID == "-1") ThreadID = ATC.Tools.IntURLParam("Thread"); //Current Forum
                if (ThreadID != String.Empty)
                {
                    WhereStatement += " AND [Th].ID = " + ThreadID;
                }

                if (/*!PortalSite.Portal.Security.IsAdministrator()*/ !DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                {
                    if (DisplayThread && Session["Portal_User"] == null)
                    {
                        WhereStatement += " AND Th.Active = 1";
                    }
                    else if (DisplayThread && Session["Portal_User"] != null)
                    {
                        WhereStatement += " AND (Th.Active = 1 OR Th.UserID = " + ((DataRow)Session["Portal_User"])["ID"].ToString() + " ) ";
                    }
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
                string OrderBy = "";// ReadData("THREAD_POST_VIEW_ORDER_FIELD");
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

        protected void lnkSave_Click(object sender, EventArgs e)
        {
            SaveReply();
        }

        public void SaveReply()
        {
            if (HttpContext.Current.Session["Portal_User"] == null) return;

            if (txtReply.Text.Trim() == "")
            {
                lblError.Text = ATC.Translate.String("You must type some text.", "", CurrentLanguageCode) + "<br/><br/>";
                return;
            }
            
            string SQL = "";
            string UserID = ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString();
            string ThreadID = ATC.Tools.URLParam("Thread");
            string published = "0";// (ProjectSettings.OPT_Publish_ForumPosts == true ? "0" : "1");

            txtReply.Text = txtReply.Text.Replace("'", "\"");

            SQL = string.Format(@"
                    INSERT INTO Portal_Forum_Posts
                        (ThreadID
                        ,ParentID
                        ,UserID
                        ,PostLevel
                        ,SortOrder
                        ,Subject
                        ,Message
                        ,PostDate
                        ,IsPublished
                        ,PostType
                        ,Active
                        ,Published_Date
                        ,Complaint_Count)
                    VALUES
                        (" + ThreadID + @"
                        , NULL
                        ," + UserID + @"
                        , 1
                        , 1
                        , NULL
                        , {0}Message
                        , getDate()
                        ," + published + @"
                        , 1
                        , 1
                        , getDate()
                        , 0)
                   ", ATC.Database.Sql_VariableIdent);
            try
            {
                ATC.Database.sqlExecuteCommandParams(SQL,
                    new string[] { ATC.Database.Sql_VariableIdent + "Message" },
                    new object[] { txtReply.Text.Trim() });

                if (true)//ProjectSettings.OPT_Publish_ForumPosts)
                    lblConfirm.Text = ATC.Translate.String("WARNING: The Forum is pre-moderated. Your post has been successfully submitted but will appear within a few hours, after approval by the Forum Moderators.", "", CurrentLanguageCode) + "<br/><br/>";

                PanelReply.Visible = false;
                PanelPosts.Visible = true;

                txtReply.Text = "";
                lblError.Text = "";

                LoadData();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            PanelReply.Visible = true;
            PanelPosts.Visible = false;

            txtReply.Text = "";
            lblError.Text = "";
            lblConfirm.Text = "";
            //Response.Redirect("?page=" + ATC.Tools.URLParam("page") + "&Thread=" + ATC.Tools.IntURLParam("Thread") + "&action=newpost" + "#PostArea");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            PanelReply.Visible = false;
            PanelPosts.Visible = true;

            txtReply.Text = "";
            lblError.Text = "";
            lblConfirm.Text = "";
        }

        protected void btnSave_Click(object sender, EventArgs e)
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
                    string SQL = "UPDATE Portal_Forum_Posts SET Published_Date = GetDate(), IsPublished = " + published + ", Active = " + active + " WHERE ID = " + postID;
                    ATC.Database.sqlExecuteCommand(SQL);
                }
                else if (postAction == "Reject")
                {
                    active = "0";
                    published = "0";
                    string SQL = "UPDATE Portal_Forum_Posts SET IsPublished = " + published + ", Active = " + active + " WHERE ID = " + postID;
                    ATC.Database.sqlExecuteCommand(SQL);
                }
                else if (postAction == "Delete")
                {
                    ATC.Database.sqlExecuteCommand("UPDATE Portal_Forum_Threads SET Delete_Count = Delete_Count + 1 WHERE ID = (SELECT ThreadID FROM Portal_Forum_Posts WHERE ID = " + postID + ")");
                    ATC.Database.sqlExecuteCommand("DELETE FROM Portal_Forum_Posts WHERE ID = " + postID);
                    return;
                }
                else
                {
                    return;
                }
            }
        }
    }
}