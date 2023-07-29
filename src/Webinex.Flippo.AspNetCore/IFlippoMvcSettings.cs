using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Webinex.Coded;

namespace Webinex.Flippo.AspNetCore
{
    public interface IFlippoMvcSettings
    {
        [MaybeNull]
        string Schema { get; }
        
        [MaybeNull]
        string Policy { get; }
        
        string ParamCookiePrefix { get; }
        
        Func<HttpContext, string, int, CodedResult<CacheControlHeaderValue>> CacheControl { get; }
        Func<HttpContext, string, CodedResult<EntityTagHeaderValue>> ETag { get; }
        Func<HttpContext, string, string, CodedResult<bool>> ETagValid { get; }
    }
    
    internal static class FlippoMvcSettingsExtensions
    {
        public static string TokenCookie(this IFlippoMvcSettings settings, HttpContext context)
        {
            return settings.CookieOrNull(context, "Token");
        }

        public static bool ETagCookie(this IFlippoMvcSettings settings, HttpContext context)
        {
            var value = settings.CookieOrNull(context, "ETag");
            return !string.IsNullOrWhiteSpace(value) && bool.Parse(value);
        }
        
        public static int? CacheControlMaxAgeCookie(this IFlippoMvcSettings settings, HttpContext context)
        {
            var value = settings.CookieOrNull(context, "MaxAge");
            return string.IsNullOrWhiteSpace(value) ? default : int.Parse(value);
        }

        private static string CookieOrNull(this IFlippoMvcSettings settings, HttpContext context, string name)
        {
            return context.Request.Cookies[settings.ParamCookiePrefix + name];
        }
    }
}