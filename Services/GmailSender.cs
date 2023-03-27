using System.Text;
using Homework.Services.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

namespace Homework.Services;

public class GmailSender : IEmailSender
{
    private readonly IConfiguration configuration;

    public GmailSender([FromServices] IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task SendEmailAsync(string name, string emailAddress, string subject, string htmlMessage)
    {
        var senderEmailAddress = configuration[key: "Authentication:Gmail:EmailAddress"];
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(Encoding.UTF8, name: "Web Shop", senderEmailAddress));
        message.To.Add(new MailboxAddress(name, emailAddress));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlMessage
        };

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(host: "smtp.gmail.com", port: 587, SecureSocketOptions.StartTls);
        var senderPassword = configuration[key: "Authentication:Gmail:AppPassword"];
        await smtpClient.AuthenticateAsync(Encoding.UTF8, senderEmailAddress, senderPassword);
        await smtpClient.SendAsync(FormatOptions.Default, message);
        await smtpClient.DisconnectAsync(quit: true);
    }
}