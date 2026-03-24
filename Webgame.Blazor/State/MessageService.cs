namespace Webgame.Blazor.State;

public sealed class MessageService
{
    private readonly List<UserMessage> _messages = new();

    public IReadOnlyList<UserMessage> Messages => _messages;

    public event Action? OnChange;

    public void Add(string text, string type = "info")
    {
        var message = new UserMessage
        {
            Text = text,
            Type = type
        };

        _messages.Add(message);
        OnChange?.Invoke();

        _ = RemoveAfterDelay(message);
    }

    private async Task RemoveAfterDelay(UserMessage message)
    {
        await Task.Delay(5000);

        if (_messages.Contains(message))
        {
            _messages.Remove(message);
            OnChange?.Invoke();
        }
    }
}