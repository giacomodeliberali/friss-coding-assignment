using System;
using Application.Contracts;
using Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web
{
    /// <summary>
    /// Base controller from which controllers should derive.
    /// </summary>
    [ApiController]
    public abstract class CustomBaseController : ControllerBase
    {
        /// <summary>
        /// The current hosting environment.
        /// </summary>
        protected readonly IHostingEnvironment HostingEnvironment;

        /// <summary>
        /// Creates the base controller.
        /// </summary>
        protected CustomBaseController(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Returns the 400 bad request with the serialized exception. If development it contains the stack trace.
        /// </summary>
        /// <param name="exception">The exception to serialize.</param>
        protected IActionResult BadRequest(ValidationException exception)
        {
            return BadRequest(SerializeException(exception));
        }

        /// <summary>
        /// Returns the 500 internal server error request with the serialized exception. If development it contains the stack trace.
        /// </summary>
        /// <param name="exception">The exception to serialize.</param>
        protected IActionResult ServerError(BusinessException exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                SerializeException(exception));
        }

        private ExceptionDto SerializeException(Exception exception)
        {
            var exceptionDto = new ExceptionDto()
            {
                Message = exception.Message,
                Name = exception.GetType().Name,
                StackTrace = HostingEnvironment.IsDevelopment() ? exception.StackTrace : null,
            };

            return exceptionDto;
        }
    }
}
