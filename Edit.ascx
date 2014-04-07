<%@ Control Language="C#" Inherits="DotNetNuke.Modules.uDebate.Edit" AutoEventWireup="false"
    CodeBehind="Edit.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register Assembly="ATC.WebControls" Namespace="ATC.WebControls" TagPrefix="atc" %>
<table cellspacing="3" cellpadding="3" class="MainTable">
    <tr>
        <td class="gridMainCaption" colspan="4">
            <table style="width: 100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td colspan="2" class="MainCaption">
                        <asp:Label ID="lbTitle" runat="server" Text="New Thread" resourcekey="ControlTitle_edit"></asp:Label>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="gridToolBar" style="padding-bottom: 4px; padding-top: 2px; height: 22px"
            valign="top" align="left" colspan="4">
            &nbsp;<asp:LinkButton ID="btnNew" CssClass="dnnPrimaryAction" runat="server" OnClick="btnNew_Click" resourcekey="New">New</asp:LinkButton>
            &nbsp;&nbsp;<asp:LinkButton ID="btnDelete" CssClass="dnnPrimaryAction" runat="server"
                OnClick="btnDelete_Click" resourcekey="Delete">Delete</asp:LinkButton>
        </td>
    </tr>
    <asp:Label ID="lblMessage" runat="server" Visible="true"></asp:Label>

    <%--<tr>
					<td style="WIDTH: 100px" noWrap><asp:Label runat="server" ID="lblID">Thread ID</asp:Label></td>
					<td><asp:textbox id="txtThreadID" runat="server" ReadOnly="True" ForeColor="Silver"></asp:textbox></td>
				</tr>
				<tr>
                    <td>
                        <asp:Label ID="lbContact" runat="server" Text="Contact Email" ></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtContact" runat="server" Width="350px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbComplaint" runat="server" Text="Complaint Email" ></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtComplaint" runat="server" Width="350px"></asp:TextBox>
                    </td>
                </tr>--%>
 
            <asp:Label ID="lblUser" runat="server" Text="Admin User" Visible="false"></asp:Label> 
            <asp:DropDownList ID="ddUser" runat="server" Visible="false" />

    <tr>
        <td>
            <asp:Label ID="lbActive" runat="server" Text="Active" resourcekey="Active"></asp:Label>
        </td>
        <td>
            <asp:RadioButtonList ID="chkActive" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="1" Selected="True" resourcekey="Yes">Yes</asp:ListItem>
                <asp:ListItem Value="0" resourcekey="No">No</asp:ListItem>
            </asp:RadioButtonList>
        </td>
        <td>
            <asp:Label ID="Label1" runat="server" Text="Active" resourcekey="European"></asp:Label>
        </td>
        <td>
            <asp:RadioButtonList ID="chkEuropean" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="1" resourcekey="Yes" >Yes</asp:ListItem>
                <asp:ListItem Value="0" resourcekey="No" Selected="True">No</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td valign="middle" style="width: 110px;">
            <asp:Label ID="lblPosition" runat="server" Text="Position"></asp:Label>
        </td>
        <td colspan="3">
            &nbsp;<asp:DropDownList ID="dropDownPosition" runat="server" Visible="false">
                <asp:ListItem Value="1" Text="1" Selected="True"></asp:ListItem>
                <asp:ListItem Value="2" Text="2"></asp:ListItem>
                <asp:ListItem Value="3" Text="3"></asp:ListItem>
                <asp:ListItem Value="4" Text="4"></asp:ListItem>
                <asp:ListItem Value="5" Text="5"></asp:ListItem>
                <asp:ListItem Value="6" Text="6"></asp:ListItem>
                <asp:ListItem Value="7" Text="7"></asp:ListItem>
                <asp:ListItem Value="8" Text="8"></asp:ListItem>
                <asp:ListItem Value="9" Text="9"></asp:ListItem>
                <asp:ListItem Value="10" Text="10"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <%--<tr>
                    <td>
                        <asp:Label ID="lbStatus" runat="server" Text="Status"></asp:Label>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="chkStatus" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Selected="True">Open</asp:ListItem>
                            <asp:ListItem Value="2">Closed</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
		            <td valign="middle" style="width:110px;">
                        <asp:Label ID="lbType" runat="server" Text=""></asp:Label></td>
		            <td>						&nbsp;<asp:DropDownList ID="ddGroupID" runat="server">
		                                                <asp:ListItem Value="1" Text="Forum Type I" />
		                                                <asp:ListItem Value="2" Text="Forum Type II"/>
		                                                </asp:DropDownList>
		                                                </td>
	            </tr>
                <tr>
		            <td valign="middle" style="width:110px;">
                        <asp:Label ID="lbThreadStatus" runat="server" Text="Progress Status"></asp:Label></td>
		            <td>						&nbsp;<asp:DropDownList ID="ddStatusID" runat="server">
		                                                </asp:DropDownList>
		                                                </td>
	            </tr>--%>
    <tr>
        <td valign="top">
            <asp:Label ID="lbDescription" runat="server" Text="Subject" resourcekey="Subject"></asp:Label>
        </td>
        <td colspan="3">
            <asp:TextBox ID="txtDescription" runat="server" Width="100%" TextMode="MultiLine"
                Rows="2"></asp:TextBox>
        </td>
    </tr>
    <%--<tr>
                    <td valign="top">
                        <asp:Label ID="lbSummary" runat="server" Text="Summary"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtSummary" runat="server"  Width="100%"  Rows="2" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>--%>
    <tr>
        <td valign="top">
            <asp:Label ID="lblText" runat="server" Text="Description" resourcekey="Description"></asp:Label>
        </td>
        <td colspan="3">
            <dnn:texteditor id="txtThread" runat="server" autodetectlanguage="true" width="100%"
                height="300px"></dnn:texteditor>
        </td>
    </tr>
    <tr>
        <td class="gridToolBar" style="padding-bottom: 4px; padding-top: 2px; height: 22px"
            valign="top" align="left" colspan="4">
            &nbsp; &nbsp;&nbsp;<asp:LinkButton ID="btnSave" CssClass="dnnPrimaryAction" runat="server"
                OnClick="btnSave_Click" resourcekey="Save">Save</asp:LinkButton>
            &nbsp;&nbsp;<asp:LinkButton ID="btnCancel" CssClass="dnnPrimaryAction" runat="server"
                OnClick="btnCancel_Click" resourcekey="Cancel">Cancel</asp:LinkButton>
        </td>
    </tr>
</table>

<script type="text/javascript">


    jQuery(document).ready(function() {

        jQuery('#<%= btnDelete.ClientID %>').dnnConfirm({
            text: 'The thread will be deleted. Are you sure?',
            yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
            noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
            title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
        });

    });    
</script>

