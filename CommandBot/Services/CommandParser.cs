using CommandBot.Models;
using CommandBot.Interfaces;
using System.Text.RegularExpressions;

namespace CommandBot.Services
{
    public class CommandParser
    {
        private static readonly Regex QuestionMarkRegex = new Regex(@"\#(menu|shop|userinfo|driverlogin)", RegexOptions.IgnoreCase);
        private static readonly Regex UpdateFieldRegex = new Regex(@"\#(update email|update social|update consent):\s*(\S*)", RegexOptions.IgnoreCase);
        private static readonly Regex UpdateOrderRegex = new Regex(@"\#(update order):\s*(\S*)", RegexOptions.IgnoreCase);
        private static readonly Regex NewSightingRegex = new Regex(@"\#(new sighting):\s*(\S*)", RegexOptions.IgnoreCase);

        public static List<ICommand> ParseCommands(ConversationContext convo)
        {
            List<ICommand> commands = new List<ICommand>();

            var questionMatch = QuestionMarkRegex.Match(convo.MessageBody);
            if (questionMatch.Success)
            {
                commands.Add(new QuestionCommand(questionMatch.Groups[1].Value));
            }

            var updateFieldMatch = UpdateFieldRegex.Match(convo.MessageBody);
            if (updateFieldMatch.Success)
            {
                commands.Add(new UpdateUserInfoCommand(updateFieldMatch.Groups[1].Value, updateFieldMatch.Groups[2].Value));
            }

            var updateOrderMatch = UpdateOrderRegex.Match(convo.MessageBody);
            if (updateOrderMatch.Success)
            {
                commands.Add(new UpdateOrderCommand(updateOrderMatch.Groups[2].Value));
            }

            var newSightingMatch = NewSightingRegex.Match(convo.MessageBody);
            if (newSightingMatch.Success)
            {
                commands.Add(new NewSightingCommand(newSightingMatch.Groups[2].Value));
            }

            return commands;
        }
    }
}
