using EWorldCup.Application.Interfaces;
using EWorldCup.Application.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// Handles match-related queries
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class MatchController : ControllerBase
    {
        private readonly ITournamentQueryService _tournamentService;
        private readonly ILogger<MatchController> _logger;

        public MatchController(
            ITournamentQueryService tournamentService,
            ILogger<MatchController> logger)
        {
            _tournamentService = tournamentService;
            _logger = logger;
        }

        /// <summary>
        /// Get opponent for a specific player in a specific round
        /// </summary>
        /// <param name="playerIndex">Player index (0-based)</param>
        /// <param name="roundNumber">Round number (1 to n-1)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Match information for the player in the round</returns>
        /// <response code="200">Returns the match information</response>
        /// <response code="400">If parameters are invalid</response>
        [HttpGet("{playerIndex:int}/{roundNumber:int}")]
        [ProducesResponseType(typeof(PlayerRoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlayerRoundResponse>> GetMatch(
            int playerIndex,
            int roundNumber,
            CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Fetching match for player {PlayerIndex} in round {RoundNumber}",
                playerIndex,
                roundNumber);

            try
            {
                var response = await _tournamentService.GetPlayerInRoundAsync(playerIndex, roundNumber, ct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid parameters: playerIndex={PlayerIndex}, round={Round}",
                    playerIndex, roundNumber);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error getting match for player {PlayerIndex} in round {Round}",
                    playerIndex, roundNumber);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Calculate remaining unique pairs after D rounds have been played
        /// </summary>
        /// <param name="playerCount">Number of players (must be even)</param>
        /// <param name="roundsPlayed">Number of rounds already played</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Number of remaining unique pairs</returns>
        /// <response code="200">Returns remaining pairs count</response>
        /// <response code="400">If parameters are invalid</response>
        [HttpGet("remaining/{playerCount:int}/{roundsPlayed:int}")]
        [ProducesResponseType(typeof(RemainingPairsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RemainingPairsResponse>> GetRemainingPairs(
            int playerCount,
            int roundsPlayed,
            CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Calculating remaining pairs for n={N}, D={D}",
                playerCount,
                roundsPlayed);

            try
            {
                var response = await _tournamentService.GetRemainingPairsAsync(playerCount, roundsPlayed, ct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid parameters: n={N}, D={D}", playerCount, roundsPlayed);
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
