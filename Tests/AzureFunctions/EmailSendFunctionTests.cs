using EmailSend;
using EmailSend.Services;
using Moq;
using Xunit;

namespace Tests.AzureFunctions;
public class EmailSendFunctionTests
{
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly EmailSenderFunction _emailSendFunction;

    public EmailSendFunctionTests()
    {
        #region Dependencies
        _mockEmailSender = new Mock<IEmailSender>();
        #endregion

        #region SUT
        _emailSendFunction = new EmailSenderFunction(_mockEmailSender.Object);
        #endregion
    }

    [Fact]
    public void Run_ShouldSendEmail()
    {
        // Arrange
        var email = "test@example.com";
        var metadata = new Dictionary<string, string>
        {
            { "Email", email }
        };

        // Act
        _emailSendFunction.Run(new MemoryStream(), metadata, "test.docx", null);

        // Assert
        _mockEmailSender.Verify(sender => sender.SendEmail(email, "test.docx"), Times.Once);
    }
}
