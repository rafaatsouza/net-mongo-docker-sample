using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDocker.Sample.Domain.Contract.DTO;
using MongoDocker.Sample.Domain.Contract.Exception;
using MongoDocker.Sample.Domain.Service.Interfaces;

namespace MongoDocker.Sample.Ui.Api.Controllers
{
    /// <summary>
    /// Controller responsible for create, request, update and delete a value
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMongoDbService mongoDbService;

        /// <summary>
        /// Receives IMongoDbService instance from dependency injection
        /// </summary>
        /// <param name="mongoDbService"></param>
        public ValuesController(IMongoDbService mongoDbService)
        {
            this.mongoDbService = mongoDbService
                ?? throw new ArgumentNullException(nameof(mongoDbService));
        }

        /// <summary>
        /// Returns full object base on its key.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>MongoDbRegistrer <see cref="MongoDbRegister"/></returns>
        [HttpGet("{key}")]
        [ProducesResponseType(200, Type = typeof(MongoDbRegister))]
        [ProducesResponseType(400, Type = typeof(MongoDbCustomException))]
        [ProducesResponseType(404, Type = typeof(MongoDbCustomException))]
        public async Task<IActionResult> GetAsync([FromRoute] Guid key)
        {
            try
            {
                var result = await mongoDbService.GetValueAsync(key);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (MongoDbCustomException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        /// <summary>
        /// Inserts new object.
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>Object identifier <see cref="Guid"/> </returns>
        [ProducesResponseType(200, Type = typeof(Guid))]
        [ProducesResponseType(400, Type = typeof(MongoDbCustomException))]
        [HttpPost("{value}")]
        public async Task<IActionResult> PostAsync([FromRoute] string value)
        {
            try
            {
                var key = await mongoDbService.InsertValueAsync(value);

                return Ok(key);
            }
            catch (MongoDbCustomException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        /// <summary>
        /// Returns a list of objects
        /// </summary>
        /// <returns>MongoDbRegistrer <see cref="MongoDbRegister"/></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MongoDbRegister>))]
        [ProducesResponseType(404, Type = typeof(MongoDbCustomException))]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await mongoDbService.GetValuesAsync();

                if (!result.Any())
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (MongoDbCustomException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        /// <summary>
        /// Updates object value.
        /// </summary>
        /// <param name="key">Objects key</param>
        /// <param name="value">Objects new value</param>
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(MongoDbCustomException))]
        [ProducesResponseType(404, Type = typeof(MongoDbCustomException))]
        [HttpPut("{key}/{value}")]
        public async Task<IActionResult> PutAsync([FromRoute] Guid key, [FromRoute] string value)
        {
            try
            {
                await mongoDbService.UpdateValueAsync(key, value);

                return Ok();
            }
            catch (MongoDbCustomException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        /// <summary>
        /// Deletes object.
        /// </summary>
        /// <param name="key">Objects key</param>
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(MongoDbCustomException))]
        [ProducesResponseType(404, Type = typeof(MongoDbCustomException))]
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteAsync(Guid key)
        {
            try
            {
                await mongoDbService.DeleteValueAsync(key);

                return Ok();
            }
            catch (MongoDbCustomException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
