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
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("{i:int}/schedule")]
        [ProducesResponseType(typeof(PlayerScheduleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSchedule([FromRoute] int i, CancellationToken ct)
        {
            try 
            {
                return Ok(await _service.GetPlayerScheduleAsync(i, ct));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ok = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="d"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("{i:int}/round/{d:int}")]
        [ProducesResponseType(typeof(PlayerRoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPlayerInRound([FromRoute] int i, [FromRoute] int d, CancellationToken ct)
        {
            try
            {
                return Ok(await _service.GetPlayerInRoundAsync(i, d, ct));
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
