using Microsoft.Extensions.Logging;
using Moq;

public static class LoggerVerifier
{
    public static void VerifyErrorLogged<T>(Mock<ILogger<T>> loggerMock, string expectedMessage, Times? times = null)
    {
        times ??= Times.Once();

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v != null &&
                    v.ToString() != null &&
                    v.ToString()!.Contains(expectedMessage)
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            (Times)times
        );
    }
}