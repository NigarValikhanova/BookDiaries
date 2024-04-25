using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net.Mail;
using System.Net;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace BookDiaries.Utility
{
    public class EmailService : IEmailService
    {
        //public readonly IConfiguration _configuration;
        private string? _host;
        private int _port;
        private bool _enableSSL;
        private string? _email;
        private string? _password;


        public EmailService(string? host, int port, bool enableSSL, string? email, string? password)
        {
            _host = host;
            _port = port;
            _enableSSL = enableSSL;
            _email = email;
            _password = password;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_email, _password),
                EnableSsl = _enableSSL
            };

            return client.SendMailAsync(new MailMessage(_email?? "", email, subject, message) { IsBodyHtml = true});
        }
    }

}
