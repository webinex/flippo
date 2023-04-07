using System.Diagnostics.CodeAnalysis;

namespace Webinex.Flippo.AspNetCore
{
    public interface IFlippoMvcSettings
    {
        [MaybeNull]
        string Schema { get; }
        
        [MaybeNull]
        string Policy { get; }
    }
}