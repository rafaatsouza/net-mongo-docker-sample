﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDockerSample.Core.Domain.Exceptions.Custom;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Core.Domain.Services;

namespace MongoDockerSample.Ui.Api.Controllers
{
    /// <summary>
    /// Controller responsible for create, request, update and delete a record
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly IRecordService recordService;

        public RecordController(IRecordService recordService)
        {
            this.recordService = recordService
                ?? throw new ArgumentNullException(nameof(recordService));
        }

        /// <summary>
        /// Returns full object base on its key.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>MongoDbRegistrer <see cref="Record"/></returns>
        [HttpGet("{key}")]
        [ProducesResponseType(200, Type = typeof(Record))]
        [ProducesResponseType(400, Type = typeof(CustomException))]
        public async Task<IActionResult> GetAsync([FromRoute] Guid key)
        {
            var result = await recordService.GetRecordAsync(key);

            if (result == null)
            {
                return NotFound();
            }

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
            var key = await recordService.InsertRecordAsync(value);

            return Ok(key);
        }

        /// <summary>
        /// Returns a list of objects
        /// </summary>
        /// <returns><see cref="Record"/></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Record>))]
        [ProducesResponseType(400, Type = typeof(CustomException))]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await recordService.GetRecordsAsync();

            if (!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
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
            await recordService.UpdateRecordAsync(key, value);

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
            await recordService.DeleteRecordAsync(key);

            return Ok();
        }
    }
}