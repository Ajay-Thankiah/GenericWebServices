using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace GenericWebService.Helpers
{
    public class EmailHelper
    {
        public static bool EmailErrorMessage(string message, String messageShownToUser)
        {
            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            string path = HttpContext.Current.Request.Url.AbsolutePath;
            string host = HttpContext.Current.Request.Url.Host;
            if (host.Equals("localhost")) return false;
            string subject = "error in the path " + path;
            SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["adminEmail"], ConfigurationManager.AppSettings["adminPassword"])
            };
            string[] developerEmails = ConfigurationManager.AppSettings["DeveloperEmailsToReceiveErrorMessages"].ToString().Split(',');

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["adminEmail"]);

                for (int i = 0; i < developerEmails.Length; i++)
                {
                    if (i == 0) mail.To.Add(developerEmails[i]);
                    else mail.CC.Add(developerEmails[i]);
                }

                mail.Subject = subject;
                StringBuilder emailBody = new StringBuilder();
                emailBody.Append("Team,\n\t");
                emailBody.Append("An error triggered in the url - ");
                emailBody.Append(url);
                emailBody.Append(", in the host - ");
                emailBody.Append(host);
                emailBody.Append("\n\n");
                emailBody.Append("The error message shown to the user is : '");
                emailBody.Append(messageShownToUser);
                emailBody.Append("'\n\n");
                emailBody.Append("The detailed error message is as follows:\n");
                emailBody.Append(message);
                emailBody.Append("\n\n\n\nCenter for Governmental Studies\n");
                mail.Body = emailBody.ToString();
                client.Send(mail);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}