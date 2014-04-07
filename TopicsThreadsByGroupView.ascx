<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="TopicsThreadsByGroupView.ascx.cs"
    Inherits="DotNetNuke.Modules.uDebate.TopicsThreadsByGroupView" %>
<%@ Register TagPrefix="uDebate" TagName="Breadcrump" Src="~/DesktopModules/uDebate/controls/ForumBreadcrumb.ascx" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls"
    TagPrefix="telerik" %>

<table border="0" cellpadding="0" cellspacing="0" width="940px">
    <tr>
        <td class='topImage tImg_<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name %>'>
        <div id='MicrosoftTranslatorWidget' style='width: 197px; border:#009899; min-height: 40px; position:relative;bottom:14px; float:right; margin-right:3px;background-color: #009899;'>
<noscript><a href='http://www.microsofttranslator.com/bv.aspx?a=http%3a%2f%2fjoin.puzzledbypolicy.eu%2f'>Translate the discussion tree</a><br />
Powered by <a href='http://www.microsofttranslator.com'>Microsoft&reg; Translator</a></noscript></div> 
               <script type='text/javascript'>                   /* <![CDATA[ */setTimeout(function() {
                       var s = document.createElement('script');
                       s.type = 'text/javascript';
                       s.charset = 'UTF-8';
                       s.src = ((location && location.href && location.href.indexOf('https') == 0) ? 'https://ssl.microsofttranslator.com' : 'http://www.microsofttranslator.com')
            + '/ajax/v2/widget.aspx?mode=manual&from=<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name %>&layout=ts';
                       var p = document.getElementsByTagName('head')[0] || document.documentElement; p.insertBefore(s, p.firstChild);
                   }, 0); /* ]]> */</script>
        </td>
    </tr>
    <tr><td>
    <udebate:breadcrump id="ctlBreadcrump" runat="server" />
    </td>
    </tr>
    <tr>
        <td>            
            <asp:PlaceHolder ID="Forums" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
</table>

<% if ( Request.IsAuthenticated ){%>
<script type="text/javascript">

    jQuery(document).ready(function() {
    		jQuery(".proposeSlider").show();
            jQuery(".proposeSlider").click(function() {
            jQuery(".ProposeTopic").toggle("slow");
            jQuery(this).toggleClass("active");
            return false;
        });  
    });    
</script>

<script type="text/javascript">

    /* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */
   
    var locale = "<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name %>";
    if (locale == "es-ES") {
        var disqus_shortname = 'puzzledbypolicyes';
        var disqus_identifier = "spanish";
    }
    else if (locale == "el-GR") {
        var disqus_shortname = 'puzzledbypolicyel';
        var disqus_identifier = "greek";
    }
    else if (locale == "it-IT") {
        var disqus_shortname = 'joinpuzzledbypolicyitalian';
        var disqus_identifier = "italian";
    }
    else if (locale == "hu-HU") {
        var disqus_shortname = 'puzzledbypolicyhu';
        var disqus_identifier = "hungarian";
    }
    else {
        var disqus_shortname = 'puzzledbypolicy';
        var disqus_identifier = "english";
    }
   
    var disqus_developer = 1;
       
    /* * * DON'T EDIT BELOW THIS LINE * * */
    (function() {
        var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
        dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    })();   
    
</script>
<noscript>Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>

<% }%>


<telerik:DnnListView ID="ThreadsListView" runat="server" DataSourceID="SqluDebateThreads"
    ItemPlaceholderID="PlaceHolder1" AllowPaging="true" OnItemDataBound="ThreadsListView_ItemDataBound">
    <LayoutTemplate>
        <div id="forumContainer">           
            <table width="100%" cellspacing="0" cellpadding="0">
                <tr>
                    <td width="3%" class="forum_topics_header" style="border-left: none;">
                        <asp:Label ID="Label4" runat="server" Text="Lang."></asp:Label>
                    </td>
                    <td width="62%" class="forum_topics_header" style="text-align:left">
                        <asp:Label ID="topicLbl" runat="server" Text="Thread & Latest post"></asp:Label>
                        <asp:HyperLink ID="newThreadLink" runat="server" Visible="false" />
                    </td>
                    <td width="10%" class="forum_topics_header">
                        <asp:Label ID="openedLbl" runat="server" resourcekey="OpenedDate"></asp:Label>
                    </td>
                    <td width="5%" class="forum_topics_header">
                        <asp:Label ID="threadsLbl" runat="server" resourcekey="Threads"></asp:Label>
                    </td>
                    <td width="5%" class="forum_topics_header">
                        <asp:Label ID="postsLbl" runat="server" resourcekey="Posts"></asp:Label>
                    </td>
                    <td width="5%" class="forum_topics_header">
                        <asp:Label ID="viewLbl" runat="server" resourcekey="Views"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" class="ordering">
                        Order by
                        <asp:LinkButton ID="orderPopular" runat="server" OnClick="Sorting_Click" CommandArgument="desc"
                            CommandName="PostsCount" ForeColor="#003399" Text="popularity" />
                        |
                        <asp:LinkButton ID="orderDate" runat="server" OnClick="Sorting_Click" CommandArgument="desc"
                            CommandName="Opened_Date" ForeColor="#003399" Text="date" />                       
                    </td>
                </tr>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            </table>
            <telerik:DnnDataPager ID="RadDataPager1" runat="server" PageSize="4" Skin="Transparent">
                <Fields>
                    <telerik:DnnDataPagerSliderField />
                </Fields>
            </telerik:DnnDataPager>
        </div>
    </LayoutTemplate>
    <ItemTemplate>       
        <tr>
             <td>
                <div id="topicLang" runat="server">
                </div>
                <asp:HyperLink ID="EditLink" runat="server" ToolTip="Edit this thread" Visible="false" />
            </td>
            <td class="topic_Sumary">
                <asp:Label ID="Thread_DescLabel" runat="server" Text='<%#Eval("Thread_Desc") %>'></asp:Label>
                 <asp:Label ID="InactiveThreadLbl" runat="server" CssClass="forum_topics_inactive_span"
                    Text='Inactive' resourcekey="Inactive" Visible="false"></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="Opened_DateLabel" runat="server" Text='<%#Eval("Opened_Date","{0:dd/MM/yyyy}") %>'></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="Thread_UserLabel" runat="server" Text='<%#Eval("Thread_User") %>'></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="PostsCountLabel" runat="server" Text='<%#Eval("PostsCount") %>'></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="Views_CountLabel" runat="server" Text='<%#Eval("Views_Count") %>'></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="topic_latestpost">
            </td>
            <td class="topic_latestpost" colspan="5">
                <asp:Label ID="latspostLbl" runat="server" Text="Latest post: "></asp:Label>
                <asp:Label ID="LastMessage" runat="server" Font-Italic="true"></asp:Label>
                |
                <asp:Label ID="LastMsgDate" runat="server"></asp:Label>
                <asp:Label ID="postBy" runat="server" resourcekey="PostBy"></asp:Label>
                <asp:HyperLink ID="userProfile" runat="server" ForeColor="#003399" />              
            </td>
        </tr>
    </ItemTemplate>
</telerik:DnnListView>

<asp:SqlDataSource ID="SqluDebateThreads" runat="server" ConnectionString="<%$ ConnectionStrings:SiteSqlServer %>"
    SelectCommand=" SELECT SUM(ThreadPosts.Posts) AS PostsCount, 
    Threads.ID AS Thread_ID, Threads.Description AS Thread_Desc, Threads.EuRelated,Threads.Language, 
    Threads.CREATED AS Opened_Date, Threads.UserID AS Thread_User_ID, Threads.Active as ThreadActive,
    Threads.Closed_Date AS Closed_Date,Threads.Status AS Thread_Status, Threads.View_Count AS Views_Count,
    User_Thread.LastName AS Surname, User_Thread.FirstName AS Name, User_Thread.Username AS Thread_User     
    FROM
    (SELECT COUNT(Posts.ID) AS 'Posts',Posts.ThreadID,Threads.View_Count
        FROM uDebate_Forum_Posts Posts
        JOIN uDebate_Forum_Threads Threads on Posts.ThreadID = Threads.ID
        GROUP BY Posts.ThreadID,Threads.View_Count) as ThreadPosts
 	RIGHT JOIN uDebate_Forum_Threads Threads on ThreadPosts.ThreadID = Threads.ID   
    INNER JOIN Users AS User_Topic ON Threads.UserID = User_Topic.UserID   
    LEFT OUTER JOIN Users AS User_Thread ON Threads.UserID = User_Thread.UserID   
   
    WHERE  Threads.TopicID = @TopicID AND ((Threads.Active = 1 AND Threads.Status=1) OR 1=@ThreadStatus) 
   
   GROUP BY Threads.ID,Threads.Description,
        Threads.EuRelated,Threads.CREATED,Threads.Language,User_Thread.Username,
        Threads.UserID,Threads.Active,Threads.Status,Threads.View_Count,Threads.Closed_Date,
        User_Thread.LastName,User_Thread.FirstName  
   ORDER BY PostsCount DESC">
    <SelectParameters>
        <asp:Parameter Name="ThreadStatus" Type="Int32" DefaultValue="0" />  
        <asp:Parameter Name="TopicID" Type="Int32" />     
    </SelectParameters>
</asp:SqlDataSource>