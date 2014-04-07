using System.Data;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using System;
using DotNetNuke.Services.Localization;
using System.Configuration;

namespace DotNetNuke.Modules.uDebate.Controls
{
    public partial class ForumBreadcrumb : PortalModuleBase
    {
        string Topic = ATC.Tools.IntURLParam("Topic");
        string Thread = ATC.Tools.IntURLParam("Thread");

        protected void Page_Load(object sender, System.EventArgs e)
        {
            LocalResourceFile = Localization.GetResourceFile(this, "ForumBreadcrumb.ascx." +
                System.Threading.Thread.CurrentThread.CurrentCulture.Name + ".resx");

            string literal = "" + Localization.GetString("breadStart", LocalResourceFile) +

             ": <a href='" +
             ConfigurationManager.AppSettings["DomainName"] + "/" + System.Threading.Thread.CurrentThread.CurrentCulture.Name +

             "/uDebate.aspx' class='bread_link'>" + Localization.GetString("debateStart", LocalResourceFile) + "</a>";

            if (Topic != string.Empty)
            {
                string TopicDesc = ATC.Database.sqlGetFirst("SELECT Description FROM uDebate_Forum_Topics WHERE ID = " + Topic);

                literal += " >  <strong>" + TopicDesc + "</strong>";

            }
            else if (Thread != string.Empty)
            {
                DataRow threadRow = ATC.Database.sqlExecuteDataRow(@"
                        SELECT
                            Thread.Description AS ThreadDesc,
                            Topic.ID AS TopicID,
                            Topic.Description AS TopicDesc
                        FROM
                            uDebate_Forum_Threads Thread
                        INNER JOIN
                            uDebate_Forum_Topics Topic
                        ON
                            Thread.TopicID = Topic.ID
                        WHERE
                            Thread.ID = " + Thread);

                //                literal += string.Format(@"
                //                &nbsp;/&nbsp;<span class='forum_link'><a href='?page=topic&Topic={0}' class='forum_link'>{1}</a></span>", threadRow["TopicID"].ToString(), threadRow["TopicDesc"].ToString());

                string TopicDesc = threadRow["TopicDesc"].ToString();
                string ThreadDesc = threadRow["ThreadDesc"].ToString();

                literal += " > <a href='" + ConfigurationManager.AppSettings["DomainName"] + "/" +
                           System.Threading.Thread.CurrentThread.CurrentCulture.Name + 
                           "/udebatethreads.aspx?TopicID=" + threadRow["TopicID"].ToString()+                           
                           "' class='bread_link'>" + TruncateAtWord(TopicDesc, 50) + "</a>";


                literal += " > <strong>" + TruncateAtWord(ThreadDesc, 80) + "</strong>";

                //                    string.Format(@"
                //                &nbsp;/&nbsp;<span class='forum_link'><a href='?page=thread&Thread={0}' class='forum_link'>{1}</a></span>", Thread, threadRow["ThreadDesc"].ToString());
            }

            Breadcrumb.Controls.Add(new LiteralControl(literal));
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


        /// <summary>
        /// PostId
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string CurrentModuleId
        {
            get
            {
                if ((ViewState["CurrentModuleId"] != null))
                {
                    return ViewState["CurrentModuleId"].ToString();
                }
                else
                {
                    return "0";
                }
            }
            set { ViewState["CurrentModuleId"] = value; }
        }
    }
}