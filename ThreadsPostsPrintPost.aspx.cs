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

namespace DotNetNuke.Modules.uDebate
{
    public partial class ThreadsPostsPrintPost : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string postID = ATC.Tools.URLParam("postID");

            if (postID.Length>0)
            {
                string sSQL = string.Empty;
                sSQL = @"SELECT [ID],[Subject],[Message],[PostDate]
                        FROM [uDebate_Forum_Posts]
                        where ID=" + postID + " and IsPublished=1 and Active=1";
                System.Data.DataSet dsPost = ATC.Database.sqlExecuteDataSet(sSQL);

                if (dsPost.Tables[0].Rows.Count > 0)
                {
                    DataRow DR = dsPost.Tables[0].Rows[0];
                    lbMessagePost.Text = DR["Subject"].ToString();
                    lbBody.Text = DR["Message"].ToString();
                }

                dsPost.Dispose();
                dsPost = null;
            }
            else
            {
                lbMessagePost.Text = "You have not selected a Post. Please close this page and try again.";
            }
        }
    }
}
