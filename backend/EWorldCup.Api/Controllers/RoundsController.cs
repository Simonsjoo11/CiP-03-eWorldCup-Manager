using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace EWorldCup.Api.Controllers
{
    /// <summary>Hanterar rund-relaterade API-anrop för turneringsschemat.</summary>
    [Route("rounds")]
    [Produces("application/json")]
    [ApiController]
    public class RoundsController : ControllerBase
    {
        private readonly ITournamentService _service;

        public RoundsController(ITournamentService service)
        {
            _service = service;
        }

        /// <summary>Returnerar max antal rundor för n deltagare (n−1).</summary>
        /// <param name="participantCount">Antalet deltagare (valfritt). Om inte angivet används listans längd.</param>
        /// <returns>{ ok, max } eller { ok, n, max } om du skickar in BigInteger-sträng.</returns>
        // GET /rounds/max?n=
        [HttpGet("max")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public IActionResult GetMaxRounds([FromQuery(Name = "n")] string? participantCount)
        {
            if (!string.IsNullOrWhiteSpace(participantCount))
            {
                if (!BigInteger.TryParse(participantCount, out var N))
                    return BadRequest(new { ok = false, message = "n must be an integer." });
                if (N < 2) return BadRequest(new { ok = false, message = "n must be ≥ 2." });
                if (!N.IsEven) return BadRequest(new { ok = false, message = "n must be even." });

                var maxBig = N - 1;
                return Ok(new { ok = true, n = N.ToString(), max = maxBig.ToString() });
            }

            try
            {
                var max = _service.GetMaxRounds(null);
                return Ok(new { ok = true, max });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ok = false, message = ex.Message });
            }
        }

        /// <summary>Returnerar alla matcher i runda d (1 ≤ d ≤ n−1).</summary>
        /// <param name="roundNumber">Runda (1..n−1)</param>
        // GET /rounds/:d
        [HttpGet("{roundNumber:int}")]
        [ProducesResponseType(typeof(RoundResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRound([FromRoute] int roundNumber, CancellationToken ct)
        {
            try
            {
                var response = await _service.GetRoundAsync(roundNumber, ct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
