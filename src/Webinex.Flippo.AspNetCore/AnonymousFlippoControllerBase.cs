using Microsoft.AspNetCore.Authorization;

namespace Webinex.Flippo.AspNetCore
{
    [AllowAnonymous]
    internal class AnonymousFlippoControllerBase : FlippoControllerBase
    {
        public AnonymousFlippoControllerBase(IFlippoMvcSettings settings, IFlippo flippo) : base(settings, flippo)
        {
        }
    }
}