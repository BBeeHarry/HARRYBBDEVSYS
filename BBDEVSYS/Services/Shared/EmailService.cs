using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Configuration;

using BBDEVSYS.Models.Shared;
using System.Configuration;
using System.IO;
using BBDEVSYS.Models.Entities;

namespace BBDEVSYS.Services.Shared
{
    public class EmailService
    {
        public static string SendEmail(List<EmailReceiver> receivers, string subject, string content)
        {
            string result = "";
            try
            {

                MailMessage message = new MailMessage();
                message.From = new MailAddress(WebConfigurationManager.AppSettings["EmailSender"], WebConfigurationManager.AppSettings["EmailSenderName"]);
                foreach (var receiver in receivers)
                {
                    MailAddress mailAddress = new MailAddress(receiver.Email, receiver.FullName);
                    if (receiver.ReceiverType == ConstantVariableService.EmailReceiverTypeTo)
                    {
                        message.To.Add(mailAddress);
                    }
                    else if (receiver.ReceiverType == ConstantVariableService.EmailReceiverTypeCc)
                    {
                        message.CC.Add(mailAddress);
                    }
                    else if (receiver.ReceiverType == ConstantVariableService.EmailReceiverTypeBcc)
                    {
                        message.Bcc.Add(mailAddress);
                    }
                }
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = WebConfigurationManager.AppSettings["EmailHost"];
                smtp.Port = Convert.ToInt32(WebConfigurationManager.AppSettings["EmailPort"]);
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Send(message);
            }
            catch (SmtpException smex)
            {
                result = "SMTP Exception: " + smex.Message;
            }
            catch (Exception ex)
            {
                result = "Exception: " + ex.Message;
            }
            return result;
        }

        public static string[] GetEmailTemplate(string emailType)
        {
            string[] emailTemplate = new string[2];

            try
            {
                //EmailTemplate template = new EmailTemplate();

                //using (var context = new BBDEVSYSDB())
                //{
                //    template = (from m in context.EmailTemplates where m.EmailType == emailType select m).FirstOrDefault();
                //}

                //if (template == null)
                //{
                //    return emailTemplate;
                //}

                //emailTemplate[0] = template.Subject;
                //emailTemplate[1] = template.Content;

                //emailTemplate[1] = emailTemplate[1].Replace("\r\n", "<br>");
                //emailTemplate[1] = emailTemplate[1].Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");                
            }
            catch (Exception ex)
            {

            }

            return emailTemplate;
        }


        //public static string[] GetEmailTemplate(string templateName)
        //{
        //    string[] emailTemplate = new string[2];

        //    try
        //    {
        //        string emailTemplateFilePath = ConfigurationManager.AppSettings["EmailTemplateFilePath"];
        //        emailTemplateFilePath = System.Web.HttpContext.Current.Server.MapPath(emailTemplateFilePath);

        //        string templateFileName = templateName + ".txt";

        //        string file = Path.Combine(emailTemplateFilePath, templateFileName);

        //        if (System.IO.File.Exists(file))
        //        {
        //            var template = System.IO.File.ReadAllLines(file);
        //            //First line is subject
        //            string emailSubject = template[0];

        //            string emailBody = "";
        //            for (int i = 1; i < template.Length; i++)
        //            {
        //                emailBody = emailBody + template[i] + "\n";
        //            }

        //            emailTemplate[0] = emailSubject;
        //            emailTemplate[1] = emailBody;
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }

        //    return emailTemplate;
        //}
    }
}