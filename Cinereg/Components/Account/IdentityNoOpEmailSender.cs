using Cinereg.Data;
using FluentEmail.Smtp;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using FluentEmail.Core;
using FluentEmail.Razor;
using System.Text;
using System.Text.RegularExpressions;

namespace Cinereg.Components.Account
{
    // Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IConfiguration _config;

        string _email = "";
        string _password = "";

        public IdentityNoOpEmailSender(IConfiguration config) 
        {
            _config = config;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            var regex = new Regex(Regex.Escape(";"));
            _email = _config["EmailSender:Email"];
            _password = _config["EmailSender:Password"];
            confirmationLink = regex.Replace(confirmationLink, "&", confirmationLink.Count(x => x == ';'));
            System.Net.NetworkCredential credentials = new(_email, _password);
            var sender = new SmtpSender(() => new SmtpClient("smtp-mail.outlook.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credentials,
                EnableSsl = true
            });

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();
            StringBuilder template = new();
            template.AppendLine("Hey @Model.Email,");
            template.AppendLine("<p>Please confirm your account by <a href='@Model.ConfirmationLink'>clicking here</a>.</p>");
            template.AppendLine("- Developer of Cinereg");
            template.AppendLine("<p>You can ignore this email if this was not you</p>");

            var emailToSend = Email
                .From("paavo_karppinen_dev@outlook.com")
                .To(email)
                .Subject("Confirm your email")
                .UsingTemplate(template.ToString(), new { Email = email, ConfirmationLink = confirmationLink })
                .Send();

            return Task.CompletedTask;
            //return emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            _email = _config["EmailSender:Email"];
            _password = _config["EmailSender:Password"];
            System.Net.NetworkCredential credentials = new(_email, _password);
            var sender = new SmtpSender(() => new SmtpClient("smtp-mail.outlook.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credentials,
                EnableSsl = true
            });

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();
            StringBuilder template = new();
            template.AppendLine("Hey @Model.Email,");
            template.AppendLine("<p>Please reset your password by <a href='@Model.ResetLink'>clicking here</a>.</p>");
            template.AppendLine("- Developer of Cinereg");
            template.AppendLine("<p>You can ignore this email if this was not you</p>");

            var emailToSend = Email
                .From("paavo_karppinen_dev@outlook.com")
                .To(email)
                .Subject("Reset your password")
                .UsingTemplate(template.ToString(), new { Email = email, ResetLink = resetLink })
                .Send();

            return Task.CompletedTask;
            //return emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        }
        
        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            _email = _config["EmailSender:Email"];
            _password = _config["EmailSender:Password"];
            System.Net.NetworkCredential credentials = new(_email, _password);
            var sender = new SmtpSender(() => new SmtpClient("smtp-mail.outlook.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credentials,
                EnableSsl = true
            });

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();
            StringBuilder template = new();
            template.AppendLine("Hey @Model.Email,");
            template.AppendLine("<p>Please reset your password using the following code: @Model.ResetCode</p>");
            template.AppendLine("- Developer of Cinereg");
            template.AppendLine("<p>You can ignore this email if this was not you</p>");

            var emailToSend = Email
                .From("paavo_karppinen_dev@outlook.com")
                .To(email)
                .Subject("Reset your password")
                .UsingTemplate(template.ToString(), new { Email = email, ResetCode = resetCode })
                .Send();

            return Task.CompletedTask;
            //return emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
        }
            
    }
}
