using MAS_Shared.Data;
using MAS_Shared.MASConstants;

public class ChatUpdate
{
    public required ApplicationUser From { get; set; }
    public required string Body { get; set; }
    public required ChatChannelType Channel { get; init; }
}