using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace api.Services
{
    public class SendGridEmailService : IEmailService
    {

        private readonly IConfiguration _config;
        public SendGridEmailService(IConfiguration config)
        {
            _config = config;
        }


        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(_config["SendGrid:SenderEmail"], _config["SendGrid:SenderName"]);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }
        }
    }
}