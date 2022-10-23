namespace MSGMicroservice.IDP.Services.EmailService;

public interface IEmailSender
{
    void SendMail(string recipient, string subject,
        string body, bool isBodyHtml = false, string sender = null);
}