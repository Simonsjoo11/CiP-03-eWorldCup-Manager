using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    [Route("match")]
    [Produces("application/json")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly ITournamentService _service;

        public MatchController(ITournamentService service)
        {
            _service = service;
        }

        /// <summary>
        /// GET /match?playerIndex=&roundNumber=
        /// Returnerar direkt vem spelare i möter i runda d (0-baserat index).
        /// </summary>
        [HttpGet("{playerIndex:int}/{roundNumber:int}")]
        [ProducesResponseType(typeof(PlayerRoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDirectMatch([FromRoute] int playerIndex, [FromRoute] int roundNumber, CancellationToken ct = default)
        {
            try
            {
                var dto = await _service.GetPlayerInRoundAsync(playerIndex, roundNumber, ct);
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /match/remaining?participantCount=&roundsPlayed=
        /// Returnerar antal återstående unika par efter att D rundor har spelats.
        /// </summary>
        [HttpGet("remaining/{participantCount:int}/{roundsPlayed:int}")]
        [ProducesResponseType(typeof(RemainingPairsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRemainingPairs([FromRoute] int? participantCount, [FromRoute] int? roundsPlayed, CancellationToken ct = default)
        {
            try
            {
                var dto = await _service.GetRemainingPairsAsync(participantCount, roundsPlayed, ct);
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
