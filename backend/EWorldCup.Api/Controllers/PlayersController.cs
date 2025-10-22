using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    [Route("player")]
    [Produces("application/json")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly ITournamentService _service;
        
        public PlayersController(ITournamentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returnerar hela schemat för en spelare
        /// </summary>
        /// <param name="playerIndex">Spelarindex</param>
        [HttpGet("{playerIndex:int}/schedule")]
        [ProducesResponseType(typeof(PlayerScheduleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSchedule([FromRoute] int playerIndex, CancellationToken ct)
        {
            try 
            {
                return Ok(await _service.GetPlayerScheduleAsync(playerIndex, ct));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ok = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Returnerar vem spelaren möter i en viss runda.
        /// </summary>
        /// <param name="playerIndex">Spelar index</param>
        /// <param name="round">Rund numemr</param>
        [HttpGet("{playerIndex:int}/round/{round:int}")]
        [ProducesResponseType(typeof(PlayerRoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPlayerInRound([FromRoute] int playerIndex, [FromRoute] int round, CancellationToken ct)
        {
            try
            {
                return Ok(await _service.GetPlayerInRoundAsync(playerIndex, round, ct));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ok = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { ok = false, message = ex.Message });
            }
        }
    }
}
