using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Configuration;
using System.Text;
using AjaxControlToolkit;
using Helper_WebPortal.App_Code; //shared code within project

namespace Helper_WebPortal
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        private Boolean verboseLogs = Convert.ToBoolean(ConfigurationManager.AppSettings["_verboseLogs"]);
        StringBuilder myLog = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {
        try
            {
            //prevent browser cache so end user cannot click back button to return to this page
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");

            MasterPage master = Page.Master as MasterPage;
            master.NavButtons.Visible = false;
            master.LogOut.Visible = false;
            sharedClass sc = new sharedClass();
            String userid = String.Empty;
            String expirationDate = String.Empty;
            //A background workflow generates a url with QueryString params and sends to end user via email after 
            //they submit their password reset request.  The Querystring params include:
            //Querystring param 'exp' is an encrypted datetime stamp and is used to enforce a 24 hr expiration of the password reset request
            //Querystring param 'userid' is a GUID to the user account record in the source system
            if (Request.QueryString["exp"] != null)
            {
                byte[] expB = System.Convert.FromBase64String(Request.QueryString["exp"]);
                expirationDate = System.Text.Encoding.UTF8.GetString(expB);
                expirationDate = sc.Decrypt(expirationDate);
                //expirationDate = "20170625055025"; //test expiration
            }
            myLog.AppendLine("exp: " + exp);
            if (Request.QueryString["userid"] != null)
                userid = Request.QueryString["userid"];
            myLog.AppendLine("userid: " + userid);
            if (userid != String.Empty && expirationDate != String.Empty)
            {
                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                int EXPyear = Convert.ToInt32(exp.Substring(0, 4));
                int EXPmonth = Convert.ToInt32(exp.Substring(4, 2));
                myLog.AppendLine("EXPmonth: " + EXPmonth.ToString());
                int EXPday = Convert.ToInt32(exp.Substring(6, 2));
                myLog.AppendLine("EXPday: " + EXPday.ToString());
                int EXPhour = Convert.ToInt32(exp.Substring(8, 2));
                myLog.AppendLine("EXPhour: " + EXPhour.ToString());
                int EXPminute = Convert.ToInt32(exp.Substring(10, 2));
                int EXPsecond = Convert.ToInt32(exp.Substring(12, 2));
                DateTime EXP = new DateTime(EXPyear, EXPmonth, EXPday, EXPhour, EXPminute, EXPsecond);
                TimeSpan ts = new TimeSpan();
                ts = now - EXP;
                myLog.AppendLine("ts.Hours: " + ts.TotalHours.ToString());
                if (ts.TotalHours > 24)
                {
                    resetPwdBtn.Visible = false;
                    LB_Error.Text += "<br>We're sorry.  Your password reset request has expired.  Please submit a new request. ";
                    loginReturnHyperlink.Visible = true;
                }
                else
                {
                    trNewPassword.Visible = true;
                    trConfirmPassword.Visible = true;
                    RequiredNewPwdFieldValidator.Enabled = true;
                    RequiredConfirmPwdFieldValidator.Enabled = true;
                    Session["ID"] = userid;
                }
            }
            else 
            {
                trEmail.Visible = true;
                RequiredEmailFieldValidator.Enabled = true;
            }
            }
            catch (Exception e)
            {
                LB_Error.Text += "Exception: " + e.Message;
            }
            if (verboseLogs) LB_Error.Text += myLog.ToString();
        }

        protected void GetService()
        {
            try
            {
                myLog.AppendLine("GetService");
                //get the crm service
            }
            catch (Exception e)
            {
                LB_Error.Text += "Exception: " + e.Message;
            }
        }

        void UpdateEntity(String entity, String key, String password, Boolean encrypted)
        {
            myLog.AppendLine("UpdateEntity(): " + key);
            Entity myEntityUpd = new Entity(entity);
            myEntityUpd.Id = new Guid(key);
            myEntityUpd["password"] = password;
            myEntityUpd["encrypted"] = encrypted;
            if (!encrypted)
                myEntityUpd["new_resetpassword"] = true;
            else
                myEntityUpd["new_resetpassword"] = false;
            var req = new UpdateRequest { Target = myEntityUpd };
            var resp = (UpdateResponse)service.Execute(req);
            myLog.AppendLine("resp: " + resp.Results.ToString()); 

            if (TBEmail.Text != "")
                LB_Error.Text += "<br> Your request has been submitted. You should receive an email notification shortly with a link to reset your password.";
            else
            {
                trNewPassword.Visible = false;
                trConfirmPassword.Visible = false;
                resetPwdBtn.Visible = false;
                LB_Error.Text += "<br> Your password has been reset.";
                loginReturnHyperlink.Visible = true;
            }
        }

        protected void resetPwdBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LB_Error.Text = String.Empty;
                    String ID = String.Empty;
                    sharedClass sc = new sharedClass();
                    
                    if (TBPassword.Text != "" && TBConfirmPassword.Text != "")
                    {
                        String userid = String.Empty;
                        userid = (String)Session["ID"];
                        if (userid != null)
                        {
                            Guid userGUID = Guid.Empty;
                            userGUID = new Guid(userid);
                            myLog.AppendLine("userGUID: " + userGUID.ToString());
                            UpdateEntity("new_userAccount", userGUID.ToString(), sc.Encrypt(TBPassword.Text), true);
                        }
                    }
                    else if (TBEmail.Text != "")
                    {
                        ID = GetRecordID("new_userAccount", "new_userAccountid", TBEmail.Text.ToLower(), "new_emailaddress");
                        if (ID != String.Empty)
                        {
                            UpdateEntity("new_userAccount", ID, "", false);
                        }
                        else
                        {
                            LB_Error.Text += "<br> This email address is not associated with a Portal Partner User Account.  Please check the spelling and try again.";
                        }
                    }
                    else
                        LB_Error.Text += "<br> Email cannot be blank.";
                
                if (verboseLogs) LB_Error += myLog.ToString();
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                LB_Error.Text += "<br>  SOAP exception: " + soapEx.Detail.InnerText + "  " + soapEx.ToString();
            }
            catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                LB_Error.Text += "<br>  FaultException:  " + ex.Data.ToString();
                LB_Error.Text += "<br>  Timestamp: " + ex.Detail.Timestamp;
                LB_Error.Text += "<br>  Code: " + ex.Detail.ErrorCode;
                LB_Error.Text += "<br>  Message: " + ex.Detail.Message;
            }
            catch (Exception ex)
            {
                LB_Error.Text += "<br> Exception: " + ex.Message + "  Inner Fault: " + ex.InnerException;
            }
        }
    }
}
