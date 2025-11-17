using EWorldCup.Application.Interfaces;
using EWorldCup.Application.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// Manages players and player-specific queries
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PlayerController : ControllerBase
    {
        private readonly ITournamentQueryService _tournamentService;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(ITournamentQueryService tournamentService, ILogger<PlayerController> logger)
        {
            _tournamentService = tournamentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tournament players
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of all players with player count and max rounds</returns>
        /// <response code="200">Returns the list of players</response>
        [HttpGet]
        [ProducesResponseType(typeof(PlayersResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlayersResponse>> GetPlayers(CancellationToken ct = default)
        {
            _logger.LogInformation("Getting all players");
            var response = await _tournamentService.GetPlayerAsync(ct);
            return Ok(response);
        }

        /// <summary>
        /// Get complete schedule for a specific player
        /// </summary>
        /// <param name="playerIndex">Player index</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Complete schedule showing all opponents across all rounds</returns>
        /// <response code="200">Returns the player's schedule</response>
        /// <response code="400">If player index is invalid</response>
        [HttpGet("{playerIndex:int}/schedule")]
        [ProducesResponseType(typeof(PlayerScheduleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlayerScheduleResponse>> GetPlayerSchedule(int playerIndex, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching schedule for player {PlayerIndex}", playerIndex);

            try
            {
                var response = await _tournamentService.GetPlayerScheduleAsync(playerIndex, ct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid player index: {PlayerIndex}", playerIndex);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{playerIndex:int}/{round:int}")]
        [ProducesResponseType(typeof(PlayerRoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlayerRoundResponse>> GetPlayerRound(int playerIndex, int round, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching round {Round} for player {PlayerIndex}", round, playerIndex);

            try
            {
                var response = await _tournamentService.GetPlayerInRoundAsync(playerIndex, round, ct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid parameters: playerIndex={PlayerIndex}, round={Round}",
                    playerIndex, round);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error getting player {PlayerIndex} in round {Round}",
                    playerIndex, round);
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
