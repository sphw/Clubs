using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Clubs.Services
{
    public class EmailSender: IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Sending email: {htmlMessage}");
        }
    }
}