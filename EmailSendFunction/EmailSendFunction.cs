using System;
using System.Collections.Generic;
using System.IO;
using EmailSendFunction.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EmailSendFunction
{
    [StorageAccount("AzureWebJobsStorage")]
    public class EmailSendFunction
    {
        private readonly IEmailSender _emailSender;

        public EmailSendFunction(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [FunctionName("EmailSendFunction")]
        public void Run([BlobTrigger("docxfiles/{name}")]
            Stream myBlob,
            IDictionary<string, string> metadata,
            string name,
            ILogger log)
        {
            string email = metadata["Email"];

            try
            {
                _emailSender.SendEmail(email, name);
            }
            catch (Exception ex)
            {
                log.LogError("Email sending fails", ex);
            }
        }
    }
}
