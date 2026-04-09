namespace Webgame.Application.Common;

public static class Errors
{
    public static readonly Error PlayerNotFound =
        new("player.not_found", "Player was not found.", ErrorType.NotFound);

    public static readonly Error InvalidName =
        new("player.invalid_name", "Name must be between 3 and 20 characters.", ErrorType.Validation);

    public static readonly Error PlayerNameAlreadyExists =
        new("player.name_already_exists", "A player with this name already exists.", ErrorType.Conflict);

    public static readonly Error InvalidPassword =
        new("player.invalid_password", "Password must be at least 6 characters.", ErrorType.Validation);

    public static readonly Error InvalidCredentials =
        new("player.invalid_credentials", "Invalid username or password.", ErrorType.Validation);
}
