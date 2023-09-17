﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailSend.Services;
public class EmailSender : IEmailSender
{
    public void SendEmail(string email, string fileName)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("", "dawn.labadie@ethereal.email"));
        message.To.Add(new MailboxAddress("", email));

        message.Subject = "Your file uploaded to blob storage";

        message.Body = new TextPart("plain")
        {
            Text = $"The file {fileName} has been successfully uploaded to the blob."
        };

        // Connect to the SMTP server and send the message
        using (var client = new SmtpClient())
        {
            client.Connect("smtp-relay.brevo.com", 587, SecureSocketOptions.Auto);
            client.Authenticate("nazarnyrka00@gmail.com", "LNBxgyrGMHTR7svh");
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
