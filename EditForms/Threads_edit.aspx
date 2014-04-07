<%@ Page language="c#" Inherits="DotNetNuke.Modules.uDebate.EditForms.Threads_Edit" Codebehind="Threads_edit.aspx.cs" %>
<%@ Register TagPrefix="atc" Namespace="ATC.WebControls" Assembly="ATC.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>
			<%=ATC.Tools.GetParam("SiteName").ToString()%>
		</title>		
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<LINK href="../../../styles/styles.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body style="background-color:#DEF1FA; margin:2 2 2 2;" >
		<form id="Form1" method="post" runat="server">
			<table cellspacing="3" cellpadding="3" class="MainTable">
				<tr>
					<td class="gridMainCaption" colspan="2">
						<table style="width:100%" cellpadding="0" cellspacing="0" border="0">
						    <tr>
						        <td colspan="2" class="MainCaption"><%=ATC.Translate.String("Επεξεργασία Thread", "", "EN")%></td>
						    </tr>
						</table>
					</td>
				</tr>			
				<tr>
					<td class="gridToolBar" style="PADDING-BOTTOM: 4px; PADDING-TOP: 2px; HEIGHT: 22px" vAlign="top"
						align="left" colSpan="10">&nbsp;<asp:linkbutton id="btnNew" runat="server" onclick="btnNew_Click">Νέο</asp:linkbutton>&nbsp;|&nbsp;<asp:linkbutton id="btnSave" runat="server" onclick="btnSave_Click">Αποθήκευση</asp:linkbutton>&nbsp;|&nbsp;<asp:linkbutton id="btnDelete" runat="server" onclick="btnDelete_Click">Διαγραφή</asp:linkbutton>
					</td>
				</tr>
				<tr>
					<td style="WIDTH: 100px" noWrap><asp:Label runat="server" ID="lblID">Α/Α</asp:Label></td>
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
                </tr>
                <tr>
		            <td valign="middle" style="width:110px;">
                        <asp:Label ID="lblUser" runat="server" Text="Χρήστης Διαχείρισης"></asp:Label></td>
		            <td>						&nbsp;<asp:DropDownList ID="ddUser" runat="server" /></td>
	            </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbActive" runat="server" Text="Active" ></asp:Label>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="chkActive" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Selected="True">Yes</asp:ListItem>
                            <asp:ListItem Value="0">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
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
	            </tr>
                <tr>
                    <td valign="top">
                        <asp:Label ID="lbDescription" runat="server" Text="Subject"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server"  Width="100%" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label ID="lbSummary" runat="server" Text="Summary"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtSummary" runat="server"  Width="100%"  Rows="2" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:Label ID="lblText" runat="server" Text="Description" ></asp:Label>
                    </td>
                    <%--<td>
                        <dnn:texteditor id="txtThreadlklklk" runat="server" autodetectlanguage="true" Width="100%" Height="400px"></dnn:texteditor>
                    </td>--%>
                </tr>
			</table>
		</form>
	</body>
</HTML>
