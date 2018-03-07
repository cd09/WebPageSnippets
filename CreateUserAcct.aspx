<%@ Page Title="Create a User Account" Language="C#" MasterPageFile="MasterPage.Master" CodeBehind="CreateUserAcct.aspx.cs" Inherits="Helper_WebPortal.CreateUserAcct" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="title">
    <h1>
        <%= Page.Title%>
    </h1>
</div>
  <table>
            <tr>
            <td class="lblStyle">
                Full Name</td>
            <td>
                <asp:TextBox ID="TBName" runat="server"  Width="250px" TabIndex="1"></asp:TextBox>
            </td>
            </tr>
        <tr>
            <td class="lblStyle">
                Email Address</td>
            <td>
                <asp:TextBox ID="TBEmail" runat="server"  Width="250px" TabIndex="2"></asp:TextBox>
            </td>
            <td>
               <asp:RequiredFieldValidator id="RequiredFieldValidatorEmail"
                    ControlToValidate="TBEmail"
                    Display="Static"
                    ErrorMessage="*"
                    runat="server"/> 
            </td>
             </tr>
        <tr>
            <td class="lblStyle">
                Password <br /></td>
            <td>
                <asp:TextBox ID="TBPassword" runat="server" 
                     TextMode="Password" Width="250px" TabIndex="3"></asp:TextBox>

                <ajaxToolkit:PasswordStrength ID="PS" runat="server"
    TargetControlID="TBPassword"
    DisplayPosition="RightSide"
    StrengthIndicatorType="Text"
    Enabled="true"
    PreferredPasswordLength="10"
    MinimumNumericCharacters="1"
    MinimumSymbolCharacters="1"
    RequiresUpperAndLowerCaseCharacters="true"
    MinimumLowerCaseCharacters="1" MinimumUpperCaseCharacters="1"     
    CalculationWeightings="50;15;15;20" 
                    TextCssClass="text"
                    TextStrengthDescriptionStyles="Weak;Weak2;Average;Average2;Strong"       
                    />
                </td>
            <td>
               <asp:RequiredFieldValidator id="RequiredFieldValidatorPassword"
                    ControlToValidate="TBPassword"
                    Display="Static"
                    ErrorMessage="*"
                    runat="server"/> 
            </td>
                    <td>
                    </td>
        </tr>
      <tr><td style="font-size:5px;line-height:0px;">   </td>
            <td class="lblStyle">
                Confirm Password</td>
            <td>
                <asp:TextBox ID="TBConfirmPassword" runat="server" 
                     TextMode="Password" Width="250px" TabIndex="4"></asp:TextBox>
            </td>
           <td>
               <asp:RequiredFieldValidator id="RequiredFieldValidatorConfirmPassword"
                    ControlToValidate="TBConfirmPassword"
                    Display="Static"
                    ErrorMessage="*"
                    runat="server"/> 
            </td>
           <td>
           <asp:CompareValidator
                 id="PasswordsMatchID"
                 ControlToValidate="TBPassword"
                 ControlToCompare="TBConfirmPassword"  
                 Type="String" 
                 Operator="Equal" 
                 ErrorMessage="Passwords must match"  
                 ForeColor="Red"
                 BackColor="White"  
                 Font-Italic="true"
                 runat="server" > 
            </asp:CompareValidator>
               </td>
             </tr>
        <tr>
            <td></td>
            <td class="lblStyle">
                <asp:Button ID="CreateUserBtn" runat="server" onclick="CreateUserBtn_Click" Text="Register" 
                    BackColor="White" BorderColor="Black" BorderStyle="Solid" Font-Bold="True" ForeColor="White"
                    Width="260px" />
            </td>       
        </tr>
        <tr>
        </tr>
        <tr>
            <td>
                <asp:Label ID="LB_Error" runat="server"></asp:Label>
            </td>
        </tr>
    </table>  

</asp:Content>
