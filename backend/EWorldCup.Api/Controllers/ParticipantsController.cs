using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// Hanterar API-anrop relaterade till deltagare (spelare i turneringen).
    /// </summary>
    [Route("participants")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantRepository _repository;

        /// <summary>
        /// Skapar en ny instans av <see cref="ParticipantsController"/>.
        /// </summary>
        public ParticipantsController(IParticipantRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returnerar alla deltagare i turneringen.
        /// </summary>
        /// <remarks>
        /// Används för att fylla dropdown-menyer eller listor i frontend (t.ex. "Välj spelare").
        /// Data hämtas från en lokal JSON-fil i mappen <c>Data/participants.json</c>.
        /// </remarks>
        /// <param name="ct">Avbrytningstoken (frivillig, används för att avbryta långa anrop).</param>
        /// <returns>En lista med alla deltagare i turneringen.</returns>
        /// <response code="200">Lyckades hämta alla deltagare.</response>
        [HttpGet]
        public async Task<ActionResult<ParticipantsResponse>> GetAll(CancellationToken ct)
        {
            var list = await _repository.GetAllAsync(ct);
            var dtos = list.Select(p => new ParticipantDto(p.Id, p.Name)).ToList();
            return Ok(new ParticipantsResponse(dtos));
        }
    }
}
