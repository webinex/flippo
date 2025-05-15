using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Webinex.Coded;
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
        public virtual async Task<IActionResult> StoreAsync(
            IFormFile file,
            string reference,
            [FromQuery] bool replace)
        {
            if (file == null)
                return BadRequest();

            var stream = file.OpenReadStream();
            Response.RegisterForDispose(stream);
            var args = new FlippoStoreArgs(stream, file.FileName, file.ContentType, reference, replace);
            var result = await _flippo.StoreAsync(args);
            if (!result.Succeed)
                return new CodedActionResult(result.Failure);

            return Ok(result.Payload);
        }

        [HttpDelete("{reference}")]
        public virtual async Task<IActionResult> DeleteAsync(string reference)
        {
            var result = await _flippo.DeleteAsync(reference);
            if (!result.Succeed)
                return new CodedActionResult(result.Failure);
            return Ok();
        }

        [HttpGet("{reference}/sas-token")]
        public virtual async Task<IActionResult> GetSasTokenAsync(IEnumerable<string> reference)
        {
            var result = await _flippo.GetSasTokenAsync(reference);
            if (!result.Succeed)
                return new CodedActionResult(result.Failure);
            return Ok(result.Payload);
        }

        [AllowAnonymous]
        [HttpGet("{reference}/open")]
        public virtual async Task<IActionResult> OpenAsync(
            string reference,
            string token = null,
            int? cacheControlMaxAge = null,
            bool? etag = null)
        {
            token ??= _settings.TokenCookie(HttpContext);

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest();

            var validation = await _flippo.VerifySasTokenAsync(token, reference);
            if (!validation.Succeed)
                return new CodedActionResult(validation.Failure);

            return await GetAsync(reference, cacheControlMaxAge, etag);
        }

        [HttpGet("{reference}")]
        public virtual async Task<IActionResult> GetAsync(string reference, int? cacheControlMaxAge, bool? etag = null)
        {
            cacheControlMaxAge ??= _settings.CacheControlMaxAgeCookie(HttpContext);
            etag ??= _settings.ETagCookie(HttpContext);
            
            if (!TryGetETag(out var etagValue))
                return await GetFileFromStorageAsync(reference, cacheControlMaxAge, etag.Value);

            if (!TryValidateETag(reference, etagValue, out var etagResult))
                return new CodedActionResult(etagResult);

            if (etagResult.Payload)
            {
                if (!TryAddCacheHeaders(reference, cacheControlMaxAge, etag.Value, out var error))
                    return new CodedActionResult(error);
                
                return StatusCode(StatusCodes.Status304NotModified);
            }

            return await GetFileFromStorageAsync(reference, cacheControlMaxAge, etag.Value);
        }

        private async Task<ActionResult> GetFileFromStorageAsync(string reference, int? cacheControlMaxAge, bool etag)
        {
            var result = await _flippo.GetAsync(reference);
            if (!result.Succeed)
                return new CodedActionResult(result.Failure);

            var content = result.Payload;

            AddContentDisposition(content);

            if (!TryAddCacheHeaders(reference, cacheControlMaxAge, etag, out var error))
                return new CodedActionResult(error);

            return new FileStreamResult(content.Stream, content.MimeType);
        }

        private bool TryValidateETag(string reference, string etagValue, out CodedResult<bool> result)
        {
            result = _settings.ETagValid(HttpContext, reference, etagValue);
            return result.Succeed;
        }

        private bool TryGetETag(out string result)
        {
            result = null;

            if (!Request.Headers.ContainsKey(HeaderNames.IfNoneMatch))
                return false;
            
            result = Request.Headers[HeaderNames.IfNoneMatch].ToString();
            return true;
        }
        
        private bool TryAddCacheHeaders(
            string reference,
            int? cacheControlMaxAge,
            bool etag,
            out CodedFailureBase error)
        {
            if (cacheControlMaxAge.HasValue &&
                !TryAddCacheControl(reference, cacheControlMaxAge.Value, out var addCacheControlError))
            {
                error = addCacheControlError;
                return false;
            }

            if (etag && !TryAddETag(reference, out var addETagError))
            {
                error = addETagError;
                return false;
            }

            error = null;
            return true;
        }

        private bool TryAddETag(string reference, out CodedFailureBase error)
        {
            error = null;

            var etagResult = _settings.ETag(HttpContext, reference);
            if (!etagResult.Succeed)
            {
                error = etagResult.Failure;
                return false;
            }

            Response.Headers[HeaderNames.ETag] = etagResult.Payload.ToString();
            return true;
        }

        private bool TryAddCacheControl(string reference, int cacheControlMaxAge, out CodedFailureBase error)
        {
            error = null;

            var cacheControlHeaderValueResult =
                _settings.CacheControl(HttpContext, reference, cacheControlMaxAge);

            if (!cacheControlHeaderValueResult.Succeed)
            {
                error = cacheControlHeaderValueResult.Failure;
                return false;
            }

            Response.Headers[HeaderNames.CacheControl] = cacheControlHeaderValueResult.Payload.ToString();
            return true;
        }

        private void AddContentDisposition(FlippoContent content)
        {
            var contentDisposition = new ContentDisposition
                { Inline = true, FileName = content.FileName, Size = content.Stream.Length }.ToString();
            Response.Headers[HeaderNames.ContentDisposition] = contentDisposition;
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