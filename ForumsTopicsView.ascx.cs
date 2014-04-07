using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using System.Configuration;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;


namespace DotNetNuke.Modules.uDebate
{
    public partial class ForumsTopicsView : uDebateModuleBase
    {
        public string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["debateCountries"] != null)
                {
                    debateCountries.Value = Session["debateCountries"].ToString();
                    SetCheckedFlags();
                }
                else
                {
                    /* Show topics of current Locale and check the relevant checkbox */
                    SqluDebateTopics.SelectParameters[culture.Replace("-", "_")].DefaultValue = culture;
                    SqluDebateTopics.SelectParameters[culture.Replace("-", "_")].DefaultValue = culture;
                    CheckBox selectedLangCheck = TopicsListView.FindControl("show_" + culture.Replace("-", "_")) as CheckBox;
                    selectedLangCheck.Checked = true;
                    selectedLangCheck.CssClass = "checkLang flag_" + culture;
                    debateCountries.Value = culture + ";";
                }
            }
            LocalResourceFile = Localization.GetResourceFile(this, "ForumsTopicsView.ascx." + culture + ".resx");
            if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
            {
                SqluDebateTopics.SelectParameters["TopicStatus"].DefaultValue = "1";
            }
            /* Populate the statistisc Labels */
            TotalUsers.Text = getRegisteredUsers();
            TotalPosts.Text = getGlobalPostsCount();
            TotalThreads.Text = getGlobalThreadsCount();
            TotalTopics.Text = getGlobalTopicsCount();
            TotalViews.Text = getGlobalThreadViewsCount();

            /* Initialize the styles of the ordeby buttons*/

            LinkButton orderPopular = TopicsListView.FindControl("orderPopular") as LinkButton;
            LinkButton orderDate = TopicsListView.FindControl("orderDate") as LinkButton;
            LinkButton orderLanguage = TopicsListView.FindControl("orderLanguage") as LinkButton;
            orderPopular.CssClass = "";
            orderDate.CssClass = "";
            orderLanguage.CssClass = "";

            if (debateCountries.Value.IndexOf(";", debateCountries.Value.IndexOf(";")) < 1)
                orderLanguage.Visible = false;
        }

        public void langCheck_Changed(object sender, EventArgs e)
        {
            CheckBox checkedLang = (CheckBox)sender;
            LinkButton orderLanguage = TopicsListView.FindControl("orderLanguage") as LinkButton;
            string language = String.Empty;
            bool rebind = false;
            switch (checkedLang.ID)
            {
                case "show_el_GR": language = "el-GR"; break;
                case "show_es_ES": language = "es-ES"; break;
                case "show_it_IT": language = "it-IT"; break;
                case "show_hu_HU": language = "hu-HU"; break;
                case "show_sl_SL": language = "sl-SL"; break;
                default: language = "en-GB"; break;
            }

            if (checkedLang.Checked)
            {
                // We add the ';' to make it easier to delete the country in the following else
                //PopulateTable(debateCountries.Value + language + ";");
                debateCountries.Value = debateCountries.Value + language + ";";
                checkedLang.CssClass = "checkLang flag_" + language;
                rebind = true;
            }
            else //Hide debates of the clicked country
            {
                string visiblecountries = debateCountries.Value;
                debateCountries.Value = debateCountries.Value.Replace(language + ";", "");

                if (debateCountries.Value != "")
                {
                    rebind = true;
                    checkedLang.CssClass = "uncheckLang flag_" + language;
                }
                else // Only one country is visible so clicking the button doesn't do anything
                {
                    debateCountries.Value = visiblecountries;
                    checkedLang.Checked = true;
                    checkedLang.CssClass = "checkLang flag_" + language;
                    orderLanguage.Visible = false;
                }
            }

            if (rebind)
            {
                SetCheckedFlags();
                Session["debateCountries"] = debateCountries.Value;
            }
        }

        protected void SetCheckedFlags()
        {
            LinkButton orderLanguage = TopicsListView.FindControl("orderLanguage") as LinkButton;
            string[] stringSeparators = new string[] { ";" };
            string[] countries = debateCountries.Value.Split(stringSeparators, System.StringSplitOptions.None);
            string[] countriesInWhere = new string[countries.Length - 1];

            /* Initialise the select parameteres to a space string (empty string causes problems)  */
            foreach (Parameter lang in SqluDebateTopics.SelectParameters)
            {
                if (lang.Name != "TopicStatus")
                    lang.DefaultValue = " ";
            }
            /* There is always a ';' at the end so we skip the last element when passing to the datasource */
            for (int i = 0; i < countries.Length - 1; i++)
            {
                SqluDebateTopics.SelectParameters[countries[i].Replace("-", "_")].DefaultValue = countries[i];
                CheckBox checkedLang = TopicsListView.FindControl("show_" + countries[i].Replace("-", "_")) as CheckBox;
                checkedLang.CssClass = "checkLang flag_" + countries[i];
                checkedLang.Checked = true;
            }

            /* Show order by language only if we have more than one languages selected */
            if (countries.Length > 2)
                orderLanguage.Visible = true;
            else
                orderLanguage.Visible = false;

            SqluDebateTopics.DataBind();
            TopicsListView.Rebind();
        }



        protected void TopicsListView_ItemDataBound(object sender, RadListViewItemEventArgs e)
        {
            if (e.Item is RadListViewDataItem)
            {
                RadListViewDataItem item = e.Item as RadListViewDataItem;
                DataRowView rowView = item.DataItem as DataRowView;
                String TopicId = rowView["TopicID"].ToString();
                String ForumId = rowView["ForumID"].ToString();

                /* Check if the topic is inactive and display the relevant label*/
                if (rowView["TopicActive"].ToString().Equals("0"))
                {
                    Label inactive = item.FindControl("InactiveTopicLbl") as Label;
                    inactive.Visible = true;
                }
                HtmlGenericControl topicLang = item.FindControl("topicLang") as HtmlGenericControl;
                topicLang.Attributes.Add("class", "Topicflag tf_" + rowView["TopicLanguage"]);
              
                /* Show edit Topic Link  if the user has edit permissions */
                if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                {
                    HyperLink editLink = item.FindControl("EditLink") as HyperLink;

                    editLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                        PortalSettings.ActiveTab.TabID + "/ctl/EditForum/mid/" + ModuleId +
                               "/ForumID/" + ForumId + "/TopicID/" + TopicId +
                               "/editItem/TOPIC/language/" + culture + "/default.aspx";

                    editLink.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";
                    editLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(editLink.NavigateUrl, this, PortalSettings, true, false));
                    editLink.Visible = true;

                    HyperLink newTopicLink = TopicsListView.FindControl("newTopicLink") as HyperLink;
                    if (!newTopicLink.Visible)
                    {
                        newTopicLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" + PortalSettings.ActiveTab.TabID + "/ctl/EditForum/mid/" + ModuleId +
                       "/ForumID/" + ForumId + "/new/TOPIC/language/" +
                        culture + "/default.aspx";
                        newTopicLink.CssClass = "forum_button_topic";
                        newTopicLink.Text = " ";
                        newTopicLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(newTopicLink.NavigateUrl, this, PortalSettings, true, false));
                        newTopicLink.Visible = true;
                    }
                }

                /* Ask for details of the latest post of this Topic */
                DataRow lastPost = getLatestPostOfTopic(TopicId);
                string lastMessage = String.Empty;
                string lastMessageUsername = String.Empty;
                string lastMessageDate = String.Empty;
                string lastMessageThreadTitle = String.Empty;
                string lastMessageThreadId = String.Empty;
                int lastMessageUserID = 1;
                bool hideSeparators = false;

                if (lastPost != null)
                {
                    lastMessage = TruncateAtWord(Server.HtmlDecode(lastPost["Subject"].ToString()), 60);
                    if (lastMessage != "")
                    {
                        lastMessageUsername = lastPost["Username"].ToString();
                        lastMessageDate = lastPost["PostDate"].ToString();
                        lastMessageUserID = Convert.ToInt32(lastPost["UserID"]);
                        lastMessageThreadTitle = TruncateAtWord(lastPost["Thread_Title"].ToString(), 60);
                        lastMessageThreadId = lastPost["Thread_ID"].ToString();

                        Label LastMessage = item.FindControl("LastMessage") as Label;
                        Label LastMsgDate = item.FindControl("LastMsgDate") as Label;

                        HyperLink userProfile = item.FindControl("userProfile") as HyperLink;
                        HyperLink ThreadofPost = item.FindControl("ThreadofPost") as HyperLink;

                        LastMessage.Text = "\"" + lastMessage + "\"";
                        LastMsgDate.Text = lastMessageDate;
                        userProfile.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(lastMessageUserID);
                        userProfile.Text = lastMessageUsername;
                        ThreadofPost.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] +
                           "/" + culture + "/udebatediscussion.aspx?Thread=" + lastMessageThreadId;
                        ThreadofPost.Text = lastMessageThreadTitle;                           
                    }
                    else
                            hideSeparators = true;
                }
                else
                {
                    hideSeparators = true;                    
                }
                if (hideSeparators)
                {
                    Label latspostLbl = item.FindControl("latspostLbl") as Label;
                    Label postBy = item.FindControl("postBy") as Label;
                    Label separator1 = item.FindControl("separator1") as Label;
                    Label separator2 = item.FindControl("separator2") as Label;
                    latspostLbl.Visible = false;
                    postBy.Visible = false;
                    separator1.Visible = false;
                    separator2.Visible = false;
                }
            }
        }

        protected void Sorting_Click(object sender, EventArgs e)
        {
            RadListViewSortExpression expression;
            LinkButton orderby = sender as LinkButton;
            TopicsListView.SortExpressions.Clear();

            expression = new RadListViewSortExpression();
            expression.FieldName = orderby.CommandName;
            expression.SortOrder = orderby.CommandArgument == "asc" ? RadListViewSortOrder.Ascending : RadListViewSortOrder.Descending;
            orderby.CommandArgument = orderby.CommandArgument == "asc" ? "desc" : "asc";
            TopicsListView.SortExpressions.AddSortExpression(expression);
            orderby.CssClass = "orderby";
            TopicsListView.Rebind();
        }

        public string getRegisteredUsers()
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT  Count(Users.UserID)
                            FROM   Users INNER JOIN UserProfile ON Users.UserID = UserProfile.UserID
                            WHERE  (UserProfile.PropertyDefinitionID = 42)";
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getGlobalThreadsCount()
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT count(Threads.Id) as [ThreadCount] FROM uDebate_Forum_Threads Threads 
                            JOIN uDebate_Forum_Topics Topics on Threads.TopicID = Topics.ID
                            WHERE Threads.Active = 1 and Topics.Active=1";
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getGlobalTopicsCount()
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT count(Id) FROM uDebate_Forum_Topics 
                            WHERE Active = 1 ";
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getGlobalPostsCount()
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT count(Posts.Id) as [PostsCount] FROM uDebate_Forum_Posts Posts 
                            JOIN uDebate_Forum_Threads Threads on Posts.ThreadID = Threads.ID
                            JOIN uDebate_Forum_Topics Topics on Threads.TopicID = Topics.ID
                            WHERE Posts.IsPublished=1 AND Threads.Active = 1 and Topics.Active=1";
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getGlobalThreadViewsCount()
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT SUM(Threads.View_Count) as [ViewsCount] FROM uDebate_Forum_Threads Threads 
                            JOIN uDebate_Forum_Topics Topics on Threads.TopicID = Topics.ID
                            WHERE Threads.Active = 1 and Topics.Active=1";
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public DataRow getLatestPostOfTopic(string topicID)
        {
            DataRow result = null;

            string sSQL = @"SELECT TOP (1) posts.Subject, posts.PostDate, posts.UserID,
                            Users.Username, threads.Description as 'Thread_Title',threads.ID as 'Thread_ID'
                            FROM uDebate_Forum_Posts AS posts INNER JOIN
                            uDebate_Forum_Threads as threads ON 
                            posts.ThreadId = threads.ID INNER JOIN
                            Users ON posts.UserID = Users.UserID
                            WHERE (threads.TopicID =" + topicID +
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
    }
}