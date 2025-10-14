using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EWorldCup.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="repo"></param>
    [Route("participants")]
    [ApiController]
    public class ParticipantsController(IParticipantRepository repo) : ControllerBase
    {
        /// <summary>
        /// Returnerar alla deltagare (för spelardropdown i frontend).
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>Alla deltagare</returns>
        [HttpGet]
        public async Task<ActionResult<ParticipantsResponse>> GetAll(CancellationToken ct)
        {
            var list = await repo.GetAllAsync(ct);
            var dtos = list.Select(p => new ParticipantDto(p.Id, p.Name)).ToList();
            return Ok(new ParticipantsResponse(dtos));
        }
    }
}
