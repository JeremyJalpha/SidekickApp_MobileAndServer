using CommandBot.Helpers;
using CommandBot.Interfaces;
using MAS_Shared.Data;

namespace CommandBot.Models
{
    public abstract class BaseCommand : ICommand
    {
        public string CommandKey { get; }

        protected BaseCommand(string commandKey)
        {
            CommandKey = commandKey;
        }
        public abstract Task<string> ExecuteAsync(CommandContext context);
    }

    public class QuestionCommand : BaseCommand
    {

        public QuestionCommand(string commandKey) : base(commandKey)
        {
        }

        public override async Task<string> ExecuteAsync(CommandContext context)
        {
            if (context == null || context.ConvoContext == null || context.BusiContext == null)
                throw new ArgumentNullException("Required context is missing.");

            switch (CommandKey.ToLowerInvariant())
            {
                case "menu":
                    return context.BusiContext.CommandMenu;
                case "shop":
                    return !string.IsNullOrWhiteSpace(context.BusiContext.PrclstAsAString())
                        ? $"{context.BusiContext.PrclstPreamble}\n\n{context.BusiContext.CatalogItems}"
                        : "Shop information unavailable.";
                case "userinfo":
                    return context.ConvoContext.User?.GetUserInfoAsAString() ?? "No user info found.";
                case "driver login":
                    return await CBJwtHelper.BeginDriverLoginAsync(
                        context.AppDbContext,
                        context.ConvoContext.MsgOriginNumber,
                        context.BusiContext.BaseUrl,
                        context.JwtConfig.Issuer,
                        context.JwtConfig.Key,
                        context.JwtConfig.Audiences,
                        context.JwtConfig.TokenLifetimeMinutes
                    ) ?? "Driver login failed.";
                default:
                    return $"Unknown question command: {CommandKey}";
            }
        }
    }

    public class UpdateUserInfoCommand : BaseCommand
    {
        private readonly string _newValue;

        public UpdateUserInfoCommand(string commandKey, string newValue)
            : base(commandKey)
        {
            _newValue = newValue;
        }

        public override async Task<string> ExecuteAsync(CommandContext comContext)
        {
            // Get the user's cell number from the conversation context
            var cellNumber = comContext.ConvoContext.User?.CellNumber;
            if (string.IsNullOrEmpty(cellNumber))
                return "Cell number is empty.";

            try
            {
                await UpdateSingularUserInfoFieldAsync(comContext, cellNumber, CommandKey, _newValue);
                return $"User info field: {CommandKey} updated to: {_newValue}";
            }
            catch (Exception ex)
            {
                comContext.Logger.LogError(ex, "Failed to update user info field: {CommandKey}", CommandKey);
                return $"Failed to update user info field: {CommandKey}, with Error: {ex.Message}";
            }
        }

        private async Task UpdateSingularUserInfoFieldAsync(CommandContext comContext, string cellNumber, string updateCol, object newValue)
        {
            if (comContext?.ConvoContext?.User == null)
                throw new ArgumentNullException("Command context or conversation context is missing.");

            if (string.IsNullOrEmpty(cellNumber) || string.IsNullOrEmpty(updateCol) || newValue == null)
                throw new ArgumentException("Cell number, update column, or new value is empty.");

            var property = typeof(ApplicationUser).GetProperty(updateCol);
            if (property == null || !property.CanWrite)
                throw new Exception($"Could not find or write to column {updateCol}");

            property.SetValue(comContext.ConvoContext.User, Convert.ChangeType(newValue, property.PropertyType));
            await comContext.AppDbContext.SaveChangesAsync();
        }
    }

    public class UpdateOrderCommand : BaseCommand
    {
        const string UpdateOrderCommandKey = "update order";

        private readonly int _itemNum;
        private readonly int _itemAmount;

        public UpdateOrderCommand(string ItemNumAndNewAmount)
            : base(UpdateOrderCommandKey)
        {
            // Split the input string into item number and new amount on ":"
            var parts = ItemNumAndNewAmount.Split(':', 2);
            if (parts.Length != 2)
                throw new ArgumentException("ItemNumAndNewAmount must be in the format 'itemNum:newAmount'.", nameof(ItemNumAndNewAmount));
            if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                throw new ArgumentException("ItemNumAndNewAmount cannot be empty.", nameof(ItemNumAndNewAmount));

            var itemNum = parts[0].Trim();
            var newAmount = parts[1].Trim();

            if (!int.TryParse(itemNum, out _itemNum))
                throw new ArgumentException("itemNum must be an integer.", nameof(itemNum));
            if (!int.TryParse(newAmount, out _itemAmount))
                throw new ArgumentException("newAmount must be an integer.", nameof(newAmount));
        }

        public override Task<string> ExecuteAsync(CommandContext comContext)
        {
            return Task.FromResult("UpdateOrderCommand execution is not yet implemented.");
        }
    }

    public class NewSightingCommand : BaseCommand
    {
        const string NewSightingCommandKey = "new sighting";

        private readonly string _sightingDetails;

        public NewSightingCommand(string sightingDetails)
            : base(NewSightingCommandKey)
        {
            _sightingDetails = sightingDetails;
        }

        public override Task<string> ExecuteAsync(CommandContext comContext)
        {
            return Task.FromResult("UpdateOrderCommand execution is not yet implemented.");
        }
    }
}