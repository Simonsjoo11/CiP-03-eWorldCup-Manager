using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Repositories;
using EWorldCup.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// Hanterar API-anrop relaterade till deltagare (spelare i turneringen).
    /// </summary>
    [Route("participants")]
    [Produces("application/json")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly ITournamentService _service;

        public ParticipantsController(ITournamentService service)
        {
            _service = service;
        }

        /// <summary>Returnerar alla deltagare i turneringen.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ParticipantsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ParticipantsResponse>> GetAll(CancellationToken ct)
        {
            var res = await _service.GetParticipantsAsync(ct);
            return Ok(res);
        }
    }
}
