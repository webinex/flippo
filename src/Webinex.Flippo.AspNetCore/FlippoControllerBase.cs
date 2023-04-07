using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Webinex.Coded.AspNetCore;

namespace Webinex.Flippo.AspNetCore
{
    [Route("/api/flippo")]
    public abstract class FlippoControllerBase : ControllerBase
    {
        private readonly IFlippoMvcSettings _settings;
        private readonly IFlippo _flippo;

        protected FlippoControllerBase(IFlippoMvcSettings settings, IFlippo flippo)
        {
            _settings = settings;
            _flippo = flippo;
        }

        [HttpPost("{reference?}")]
        public virtual async Task<IActionResult> StoreAsync([FromForm(Name = "file")] IFormFile file, string reference,
            [FromQuery] bool replace)
        {
            if (file == null)
                return BadRequest();

            var stream = file.OpenReadStream();
            Response.RegisterForDispose(stream);
            var args = new FlippoStoreArgs(stream, file.FileName, file.ContentType, reference, replace);
            var result = await _flippo.StoreAsync(args);
            if (!result.Succeed) return new CodedActionResult(result.Failure);

            return Ok(result.Payload);
        }

        [HttpDelete("{reference}")]
        public virtual async Task<IActionResult> DeleteAsync(string reference)
        {
            var result = await _flippo.DeleteAsync(reference);
            if (!result.Succeed) return new CodedActionResult(result.Failure);
            return Ok();
        }

        [HttpGet("{reference}")]
        public virtual async Task<IActionResult> GetAsync(string reference)
        {
            var result = await _flippo.GetAsync(reference);
            if (!result.Succeed) return new CodedActionResult(result.Failure);

            var content = result.Payload;

            var contentDisposition = new ContentDisposition
                { Inline = true, FileName = content.FileName, Size = content.Stream.Length }.ToString();
            Response.Headers.Add(HeaderNames.ContentDisposition, contentDisposition);

            return new FileStreamResult(content.Stream, content.MimeType);
        }

        protected virtual IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        protected virtual IAuthenticationService AuthenticationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

        protected virtual async Task<bool> AuthorizeAsync()
        {
            if (_settings.Policy == null)
                return true;

            var authenticationResult = await AuthenticationService.AuthenticateAsync(HttpContext, _settings.Schema);
            if (!authenticationResult.Succeeded)
                return false;

            var authorizationResult =
                await AuthorizationService.AuthorizeAsync(authenticationResult.Principal!, _settings.Policy);
            return authorizationResult.Succeeded;
        }
    }
}