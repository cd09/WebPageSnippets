using System;
using System.Collections.Generic;
using System.Web.UI;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;
using System.Configuration;
using System.Text;
using AjaxControlToolkit;

namespace Helper_Webportal
{
    public partial class CreateUserAcct : System.Web.UI.Page
    {
      private Boolean verboseLogs = Convert.ToBoolean(ConfigurationManager.AppSettings["_verboseLogs"]);
      StringBuilder myLog = new StringBuilder();
    
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage master = Page.Master as MasterPage;
            master.NavButtons.Visible = false;
            master.LogOut.Visible = false;
        }

        protected void GetService()
        {
            try
            {
                myLog.AppendLine("GetService");
                //get crm service
                if (verboseLogs) LB_Error.Text += myLog.ToString();
            }
            catch (Exception e)
            {
                LB_Error.Text += "Exception: " + e.Message;
            }
        }

        String GetRecordID(String entity, String Column, String fieldValue2Filter, String fieldName2Filter)
        {
            String myFieldValue = String.Empty;
            myLog.AppendLine("GetRecordID()");
            QueryExpression myQuery1 = new QueryExpression(entity);
            myQuery1.ColumnSet = new ColumnSet(new string[] { Column });
            FilterExpression myFilter1 = new FilterExpression();
            if (fieldValue2Filter != String.Empty)
                myFilter1.AddCondition(new ConditionExpression(fieldName2Filter, ConditionOperator.Equal, new Object[] { fieldValue2Filter }));
            myQuery1.Criteria.AddFilter(myFilter1);
            EntityCollection myRetrieved1 = service.RetrieveMultiple(myQuery1);
            myLog.AppendLine("myRetrieved1.Entities.Count: " + myRetrieved1.Entities.Count);
            if (myRetrieved1.Entities.Count > 0)
            {
                Entity myEntity1 = myRetrieved1.Entities[0];
                myFieldValue = myEntity1.Attributes.Contains(Column) ? ((Guid)myEntity1.Attributes[Column]).ToString() : String.Empty;
                myLog.AppendLine(Column + ": " + myFieldValue);

            }
            if (verboseLogs) LB_Error.Text += myLog.ToString();
            return myFieldValue;
        }

        String CreateUser(String entity, String emailValue, String passwordValue, string userNameValue, Boolean encrypted)
        {
            Entity myUser = new Entity(entity);
            myUser["name"] = userNameValue;
            myUser["email"] = emailValue;
            myUser["password"] = passwordValue;
            if (encrypted)
                myUser["encrypted"] = true;
            Guid myUserId = service.Create(myEntity);

            return myUserId.ToString();
        }

        protected void CreateUserBtn_Click(object sender, EventArgs e)
        {
        myLog.AppendLine("CreateUserBtn_Click");
            try
            {
                LB_Error.Text = String.Empty;
                    String ID = String.Empty;
                    if (TBPassword.Text != "" && TBEmail.Text != "" && TBConfirmPassword.Text != "")
                    {
                        sharedClass sc = new sharedClass();
                        
                        //check if User Account already exists
                        ID = GetRecordID("new_userAccount", "new_userAccountid", TBEmail.Text.ToLower(), "new_emailaddress");
                        
                        if (ID != String.Empty)
                        {
                            LB_Error.Text += "<br> A user account is already registered with this email address";

                        }
                        else
                        {
                            ID = CreateUser("new_userAccount", TBEmail.Text.ToLower(), sc.Encrypt(TBPassword.Text), TBName.Text, true);
                            LB_Error.Text += "<br> Thank you for registering";
                        }
                           myLog.AppendLine("ID: " + ID);
                    }
                    else
                        LB_Error.Text += "<br>You must fill In both User ID and Password.";

                if (verboseLogs) LB_Error.Text += myLog.ToString();
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
