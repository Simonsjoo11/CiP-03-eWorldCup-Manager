using EWorldCup.Application.Interfaces;
using EWorldCup.Application.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// Manages tournament rounds and matches
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RoundsController : ControllerBase
    {
        private readonly ITournamentQueryService _tournamentService;
        private readonly IPlayerService _playerService;
        private readonly IRoundSchedulingService _roundSchedulingService;
        private readonly ILogger<RoundsController> _logger;

        public RoundsController(
            ITournamentQueryService tournamentService,
            IPlayerService playerService,
            IRoundSchedulingService roundSchedulingService,
            ILogger<RoundsController> logger)
        {
            _tournamentService = tournamentService;
            _playerService = playerService;
            _roundSchedulingService = roundSchedulingService;
            _logger = logger;
        }

        /// <summary>
        /// Get all matches in a specific round
        /// </summary>
        /// <param name="roundNumber">Round number (1 to n-1</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>All matches for the specified round</returns>
        /// <response code="200">Returns the matches for the round</response>
        /// <response code="400">If round number is invalid</response>
        [HttpGet("{roundNumber:int}")]
        [ProducesResponseType(typeof(RoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoundResponse>> GetRound(int roundNumber, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching round {RoundNumber}", roundNumber);

            try
            {
                var response = await _tournamentService.GetRoundAsync(roundNumber, ct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid round number: {RoundNumber}", roundNumber);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get maximum number of rounds for the tournament
        /// </summary>
        /// <param name="n">Optional: Number of players (if not provided, uses current count)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Maximum number of rounds</returns>
        /// <response code="200">Returns the maximum rounds</response>
        /// <response code="400">If player count is invalid</response>
        [HttpGet("max")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<long>> GetMaxRounds([FromQuery] long? n = null, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching max rounds for n={N}", n);

            try
            {
                // Validate range for long
                if (n.HasValue && (n.Value < 0 || n.Value > long.MaxValue - 1))
                {
                    return BadRequest(new { error = "Player count is out of valid range" });
                }

                long playerCount = n ?? await _playerService.GetCountAsync(ct);

                // Validate player count
                if (playerCount < 2)
                {
                    return BadRequest(new { error = "Player count must be at least 2" });
                }

                if (playerCount % 2 != 0)
                {
                    return BadRequest(new { error = "Player count must be even" });
                }

                // Check for potential overflow when calculating max rounds
                if (playerCount == long.MaxValue)
                {
                    return BadRequest(new { error = "Player count is too large to calculate max rounds" });
                }

                // Calculate max rounds (n - 1)
                var maxRounds = playerCount - 1;
                return Ok(maxRounds);
            }
            catch (OverflowException ex)
            {
                _logger.LogError(ex, "Overflow calculating max rounds for n={N}", n);
                return BadRequest(new { error = "Player count is too large for calculation" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating max rounds");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
