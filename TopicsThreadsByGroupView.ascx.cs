using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATC.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using System.Configuration;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;

namespace DotNetNuke.Modules.uDebate
{
    public partial class TopicsThreadsByGroupView : uDebateModuleBase
    {
        ImageButtonExt myExtBtn = new ImageButtonExt();
        string newThreadUrl = string.Empty;
        public string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        protected void Page_Load(object sender, EventArgs e)
        {

            SqluDebateThreads.SelectParameters["TopicID"].DefaultValue = ATC.Tools.URLParam("Topic");

            if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
            {
                SqluDebateThreads.SelectParameters["ThreadStatus"].DefaultValue = "1";
            }

            LinkButton orderPopular = ThreadsListView.FindControl("orderPopular") as LinkButton;
            LinkButton orderDate = ThreadsListView.FindControl("orderDate") as LinkButton;            
            orderPopular.CssClass = "";
            orderDate.CssClass = "";
           

            /* missing variables */
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            string PreviousTopic = "";
            //string editLink = "";            
            string newThread = "";
            bool Inactive = false;

            //newThreadUrl = "javascript:OpenDialog('Satellites/Threads_Edit.aspx?Topic=" + ATC.Tools.URLParam("Topic") + "&ThreadID=','ThreadEdit',800,730)";
            newThreadUrl =  "/tabid/" + PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                            "/Topic/" + ATC.Tools.URLParam("Topic") + "/ThreadID/-1" +
                            "/language/" + currentCulture + "/default.aspx";

            myExtBtn.OnClientClick = "return " + UrlUtils.PopUpUrl(newThreadUrl, this, PortalSettings, true, false);
           //TODO: new thread button in all languages
            myExtBtn.ImageUrl = "images/buttons/new_thread_" + currentCulture +".png";
            myExtBtn.ImageOverUrl = "images/buttons/new_thread_" + currentCulture + "_sel.png";
           

            bool DisplayTopic = true;// (ReadData("TOPIC_THREAD_VIEW_DISPLAY_TOPIC") == "1");
            string TopicLinkCode = "";// ReadData("TOPIC_THREAD_VIEW_TOPIC_LINK");

            bool DisplayThread = true;//(ReadData("TOPIC_THREAD_VIEW_DISPLAY_THREAD") == "1");
            string ThreadLinkCode = "";// ReadData("TOPIC_THREAD_VIEW_THREAD_LINK");
           // if (ThreadLinkCode == String.Empty) ThreadLinkCode = PageCode;

            LocalResourceFile = Localization.GetResourceFile(this, "ForumsTopicsView.ascx." + currentCulture + ".resx");
 
            string SQL = CreateSQLStatement(DisplayTopic, DisplayThread);
            Response.Write(SQL);

            if (ATC.Tools.URLParam("PrintSQL") == "1")
                Response.Write(SQL);

            try
            {
                DataSet ds = ATC.Database.sqlExecuteDataSet(SQL);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    IDbConnection Connection = ATC.Database.GenerateConnection();
                    Connection.Open();
                    bool showNewThread = false;
                    String description = ATC.Database.sqlGetFirst("SELECT Description FROM uDebate_forum_topics where id=" + ATC.Tools.URLParam("Topic"), Connection);
                                       
                    if ( Request.IsAuthenticated)
                    {
                        showNewThread = true;
                      }

                    Forums.Controls.Add(new LiteralControl(@"<table cellspacing='0' cellpadding='0' width='100%'>
                                                            <tr>
                                                                <td class='topic_threads_topic_header1'>
                                                                    <table width='100%' cellpadding='0' cellspacing='0' border=0>
                                                                        <tr>
                                                                            <td class='topic_threads_topic_header2'>
                                                                                "));
                    
                    Forums.Controls.Add(new LiteralControl(@"" +
                    description + (Inactive ? " - <span class='topic_threads_inactive_span'>" +
                    Localization.GetString("Inactive", LocalResourceFile) + "</span>" : "")));

                    if (showNewThread)
                    {

                        Forums.Controls.Add(new LiteralControl(@"<td class='topic_threads_topic_header2' align=right>"));
                        Forums.Controls.Add(myExtBtn);
                        Forums.Controls.Add(new LiteralControl(@"</td>"));
                    }
                    Forums.Controls.Add(new LiteralControl(@"
                                                                        </tr>                                                                       
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                                    <tr><td>"                               
                                                + Localization.GetString("NoData", LocalResourceFile) + @"</td><tr></table>
                                                            ")); 
                    return;
                }

                Forums.Controls.Add(new LiteralControl("<table cellspacing='0' cellpadding='0' width='100%'>"));

                int i = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (DisplayTopic && PreviousTopic != row["Topic_ID"].ToString())
                    {
                        Inactive = (row["TopicActive"].ToString() == "0");
                        bool showNewThread = false;
                        HyperLink editLink = new HyperLink();

                        if (
                              (
                                (Request.IsAuthenticated /*&& Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == row["Topic_User_ID"].ToString())*/
                                &&
                                DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                              )
                              &&
                              (
                                  row["Topic_Status"].ToString() == "1"
                              )
                            )
                        {
                            showNewThread = true;
                            
                            editLink.NavigateUrl = "/tabid/" + PortalSettings.ActiveTab.TabID + "/ctl/EditForum/mid/" + ModuleId +
                           "/TopicID/" + row["Topic_ID"].ToString() + "/editItem/TOPIC/language/" +
                           currentCulture + "/default.aspx";
                            editLink.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";                            
                            editLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(editLink.NavigateUrl, this, PortalSettings, true, false));                          
                                                  
                            //<input type=\"button\" id=\"btnNewThread\" class=\"forum_button\" value=\"" + ATC.Translate.String("New Thread", "", CurrentLanguageCode) + "\" onclick=\"" + newThreadUrl + "\" />";
                        }

                        Forums.Controls.Add(new LiteralControl(@"
                                                            <tr>
                                                                <td class='topic_threads_topic_header1'>
                                                                    <table width='100%' cellpadding='0' cellspacing='0' border=0>
                                                                        <tr>
                                                                            <td class='topic_threads_topic_header2'>
                                                                                "));
                        Forums.Controls.Add(editLink);                        
                        Forums.Controls.Add(new LiteralControl(@"" +
                        row["Topic_Desc"].ToString() + (Inactive ? " - <span class='topic_threads_inactive_span'>" + 
                        Localization.GetString("Inactive", LocalResourceFile)  + "</span>" : "")));

                        if (showNewThread)
                        {

                            Forums.Controls.Add(new LiteralControl(@"<td class='topic_threads_topic_header2' align=right>"));
                            Forums.Controls.Add(myExtBtn);
                            Forums.Controls.Add(new LiteralControl(@"</td>"));
                        }
                        Forums.Controls.Add(new LiteralControl(@"
                                                                        </tr>                                                                       
                                                                    </table>
                                                                </td>
                                                            </tr><tr><td height='5'></td></tr>
                                                            "));  
                    }
               
                    HyperLink editThread = new HyperLink();
                    if (DisplayThread && row["Thread_User_ID"].ToString() != "")
                    {
                        Inactive = (row["ThreadActive"].ToString() == "0");
                        if (
                            (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == row["Topic_User_ID"].ToString())
                            ||
                            (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == row["Thread_User_ID"].ToString())
                            ||
                            (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                            )
                        {                      
                           
                        editThread.NavigateUrl = "/tabid/" + PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                            "/TopicID/" + row["Topic_ID"].ToString() + "/ThreadID/" + row["Thread_ID"].ToString() +
                            "/language/" + currentCulture + "/default.aspx";
                        editThread.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";
                        editThread.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(editThread.NavigateUrl, this, PortalSettings, true, false));                          
                        }

                        Forums.Controls.Add(new LiteralControl(@"  
                            <tr><td></td></tr>
                            <tr>
                                <td class='topic_threads_thread_outer_td'>
                                    <table width='100%' cellpadding='0' cellspacing='0' class='topic_threads_thread_table'>
                                        <tr>                                           
                                            <td class='topic_threads_thread_desc'><table  cellpadding='0' cellspacing='0'><tr><td class='threadImage'>"));

                        String threadtype = String.Empty;
                        if (row["EuRelated"].ToString() == "True")
                        {
                            threadtype = Localization.GetString("ThreadEu", LocalResourceFile);
                            Forums.Controls.Add(new LiteralControl("<div class='topicType tf_en-GB'>" + threadtype + "</div>"));
                        }
                        /* Only used for the 2 slovenian threads under the slovenian topic 81*/
                        else if (row["Topic_ID"].ToString() == "81")
                        {
                            threadtype = Localization.GetString("ThreadNational", LocalResourceFile);
                            Forums.Controls.Add(new LiteralControl("<div class='topicType tf_sl-SL'>" + threadtype + "</div>"));
                        }
                        else
                        {
                            threadtype = Localization.GetString("ThreadNational", LocalResourceFile);
                            Forums.Controls.Add(new LiteralControl("<div class='topicType tf_" + row["Language"] + "'>" + threadtype + "</div>"));
                        }

                        if (isThreadHot(row["Thread_ID"].ToString()))
                        {
                            Forums.Controls.Add(new LiteralControl("<div class='hotness'></div>"));
                        }

                         /* Ask for details of the latest post of this thread */
                         DataRow lastPost = getLatestPostOfThread(row["Thread_ID"].ToString());
                         string lastMessage = String.Empty;
                         string lastMessageUsername = String.Empty;
                         string lastMessageDate = String.Empty;
                         int lastMessageUserID = 1;

                         if (lastPost != null)
                         {
                             lastMessage = TruncateAtWord(Server.HtmlDecode(lastPost["Subject"].ToString()), 60);
                             lastMessageUsername = lastPost["Username"].ToString();
                             lastMessageDate = lastPost["PostDate"].ToString();
                             lastMessageUserID = Convert.ToInt32(lastPost["UserID"]);
                         }
                                                                                     
                         Forums.Controls.Add(editThread);                                               
                         Forums.Controls.Add(new LiteralControl(@"
                                     </td><td><div class='topic_threads_title'>
                                        <a class='forum_link' href='" +  ConfigurationManager.AppSettings["DomainName"] +
                                        "/tabid/" + PortalSettings.ActiveTab.TabID +
                                         "/ctl/TreeView/mid/" + ModuleId +
                                         "/Thread/" + row["Thread_ID"].ToString() + "/language/" +
                                         currentCulture + "/uDebate.aspx" + "'>"
                                         + row["Thread_Desc"].ToString() + @"
                                         </a>
                                     </div>
                                     " + (Inactive ? " - <span style='color:red;font-weight:bold'>" + 
                                       Localization.GetString("Inactive", LocalResourceFile)  + "</span>" : "") + @"
                                  </td></tr></table> 
                                </td>                                            
                             </tr>
                             <tr><td><table  width='100%' cellspacing='0' cellpadding='0' ><tr> " +                                          
                                @"<td class='topic_threads_thread_latestpost'>")); 
                           
                         /* Print the last message in this thread, if any */ 
                          if(lastMessage != "")
                          {
                              Forums.Controls.Add(new LiteralControl(""
                                     + lastMessageDate + " " +
                                     Localization.GetString("PostBy", LocalResourceFile) + " <a href='"
                                     + DotNetNuke.Common.Globals.UserProfileURL(lastMessageUserID) + "'>" + lastMessageUsername + "</a><br />" +
                                     "<span class='lastMessage'>" + lastMessage + "</span>" +
                                     @"</td>"));  
                          }      
                          Forums.Controls.Add(new LiteralControl(@"</td><td class='topic_threads_thread_summary'>
                                     " + row["Thread_User"].ToString() + @"
                                     <td class='topic_threads_thread_details'>
                                     " + Localization.GetString("OpenedDate", LocalResourceFile) + ":&nbsp;" 
                                       + DateToDayMonthYear(row["Opened_Date"].ToString()) + @"<br />
                                     " + (row["Thread_Status"].ToString() == "2" && row["Closed_Date"].ToString() != string.Empty ? ATC.Translate.String("Closed date:", "", "EN")
                                       + DateToDayMonthYear(row["Closed_Date"].ToString()) + "<br />" : "") + @"
                                     </td>
                                    <td class='topic_threads_thread_details'>
                                     " + Localization.GetString("Posts", LocalResourceFile) + ":&nbsp;"
                                       + ATC.Database.sqlGetFirst("SELECT Count(ID) FROM uDebate_Forum_Posts WHERE Active = 1 AND IsPublished = 1 AND ThreadID = "
                                       + row["Thread_ID"].ToString()) + @"<br />
                                     " + Localization.GetString("Views", LocalResourceFile) + ":&nbsp;" 
                                       + row["Thread_ViewCount"].ToString() + @"
                                    </td>
                                 </tr></table></td>
                                  </tr>
                                 </table>
                             </td>
                         </tr>
                         <tr>
                             <td>
                                 &nbsp;
                             </td>
                         </tr>"));
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
                    //editLink = "";
                    i++;
                }

                Forums.Controls.Add(new LiteralControl("</table>"));
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
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

        private string CreateSQLStatement(bool DisplayTopic, bool DisplayThread)
        {
            string SQL = "";
            try
            {
                string SelectStatement = "";
                string WhereStatement = "";
                string OrderByStatement = "";

                string Status = "1";// ReadData("TOPIC_THREAD_VIEW_STATUS");
                if (Status == String.Empty) Status = "3";

                //Select statement
                SelectStatement = @"SELECT ";
                SelectStatement += " T.ID AS Topic_ID, T.Description AS Topic_Desc, T.Summary AS Topic_Sum, T.Text AS Topic_Text, User_Topic.Username AS Topic_User, T.UserID AS Topic_User_ID, T.CREATED AS Topic_Date, T.Active as TopicActive, T.Status AS Topic_Status,";

                if (DisplayThread)
                    SelectStatement += " Th.ID AS Thread_ID, Th.Description AS Thread_Desc, Th.EuRelated,Th.Language, Th.Summary AS Thread_Sum, Th.Text AS Thread_Text, Th.CREATED AS Opened_Date, User_Thread.Username AS Thread_User, Th.UserID AS Thread_User_ID, Th.Active as ThreadActive, User_Thread.LastName AS Surname, User_Thread.FirstName AS Name, Th.Closed_Date AS Closed_Date, Th.Status AS Thread_Status, Th.View_Count AS Thread_ViewCount,";
                else
                    SelectStatement += " NULL AS Thread_ID, NULL AS Thread_Desc, NULL AS Thread_Sum, NULL AS Thread_Text, NULL AS Opened_Date, NULL AS Thread_User, NULL as ThreadActive, NULL AS Name, NULL AS Surname, NULL AS Closed_Date, NULL AS Thread_Status,NULL AS Thread_ViewCount,";
                SelectStatement = SelectStatement.TrimEnd(',');

                //From statement
                SelectStatement += " FROM ";
                SelectStatement += " uDebate_Forum_Topics AS T ";
                SelectStatement += " INNER JOIN Users AS User_Topic ON T.UserID = User_Topic.UserID ";
                if (DisplayThread) SelectStatement += " LEFT OUTER JOIN uDebate_Forum_Threads AS Th ON Th.TopicID = T.ID ";
                if (DisplayThread) 
                    SelectStatement += " LEFT OUTER JOIN Users AS User_Thread ON Th.UserID = User_Thread.UserID ";

                string TopicID = "";// ReadData("TOPIC_THREAD_VIEW_ID");
                //if (TopicID == "-1") TopicID = ATC.Tools.IntURLParam("Topic"); //Current Forum

                TopicID = ATC.Tools.IntURLParam("Topic");
                if (TopicID != String.Empty)
                    WhereStatement += " AND T.ID = " + TopicID;

                if (
                    DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders")
                    ||
                    (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == ATC.Database.sqlGetFirst("SELECT UserID FROM uDebate_Forum_Topics WHERE ID = " + TopicID))
                    )
                {
                    if (Status != "3")
                    {
                        if (DisplayTopic && DisplayThread) WhereStatement += " AND Th.Status = " + Status;
                        else if (DisplayThread) WhereStatement += " AND Th.Status = " + Status;
                        else if (DisplayTopic) WhereStatement += " AND T.Status = " + Status;
                    }
                }
                else
                {
                    if (DisplayTopic) WhereStatement += " AND T.Active = 1";                 
                    if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                        WhereStatement += " AND (Th.Active = 1 OR Th.Active = 0)";
                    else WhereStatement += " AND Th.Active = 1";

                    if (Status != "3")
                    {
                        if (DisplayThread) SelectStatement += " AND Th.Status = " + Status;
                        else if (DisplayTopic) WhereStatement += " AND T.Status = " + Status;
                    }
                }
                if (WhereStatement != String.Empty) WhereStatement = " WHERE " + WhereStatement.Substring(4);

                //An o user echei administrative permissions
                //bool ThreadViewPermission = false;
                //if (
                //    (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == getUserIdByThread())
                //    ||
                //    (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == getUserIdByTopic())
                //    ||
                //    (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                //   )
                //{
                //    ThreadViewPermission = true;
                //}

                //if (!ThreadViewPermission) WhereStatement += " AND Th.Active=1";

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
        public string getUserIdByThread()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Threads] where [ID]=" + ATC.Tools.URLParam("Thread"));
            return sUserId;
        }
        public string getUserIdByTopic()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Topics] where [ID]=" + ATC.Tools.URLParam("Thread"));
            return sUserId;
        }

        public DataRow getLatestPostOfThread(string threadID)
        {
            DataRow result = null;
            
            string sSQL = @"SELECT TOP (1) posts.Subject, posts.PostDate, posts.UserID,Users.Username
                            FROM uDebate_Forum_Posts AS posts INNER JOIN
                            Users ON posts.UserID = Users.UserID
                            WHERE     (posts.ThreadID =" + threadID +
                            ") ORDER BY posts.PostDate DESC ";
            try
            {
                result = ATC.Database.sqlExecuteDataRow(sSQL);

                string message = result["Subject"].ToString();
            }
            catch (Exception x)
            {
            }
            return result;
        }

        /* Threads characterised as hot are trheads having at least 
         * 1 new post in past week or at least 5 new posts in past month
         */
        public bool isThreadHot(string threadID)
        {
            DataRow result = null;
            
            string sSQL = @"SELECT TOP (5) posts.PostDate
                            FROM uDebate_Forum_Posts AS posts 
                            WHERE     (posts.ThreadID =" + threadID +
                            " AND PostDate>DATEADD(MONTH, -1, GETDATE())) ORDER BY posts.PostDate DESC ";
            try
            {
                DataSet ds = ATC.Database.sqlExecuteDataSet(sSQL);
                if (ds.Tables[0].Rows.Count >= 5)
                    return true;
                else if (ds.Tables[0].Rows.Count > 0)
                {
                    DateTime postDate = DateTime.Parse(ds.Tables[0].Rows[0]["PostDate"].ToString());
                    return (postDate - DateTime.Now).Days < 5;
                }
                else
                {
                    return false;
                }               
            }
            catch (Exception x)
            {
                return false;
            }            
        }
        

        public static string TruncateAtWord(string input, int length)
        {
            string result = String.Empty;
            if (input == null || input.Length < length)
                result = input;
            else
            {
                int iNextSpace = input.LastIndexOf(" ", length);
                result = string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
            }
            return result.Replace("<p>", "").Replace("</p>", "");
        }

        protected void ThreadsListView_ItemDataBound(object sender, RadListViewItemEventArgs e)
        {
            if (e.Item is RadListViewDataItem)
            {
                RadListViewDataItem item = e.Item as RadListViewDataItem;
                DataRowView rowView = item.DataItem as DataRowView;
                String Thread_ID = rowView["Thread_ID"].ToString();                

                /* Check if the thread is inactive and display the relevant label*/
                if (rowView["ThreadActive"].ToString().Equals("0"))
                {
                    Label inactive = item.FindControl("InactiveThreadLbl") as Label;
                    inactive.Visible = true;
                }
                HtmlGenericControl topicLang = item.FindControl("topicLang") as HtmlGenericControl;

                topicLang.Attributes.Add("class", "Topicflag tf_" + rowView["Language"]);
              
                /* Show edit Topic Link  if the user has edit permissions */
                if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                {
                    HyperLink editLink = item.FindControl("EditLink") as HyperLink;
                    editLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                        PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                                "/TopicID/" + ATC.Tools.URLParam("Topic") + "/ThreadID/" + Thread_ID +
                            "/language/" + culture + "/default.aspx";
                    editLink.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";
                    editLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(editLink.NavigateUrl, this, PortalSettings, true, false));
                    editLink.Visible = true;

                    HyperLink newThreadLink = ThreadsListView.FindControl("newThreadLink") as HyperLink;
                    if (!newThreadLink.Visible)
                    {                       
                        newThreadLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                            PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                       "/Topic/" + ATC.Tools.URLParam("Topic") + "/ThreadID/-1" +
                            "/language/" + culture + "/default.aspx";

                        newThreadLink.CssClass = "forum_button_thread";
                        newThreadLink.Text = " ";
                        newThreadLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(newThreadLink.NavigateUrl, this, PortalSettings, true, false));
                        newThreadLink.Visible = true;
                    }
                }

                /* Ask for details of the latest post of this Topic */
                DataRow lastPost = getLatestPostOfThread(Thread_ID);
                string lastMessage = String.Empty;
                string lastMessageUsername = String.Empty;
                string lastMessageDate = String.Empty;
                string lastMessageThreadTitle = String.Empty;
                string lastMessageThreadId = String.Empty;
                int lastMessageUserID = 1;

                if (lastPost != null)
                {
                    lastMessage = TruncateAtWord(Server.HtmlDecode(lastPost["Subject"].ToString()), 90);
                    if (lastMessage != "")
                    {
                        lastMessageUsername = lastPost["Username"].ToString();
                        lastMessageDate = lastPost["PostDate"].ToString();
                        lastMessageUserID = Convert.ToInt32(lastPost["UserID"]);                     
                       

                        Label LastMessage = item.FindControl("LastMessage") as Label;
                        Label LastMsgDate = item.FindControl("LastMsgDate") as Label;

                        HyperLink userProfile = item.FindControl("userProfile") as HyperLink;
                        HyperLink ThreadofPost = item.FindControl("ThreadofPost") as HyperLink;

                        LastMessage.Text = "\"" + lastMessage + "\"";
                        LastMsgDate.Text = lastMessageDate;
                        userProfile.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(lastMessageUserID);
                        userProfile.Text = lastMessageUsername;                       
                    }
                }
            }
        }

        protected void Sorting_Click(object sender, EventArgs e)
        {
            RadListViewSortExpression expression;
            LinkButton orderby = sender as LinkButton;
            ThreadsListView.SortExpressions.Clear();

            expression = new RadListViewSortExpression();
            expression.FieldName = orderby.CommandName;
            expression.SortOrder = orderby.CommandArgument == "asc" ? RadListViewSortOrder.Ascending : RadListViewSortOrder.Descending;
            orderby.CommandArgument = orderby.CommandArgument == "asc" ? "desc" : "asc";
            ThreadsListView.SortExpressions.AddSortExpression(expression);
            orderby.CssClass = "orderby";
            ThreadsListView.Rebind();
        }

    }
}