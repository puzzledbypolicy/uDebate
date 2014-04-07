using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using ATC;
using ATC.WebControls;

namespace DotNetNuke.Modules.uDebate.EditForms
{
	/// <summary>
	/// Summary description for Threads_Edit.
	/// </summary>
    public partial class Threads_Edit : System.Web.UI.Page
	{

		private bool IsNew = false;
		public string TopicID;
        public string ThreadID;
        protected string CurrentUserID = "";
        string CurrentLanguageCode = "EN";
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            //if (HttpContext.Current.Session["CurrentUserInfo"] != null)
            //    CurrentUserID = ((DataRow)HttpContext.Current.Session["CurrentUserInfo"])["ID"].ToString();
            //else
            //    CurrentUserID = ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString();
            CurrentUserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString(); 

            lbType.Text = ATC.Translate.String("Forum Type", "", CurrentLanguageCode);
            ddGroupID.Items.FindByValue("1").Text = ATC.Translate.String("Forum Type", "", CurrentLanguageCode) + " 1";
            ddGroupID.Items.FindByValue("2").Text = ATC.Translate.String("Forum Type", "", CurrentLanguageCode) + " 2";

            //Αν υπάρχει η παράμετρος ID στο URL κάνε edit την εγγραφή ...
			if (ATC.Tools.URLParam("ThreadID") != "")
			{
				IsNew = false;
                ThreadID = ATC.Tools.URLParam("ThreadID");

                TopicID = ATC.Database.sqlGetFirst("SELECT TopicID FROM uDebate_Forum_Threads WHERE ID = " + ThreadID);
                
                //για να μπορείς να τροποποιήσεις ένα συγκεκριμένο Thread αρκεί να είσαι ο Moderator του Thread 
                //ή να είσαι Administrator
                //ΝΕΟ ή να είσαι ο Coordinator του Topic στο οποίο ανήκει το Thread :) 

                string ThreadModerator = ATC.Database.sqlGetFirst("SELECT userID FROM uDebate_Forum_Threads WHERE ID = " + ThreadID);
                string TopicModerator = ATC.Database.sqlGetFirst("SELECT userID FROM uDebate_Forum_Topics WHERE ID = (SELECT TopicID FROM uDebate_Forum_Threads WHERE ID = " + ThreadID + ")");
                if (
                        !(Request.IsAuthenticated
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
                btnDelete.Attributes["href"] = "javascript:ConfirmDelete('btnDelete')";

                if (!IsPostBack) LoadData();			    
			}
			else
			{
                TopicID = ATC.Tools.GetParam("Topic");
                if (TopicID == String.Empty)
                {
                    Response.Write("<table><tr><td>" + ATC.Translate.String("Δεν έχει επιλεγεί Θέμα.", "", CurrentLanguageCode) + "</td></tr></table>");
                    Response.End();
                }

                //για να μπορείς να δημιουργήσεις ένα νέο Thread αρκεί να είσαι ο Moderator του Topic 
                //για το οποίο θέλεις να δημιουργήσεις αυτό το Thread ή να είσαι Administrator
                string TopicModerator = ATC.Database.sqlGetFirst("SELECT userID FROM uDebate_Forum_Topics WHERE ID = " + TopicID);
                if (!Request.IsAuthenticated) //!(Portal.Security.HasChangeContentPermission() || (Session["uDebate_User"] != null && ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString() == TopicModerator)))
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
                    ATC.Database.FillLookUp(ddStatusID, false, "SELECT [ID],[Title] FROM [uDebate_Forum_ThreadStatus]", "", "", "", true);
					try
                    {
                        ddUser.SelectedValue = CurrentUserID;
                    }
                    catch{}
                    txtThreadID.Text = "";
				}
			}		
		}

		private void LoadData()
		{
            
			DataSet ds = Database.sqlExecuteDataSet("SELECT * FROM uDebate_Forum_Threads WHERE ID = " + ThreadID);
			txtThreadID.Text = ds.Tables[0].Rows[0]["ID"].ToString();

            txtDescription.Text = ds.Tables[0].Rows[0]["Description"].ToString();
            txtSummary.Text = ds.Tables[0].Rows[0]["Summary"].ToString();
            //txtThread.Text = ds.Tables[0].Rows[0]["Text"].ToString();
            txtContact.Text = ds.Tables[0].Rows[0]["Contact_email"].ToString();
            txtComplaint.Text = ds.Tables[0].Rows[0]["Complain_email"].ToString();
            chkStatus.SelectedValue = ds.Tables[0].Rows[0]["Status"].ToString();
            chkActive.SelectedValue = ds.Tables[0].Rows[0]["Active"].ToString();

            ATC.Database.FillLookUp(ddUser, false, "SELECT userID, username" + ATC.Database.sqlConcatenate() + 
                "' ('" + ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Lastname)" + ATC.Database.sqlConcatenate() +
                "' '" + ATC.Database.sqlConcatenate() + ATC.Database.FNUpper() + "(Firstname)" + ATC.Database.sqlConcatenate() +
                "')'" + " AS NameOfUser FROM USERS WHERE (IsDeleted = 0) ORDER BY username", "", "", "", true);
                //WHERE (GroupID<>1 OR (GroupID=2 AND PublishPersonalInfo=1)) ORDER BY username", "", "", "", true);
            ddUser.SelectedValue = ds.Tables[0].Rows[0]["UserID"].ToString();
            ATC.Database.FillLookUp(ddStatusID, false, "SELECT [ID],[Title] FROM [uDebate_Forum_ThreadStatus]", "", "", "", true);
            ddStatusID.SelectedValue = ds.Tables[0].Rows[0]["StatusID"].ToString();
            ddGroupID.SelectedValue = ds.Tables[0].Rows[0]["GroupID"].ToString();
		}		
		
        		
		protected void btnNew_Click(object sender, System.EventArgs e)
		{
            ATC.Tools.RefreshPage("Topic=" + TopicID + ";ThreadID=");
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
				ATC.Database.sqlExecuteCommand("DELETE FROM uDebate_Forum_Threads WHERE ID = "+ ThreadID);
				
				ATC.Tools.CloseWindow();
			}
			catch (Exception ex)
			{
				int ErrNumber;
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία διαγραφής της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + Database.ErrorDescription(ex, out ErrNumber));
			}			
		}

        private void SaveNew()
        {
            try
            {
                if (txtDescription.Text.Trim() == "" || txtSummary.Text == "" || /*txtThread.Text.Trim()*/"hh" == string.Empty || txtContact.Text.Trim() == string.Empty || txtComplaint.Text.Trim() == string.Empty)
                    throw new Exception(ATC.Translate.String("All fields are mandatory!", "", CurrentLanguageCode));

                string SQL = "";

                string UserID = "";
                //if (HttpContext.Current.Session["uDebate_User"] != null)
                //    UserID = ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString();
                //else
                //    UserID = ((DataRow)HttpContext.Current.Session["CurrentUserInfo"])["ID"].ToString();

                UserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();
                //txtThread.Text = txtThread.Text.Replace("'", "\"");

                SQL = String.Format(
                        @"INSERT INTO uDebate_Forum_Threads
                           (TopicID
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
                           ,GroupID)
                        VALUES
                            (" + TopicID + @"
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
                            ," + chkStatus.SelectedValue.ToString() + @"
                            ," + chkActive.SelectedValue.ToString() + @"
                            ," + ddGroupID.SelectedValue.ToString() + @")
                       ", Database.Sql_VariableIdent.ToString());

                ATC.Database.sqlExecuteCommandParams(SQL,
                    new string[] { ATC.Database.Sql_VariableIdent + "Description", ATC.Database.Sql_VariableIdent + "Summary", ATC.Database.Sql_VariableIdent + "Text", ATC.Database.Sql_VariableIdent + "Complain_Email", ATC.Database.Sql_VariableIdent + "Contact_Email" },
                    new object[] { txtDescription.Text.Trim(), txtSummary.Text.Trim(), /*txtThread.Text.Trim()*/"", txtComplaint.Text.Trim(), txtContact.Text.Trim() });
               

                string NewID = ATC.Database.sqlGetFirst("SELECT MAX(ID) FROM uDebate_Forum_Threads");

                Response.Redirect("Threads_Edit.aspx?topicID=" + TopicID + "&threadID=" + NewID);
            }
            catch (Exception ex)
            {
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία αποθήκευσης των στοιχείων της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + ex.Message);
            }
        }

        private void SaveEdit()
        {
            try
            {
                if (txtDescription.Text.Trim() == "" || txtSummary.Text == "" || /*txtThread.Text.Trim()*/"jj" == string.Empty)
                    throw new Exception(ATC.Translate.String("All fields are mandatory!", "", CurrentLanguageCode));

                string SQL = "";

                string UserID = "";
                //if (HttpContext.Current.Session["uDebate_User"] != null)
                //    UserID = ((DataRow)HttpContext.Current.Session["uDebate_User"])["ID"].ToString();
                //else
                //    UserID = ((DataRow)HttpContext.Current.Session["CurrentUserInfo"])["ID"].ToString();

                UserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();

                string closedDate = "";
                if (chkStatus.SelectedValue == "0")
                    closedDate = "getDate()";
                else
                    closedDate = "NULL";

                //txtThread.Text = txtThread.Text.Replace("'", "\"");

                SQL = String.Format(@" UPDATE uDebate_Forum_Threads
                           SET Description = {0}Description
                              ,Summary = {0}Summary
                              ,Text = {0}Text
                              ,LAST_UPDATED = getDate()
                              ,UPDATED_BY = " + UserID + @"
                              ,Complain_email = {0}Complain_Email
                              ,Contact_email = {0}Contact_Email
                              ,Active = " + chkActive.SelectedValue.ToString() + @"
                              ,Status = " + chkStatus.SelectedValue.ToString() + @"
                              ,Closed_Date = " + closedDate + @"
                              ,UserID = " + ddUser.SelectedValue + @"
                              ,StatusID = " + ddStatusID.SelectedValue + @"
                              ,GroupID = " + ddGroupID.SelectedValue + @"
                         WHERE ID = " + ThreadID, Database.Sql_VariableIdent.ToString());
                ATC.Database.sqlExecuteCommandParams(SQL,
                    new string[] { ATC.Database.Sql_VariableIdent + "Description", ATC.Database.Sql_VariableIdent + "Summary", ATC.Database.Sql_VariableIdent + "Text", ATC.Database.Sql_VariableIdent + "Complain_Email", ATC.Database.Sql_VariableIdent + "Contact_Email" },
                    new object[] { txtDescription.Text, txtSummary.Text, /*txtThread.Text.Trim()*/"", txtComplaint.Text.Trim(), txtContact.Text.Trim() });

                Response.Redirect("Threads_Edit.aspx?topicID=" + TopicID + "&threadID=" + ThreadID);
            }
            catch (Exception ex)
            {
                ATC.MessageBox.Show(ATC.Translate.String("Αδυναμία αποθήκευσης των στοιχείων της εγγραφής.", "", CurrentLanguageCode) + "\r\n" + ATC.Translate.String("Σφάλμα: ", "", CurrentLanguageCode) + ex.Message);
            }
        }
	}
}
