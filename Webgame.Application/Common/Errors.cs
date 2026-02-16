using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webgame.Application.Common;

public static class Errors
{
    public static readonly Error PlayerNotFound =
        new("player.not_found", "Player was not found.", ErrorType.NotFound);

    public static readonly Error InvalidName =
        new("player.invalid_name", "Name must be between 3 and 20 characters.", ErrorType.Validation);

    public static readonly Error InvalidAmount =
        new("game.invalid_amount", "Amount must be positive.", ErrorType.Validation);

    public static readonly Error NotEnoughCoins =
        new("game.not_enough_coins", "Not enough coins.", ErrorType.Conflict);

    public static readonly Error InvalidTop =
        new("leaderboard.invalid_top", "Parameter 'top' must be between 1 and 100.", ErrorType.Validation);

}


