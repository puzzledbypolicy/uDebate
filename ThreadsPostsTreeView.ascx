<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ThreadsPostsTreeView.ascx.cs"
    Inherits="DotNetNuke.Modules.uDebate.ThreadsPostsTreeView" %>
<%@ Register Assembly="ATC.WebControls" Namespace="ATC.WebControls" TagPrefix="atc" %>
<%@ Register TagPrefix="uDebate" TagName="Attachment" Src="~/DesktopModules/uDebate/controls/AttachmentControl.ascx" %>
<%@ Register TagPrefix="uDebate" TagName="Breadcrump" Src="~/DesktopModules/uDebate/controls/ForumBreadcrumb.ascx" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>

<script type="text/javascript">
    function popup(url, winname) {
        var width = 640;
        var height = 480;
        var left = (screen.width - width) / 2;
        var top = (screen.height - height) / 2;
        var params = 'width=' + width + ', height=' + height;
        params += ', top=' + top + ', left=' + left;
        params += ', directories=no';
        params += ', location=no';
        params += ', menubar=no';
        params += ', resizable=no';
        params += ', scrollbars=no';
        params += ', status=no';
        params += ', toolbar=no';
        newwin = window.open(url, winname, params);
        if (window.focus) { newwin.focus() }
        return false;
    }

    function DeletePost(path) {
        if (confirm('<%=ATC.Translate.String("Are you sure?","","EN") %>\n<%=ATC.Translate.String("The post will be permanently deleted!","","EN") %>'))
            window.location = path;
    }
    function go(thisval, typetext) {
        var txtPostTypeId = returnObjById("<%=txtPostTypeId.ClientID%>");
        txtPostTypeId.value = thisval;

        var postButtonsSelectedType = returnObjById("<%=postButtonsSelectedType.ClientID%>");

        switch(typetext)
        {
            case "Issue":
                postButtonsSelectedType.innerHTML = '<%= Localization.GetString("Issue.Text", LocalResourceFile) %>';
                break;
             case "Alternative":
                 postButtonsSelectedType.innerHTML = '<%= Localization.GetString("Alternative.Text", LocalResourceFile) %>';
                 break;
             case "Pro Argument":
                 postButtonsSelectedType.innerHTML = '<%= Localization.GetString("ProArgument.Text", LocalResourceFile) %>';
                 break;
             case "Con Argument":
                 postButtonsSelectedType.innerHTML = '<%= Localization.GetString("ConArgument.Text", LocalResourceFile) %>';
                 break;
             case "Comment":
                 postButtonsSelectedType.innerHTML = '<%= Localization.GetString("Comment.Text", LocalResourceFile) %>';
                 break;
            default:
           postButtonsSelectedType.innerHTML = typetext;
        }     

    }
    function goStart(thisval, typetext) {
        var txtPostTypeId = returnObjById("<%=txtPostTypeId.ClientID%>");
        txtPostTypeId.value = thisval;

        var postButtonsSelectedType = returnObjById("<%=postButtonsStartingSelectedType.ClientID%>");

        switch (typetext) {
            case "Issue":
                postButtonsSelectedType.innerHTML = '<%= Localization.GetString("Issue.Text", LocalResourceFile) %>';
                break;
            case "Alternative":
                postButtonsSelectedType.innerHTML = '<%= Localization.GetString("Alternative.Text", LocalResourceFile) %>';
                break;
            case "Pro Argument":
                postButtonsSelectedType.innerHTML = '<%= Localization.GetString("ProArgument.Text", LocalResourceFile) %>';
                break;
            case "Con Argument":
                postButtonsSelectedType.innerHTML = '<%= Localization.GetString("ConArgument.Text", LocalResourceFile) %>';
                break;
            case "Comment":
                postButtonsSelectedType.innerHTML = '<%= Localization.GetString("Comment.Text", LocalResourceFile) %>';
                break;
            default:
                postButtonsSelectedType.innerHTML = typetext;
        }     
    }

    function returnObjById(id) {
        if (document.getElementById)
            var returnVar = document.getElementById(id);
        else if (document.all)
            var returnVar = document.all[id];
        else if (document.layers)
            var returnVar = document.layers[id];
        return returnVar;
    }
    function OpenMyWindow() {
        var sURL = 'http://<%=Request.Url.Host.ToString()%>/<%=ATC.Tools.GetParam("VirtualFolder").ToString()%>FileHandler.aspx';
        //alert(sURL);
        myRef = window.open(sURL, 'mywinfiles', 'left=20,top=20,width=600,height=500,toolbar=1,resizable=0');
    }

    
    function openTreeViewPrinter() {
        var txtPostTypeId = returnObjById("<%=hdThreadId.ClientID%>");
        var threadID = txtPostTypeId.value;

        var sURL = 'http://<%=Request.Url.Host.ToString()%>/DesktopModules/uDebate/ThreadsPostsPrintTreeView.aspx?threadID=' + threadID;
        //alert(sURL);
        myRef = window.open(sURL, 'mywinfiles', 'left=20,top=20,width=600,height=800,toolbar=1,resizable=1,scrollbars=1');
    }

    function openSelectedPostPrinter() {
        var txtPostId = returnObjById("<%=hdParentId.ClientID%>");
        var postId = txtPostId.value;

        var sURL = 'http://<%=Request.Url.Host.ToString()%>/DesktopModules/uDebate/ThreadsPostsPrintPost.aspx?postID=' + postId;
        //alert(sURL);
        myRef = window.open(sURL, 'mywinfiles', 'left=20,top=20,width=600,height=800,toolbar=1,resizable=1,scrollbars=1');
    }

    function LoadEvent() {
        try {
            var elem = document.getElementById("<%=hdParentId.ClientID%>");
            if (elem != null) {
                var node = document.getElementById(elem.value);
                if (node != null) {
                    node.scrollIntoView(true);
                    Panel1.scrollLeft = 0;
                }
            }
        }
        catch (oException)
      { }
  }

  function treeviewFinder() {
      try {
          TreeViewGotFocus();          
     }
      catch (err) {
      }
  }

    // -->

</script>

<table border="0" width="940px" cellpadding="0" cellspacing="0">
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
            <asp:PlaceHolder ID="Forums" runat="server" Visible="true">            
            </asp:PlaceHolder>             
            <table width="100%" cellpadding="0" cellspacing="0" border="0" runat="server" id="tblForums">
                <tr>
                    <td valign="top" align="left">
                        <table cellpadding="0" cellspacing="0" width="490" border="0" runat="server" id="Table1">
                            <tr>
                                <td colspan="3" class="totalResults_Header">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td class="forum_header_title_text">
                                               <asp:Label runat="server" ID="TotalPostsLbl" resourcekey="TotalPosts"></asp:Label>                                                
                                            </td>
                                        </tr>
                                    </table>                                    
                                    <div class='tooltipWrapper'>
                                    <asp:CheckBox ID="notifyCheck" runat="server" AutoPostBack="true" OnCheckedChanged="notifyCheck_CheckedChanged" />
                                        <div class='dnnTooltip'>
                                            <label id='label' runat='server'>
                                                 <asp:LinkButton ID='cmdHelp' TabIndex='-1' runat='server' CausesValidation='False'
                                                                 EnableViewState='False' CssClass='dnnFormHelp'>
                                                    <asp:Label ID='notifyLabel' CssClass="notification" 
                                                                runat='server' resourcekey="Notification" EnableViewState='False' />
                                                </asp:LinkButton>
                                                <asp:Label ID='lblNoHelpLabel' runat='server' EnableViewState='False' Visible='false' />
                                            </label>
                                            <asp:Panel ID='pnlHelp' runat='server' CssClass='dnnFormHelpContent dnnClear' 
                                                        EnableViewState='False' style='display:none;'>
                                                <asp:Label ID='lblHelp' runat='server' EnableViewState='False' resourcekey="NotificationHelp" 
                                                class='dnnHelpText' />
                                                        <a href='#' class='pinHelp'></a>
                                            </asp:Panel>
                                       </div>   
                                    </div>
                                </td>
                            </tr>
                            <tr bgcolor="#FFFFFF">
                                <td colspan="3" valign="top">
                                    <div style="height: 250px; overflow: auto; background-color: #eeeeee;">
                                        <asp:TreeView ID="TreeView1" ExpandDepth="0" PopulateNodesFromClient="False" ShowLines="True"
                                            runat="server" EnableViewState="false" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged"
                                            ImageSet="BulletedList" LineImagesFolder="~/DesktopModules/uDebate/images/TreeLineImages"
                                            NodeWrap="True">
                                            <SelectedNodeStyle Font-Bold="True" Font-Names="Tahoma" Font-Size="12px" />
                                            <NodeStyle HorizontalPadding="3px" CssClass="thread_post_post_text" />
                                        </asp:TreeView>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="treePrintBg" colspan="3" align="center">
                                    <atc:imagebuttonext id="ibxStartNewPost" runat="server" imageurl="~/img/main/buttons/discussion_norm_gr.gif"
                                        imageoverurl="~/img/main/buttons/discussion_roll_gr.gif" onclick="ibxStartNewPost_Click" />
                                    <atc:imagebuttonext id="ibxOpenTreePrint" runat="server" imageurl="~/img/main/buttons/print_norm_gr.gif"
                                        imageoverurl="~/img/main/buttons/print_roll_gr.gif" onclientclick="openTreeViewPrinter();return false;" />                                      
                                    <asp:Hyperlink id="threadSummaryBtn" CssClass="threadSummary" runat="server" Text="&nbsp;"  onclick="threadSummaryBtn_Click" /> 
                                     
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" align="right">
                        <table cellpadding="0" cellspacing="0" width="440" border="0" runat="server" id="postMessageView">
                            <tr>
                                <td colspan="3" class="postText_Header">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td class="forum_header_title_text">
                                                <asp:Label ID="lbMessagePostHead" runat="server" Text="" Font-Bold="true" CssClass="forum_header_title_text"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr bgcolor="#FFFFFF">
                                <td colspan="3" valign="top">
                                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                        <tr>
                                            <td valign="top">
                                                <table cellpadding="3" cellspacing="0" border="0" width="100%" bgcolor="#7992cb"
                                                    height="35">
                                                    <tr>
                                                        <td class="forum_title_text<%=SelectedPostType %>">
                                                            <asp:Label ID="lbMessagePost" runat="server" Text="&nbsp;" Font-Bold="true"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div style="width: 440px; height: 215px; overflow: auto; background-color: #eeeeee;">
                                                    <table cellpadding="3" cellspacing="0" border="0">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lbBody" runat="server" Text="" Width="420" style="height:176px; overflow: auto;" CssClass="lexis_text"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="treePostBg">
                                    <table cellpadding="0" cellspacing="0" width="100%" border="0">
                                        <tr>
                                            <td style="height: 20px">
                                                <asp:Literal ID="lbBodyFiles" runat="server"></asp:Literal>&nbsp;
                                            </td>
                                            <td width="120" align="center">
                                                <atc:imagebuttonext id="ibxPrinterPost" runat="server" imageurl="~/img/main/buttons/print_norm_gr.gif"
                                                    imageoverurl="~/img/main/buttons/print_roll_gr.gif" onclientclick="openSelectedPostPrinter();return false;" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">      
                        
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdThreadId" runat="server" Value="" />
            <asp:HiddenField ID="hdParentId" runat="server" Value="" />
            <asp:HiddenField ID="txtPostTypeId" runat="server" Value="" />
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <a name="post"></a>
            <asp:Label ID="lbStartPosting" runat="server" Text="" CssClass="forum_login_fail_bigger"
                Visible="false"></asp:Label>
                 
            <asp:Panel Visible="false" ID="PanelHolder" runat="server">
                <table cellpadding="0" cellspacing="0" width="940" border="0" runat="server" id="Table2">
                    <tr>
                        <td colspan="3" class="replyHeader">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td width="50">
                                        &nbsp;
                                    </td>
                                    <td class="forum_header_title_text">
                                       <asp:Label ID="ReplyForm" runat="server" resourcekey="replyForm"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr bgcolor="#DEDEDE">
                        <td colspan="3" valign="top">
                            <div style="background-color: #eeeeee;">
                                &nbsp;
                                <asp:Panel Visible="false" ID="PanelReply" runat="server" Width="100%">
                                    <table width="100%" cellpadding="2" cellspacing="0" border="0">
                                        <tr>
                                            <td width="5" rowspan="5">
                                                &nbsp;
                                            </td>
                                            <td width="100">
                                                <asp:Label ID="lbSubjectD" runat="server" Text="" Visible="False" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSubjectPost" runat="server" Width="300"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100">
                                                <asp:Label ID="lbFiles" runat="server" Text="" Visible="True" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>
                                                
                                                <udebate:attachment id="ctlAddAttachment" runat="server" width="375px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100">
                                                <asp:Label ID="lbPostType" runat="server" Text="" Visible="False" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>
                                                <table width="100%" cellpadding="2" cellspacing="1" border="0" bgcolor="lightgrey">
                                                    <tr bgcolor="white">
                                                        <td id="postButtons" runat="server" width="200">
                                                            <asp:ImageButton ID="ImageButton1" runat="server" Visible="false" OnClientClick="go('1', 'Issue');return false;" />
                                                            <asp:ImageButton ID="ImageButton2" runat="server" Visible="false" OnClientClick="go('2', 'Alternative');return false;" />
                                                            <asp:ImageButton ID="ImageButton3" runat="server" Visible="false" OnClientClick="go('3', 'Pro Argument');return false;" />
                                                            <asp:ImageButton ID="ImageButton4" runat="server" Visible="false" OnClientClick="go('4', 'Con Argument');return false;" />
                                                            <asp:ImageButton ID="ImageButton5" runat="server" Visible="false" OnClientClick="go('5', 'Question');return false;" />
                                                            <asp:ImageButton ID="ImageButton6" runat="server" Visible="false" OnClientClick="go('6', 'Answer');return false;" />
                                                            <asp:ImageButton ID="ImageButton7" runat="server" Visible="false" OnClientClick="go('7', 'Comment');return false;" />
                                                            <asp:ImageButton ID="ImageButton8" runat="server" Visible="false" OnClientClick="go('8', 'Comment');return false;" />
                                                        </td>
                                                        <td id="postButtonsSelectedType" runat="server">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbReply" runat="server" Text="" Visible="False" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>
                                               <dnn:texteditor id="txtReply" runat="server" height="200px" autodetectlanguage="true"
                                                    width="100%"></dnn:texteditor>
                                                    <%--<asp:TextBox id="txtReply" TextMode="MultiLine" runat="server" height="200px" width="99%"></asp:TextBox>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Label ID="lblError" runat="server" Text="" CssClass="forum_error"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Label ID="lblConfirm" runat="server" Text="" CssClass="forum_confirmation"></asp:Label>
                                <asp:Panel Visible="false" ID="PanelStartPosting" runat="server" Height="400px" Width="100%">
                                    <table width="100%" cellpadding="2" cellspacing="0" border="0">
                                        <tr>
                                            <td width="5" rowspan="5">
                                                &nbsp;
                                            </td>
                                            <td width="100">
                                                <asp:Label ID="lbStartingSubject" runat="server" Text="Subject" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtStartingSubject" runat="server" Width="300"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100">
                                                <asp:Label ID="lbStartFiles" runat="server" Text="Files" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>                                               
                                                <udebate:attachment id="ctlNewAttachment" runat="server" width="375px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="100">
                                                <asp:Label ID="lbStartingPostType" runat="server" Text="Type" Visible="False" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>
                                                <table width="100%" cellpadding="2" cellspacing="1" border="0" bgcolor="lightgrey">
                                                    <tr bgcolor="white">
                                                        <td id="postStartingButtons" runat="server" width="150">
                                                            <asp:ImageButton ID="ImageButton11" runat="server" Visible="false" OnClientClick="goStart('1', 'Issue');return false;" />
                                                            <asp:ImageButton ID="ImageButton12" runat="server" Visible="false" OnClientClick="goStart('5', 'Question');return false;" />
                                                        </td>
                                                        <td id="postButtonsStartingSelectedType" runat="server">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbStartingReply" runat="server" resourcekey="Description" Visible="False" CssClass="forum_reply_td_titles"></asp:Label>
                                            </td>
                                            <td>
                                            <%--<asp:TextBox id="txtStartingReply" TextMode="MultiLine" runat="server" height="200px" width="99%"></asp:TextBox>
                                             --%>   <dnn:texteditor id="txtStartingReply" runat="server" autodetectlanguage="true" height="200px"
                                                    width="100%"></dnn:texteditor>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="center">
                                                <asp:Label ID="lblStartingError" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td  colspan="3" class="replyBg" align="center">
                            <asp:Panel ID="PanelReplyButtons" runat="Server" Visible="false">
                                <atc:imagebuttonext id="ibxSave" runat="server" imageurl="~/DesktopModules/uDebate/images/buttons/send_norm_gr.gif"
                                    imageoverurl="~/DesktopModules/uDebate/images/buttons/send_roll_gr.gif" onclick="ibxSave_Click" />
                                &nbsp;&nbsp;
                                <atc:imagebuttonext id="ibxRemoveStartFiles" runat="server" imageurl="~/DesktopModules/uDebate/images/buttons/cancel_norm_gr.gif"
                                    imageoverurl="~/DesktopModules/uDebate/images/buttons/cancel_roll_gr.gif" onclick="ibxRemoveStartFiles_Click" />
                            </asp:Panel>
                            <asp:Panel ID="PanelStartPostingButtons" runat="server" Visible="false">
                                <atc:imagebuttonext id="ibxStartingSave" runat="server" imageurl="~/DesktopModules/uDebate/images/buttons/send_norm_gr.gif"
                                    imageoverurl="~/DesktopModules/uDebate/images/buttons/send_roll_gr.gif" onclick="ibxStartingSave_Click" />
                                &nbsp;&nbsp;
                                <atc:imagebuttonext id="ibxCancelStartFiles" runat="server" imageurl="~/DesktopModules/uDebate/images/buttons/cancel_norm_gr.gif"
                                    imageoverurl="~/DesktopModules/uDebate/images/buttons/cancel_roll_gr.gif" onclick="ibxCancelStartFiles_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<div class="legend">
<asp:Image ID="legendIssueImg" runat="server" /><asp:Label ID="legendIssueLbl" runat="server" />
<asp:Image ID="legendAltImg" runat="server" /><asp:Label ID="legendAltLbl" runat="server" />
<asp:Image ID="legendProImg" runat="server" /><asp:Label ID="legendProLbl" runat="server" />
<asp:Image ID="legendConImg" runat="server" /><asp:Label ID="legendConLbl" runat="server" />
<asp:Image ID="legendCommentImg" runat="server" /><asp:Label ID="legendCommentLbl" runat="server" />                   
</div>

<script type="text/javascript">
    jQuery(document).ready(function() {
        treeviewFinder();

        jQuery('<%=TreeView1.ClientID%>').live("click", function() {
            setTimeout(function() {
                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.charset = 'UTF-8';
                s.src = ((location && location.href && location.href.indexOf('https') == 0) ? 'https://ssl.microsofttranslator.com' : 'http://www.microsofttranslator.com')
            + '/ajax/v2/widget.aspx?mode=manual&from=<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name %>&layout=ts';
                var p = document.getElementsByTagName('head')[0] || document.documentElement; p.insertBefore(s, p.firstChild);
            }, 100);
            jQuery(".DocumentManager").hide();  /*Hide doc manager tool from text editor*/
        });

        /* Click handler for the Clear menu item */
        jQuery('.admin_link').dnnConfirm({
            text: 'Are you sure?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Action Confirmation'
        });

        jQuery('.dnnTooltip').dnnTooltip();

        jQuery(".proposeSlider").show();

        jQuery(".proposeSlider").click(function() {
            jQuery(".ProposeTopic").toggle("slow");
            jQuery(this).toggleClass("active");

            return false;
        });
        jQuery('.DocumentManager').parent().hide();
        jQuery('div [id$="_PanelView"]').hide();
        

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

