using Microsoft.EntityFrameworkCore;
using MAS_Shared.Data;

namespace CommandBot.Models
{
    public class ConversationContextFactory
    {
        private readonly ILogger<ConversationContextFactory> _logger;

        public ConversationContextFactory(ILogger<ConversationContextFactory> logger)
        {
            _logger = logger;
        }

        public ConversationContext CreateConversationContext(AppDbContext dbContext, ChatUpdate chatUpdate)
        {
            // Validate cell number is provided and numeric.
            if (string.IsNullOrWhiteSpace(chatUpdate.From.CellNumber))
            {
                throw new ArgumentException("Cell number cannot be null or empty.", nameof(chatUpdate.From.CellNumber));
            }
            if (!int.TryParse(chatUpdate.From.CellNumber, out int parsedCellNumber))
            {
                throw new ArgumentException("Cell number must be a valid integer.", nameof(chatUpdate.From.CellNumber));
            }
            // Validate message body is provided.
            if (string.IsNullOrWhiteSpace(chatUpdate.Body))
            {
                throw new ArgumentException("Message body cannot be null or empty.", nameof(chatUpdate.Body));
            }

            var user = dbContext.Users
                .FirstOrDefaultAsync(u => u.CellNumber == chatUpdate.From.CellNumber)
                .Result;

            if (user != null)
            {
                return new ConversationContext(user, true, chatUpdate.From.CellNumber, chatUpdate.Body, chatUpdate.Channel);
            }
            else
            {
                _logger.LogInformation($"User with phone number {chatUpdate.From.CellNumber} does not exist. Creating a new user.");
                dbContext.Users.Add(chatUpdate.From);
                dbContext.SaveChangesAsync().Wait();

                return new ConversationContext(chatUpdate.From, false, chatUpdate.From.CellNumber, chatUpdate.Body, chatUpdate.Channel);
            }
        }
    }
}
