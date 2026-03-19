using Webgame.Contracts.Players;

namespace Webgame.Blazor.State;

public sealed class PlayerUiState
{
    public PlayerResponse? Player { get; private set; }

    public event Action? OnChange;

    public void SetPlayer(PlayerResponse? player)
    {
        Player = player;
        OnChange?.Invoke();
    }

    public void Clear()
    {
        Player = null;
        OnChange?.Invoke();
    }
}