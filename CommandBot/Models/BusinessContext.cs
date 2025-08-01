using MAS_Shared.DBModels;
using MAS_Shared.MASConstants;

namespace CommandBot.Models
{
    public class BusinessContext
    {
        public int CellNumber { get; }
        public string BaseUrl { get; init; } = string.Empty;
        public Business? Business { get; init; }
        public string CommandMenu { get; init; } = "#MainMenu#";
        public string Catalog { get; init; } = string.Empty;
        public string PrclstPreamble { get; init; } = string.Empty;
        public IReadOnlyList<CatalogItem> CatalogItems { get; init; } = Array.Empty<CatalogItem>();
        public ChatChannelType Channel { get; init; } = ChatChannelType.WhatsApp;

        public BusinessContext(int cellNumber, string baseurl, Func<string> commandMenuProvider, ChatChannelType chatChannelType)
        {
            CellNumber = cellNumber;
            BaseUrl = baseurl;
            CommandMenu = commandMenuProvider();
            Channel = chatChannelType;
        }
        public string PrclstAsAString() => string.Join('\n', CatalogItems);
    }
}
