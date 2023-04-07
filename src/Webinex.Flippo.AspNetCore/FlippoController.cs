namespace Webinex.Flippo.AspNetCore
{
    internal class FlippoController : FlippoControllerBase
    {
        public FlippoController(IFlippoMvcSettings settings, IFlippo flippo) : base(settings, flippo)
        {
        }
    }
}