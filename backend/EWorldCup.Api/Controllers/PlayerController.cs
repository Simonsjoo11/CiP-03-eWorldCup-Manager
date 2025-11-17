using EWorldCup.Application.DTOs;
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
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(ITournamentQueryService tournamentService, IPlayerService playerService, ILogger<PlayerController> logger)
        {
            _tournamentService = tournamentService;
            _playerService = playerService;
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

        /// <summary>
        /// Get opponent for a specific player in a specific round (alias endpoint)
        /// </summary>
        /// <param name="playerIndex">Player index (0-based)</param>
        /// <param name="round">Round number (1 to n-1)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Match information for the player in the round</returns>
        /// <response code="200">Returns the match information</response>
        /// <response code="400">If parameters are invalid</response>
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

        /// <summary>
        /// Add a new player to the tournament
        /// </summary>
        /// <param name="request">Player creation request containing the player's name</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The created player with ID and UID</returns>
        /// <response code="201">Player created successfully</response>
        /// <response code="400">If the request is invalid</response>
        [HttpPost]
        [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlayerDto>> AddPlayer([FromBody] CreatePlayerRequest request, CancellationToken ct = default)
        {
            _logger.LogInformation("Adding new player: {PlayerName}", request.Name);

            try
            {
                var player = await _playerService.AddPlayerAsync(request.Name, ct);

                var playerDto = new PlayerDto
                {
                    Id = player.Id,
                    Uid = player.Uid,
                    Name = player.Name
                };

                _logger.LogInformation(
                    "Player added successfully: ID={PlayerId}, UID={PlayerUid}, Name={PlayerName}",
                    player.Id,
                    player.Uid,
                    player.Name);

                return CreatedAtAction(nameof(GetPlayers), null, playerDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid player name: {PlayerName}", request.Name);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding player: {PlayerName}", request.Name);
                return StatusCode(500, new { error = "An error occurred while adding the player" });
            }
        }

        /// <summary>
        /// Remove a player from the tournament by their unique identifier
        /// </summary>
        /// <param name="uid">Player's unique identifier (Guid)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Player removed successfully</response>
        /// <response code="404">Player not found</response>
        [HttpDelete("{uid:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePlayer(Guid uid, CancellationToken ct = default)
        {
            _logger.LogInformation("Removing player with UID: {PlayerUid}", uid);

            try
            {
                var success = await _playerService.RemovePlayerByUidAsync(uid, ct);

                if (!success)
                {
                    _logger.LogWarning("Player not found: {PlayerUid}", uid);
                    return NotFound(new { error = $"Player with UID {uid} not found" });
                }

                var remainingCount = await _playerService.GetCountAsync(ct);

                _logger.LogInformation("Player removed successfully: {PlayerUid}", uid);

                return Ok(new
                {
                    message = "Player removed successfully",
                    uid = uid,
                    remainingPlayerCount = remainingCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing player: {PlayerUid}", uid);
                return StatusCode(500, new { error = "An error occurred while removing the player" });
            }
        }

        /// <summary>
        /// Request model for creating a new player
        /// </summary>
        public record CreatePlayerRequest
        {
            /// <summary>
            /// Player name (required)
            /// </summary>
            public required string Name { get; init; }
        }
    }
}
