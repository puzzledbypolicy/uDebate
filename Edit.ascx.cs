/*
' Copyright (c) 2010 DotNetNuke Corporation
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using DotNetNuke.Services.Exceptions;
using ATC;
using ATC.WebControls;
using System.Data;
using DotNetNuke.Common;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.uDebate
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The EdituDebate class is used to manage content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Edit : uDebateModuleBase
    {
        private bool IsNew = false;
        public string TopicID;
        public string ThreadID;
        protected string CurrentUserID = "";
        string CurrentLanguageCode = "EN";

        #region Event Handlers

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// -----------------------------------------------------------------------------
        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {               
                CurrentUserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();             

                //Αν υπάρχει η παράμετρος ID στο URL κάνε edit την εγγραφή ...
                int thID = Convert.ToInt32(ATC.Tools.URLParam("ThreadID"));

                if ( thID > 0)
                {
                    IsNew = false;
                    ThreadID = ATC.Tools.URLParam("ThreadID");
                    TopicID = Request.Url.ToString();
                    TopicID = ATC.Database.sqlGetFirst("SELECT TopicID FROM uDebate_Forum_Threads WHERE ID = " + ThreadID);

                    //για να μπορείς να τροποποιήσεις ένα συγκεκριμένο Thread αρκεί να είσαι ο Moderator του Thread 
                    //ή να είσαι Administrator
                    //ΝΕΟ ή να είσαι ο Coordinator του Topic στο οποίο ανήκει το Thread :) 

                    string ThreadModerator = ATC.Database.sqlGetFirst("SELECT userID FROM uDebate_Forum_Threads WHERE ID = " + ThreadID);
                    string TopicModerator = ATC.Database.sqlGetFirst("SELECT userID FROM uDebate_Forum_Topics WHERE ID = (SELECT TopicID FROM uDebate_Forum_Threads WHERE ID = " + ThreadID + ")");
                    if (
                            !(Request.IsAuthenticated && DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders")
                        //Portal.Security.HasChangeContentPermission()
                        //||
                        //(Session["uDebate_User"] != null && ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString() == ThreadModerator)
                        //||
                        //(Session["uDebate_User"] != null && ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString() == TopicModerator)
                             )
                        )
                    {
                        Response.Redirect(ATC.Tools.GetParam("RootURL") + "AccessDenied.aspx");
                        return;
                    }

                    btnDelete.Visible = true;// (Portal.Security.IsAdministrator() || (Session["uDebate_User"] != null && ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString() == ThreadModerator) || (Session["uDebate_User"] != null && ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString() == TopicModerator));                

                    btnDelete.Attributes.Remove("href");
                    //btnDelete.Attributes["href"] = "javascript:ConfirmDelete('btnDelete')";

                    if (!IsPostBack) LoadData();
                }
                else
                {
                    TopicID = Request.Url.ToString();
                    TopicID = ATC.Tools.URLParam("Topic");
                    if (TopicID == String.Empty)
                    {
                        Response.Write("<table><tr><td>" + ATC.Translate.String("Δεν έχει επιλεγεί Θέμα.", "", CurrentLanguageCode) + "</td></tr></table>");
                        Response.End();
                    }

                    //για να μπορείς να δημιουργήσεις ένα νέο Thread αρκεί να είσαι ο Moderator του Topic 
                    //για το οποίο θέλεις να δημιουργήσεις αυτό το Thread ή να είσαι Administrator
                    string TopicModerator = ATC.Database.sqlGetFirst("SELECT userID FROM uDebate_Forum_Topics WHERE ID = " + TopicID);
                    if (!Request.IsAuthenticated /*&& DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders")*/) //!(Portal.Security.HasChangeContentPermission() || (Session["uDebate_User"] != null && ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString() == TopicModerator)))
                    {
                        Response.Redirect(ATC.Tools.GetParam("RootURL") + "AccessDenied.aspx");
                        return;
                    }

                    IsNew = true;

                    if (!IsPostBack)
                    {
                        ATC.Database.FillLookUp(ddUser, false, "SELECT userID, username" + ATC.Database.sqlConcatenate() +
                            "' ('" + ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Lastname)" + ATC.Database.sqlConcatenate() +
                            "' '" + ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Firstname)" + ATC.Database.sqlConcatenate() +
                            "')'" + " AS NameOfUser FROM USERS WHERE (IsDeleted =0) ORDER BY username", "", "", "", true);
                        //WHERE (GroupID<>1 OR (GroupID=2 AND PublishPersonalInfo=1)) ORDER BY username", "", "", "", true);
                        //ATC.Database.FillLookUp(ddStatusID, false, "SELECT [ID],[Title] FROM [uDebate_Forum_ThreadStatus]", "", "", "", true);
                        try
                        {
                            ddUser.SelectedValue = CurrentUserID;
                        }
                        catch { }
                        //txtThreadID.Text = "";
                    }
                }

                // Registered users can only create new thread in inactive mode
                if (!DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                {
                    btnDelete.Visible = false;
                    btnNew.Visible = false;
                    chkActive.SelectedIndex = 0;
                    chkActive.Enabled = false;
                }
                LocalResourceFile = Localization.GetResourceFile(this, "Edit.ascx." + System.Threading.Thread.CurrentThread.CurrentCulture.Name + ".resx");
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion



        private void LoadData()
        {

            DataSet ds = Database.sqlExecuteDataSet("SELECT * FROM uDebate_Forum_Threads WHERE ID = " + ThreadID);
            //txtThreadID.Text = ds.Tables[0].Rows[0]["ID"].ToString();

            txtDescription.Text = ds.Tables[0].Rows[0]["Description"].ToString();
            //txtSummary.Text = ds.Tables[0].Rows[0]["Summary"].ToString();
            txtThread.Text = ds.Tables[0].Rows[0]["Text"].ToString();
            //txtContact.Text = ds.Tables[0].Rows[0]["Contact_email"].ToString();
            //txtComplaint.Text = ds.Tables[0].Rows[0]["Complain_email"].ToString();
            //chkStatus.SelectedValue = ds.Tables[0].Rows[0]["Status"].ToString();
            chkActive.SelectedValue = ds.Tables[0].Rows[0]["Active"].ToString();
            chkEuropean.SelectedValue = ds.Tables[0].Rows[0]["EuRelated"].ToString() == "True" ? "1" : "0";

            ATC.Database.FillLookUp(ddUser, false, "SELECT userID, username" + ATC.Database.sqlConcatenate() +
                "' ('" + ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Lastname)" + ATC.Database.sqlConcatenate() +
                "' '" + ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Firstname)" + ATC.Database.sqlConcatenate() +
                "')'" + " AS NameOfUser FROM USERS WHERE (IsDeleted = 0) ORDER BY username", "", "", "", true);
            //WHERE (GroupID<>1 OR (GroupID=2 AND PublishPersonalInfo=1)) ORDER BY username", "", "", "", true);
            ddUser.SelectedValue = ds.Tables[0].Rows[0]["UserID"].ToString();
           // ATC.Database.FillLookUp(ddStatusID, false, "SELECT [ID],[Title] FROM [uDebate_Forum_ThreadStatus]", "", "", "", true);
            //ddStatusID.SelectedValue = ds.Tables[0].Rows[0]["StatusID"].ToString();
            //ddGroupID.SelectedValue = ds.Tables[0].Rows[0]["GroupID"].ToString();
        }


        protected void btnNew_Click(object sender, System.EventArgs e)
        {
            //ATC.Tools.RefreshPage("Topic=" + TopicID + ";ThreadID=");
            Response.Redirect(EditUrl(PortalSettings.ActiveTab.TabID, "Edit", true, "mid=" + ModuleId + "&Topic=" + TopicID + "&ThreadID=-1"));
            //Response.Redirect("/tabid/" + PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId + "/Topic/" + 
              //  TopicID+ "/ThreadID/-1/Udebate.aspx");
        }

        protected void btnSave_Click(object sender, System.EventArgs e)
        {
            if (!IsNew)
                SaveEdit();
            else
                SaveNew();
        }

        protected void btnDelete_Click(object sender, System.EventArgs e)
        {
            if (ThreadID == String.Empty) return;

            try
            {
                ATC.Database.sqlExecuteCommand("DELETE FROM uDebate_Forum_Threads WHERE ID = " + ThreadID);

               // Response.Redirect(Globals.NavigateURL(), true);
           
                ATC.Tools.CloseWindow();
            }
            catch (Exception ex)
            {
                int ErrNumber;
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία διαγραφής της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + Database.ErrorDescription(ex, out ErrNumber));
            }
        }

        protected void btnCancel_Click(object sender, System.EventArgs e)
        {            
            ATC.Tools.CloseWindow();
        }

        private void SaveNew()
        {
            try
            {
                if (txtDescription.Text.Trim() == "" /*|| txtSummary.Text == ""*/ || txtThread.Text.Trim() == string.Empty /*|| txtContact.Text.Trim() == string.Empty || txtComplaint.Text.Trim() == string.Empty*/)
                    throw new Exception(ATC.Translate.String("All fields are mandatory!", "", CurrentLanguageCode));

                string SQL = "";

                string UserID = "";
                //if (HttpContext.Current.Session["uDebate_User"] != null)
                //    UserID = ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString();
                //else
                //    UserID = ((DataRow)HttpContext.Current.Session["CurrentUserInfo"])["ID"].ToString();

                UserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();
                txtThread.Text = txtThread.Text.Replace("'", "\"");

                SQL = String.Format(
                        @"INSERT INTO uDebate_Forum_Threads
                           (TopicID
                           ,Language
                           ,EuRelated
                           ,Description
                           ,Summary
                           ,Text
                           ,UserID
                           ,Complain_email
                           ,Contact_email
                           ,CREATED
                           ,LAST_UPDATED
                           ,CREATED_BY
                           ,UPDATED_BY
                           ,Closed_Date
                           ,Opened_Date
                           ,STATUS
                           ,Active
                           ,GroupID
                           ,ModuleID
                           ,Position)
                        VALUES
                            (" + TopicID + @"
                            ,'" + System.Threading.Thread.CurrentThread.CurrentCulture.Name + @"'
                            ," + chkEuropean.SelectedValue.ToString() + @"
                            , {0}Description
                            , {0}Summary
                            , {0}Text
                            ," + UserID + @"
                            ,{0}Complain_Email
                            ,{0}Contact_Email
                            , getDate()
                            , getDate()
                            ," + UserID + @"
                            ," + UserID + @"
                            , NULL
                            , getDate()
                            ," + /*chkStatus.SelectedValue.ToString()*/ "1" + @"
                            ," + chkActive.SelectedValue.ToString() + @"
                            ,1" /*+ ddGroupID.SelectedValue.ToString()*/ + @"
                            ," + ModuleId + "," + dropDownPosition.SelectedValue + ")", Database.Sql_VariableIdent.ToString());

              
                ATC.Database.sqlExecuteCommandParams(SQL,
                    new string[] { ATC.Database.Sql_VariableIdent + "Description", ATC.Database.Sql_VariableIdent + "Summary", ATC.Database.Sql_VariableIdent + "Text", ATC.Database.Sql_VariableIdent + "Complain_Email", ATC.Database.Sql_VariableIdent + "Contact_Email" },
                    new object[] { txtDescription.Text.Trim(), /*txtSummary.Text.Trim()*/"", txtThread.Text.Trim(), ""/*txtComplaint.Text.Trim()*/, /*txtContact.Text.Trim()*/"" });


                string NewID = ATC.Database.sqlGetFirst("SELECT MAX(ID) FROM uDebate_Forum_Threads");
                
                Response.Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "TreeView",
                                 "mid=" + ModuleId + "&Thread=" + NewID), true);
                                
            }
            catch (Exception ex)
            {
                ATC.MessageBox.Show(ex.Message);
            }
        }

        private void SaveEdit()
        {
            try
            {
                if (txtDescription.Text.Trim() == "" ||/* txtSummary.Text == "" ||*/ txtThread.Text.Trim() == string.Empty)
                    throw new Exception(ATC.Translate.String("All fields are mandatory!", "", CurrentLanguageCode));

                string SQL = "";

                string UserID = "";
                //if (HttpContext.Current.Session["uDebate_User"] != null)
                //    UserID = ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString();
                //else
                //    UserID = ((DataRow)HttpContext.Current.Session["CurrentUserInfo"])["ID"].ToString();

                UserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();

                string closedDate = "NULL";
                /*if (chkStatus.SelectedValue == "0")
                    closedDate = "getDate()";
                else
                    closedDate = "NULL";*/

                txtThread.Text = txtThread.Text.Replace("'", "\"");

                SQL = String.Format(@" UPDATE uDebate_Forum_Threads
                           SET Description = {0}Description
                              ,Summary = {0}Summary
                              ,Text = {0}Text
                              ,LAST_UPDATED = getDate()
                              ,UPDATED_BY = " + UserID + @"
                              ,Complain_email = {0}Complain_Email
                              ,Contact_email = {0}Contact_Email
                              ,EuRelated = " + chkEuropean.SelectedValue.ToString() + @"
                              ,Active = " + chkActive.SelectedValue.ToString() /*+ @"
                              ,Status = " + chkStatus.SelectedValue.ToString()*/ + @"
                              ,Closed_Date = " + closedDate + @"
                              ,UserID = " + ddUser.SelectedValue /*+ @"
                              ,StatusID = " + ddStatusID.SelectedValue */+ @"
                              ,GroupID = 1, Position = " + dropDownPosition.SelectedValue /*+ ddGroupID.SelectedValue */+ @"
                         WHERE ID = " + ThreadID, Database.Sql_VariableIdent.ToString());
                ATC.Database.sqlExecuteCommandParams(SQL,
                    new string[] { ATC.Database.Sql_VariableIdent + "Description", ATC.Database.Sql_VariableIdent + "Summary", ATC.Database.Sql_VariableIdent + "Text", ATC.Database.Sql_VariableIdent + "Complain_Email", ATC.Database.Sql_VariableIdent + "Contact_Email" },
                    new object[] { txtDescription.Text, ""/*txtSummary.Text*/, txtThread.Text.Trim(),"","" /*txtComplaint.Text.Trim(), txtContact.Text.Trim()*/ });

                lblMessage.Visible = true;
                lblMessage.Text = "<div class='dnnFormMessage dnnFormSuccess'>" +
                        "Thread succesfully updated!" +
                        "</div>";
                //Response.Redirect("Threads_Edit.aspx?topicID=" + TopicID + "&threadID=" + ThreadID);
            }
            catch (Exception ex)
            {
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία αποθήκευσης των στοιχείων της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + ex.Message);
            }
        }
    }

}