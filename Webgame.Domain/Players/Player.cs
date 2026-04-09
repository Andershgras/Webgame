using System;
using Webgame.Domain.Common;

namespace Webgame.Domain.Players;

public sealed class Player : Entity<PlayerId>
{
    private Player() : base() { } // EF Core

    public string Name { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Stats Stats { get; private set; } = new(0);

    public Player(PlayerId id, string name, string passwordHash) : base(id)
    {
        Name = ValidateName(name);
        PasswordHash = ValidatePasswordHash(passwordHash);
        Stats = new Stats(0);
    }

    public static bool TryCreate(string name, string passwordHash, out Player? player)
    {
        name = (name ?? "").Trim();
        passwordHash = (passwordHash ?? "").Trim();

        if (name.Length is < 3 or > 20 || string.IsNullOrWhiteSpace(passwordHash))
        {
            player = null;
            return false;
        }

        player = new Player(PlayerId.New(), name, passwordHash);
        return true;
    }

    public bool TryRename(string newName)
    {
        newName = (newName ?? "").Trim();
        if (newName.Length is < 3 or > 20)
            return false;

        Name = newName;
        return true;
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        PasswordHash = ValidatePasswordHash(newPasswordHash);
    }

    public void SetCurrency(int currency)
    {
        Stats.SetCurrency(currency);
    }

    public void UnlockFirstGame()
    {
        Stats.UnlockFirstGame();
    }

    private static string ValidateName(string name)
    {
        name = (name ?? "").Trim();

        if (name.Length is < 3 or > 20)
            throw new ArgumentException("Name must be between 3 and 20 characters.", nameof(name));

        return name;
    }

    private static string ValidatePasswordHash(string passwordHash)
    {
        passwordHash = (passwordHash ?? "").Trim();

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        return passwordHash;
    }
}
