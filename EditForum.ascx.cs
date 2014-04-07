using System;
using DotNetNuke.Services.Exceptions;
using ATC;
using ATC.WebControls;
using System.Data;
using DotNetNuke.Common;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.uDebate
{
    public partial class EditForum : uDebateModuleBase
    {
        protected string New = ATC.Tools.URLParam("new", "").ToString();
        protected string Edit = ATC.Tools.URLParam("editItem", "").ToString();
        protected string ForumID = ATC.Tools.URLParam("forumid", "").ToString();
        protected string TopicID = ATC.Tools.URLParam("TopicID", "").ToString();
        protected string TopicUserID = "";
        protected string CurrentUserID = "";
        protected string CurrentLanguageCode = "ENG1";

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {            
            try
            {
                CurrentUserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();

                if (TopicID != String.Empty)
                {
                    TopicUserID = ATC.Database.sqlGetFirst("Select UserID FROM uDebate_Forum_Topics WHERE ID = " + TopicID);
                    if (TopicUserID != CurrentUserID && !DotNetNuke.Security.PortalSecurity.IsInRole("Pilot Leaders")/*!Portal.Security.HasChangeContentPermission()*/)
                    {
                        Response.Redirect(ATC.Tools.GetParam("RootURL") + "AccessDenied.aspx");
                        return;
                    }
                }
                else
                {
                    if (!DotNetNuke.Security.PortalSecurity.IsInRole("Pilot Leaders")/*!Portal.Security.HasChangeContentPermission()*/)
                    {
                        Response.Redirect(ATC.Tools.GetParam("RootURL") + "AccessDenied.aspx");
                        return;
                    }
                }

                if (!IsPostBack)
                {
                    if (Edit != String.Empty)
                    {
                        LoadRecord();
                        deleteBtn.Attributes.Remove("href");
                        //deleteBtn.Attributes["href"] = "javascript:ConfirmDelete('deleteBtn')";
                    }
                    else
                    {
                        if (New.ToUpper() == "FORUM")
                        {
                            headerLbl.Text = "Νέο Forum";
                        }
                        else if (New.ToUpper() == "TOPIC")
                        {
                            headerLbl.Text = Localization.GetString("TopicNew", LocalResourceFile);
                            ATC.Database.FillLookUp(ddUser, false, "SELECT UserID, username" + ATC.Database.sqlConcatenate() +
                                "' ('" + ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Lastname)" +
                                ATC.Database.sqlConcatenate() + "' '" + ATC.Database.sqlConcatenate() +
                                ATC.Database.FNUpper() + "(Firstname)" + ATC.Database.sqlConcatenate() + "')'" +
                                " AS NameOfUser FROM Users WHERE (IsDeleted =0) ORDER BY username", "", "", "", true);
                            try
                            {
                                ddUser.SelectedValue = CurrentUserID;
                            }
                            catch
                            {
                            }
                        }
                    }

                    deleteBtn.Visible = (New.ToUpper() == String.Empty && (/*Portal.Security.IsAdministrator()*/ DotNetNuke.Security.PortalSecurity.IsInRole("Pilot Leaders") || TopicUserID == CurrentUserID));
                    btnNew.Enabled = (New.ToUpper() == String.Empty && Request.IsAuthenticated /*Portal.Security.HasChangeContentPermission()*/);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            LocalResourceFile = Localization.GetResourceFile(this, "Edit.ascx." + System.Threading.Thread.CurrentThread.CurrentCulture.Name + ".resx");
        }

        public void LoadRecord()
        {
            string SQL = "";

            if (Edit.ToUpper() == "FORUM" && ForumID != String.Empty)
            {
                headerLbl.Text = "Επεξεργασία Forum";
                SQL = "SELECT * from uDebate_forums where id=" + ForumID;
            }
            else if (Edit.ToUpper() == "TOPIC" && TopicID != String.Empty)
            {
                headerLbl.Text = "Επεξεργασία Θέματος";
                SQL = "SELECT * from uDebate_forum_topics where id=" + TopicID;
            }
            else
            {
                return;
            }

            IDbConnection Connection = ATC.Database.GenerateConnection();
            Connection.Open();
            try
            {

                DataRow dr = ATC.Database.sqlExecuteDataRow(SQL, Connection);

                descTxt.Text = dr["description"].ToString();
                summaryTxt.Text = dr["summary"].ToString();

                activeChk.Checked = (dr["active"].ToString() == "1");
                //StatusChk.Checked = (dr["Status"].ToString() == "1");

                if (Edit.ToUpper() == "TOPIC")
                {
                    lblUser.Visible = true;
                    ddUser.Visible = true;
                    ATC.Database.FillLookUp(ddUser, false, "SELECT UserID, username" +
                        ATC.Database.sqlConcatenate() + "' ('" + ATC.Database.sqlConcatenate() + 
                        ATC.Database.FNUpper() + "(Lastname)" + ATC.Database.sqlConcatenate() + "' '" +
                        ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Firstname)" +
                        ATC.Database.sqlConcatenate() + "')'" + " AS NameOfUser FROM Users WHERE IsDeleted =0 ORDER BY username", "", "", "", true);
                    ddUser.SelectedValue = dr["UserID"].ToString();
                    dropDownPosition.SelectedValue = dr["Position"].ToString();
                }
                else
                {
                    lblUser.Visible = false;
                    ddUser.Visible = false;

                }
            }
            catch (Exception ex)
            {
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία ανάκτησης των στοιχείων της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        protected void saveBtn_Click(object sender, EventArgs e)
        {
            string SQL = "";

            IDbConnection Connection = ATC.Database.GenerateConnection();
            Connection.Open();
            try
            {
                if (New != String.Empty)
                {
                    if (New.ToUpper() == "FORUM")
                    {
                        SQL = String.Format(@"INSERT INTO uDebate_forums(Language,description, summary, active, created_by, updated_by, status) 
                                              VALUES ({0}language,{0}Language, {0}summary, {1}, {2}, {2}, 1)", ATC.Database.Sql_VariableIdent, (activeChk.Checked ? "1" : "0"), CurrentUserID);

                        ATC.Database.sqlExecuteCommandParams(SQL, Connection,
                            new string[] { ATC.Database.Sql_VariableIdent + "Language", ATC.Database.Sql_VariableIdent + "description", ATC.Database.Sql_VariableIdent + "summary" },
                            new object[] { System.Threading.Thread.CurrentThread.CurrentCulture.Name,descTxt.Text, summaryTxt.Text });

                        ForumID = ATC.Database.sqlGetFirst("SELECT max(id) FROM uDebate_forums", Connection);

                        lblMessage.Visible = true;
                        lblMessage.Text = "<div class='dnnFormMessage dnnFormSuccess'>" +
                                "Forum succesfully created!" +
                                "</div>";
                    }
                    else if (New.ToUpper() == "TOPIC")
                    {
                        if (ForumID == String.Empty)
                            throw new Exception(ATC.Translate.String("Το Forum του θέματος δεν έχει οριστεί.", "", CurrentLanguageCode));

                        SQL = String.Format(@"INSERT INTO uDebate_forum_topics(Description, Summary, Active, ForumID, created_by, updated_by, userid, status, ModuleID, Language,Position) 
                                              VALUES ({0}description, {0}summary, {1}, {2}, {3}, {3}, {3}, 1,{4},{0}language),{5}",
                                ATC.Database.Sql_VariableIdent,
                                (activeChk.Checked ? "1" : "0"),
                                ForumID,CurrentUserID,ModuleId,dropDownPosition.SelectedValue);

                        ATC.Database.sqlExecuteCommandParams(SQL, Connection,
                            new string[] { ATC.Database.Sql_VariableIdent + "description", ATC.Database.Sql_VariableIdent + "summary", ATC.Database.Sql_VariableIdent + "language" },
                            new object[] { descTxt.Text, summaryTxt.Text, System.Threading.Thread.CurrentThread.CurrentCulture.Name });

                        TopicID = ATC.Database.sqlGetFirst("SELECT max(id) FROM uDebate_forum_topics", Connection);

                        //Response.Redirect("forums_edit.aspx?edit=topic&forumid=" + ForumID + "&TopicID=" + TopicID);

                        lblMessage.Visible = true;
                        lblMessage.Text = "<div class='dnnFormMessage dnnFormSuccess'>" +
                                "Topic succesfully created!" +
                                "</div>";
                    }
                }
                else if (Edit != String.Empty)
                {
                    if (Edit.ToUpper() == "FORUM")
                    {
                        SQL = String.Format(@"UPDATE uDebate_forums 
                                                 SET description = {0}description, 
                                                     summary = {0}summary, 
                                                     active = {1}, 
                                                     updated_by = {2}
                                              WHERE ID = {3}",
                                ATC.Database.Sql_VariableIdent,
                                (activeChk.Checked ? "1" : "0"),
                                CurrentUserID,
                                ForumID);

                        ATC.Database.sqlExecuteCommandParams(SQL, Connection,
                            new string[] { ATC.Database.Sql_VariableIdent + "description", ATC.Database.Sql_VariableIdent + "summary" },
                            new object[] { descTxt.Text, summaryTxt.Text });

                        lblMessage.Visible = true;
                        lblMessage.Text = "<div class='dnnFormMessage dnnFormSuccess'>" +
                                "Forum succesfully updated!" +
                                "</div>";
                    }
                    else if (Edit.ToUpper() == "TOPIC")
                    {
                        SQL = String.Format(@"UPDATE uDebate_forum_topics 
                                                 SET description = {0}description, 
                                                     summary = {0}summary, 
                                                     active = {1},
                                                     updated_by = {2}, 
                                                     userID = {3},
                                                     Position = {5}
                                              WHERE ID = {4}",
                                 ATC.Database.Sql_VariableIdent,
                                (activeChk.Checked ? "1" : "0"),
                                CurrentUserID,
                                ddUser.SelectedValue,
                                TopicID,dropDownPosition.SelectedValue);

                        ATC.Database.sqlExecuteCommandParams(SQL, Connection,
                            new string[] { ATC.Database.Sql_VariableIdent + "description", ATC.Database.Sql_VariableIdent + "summary" },
                            new object[] { descTxt.Text, summaryTxt.Text });

                        lblMessage.Visible = true;
                        lblMessage.Text = "<div class='dnnFormMessage dnnFormSuccess'>" +
                                "Topic succesfully updated!" +
                                "</div>";
                    }
                }
            }
            catch (Exception ex)
            {
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία αποθήκευσης των στοιχείων της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        protected void deleteBtn_Click(object sender, EventArgs e)
        {
            string SQL = "";
            if (Edit.ToUpper() == "FORUM")
            {
                SQL = "DELETE FROM uDebate_forums WHERE id =" + ForumID;
            }
            else if (Edit.ToUpper() == "TOPIC")
            {
                SQL = "DELETE FROM uDebate_forum_topics WHERE id =" + TopicID;
            }
            else
            {
                return;
            }

            try
            {
                int RowsAffected = ATC.Database.sqlExecuteCommand(SQL);
                if (RowsAffected == 0)
                    throw new Exception(ATC.Translate.String("Η εγγραφή φαίνεται να έχει διαγραφεί από άλλον χρήστη.", "", CurrentLanguageCode));

                ATC.Tools.CloseWindow();
            }
            catch (Exception ex)
            {
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία διαγραφής της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + ex.Message.ToString());
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (Edit.ToUpper() == "FORUM")
                Response.Redirect("Forums_Edit.aspx?new=forum&forumid=");
            else if (Edit.ToUpper() == "TOPIC")
            {
                ForumID = ATC.Database.sqlGetFirst("SELECT ForumID FROM uDebate_forum_topics WHERE ID = " + TopicID);
                if (ForumID != String.Empty)
                   // Response.Redirect("Forums_Edit.aspx?new=topic&forumid=" + ForumID + "&topicid=");
                   Response.Redirect(EditUrl(PortalSettings.ActiveTab.TabID, "EditForum", true, "mid=" + ModuleId +
                       "&forumid=" + ForumID + "&new=topic&topicid=-1"));
               else
                    ATC.MessageBox.Show(ATC.Translate.String("Το Forum του θέματος δεν έχει οριστεί.", "", CurrentLanguageCode));
            }
        }

        protected void cancelBtn_Click(object sender, System.EventArgs e)
        {
            ATC.Tools.CloseWindow();
        }

        private bool ValidateForm()
        {
            bool validator = true;
            string invalidChars = "'";// "'\"";//~!$&*()-={}[];'|,? ";

            if (descTxt.Text.Length == 0 || summaryTxt.Text.Length == 0)
            {
                ATC.MessageBox.Show(ATC.Translate.String("Τα πεδία 'Περιγραφή' & 'Περίληψη' πρέπει να συμπληρωθούν", "", CurrentLanguageCode));
                validator = false;
            }
            else
            {

                foreach (char c in invalidChars)
                {
                    if (descTxt.Text.Contains(c.ToString()) || summaryTxt.Text.Contains(c.ToString()))
                    {
                        validator = false;
                        ATC.MessageBox.Show(ATC.Translate.String("Οι παρακάτω χαρακτήρες δεν είναι αποδεκτοί: ~ ! $ & * ( ) - = { } [ ] ; ' | , ?  ", "", CurrentLanguageCode));
                        break;
                    }
                }
            }

            return validator;
        }
    }
}
