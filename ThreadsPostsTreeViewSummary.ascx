<%@ Control Language="c#" AutoEventWireup="True" Inherits="DotNetNuke.Modules.uDebate.ThreadsPostsTreeViewSummary"
    CodeBehind="ThreadsPostsTreeViewSummary.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<table style="padding: 20px 12px;" cellpadding="0" cellspacing="0" border="0" width="99%">
    <tr>
        <td class="forum_topics_forum_header1">
            <asp:Label ID="lbThreadTitle" runat="server" Text="" CssClass="forum_link"></asp:Label><br />
            <br />
        </td>
    </tr>
    <tr>
        <td valign="top" align="left">
            <asp:TreeView ID="TreeView1" ExpandDepth="0" PopulateNodesFromClient="False" ShowLines="True"
                Width="320" runat="server" EnableViewState="False" ImageSet="BulletedList" 
                LineImagesFolder="~/DesktopModules/uDebate/images/TreeLineImages">
                <SelectedNodeStyle Font-Bold="True" Font-Names="Tahoma" Font-Size="12px" />
                <NodeStyle HorizontalPadding="3px" CssClass="thread_post_post_text" />
            </asp:TreeView>
        </td>
    </tr>
</table>
<br />
<br />
<div class="summaryResults">
    <table width="500px" class="totalResults_Header">
        <tr>
            <td class="forum_header_title_text">
                <asp:Label ID="TotalPosts" runat="server" />
                posts by
                <asp:Label ID="TotalUsers" runat="server" />
                users.
            </td>
        </tr>
    </table>
    <div class="notesText">
        <h3>
            Notes:</h3>
        <br />
        <asp:Label ID="NotesLabel" runat="server" />
    </div>
    <dnn:texteditor id="txtNotesEditor" runat="server" height="200px" autodetectlanguage="true"
        width="100%"></dnn:texteditor>
    <div style="text-align: center">
        <asp:Button ID="HideEditorBtn" runat="server" Text="Hide editor" class="art-button"
            OnClick="HideEditorBtn_Click" />
        <asp:Button ID="SaveNotesBtn" runat="server" Text="Save Notes" class="art-button"
            OnClick="SaveNotesBtn_Click" />
        <span class="art-button-wrapper"><span class="l"></span><span class="r"></span><a
            onclick="window.print();return false;" class="art-button" href="/">Print</a></span>
        <span class="art-button-wrapper"><span class="l"></span><span class="r"></span><a
            onclick="history.back();return false;" class="art-button" href="/">Back</a></span>
    </div>
</div>
