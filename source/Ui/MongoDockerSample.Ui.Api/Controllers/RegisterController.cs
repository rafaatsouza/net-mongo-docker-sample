using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDockerSample.Core.Domain.Exceptions;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Core.Domain.Services;

namespace MongoDockerSample.Ui.Api.Controllers
{
    /// <summary>
    /// Controller responsible for create, request, update and delete a register
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService registerService;

        public RegisterController(IRegisterService registerService)
        {
            this.registerService = registerService
                ?? throw new ArgumentNullException(nameof(registerService));
        }

        /// <summary>
        /// Returns full object base on its key.
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns>MongoDbRegistrer <see cref="Register"/></returns>
        [HttpGet("{key}")]
        [ProducesResponseType(200, Type = typeof(Register))]
        [ProducesResponseType(400, Type = typeof(CustomException<CustomError>))]
        public async Task<IActionResult> GetAsync([FromRoute] Guid key)
        {
            var result = await registerService.GetRegisterAsync(key);

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
        [ProducesResponseType(400, Type = typeof(CustomException<CustomError>))]
        [HttpPost("{value}")]
        public async Task<IActionResult> PostAsync([FromRoute] string value)
        {
            var key = await registerService.InsertRegisterAsync(value);

            return Ok(key);
        }

        /// <summary>
        /// Returns a list of objects
        /// </summary>
        /// <returns>MongoDbRegistrer <see cref="Register"/></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Register>))]
        [ProducesResponseType(400, Type = typeof(CustomException<CustomError>))]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await registerService.GetRegistersAsync();

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
        [ProducesResponseType(400, Type = typeof(CustomException<CustomError>))]
        [ProducesResponseType(404, Type = typeof(CustomException<CustomError>))]
        [HttpPut("{key}/{value}")]
        public async Task<IActionResult> PutAsync([FromRoute] Guid key, [FromRoute] string value)
        {
            await registerService.UpdateRegisterAsync(key, value);

            return Ok();
        }

        /// <summary>
        /// Deletes object.
        /// </summary>
        /// <param name="key">Objects key</param>
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(CustomException<CustomError>))]
        [ProducesResponseType(404, Type = typeof(CustomException<CustomError>))]
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteAsync(Guid key)
        {
            await registerService.DeleteRegisterAsync(key);

            return Ok();
        }
    }
}