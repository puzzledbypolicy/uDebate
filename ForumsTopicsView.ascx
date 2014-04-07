<%@ Control Language="C#" Inherits="DotNetNuke.Modules.uDebate.ForumsTopicsView"
    AutoEventWireup="True" CodeBehind="ForumsTopicsView.ascx.cs" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls"
    TagPrefix="telerik" %>
<asp:HiddenField ID="debateCountries" runat="server" />
<div id="fb-root"></div>
<script>    (function(d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=274597442564126";
        fjs.parentNode.insertBefore(js, fjs);
    } (document, 'script', 'facebook-jssdk'));</script>
<telerik:DnnListView ID="TopicsListView" runat="server" DataSourceID="SqluDebateTopics"
    ItemPlaceholderID="PlaceHolder1" AllowPaging="true" OnItemDataBound="TopicsListView_ItemDataBound">
    <LayoutTemplate>
        <div id="forumContainer">
            <div id="languageChecks">
                <asp:CheckBox ID="show_el_GR" runat="server" AutoPostBack="true" OnCheckedChanged="langCheck_Changed"
                    CssClass="uncheckLang flag_el-GR" resourcekey="GrLabel" />
                <asp:CheckBox ID="show_es_ES" runat="server" AutoPostBack="true" OnCheckedChanged="langCheck_Changed"
                    CssClass="uncheckLang flag_es-ES" resourcekey="EsLabel" />
                <asp:CheckBox ID="show_hu_HU" runat="server" AutoPostBack="true" OnCheckedChanged="langCheck_Changed"
                    CssClass="uncheckLang flag_hu-HU" resourcekey="HuLabel" />
                <asp:CheckBox ID="show_it_IT" runat="server" AutoPostBack="true" OnCheckedChanged="langCheck_Changed"
                    CssClass="uncheckLang flag_it-IT" resourcekey="ItLabel" />
                <asp:CheckBox ID="show_sl_SL" runat="server" AutoPostBack="true" OnCheckedChanged="langCheck_Changed"
                    CssClass="uncheckLang flag_sl-SL" resourcekey="SlLabel" />
                <asp:CheckBox ID="show_en_GB" runat="server" AutoPostBack="true" OnCheckedChanged="langCheck_Changed"
                    CssClass="uncheckLang flag_en-GB" resourcekey="EuLabel" />
            </div>
            <asp:Label ID="SelectCountry" runat="server" CssClass="InitLabel" resourcekey="InitiatedCountry"></asp:Label>
            <div style="position:absolute">
            <fb:like send="false" width="80" show_faces="false" layout="button_count"></fb:like></div>
            <table width="100%" cellspacing="0" cellpadding="0">
                <tr>
                    <td width="3%" class="forum_topics_header" style="border-left: none;">
                        <asp:Label ID="langLbl" runat="server" resourcekey="Lang"></asp:Label>
                    </td>
                    <td width="60%" class="forum_topics_header" style="text-align:left">
                        <asp:Label ID="topicLbl" runat="server" resourcekey="TopicPost"></asp:Label>
                        <asp:HyperLink ID="newTopicLink" runat="server" Visible="false" />
                    </td>
                    <td width="12%" class="forum_topics_header">
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
                        <asp:Label ID="orderLbl" runat="server" resourcekey="Orderby"></asp:Label>
                        <asp:LinkButton ID="orderPopular" runat="server" OnClick="Sorting_Click" CommandArgument="desc"
                            CommandName="PostsCount" ForeColor="#003399" resourcekey="popularity" />
                        |
                        <asp:LinkButton ID="orderDate" runat="server" OnClick="Sorting_Click" CommandArgument="desc"
                            CommandName="Topic_CreatedDate" ForeColor="#003399" resourcekey="date" />
                        |
                        <asp:LinkButton ID="orderLanguage" runat="server" OnClick="Sorting_Click" CommandArgument="desc"
                            CommandName="TopicLanguage" Visible="false" ForeColor="#003399" resourcekey="orderLang" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            </table>
            <telerik:DnnDataPager ID="RadDataPager1" runat="server" PageSize="8" Skin="Transparent">
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
            </td>
            <td colspan="5" style="padding: 5px;">
                <a href="<%#ConfigurationManager.AppSettings["DomainName"] %>/<%=culture %>/udebatethreads.aspx?TopicID=<%#Eval("TopicID") %>"
                    class="topic_Descr_link">
                    <asp:Label ID="TopicDescriptionLabel" runat="server" Text='<%#Eval("TopicDescription") %>'></asp:Label>
                </a>
                <asp:Label ID="InactiveTopicLbl" runat="server" CssClass="forum_topics_inactive_span"
                    Text='Inactive' resourcekey="Inactive" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HyperLink ID="EditLink" runat="server" ToolTip="Edit this topic" Visible="false" />
            </td>
            <td class="topic_Sumary">
                <asp:Label ID="TopicSummaryLabel" runat="server" Text='<%#Eval("TopicSummary") %>'></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="CreatedLabel" runat="server" Text='<%#Eval("Topic_CreatedDate","{0:dd-MM-yyyy}") %>'></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="ThreadCountLabel" runat="server" Text='<%#Eval("ThreadsCount") %>'></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="PostsCountLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("PostsCount").ToString()) ? "0" : Eval("PostsCount")) %>'></asp:Label>
            </td>
            <td class="forum_topics_cell">
                <asp:Label ID="Views_CountLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("Views_Count").ToString()) ? "0" : Eval("Views_Count")) %>'></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="topic_latestpost">
            </td>
            <td class="topic_latestpost" colspan="5">
                <asp:Label ID="latspostLbl" runat="server" resourcekey="LatestPost"></asp:Label>
                <asp:Label ID="LastMessage" runat="server" Font-Italic="true"></asp:Label>
                <asp:Label ID="separator1" runat="server" Text="| "></asp:Label>                
                <asp:Label ID="LastMsgDate" runat="server"></asp:Label>
                <asp:Label ID="postBy" runat="server" resourcekey="PostBy"></asp:Label>
                <asp:HyperLink ID="userProfile" runat="server" ForeColor="#003399" />
                <asp:Label ID="separator2" runat="server" resourcekey="sepAt" Text="| At: "></asp:Label>
                <asp:HyperLink ID="ThreadofPost" ForeColor="#003399" runat="server" />
            </td>
        </tr>
    </ItemTemplate>
</telerik:DnnListView>
<div id="StatisticsSection">
    <asp:Label ID="StatsLabel" runat="server" Font-Bold="true" resourcekey="Statistics"></asp:Label>:
    <asp:Label ID="USerLbl" runat="server" resourcekey="TotalUsers"></asp:Label>:
    <asp:Label ID="TotalUsers" runat="server" Font-Bold="true"></asp:Label>
    <asp:Label ID="TopicsLbl" runat="server" resourcekey="TotalTopics"></asp:Label>:
    <asp:Label ID="TotalTopics" runat="server" Font-Bold="true"></asp:Label>
    <asp:Label ID="ThreadsLbl" runat="server" resourcekey="TotalThreads"></asp:Label>:
    <asp:Label ID="TotalThreads" runat="server" Font-Bold="true"></asp:Label>
    <asp:Label ID="PostsLbl" runat="server" resourcekey="TotalPosts"></asp:Label>:
    <asp:Label ID="TotalPosts" runat="server" Font-Bold="true"></asp:Label>
    <asp:Label ID="ViewsLbl" runat="server" resourcekey="TotalViews"></asp:Label>:
    <asp:Label ID="TotalViews" runat="server" Font-Bold="true"></asp:Label>
</div>
<asp:SqlDataSource ID="SqluDebateTopics" runat="server" ConnectionString="<%$ ConnectionStrings:SiteSqlServer %>"
    SelectCommand=" SELECT SUM(TopicThreads.Posts)as PostsCount,COUNT(TopicThreads.ID) as ThreadsCount,
                    SUM(TopicThreads.View_Count)as 'Views_Count',
                    T.ID as TopicID, T.Language as TopicLanguage,
                    T.Description as TopicDescription, T.Summary as TopicSummary,
                    T.UserID as TopicUser, T.CREATED as Topic_CreatedDate, T.Active as TopicActive,
                    T.Closed_Date As Closed_Date,T.Status AS Topic_Status, T.Position as Position,
                    F.ID as ForumId, F.Active as ForumActive,U.LastName AS Surname, U.FirstName AS Name
                    FROM
                    ( SELECT ThreadPosts.Posts as 'Posts',Threads.ID,Threads.TopicID, Threads.View_Count from 
                    (SELECT COUNT(Posts.ID) AS 'Posts',Posts.ThreadID,Threads.View_Count
                        FROM   uDebate_Forum_Posts Posts
                        JOIN uDebate_Forum_Threads Threads on Posts.ThreadID = Threads.ID
                        GROUP BY Posts.ThreadID,Threads.View_Count) as ThreadPosts    
                    RIGHT JOIN uDebate_Forum_Threads Threads on ThreadPosts.ThreadID = Threads.ID
                    where Threads.Active=1
                    ) as TopicThreads
                    RIGHT JOIN uDebate_Forum_Topics T on T.ID = TopicThreads.TopicID
                    RIGHT JOIN uDebate_Forums F ON T.ForumID = F.Id 
                    LEFT OUTER JOIN Users U ON T.UserID = U.UserID    
                    WHERE T.Language in (@en_GB,@el_GR,@es_ES,@it_IT,@hu_HU,@sl_SL)  AND F.Active = 1 
                    		AND ((T.Active = 1 AND T.Status=1) OR 1=@TopicStatus) 
                    GROUP BY T.ID,T.Language,T.Description,T.Summary,
                    		T.UserID,T.CREATED,T.Active,T.Closed_Date,T.Status,T.Position,
                    		F.ID,F.Description,F.Summary,F.Opened_Date,F.Active,
                    		U.LastName,U.FirstName  
                    ORDER BY F.ID,Position, PostsCount DESC">
    <SelectParameters>
        <asp:Parameter Name="TopicStatus" Type="Int32" DefaultValue="0" />
        <asp:Parameter Name="en_GB" Type="String" DefaultValue=" " />
        <asp:Parameter Name="el_GR" Type="String" DefaultValue=" " />
        <asp:Parameter Name="es_ES" Type="String" DefaultValue=" " />
        <asp:Parameter Name="it_IT" Type="String" DefaultValue=" " />
        <asp:Parameter Name="hu_HU" Type="String" DefaultValue=" " />
        <asp:Parameter Name="sl_SL" Type="String" DefaultValue=" " />
    </SelectParameters>
</asp:SqlDataSource>
<% if (Request.IsAuthenticated)
   {%>

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

<noscript>
    Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments
        powered by Disqus.</a></noscript>
<% }%>