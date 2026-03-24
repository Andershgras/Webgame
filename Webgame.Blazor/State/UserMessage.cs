namespace Webgame.Blazor.State;

public sealed class UserMessage
{
    public string Text { get; set; } = "";
    public string Type { get; set; } = "info"; // success, warning, error
}