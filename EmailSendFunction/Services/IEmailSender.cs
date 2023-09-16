namespace EmailSend.Services;
public interface IEmailSender
{
    void SendEmail(string email, string fileName);
}
