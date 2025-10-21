using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// Hanterar rund-relaterade API-anrop för turneringsschemat.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoundsController : ControllerBase
    {
        private readonly IRoundRepository _roundRepository;

        /// <summary>
        /// Skapar en ny instans av <see cref="RoundsController"/>
        /// </summary>
        public RoundsController()
        {
            _roundRepository = new InMemoryRoundRepository();
        }

        /// <summary>
        /// Returnerar det maximala antalet rundor för angivet antal deltagare.
        /// </summary>
        /// <param name="n">Antalet deltagare (valfritt). Om inte angivet används listans längd.</param>
        /// <returns>Objekt med { ok, n, max } där max = n - 1.</returns>
        /// <response code="200">Beräkning lyckades.</response>
        /// <response code="400">Felaktig indata (t.ex. udda eller för litet n).</response>
        [HttpGet("max")]
        public IActionResult GetMaxRounds([FromQuery] int? n)
        {
            try
            {
                var max = _roundRepository.GetMaxRounds(n);
                return Ok(new { ok = true, n = n ?? _roundRepository.GetMaxRounds(), max });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {ok = false, message = ex.Message});
            }
        }

        /// <summary>
        /// Returnerar alla matcher för en specifik runda i turneringen.
        /// </summary>
        /// <param name="round">Rundnummer (1 ≤ d ≤ n−1).</param>
        /// <param name="n">Antalet deltagare (valfritt). Om inte angivet används listans längd.</param>
        /// <returns>En lista med par (home, away) för angiven runda.</returns>
        /// <response code="200">Rundan genererades korrekt.</response>
        /// <response code="400">Felaktig runda eller deltagarantal.</response>
        [HttpGet("{round:int}")]
        public IActionResult GetRound(int round, [FromQuery] int? n)
        {
            try
            {
                var pairs = _roundRepository.GetRoundPairs(round, n);
                var response = new RoundResponse
                {
                    Round = round,
                    Pairs = pairs
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {ok = false, message = ex.Message});
            }
        }
    }
}
