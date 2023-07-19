using Kolokwium_Poprawa.Services;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Kolokwium_Poprawa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly IDbServices _dbservices;
        public OwnersController(IDbServices dbservices)
        {
            _dbservices = dbservices;
        }

        [HttpGet("{ownerId}")]
        public async Task<IActionResult> GetObjects(int ownerId)
        {
            if (!await _dbservices.OwnerExists(ownerId))
            {
                return NotFound();
            }
            return Ok(await _dbservices.OwnerList(ownerId));
        }

        [HttpPost("{ownerId}/objects/{objectId}")]
        public async Task<IActionResult> AddObjectToOwner(int ownerId, int objectId)
        {
            if (!await _dbservices.OwnerExists(ownerId))
            {
                return NotFound();
            }
            if (!await _dbservices.ObjectExists(objectId))
            {
                return NotFound();
            }
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await _dbservices.AddObjectToOwner(ownerId, objectId);
            transaction.Complete();

            return Created("", "");
        }
    }
}
