using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

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
        public RoundsController(IRoundRepository roundRepository)
        {
            _roundRepository = roundRepository;
        }

        /// <summary>
        /// Returnerar det maximala antalet rundor för angivet antal deltagare.
        /// </summary>
        /// <param name="n">Antalet deltagare (valfritt). Om inte angivet används listans längd.</param>
        /// <returns>Objekt med { ok, n, max } där max = n - 1.</returns>
        /// <response code="200">Beräkning lyckades.</response>
        /// <response code="400">Felaktig indata (t.ex. udda eller för litet n).</response>
        [HttpGet("max")]
        public IActionResult GetMaxRounds([FromQuery] string? n)
        {
            if (string.IsNullOrWhiteSpace(n))
            {
                try
                {
                    var maxInt = _roundRepository.GetMaxRounds(null); // validates even & ≥ 2
                    var nInt = maxInt + 1;

                    return Ok(new
                    {
                        ok = true,
                        n = nInt.ToString(),
                        max = maxInt.ToString()
                    });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { ok = false, message = ex.Message });
                }
            }

            // If n is provided: parse as BigInteger and compute max = n - 1 with no size limit
            if (!BigInteger.TryParse(n, out var N))
                return BadRequest(new { ok = false, message = "n must be an integer." });
            if (N < 2)
                return BadRequest(new { ok = false, message = "n must be ≥ 2." });
            if (!N.IsEven)
                return BadRequest(new { ok = false, message = "n must be even." });

            var maxBig = N - 1;
            return Ok(new
            {
                ok = true,
                n = N.ToString(),
                max = maxBig.ToString()
            });
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
