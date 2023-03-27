namespace Homework.Services.Abstractions;

public interface IEmailSender
{
    public Task SendEmailAsync(string name, string emailAddress, string subject, string htmlMessage);
}