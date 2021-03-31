using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDockerSample.Core.Domain.Exceptions.Custom;
using MongoDockerSample.Core.Domain.Services;
using MongoDockerSample.Ui.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDockerSample.Ui.Api.Controllers
{
    /// <summary>
    /// Controller responsible for create, request, update and delete a record
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IEntryService entryService;

        public EntriesController(IMapper mapper, IEntryService entryService)
        {
            this.mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));
            this.entryService = entryService
                ?? throw new ArgumentNullException(nameof(entryService));
        }

        /// <summary>
        /// Returns full object base on its key.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>MongoDbRegistrer <see cref="EntryDto"/></returns>
        [HttpGet("{key}")]
        [ProducesResponseType(200, Type = typeof(EntryDto))]
        [ProducesResponseType(400, Type = typeof(CustomException))]
        public async Task<IActionResult> GetAsync([FromRoute] Guid key)
        {
            var result = await entryService.GetEntryAsync(key);

            if (result == null)
            {
                return NotFound();
            }

            var record = mapper.Map<EntryDto>(result);

            return Ok(result);
        }

        /// <summary>
        /// Inserts new object.
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>Object identifier <see cref="Guid"/> </returns>
        [ProducesResponseType(200, Type = typeof(Guid))]
        [ProducesResponseType(400, Type = typeof(CustomException))]
        [HttpPost("{value}")]
        public async Task<IActionResult> PostAsync([FromRoute] string value)
        {
            var key = await entryService.InsertEntryAsync(value);

            return Ok(key);
        }

        /// <summary>
        /// Returns a list of objects
        /// </summary>
        /// <returns><see cref="EntryDto"/></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EntryDto>))]
        [ProducesResponseType(400, Type = typeof(CustomException))]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await entryService.GetEntriesAsync();

            if (!result.Any())
            {
                return NoContent();
            }

            var records = mapper.Map<IEnumerable<EntryDto>>(result);

            return Ok(records);
        }

        /// <summary>
        /// Updates object value.
        /// </summary>
        /// <param name="key">Objects key</param>
        /// <param name="value">Objects new value</param>
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(CustomException))]
        [ProducesResponseType(404, Type = typeof(CustomException))]
        [HttpPut("{key}/{value}")]
        public async Task<IActionResult> PutAsync([FromRoute] Guid key, [FromRoute] string value)
        {
            await entryService.UpdateEntryAsync(key, value);

            return Ok();
        }

        /// <summary>
        /// Deletes object.
        /// </summary>
        /// <param name="key">Objects key</param>
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(CustomException))]
        [ProducesResponseType(404, Type = typeof(CustomException))]
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteAsync(Guid key)
        {
            await entryService.DeleteEntryAsync(key);

            return Ok();
        }
    }
}