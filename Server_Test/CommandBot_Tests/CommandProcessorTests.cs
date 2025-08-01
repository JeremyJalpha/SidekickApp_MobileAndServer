using CommandBot.Interfaces;
using CommandBot.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MAS_Shared.Data;
using MAS_Shared.Models;

namespace SidekickApp_Test.CommandProcessing
{
    public class CommandProcessorTests
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        [Fact]
        public async Task ProcessCommandsAsync_ShouldReturnEmptyMessage_WhenNoCommandsProvided()
        {
            var dbMock = new Mock<AppDbContext>();
            var convoMock = new Mock<ConversationContext>();
            var busiMock = new Mock<BusinessContext>();
            var jwtMock = new Mock<JwtIssueConfig>();
            var loggerMock = new Mock<ILogger<CommandContext>>();

            var context = new CommandContext(dbMock.Object, convoMock.Object, busiMock.Object, jwtMock.Object, loggerMock.Object);
            var processor = new CommandProcessor(new List<ICommand>());

            // Act
            var result = await processor.ProcessCommandsAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            var dispatch = result[0];
            dispatch.ChatUpdate.Body.Should().Be("No commands to process.");
            dispatch.ChatUpdate.From.Should().Be(context.ConvoContext.User);
            dispatch.ChatUpdate.Channel.Should().Be(context.ConvoContext.Channel);

            dispatch.Tags.Should().ContainKey("status");
            dispatch.Tags["status"].Should().Be("noop");

            dispatch.BusinessID.Should().BeNull();
            dispatch.CorrelationId.Should().NotBeEmpty();

            loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ProcessCommandsAsync_ShouldExecuteSingleCommandCorrectly()
        {
            // Arrange
            var dbMock = new Mock<AppDbContext>();
            var convoMock = new Mock<ConversationContext>();
            var busiMock = new Mock<BusinessContext>();
            var jwtMock = new Mock<JwtIssueConfig>();
            var loggerMock = new Mock<ILogger<CommandContext>>();

            var context = new CommandContext(dbMock.Object, convoMock.Object, busiMock.Object, jwtMock.Object, loggerMock.Object);

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(c => c.ExecuteAsync(context)).ReturnsAsync("Success");

            var processor = new CommandProcessor(new List<ICommand> { commandMock.Object });

            // Act
            var result = await processor.ProcessCommandsAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);

            var dispatch = result[0];
            dispatch.ChatUpdate.Body.Should().Be("Success");
            dispatch.ChatUpdate.From.Should().Be(context.ConvoContext.User);
            dispatch.ChatUpdate.Channel.Should().Be(context.ConvoContext.Channel);

            dispatch.Tags.Should().ContainKey("command");
            dispatch.Tags["command"].Should().Be("mockcommand");
            dispatch.Tags.Should().ContainKey("status");
            dispatch.Tags["status"].Should().Be("ok");

            dispatch.CorrelationId.Should().NotBeEmpty();
            dispatch.BusinessID.Should().BeNull();

            commandMock.Verify(c => c.ExecuteAsync(context), Times.Once);
            loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ProcessCommandsAsync_ShouldExecuteMultipleCommandsCorrectly()
        {
            // Arrange
            var dbMock = new Mock<AppDbContext>();
            var convoMock = new Mock<ConversationContext>();
            var busiMock = new Mock<BusinessContext>();
            var jwtMock = new Mock<JwtIssueConfig>();
            var loggerMock = new Mock<ILogger<CommandContext>>();

            var context = new CommandContext(dbMock.Object, convoMock.Object, busiMock.Object, jwtMock.Object, loggerMock.Object);

            var commandMock1 = new Mock<ICommand>();
            commandMock1.Setup(c => c.ExecuteAsync(context)).ReturnsAsync("Command1 executed");

            var commandMock2 = new Mock<ICommand>();
            commandMock2.Setup(c => c.ExecuteAsync(context)).ReturnsAsync("Command2 executed");

            var processor = new CommandProcessor(new List<ICommand> { commandMock1.Object, commandMock2.Object });

            // Act
            var result = await processor.ProcessCommandsAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);

            result[0].ChatUpdate.Body.Should().Be("Command1 executed");
            result[1].ChatUpdate.Body.Should().Be("Command2 executed");

            result[0].Tags.Should().ContainKey("command");
            result[0].Tags["command"].Should().Be("mockcommand1");
            result[1].Tags.Should().ContainKey("command");
            result[1].Tags["command"].Should().Be("mockcommand2");

            result[0].Tags.Should().ContainKey("status");
            result[0].Tags["status"].Should().Be("ok");
            result[1].Tags.Should().ContainKey("status");
            result[1].Tags["status"].Should().Be("ok");

            result[0].ChatUpdate.From.Should().Be(context.ConvoContext.User);
            result[1].ChatUpdate.From.Should().Be(context.ConvoContext.User);
            result[0].ChatUpdate.Channel.Should().Be(context.ConvoContext.Channel);
            result[1].ChatUpdate.Channel.Should().Be(context.ConvoContext.Channel);

            commandMock1.Verify(c => c.ExecuteAsync(context), Times.Once);
            commandMock2.Verify(c => c.ExecuteAsync(context), Times.Once);
            loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ProcessCommandsAsync_ShouldHandleCommandFailureGracefully()
        {
            // Arrange
            var user = new ApplicationUser
            {
                CellNumber = "1234567890"
            };

            var convoMock = new ConversationContext(user, true, "1234567890", "Test message", MAS_Shared.MASConstants.ChatChannelType.WhatsApp);
            var busiMock = new BusinessContext(123, "sidekickapp.xyz", BusinessContextFactory.GetCommandMenu, MAS_Shared.MASConstants.ChatChannelType.WhatsApp);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new AppDbContext(options);

            var jwtMock = new Mock<JwtIssueConfig>();
            var loggerMock = new Mock<ILogger<CommandContext>>();

            var context = new CommandContext(dbContext, convoMock, busiMock, jwtMock.Object, loggerMock.Object);

            var failingCommandMock = new Mock<ICommand>();
            failingCommandMock
                .Setup(c => c.ExecuteAsync(context))
                .ThrowsAsync(new Exception("Database error"));

            var successfulCommandMock = new Mock<ICommand>();
            successfulCommandMock
                .Setup(c => c.ExecuteAsync(context))
                .ReturnsAsync("Command succeeded");

            var processor = new CommandProcessor(new List<ICommand> { failingCommandMock.Object, successfulCommandMock.Object });

            // Act
            var result = await processor.ProcessCommandsAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);

            result[0].ChatUpdate.Body.Should().Be("Command succeeded");
            result[1].ChatUpdate.Body.Should().Be("An unexpected error occurred while processing your request.");

            result[0].Tags.Should().ContainKey("command");
            result[0].Tags["command"].Should().Be("mockcommand2");

            result[1].Tags.Should().ContainKey("error");
            result[1].Tags["error"].Should().Be("true");

            result[0].Tags.Should().ContainKey("status");
            result[0].Tags["status"].Should().Be("ok");

            result[1].Tags.Should().ContainKey("status");
            result[1].Tags["status"].Should().Be("error");

            result[0].ChatUpdate.From.Should().Be(context.ConvoContext.User);
            result[1].ChatUpdate.From.Should().Be(context.ConvoContext.User);

            result[0].ChatUpdate.Channel.Should().Be(context.ConvoContext.Channel);
            result[1].ChatUpdate.Channel.Should().Be(context.ConvoContext.Channel);

            failingCommandMock.Verify(c => c.ExecuteAsync(context), Times.Once);
            successfulCommandMock.Verify(c => c.ExecuteAsync(context), Times.Once);

            loggerMock.Verify(l => l.LogError(It.IsAny<Exception>(), "Failed to execute command mockcommand1"), Times.Once);
            loggerMock.VerifyNoOtherCalls();
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
