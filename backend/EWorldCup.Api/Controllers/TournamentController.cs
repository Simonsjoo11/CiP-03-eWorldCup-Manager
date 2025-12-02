using EWorldCup.Application.Interfaces;
using EWorldCup.Application.Responses;
using EWorldCup.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// Manages Rock-Paper-Scissors tournament operations
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly ILogger<TournamentController> _logger;

        public TournamentController(
            ITournamentService tournamentService,
            ILogger<TournamentController> logger)
        {
            _tournamentService = tournamentService;
            _logger = logger;
        }

        /// <summary>
        /// Start a new tournament with specified player name and number of participants
        /// </summary>
        /// <param name="request">Tournament start request containing player name and total players</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Tournament start response with initial state and first opponent</returns>
        /// <response code="200">Tournament started successfully</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="500">Server error</response>
        [HttpPost("start")]
        [ProducesResponseType(typeof(TournamentStartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TournamentStartResponse>> StartTournament(
            [FromBody] TournamentStartRequest request,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Starting tournament: Player={PlayerName}, TotalPlayers={TotalPlayers}",
                request.PlayerName, request.TotalPlayers);

            try
            {
                var response = await _tournamentService.StartTournamentAsync(
                    request.PlayerName,
                    request.TotalPlayers,
                    ct);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid tournament start request: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot start tournament: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting tournament");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An error occurred while starting the tournament" });
            }
        }

        /// <summary>
        /// Get the current status of a tournament including scoreboard and current match
        /// </summary>
        /// <param name="tournamentId">Tournament ID</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Current tournament status</returns>
        /// <response code="200">Returns tournament status</response>
        /// <response code="404">Tournament not found</response>
        [HttpGet("{tournamentId:int}/status")]
        [ProducesResponseType(typeof(TournamentStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TournamentStatusResponse>> GetStatus(
            int tournamentId,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Getting status for tournament {TournamentId}", tournamentId);

            try
            {
                var response = await _tournamentService.GetStatusAsync(tournamentId, ct);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Tournament {TournamentId} not found", tournamentId);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tournament status");
                return StatusCode(500, new { error = "An error occurred while retrieving tournament status" });
            }
        }

        /// <summary>
        /// Play a round in the current match by submitting the player's choice
        /// </summary>
        /// <param name="tournamentId">Tournament ID</param>
        /// <param name="request">Player's RPS choice</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Result of the game round</returns>
        /// <response code="200">Round played successfully</response>
        /// <response code="400">Invalid request or game state</response>
        /// <response code="404">Tournament not found</response>
        [HttpPost("{tournamentId:int}/play")]
        [ProducesResponseType(typeof(PlayRoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlayRoundResponse>> PlayRound(
            int tournamentId,
            [FromBody] PlayRoundRequest request,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Playing round in tournament {TournamentId}: {Choice}",
                tournamentId, request.Choice);

            try
            {
                var response = await _tournamentService.PlayRoundAsync(
                    tournamentId,
                    request.Choice,
                    ct);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot play round in tournament {TournamentId}", tournamentId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error playing round");
                return StatusCode(500, new { error = "An error occurred while playing the round" });
            }
        }

        /// <summary>
        /// Advance to the next round by simulating all AI vs AI matches
        /// </summary>
        /// <param name="tournamentId">Tournament ID</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Result of advancing to next round</returns>
        /// <response code="200">Round advanced successfully</response>
        /// <response code="400">Cannot advance round (e.g., player match not complete)</response>
        /// <response code="404">Tournament not found</response>
        [HttpPost("{tournamentId:int}/advance")]
        [ProducesResponseType(typeof(AdvanceRoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdvanceRoundResponse>> AdvanceRound(
            int tournamentId,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Advancing round in tournament {TournamentId}", tournamentId);

            try
            {
                var response = await _tournamentService.AdvanceRoundAsync(tournamentId, ct);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot advance round in tournament {TournamentId}", tournamentId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error advancing round");
                return StatusCode(500, new { error = "An error occurred while advancing the round" });
            }
        }

        /// <summary>
        /// Get final tournament results including winner and full scoreboard
        /// </summary>
        /// <param name="tournamentId">Tournament ID</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Final tournament results</returns>
        /// <response code="200">Returns final results</response>
        /// <response code="400">Tournament not yet completed</response>
        /// <response code="404">Tournament not found</response>
        [HttpGet("{tournamentId:int}/final")]
        [ProducesResponseType(typeof(FinalResultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinalResultResponse>> GetFinalResult(
            int tournamentId,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Getting final results for tournament {TournamentId}", tournamentId);

            try
            {
                var response = await _tournamentService.GetFinalResultAsync(tournamentId, ct);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot get final results for tournament {TournamentId}", tournamentId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting final results");
                return StatusCode(500, new { error = "An error occurred while retrieving final results" });
            }
        }
    }

    /// <summary>
    /// Request to start a new tournament
    /// </summary>
    public record TournamentStartRequest
    {
        /// <summary>
        /// Name of the human player
        /// </summary>
        public required string PlayerName { get; init; }

        /// <summary>
        /// Total number of players in the tournament (must be even, minimum 2)
        /// </summary>
        public required int TotalPlayers { get; init; }
    }

    /// <summary>
    /// Request to play a round with the player's choice
    /// </summary>
    public record PlayRoundRequest
    {
        /// <summary>
        /// Player's Rock-Paper-Scissors choice
        /// </summary>
        public required RpsChoice Choice { get; init; }
    }
}