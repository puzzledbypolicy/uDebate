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
    public partial class TopicsThreadsView : uDebateModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /* missing variables */
            string CurrentLanguageCode = "en-US";

            string PreviousTopic = "";
            string editLink = "";
            string newThread = "";
            bool Inactive = false;

            bool DisplayTopic = true;// (ReadData("TOPIC_THREAD_VIEW_DISPLAY_TOPIC") == "1");
            string TopicLinkCode = "";// ReadData("TOPIC_THREAD_VIEW_TOPIC_LINK");

            bool DisplayThread = true;// (ReadData("TOPIC_THREAD_VIEW_DISPLAY_THREAD") == "1");
            string ThreadLinkCode = "";// ReadData("TOPIC_THREAD_VIEW_THREAD_LINK");
            //if (ThreadLinkCode == String.Empty) ThreadLinkCode = PageCode;
            
            string SQL = CreateSQLStatement(DisplayTopic, DisplayThread);

            try
            {
                DataSet ds = ATC.Database.sqlExecuteDataSet(SQL);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Forums.Controls.Add(new LiteralControl("<table cellspacing='0' cellpadding='0' width='100%'><tr><td>" + ATC.Translate.String("Sorry. No data found!", "", CurrentLanguageCode) + "</td><tr></table>"));
                    return;
                }

                Forums.Controls.Add(new LiteralControl("<table cellspacing='0' cellpadding='0' width='100%'>"));

                int i = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (DisplayTopic && PreviousTopic != row["Topic_ID"].ToString())
                    {
                        Inactive = (row["TopicActive"].ToString() == "0");

                        if (
                              (
                                (HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == row["Topic_User_ID"].ToString())
                                ||
                                (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                              )
                              &&
                              (
                                  row["Topic_Status"].ToString() == "1"
                              )
                            )
                        {
                            editLink = "<a href=javascript:OpenDialog('Satellites/Forums_Edit.aspx?TopicID=" + row["Topic_ID"].ToString() + "&edit=TOPIC','ForumEdit',500,400)><img alt='" + ATC.Translate.String("Αλλαγή", "", CurrentLanguageCode) + "' border=0 src='" + ATC.Tools.GetParam("RootURL") + "img/admin/edit.gif'></a>&nbsp;";
                            string newThreadUrl = "javascript:OpenDialog('Satellites/Threads_Edit.aspx?Topic=" + ATC.Tools.URLParam("Topic") + "&ThreadID=','ThreadEdit',800,730)";
                            newThread = "<td class='topic_threads_new_thread_td'><input type=\"button\" id=\"btnNewThread\" class=\"forum_button\" value=\"" + ATC.Translate.String("New Thread", "", CurrentLanguageCode) + "\" onclick=\"" + newThreadUrl + "\" /></td>";
                        }

                        Forums.Controls.Add(new LiteralControl(@"
                                                            <tr>
                                                                <td class='topic_threads_topic_header1'>
                                                                    <table width='100%' cellpadding='0' cellspacing='0'>
                                                                        <tr>
                                                                            <td class='topic_threads_topic_header2'>
                                                                                " + editLink + row["Topic_Desc"].ToString() + (Inactive ? " - <span class='topic_threads_inactive_span'>" + (ATC.Translate.String("INACTIVE", "", CurrentLanguageCode)) + "</span>" : "") + newThread + @"
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            "));
                        
                    }
                        
                    string editThread = "";
                    if (DisplayThread && row["Thread_User_ID"].ToString() != "")
                    {
                        Inactive = (row["ThreadActive"].ToString() == "0");

                        if (
                            (HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == row["Topic_User_ID"].ToString())
                            ||
                            (HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == row["Thread_User_ID"].ToString())
                            ||
                            (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                            )
                        {
                            editThread = "<a href=javascript:OpenDialog('Satellites/Threads_Edit.aspx?TopicID=" + row["Topic_ID"].ToString() + "&ThreadID=" + row["Thread_ID"].ToString() + "','ThreadsEdit',800,730)><img alt='" + ATC.Translate.String("Edit this Thread", "", CurrentLanguageCode) + "' border=0 src='" + ATC.Tools.GetParam("RootURL") + "img/admin/edit.gif'></a>";
                        }

                        Forums.Controls.Add(new LiteralControl(@"  
                            <tr><td height='5'></td></tr>
                            <tr>
                                <td class='topic_threads_thread_outer_td'>
                                    <table width='100%' cellpadding='0' cellspacing='0' class='topic_threads_thread_table'>
                                        <tr>
                                            <td class='topic_threads_left_blank_area'></td>
                                            <td class='topic_threads_thread_desc'>" + editThread + @"
                                                <span class='forum_link'>
                                                    <a class='forum_link' href='?page=" + ThreadLinkCode + @"&Thread=" + row["Thread_ID"].ToString() + "'>" + row["Thread_Desc"].ToString() + @"
                                                    </a>
                                                </span>
                                                " + (Inactive ? " - <span style='color:red;font-weight:bold'>" + (ATC.Translate.String("INACTIVE", "", CurrentLanguageCode)) + "</span>" : "") + @"
                                            </td>
                                            <td class='topic_threads_thread_details'>
                                                " + row["Surname"].ToString() + @"&nbsp;" + row["Name"].ToString() + @"<br />
                                                " + ATC.Translate.String("Opened date:", "", CurrentLanguageCode) + Convert.ToDateTime(row["Opened_Date"].ToString()).ToShortDateString() + @"<br />
                                                " + (row["Thread_Status"].ToString() == "2" && row["Closed_Date"].ToString() != string.Empty ? ATC.Translate.String("Closed date:", "", CurrentLanguageCode) + Convert.ToDateTime(row["Closed_Date"].ToString()).ToShortDateString() + "<br />" : "") + @"
                                                " + ATC.Translate.String("Posts: ", "", CurrentLanguageCode) + ATC.Database.sqlGetFirst("SELECT Count(ID) FROM uDebate_Forum_Posts WHERE Active = 1 AND IsPublished = 1 AND ThreadID = " + row["Thread_ID"].ToString()) + @"<br />
                                                " + ATC.Translate.String("Views: ", "", CurrentLanguageCode) + row["Thread_ViewCount"].ToString() + @"
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class='topic_threads_left_blank_area'></td>
                                            <td class='topic_threads_thread_summary' colspan='2'><a class='topic_threads_thread_summary' href='?page=" + ThreadLinkCode + @"&Thread=" + row["Thread_ID"].ToString() + "'>" +
                                                row["Thread_Sum"].ToString() +
                                            @"</a></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                        "));
                    }
                    else
                    {
                        Forums.Controls.Add(new LiteralControl(@"
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>"));
                    }
                    PreviousTopic = row["Topic_ID"].ToString();
                    editLink = "";
                    i++;
                }

                Forums.Controls.Add(new LiteralControl("</table>"));
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

        }

        private string CreateSQLStatement(bool DisplayTopic, bool DisplayThread)
        {
            string SQL = "";
            try
            {
                string SelectStatement = "";
                string WhereStatement = "";
                string OrderByStatement = "";

                string Status = "";// ReadData("TOPIC_THREAD_VIEW_STATUS");
                if (Status == String.Empty) Status = "3";

                //Select statement
                SelectStatement = @"SELECT ";
                SelectStatement += " T.ID AS Topic_ID, T.Description AS Topic_Desc, T.Summary AS Topic_Sum, T.Text AS Topic_Text, User_Topic.Username AS Topic_User, T.UserID AS Topic_User_ID, T.CREATED AS Topic_Date, T.Active as TopicActive, T.Status AS Topic_Status,";

                if (DisplayThread)
                    //SelectStatement += " Th.ID AS Thread_ID, Th.Description AS Thread_Desc, Th.Summary AS Thread_Sum, Th.Text AS Thread_Text, Th.CREATED AS Opened_Date, User_Thread.Username AS Thread_User, Th.UserID AS Thread_User_ID, Th.Active as ThreadActive, User_Thread.SurName AS Surname, User_Thread.FirstName AS Name, Th.Closed_Date AS Closed_Date, Th.Status AS Thread_Status, Th.View_Count AS Thread_ViewCount,";
                    SelectStatement += " Th.ID AS Thread_ID, Th.Description AS Thread_Desc, Th.Summary AS Thread_Sum, Th.Text AS Thread_Text, Th.CREATED AS Opened_Date, User_Thread.Username AS Thread_User, Th.UserID AS Thread_User_ID, Th.Active as ThreadActive, User_Thread.LastName AS Surname, User_Thread.FirstName AS Name, Th.Closed_Date AS Closed_Date, Th.Status AS Thread_Status, Th.View_Count AS Thread_ViewCount,";
                
                else
                    SelectStatement += " NULL AS Thread_ID, NULL AS Thread_Desc, NULL AS Thread_Sum, NULL AS Thread_Text, NULL AS Opened_Date, NULL AS Thread_User, NULL as ThreadActive, NULL AS Name, NULL AS Surname, NULL AS Closed_Date, NULL AS Thread_Status,NULL AS Thread_ViewCount,";
                SelectStatement = SelectStatement.TrimEnd(',');

                //From statement
                SelectStatement += " FROM ";
                SelectStatement += " uDebate_Forum_Topics AS T ";
                //SelectStatement += " INNER JOIN uDebate_Users AS User_Topic ON T.UserID = User_Topic.ID ";
                SelectStatement += " INNER JOIN Users AS User_Topic ON T.UserID = User_Topic.UserID ";
                if (DisplayThread) SelectStatement += " LEFT OUTER JOIN uDebate_Forum_Threads AS Th ON Th.TopicID = T.ID ";
                //if (DisplayThread) SelectStatement += " LEFT OUTER JOIN uDebate_Users AS User_Thread ON Th.UserID = User_Thread.ID ";
                if (DisplayThread)
                    SelectStatement += " LEFT OUTER JOIN Users AS User_Thread ON Th.UserID = User_Thread.UserID ";

                string TopicID = "-1";// ReadData("TOPIC_THREAD_VIEW_ID");
                if (TopicID == "-1") TopicID = ATC.Tools.IntURLParam("Topic"); //Current Forum
                if (TopicID != String.Empty)
                    WhereStatement += " AND T.ID = " + TopicID;

                if (
                    DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders")
                    ||
                    (HttpContext.Current.Session["Portal_User"] != null && ((DataRow)HttpContext.Current.Session["Portal_User"])["ID"].ToString() == ATC.Database.sqlGetFirst("SELECT UserID FROM uDebate_Forum_Topics WHERE ID = " + TopicID))
                    )
                {
                    if (Status != "3")
                    {
                        if (DisplayTopic && DisplayThread) SelectStatement += " AND Th.Status = " + Status;
                        else if (DisplayThread) SelectStatement += " AND Th.Status = " + Status;
                        else if (DisplayTopic) WhereStatement += " AND T.Status = " + Status;
                    }
                }
                else
                {
                    if (DisplayTopic) WhereStatement += " AND T.Active = 1";

                    if (Session["Portal_User"] != null && DisplayThread)
                        WhereStatement += " AND (Th.Active = 1 OR Th.UserID = " + ((DataRow)Session["Portal_User"])["ID"].ToString() + ") ";

                    if (Session["Portal_User"] == null && DisplayThread)
                        WhereStatement += " AND Th.Active = 1 ";

                    if (Status != "3")
                    {
                        if (DisplayThread) SelectStatement += " AND Th.Status = " + Status;
                        else if (DisplayTopic) WhereStatement += " AND T.Status = " + Status;
                    }
                }
                if (WhereStatement != String.Empty) WhereStatement = " WHERE " + WhereStatement.Substring(4);

                //Order By 
                string OrderBy = "";// ReadData("TOPIC_THREAD_VIEW_ORDER_FIELD");
                if (OrderBy != String.Empty)
                {
                    OrderByStatement += " ORDER BY ";
                    if (DisplayTopic) OrderByStatement += " T." + OrderBy + ",";
                    if (DisplayThread) OrderByStatement += " [Th]." + OrderBy + ",";
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
    }
}