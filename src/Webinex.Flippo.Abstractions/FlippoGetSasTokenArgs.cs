using System.Collections.Generic;
using System.Linq;

namespace Webinex.Flippo
{
    public class FlippoGetSasTokenArgs
    {
        public FlippoGetSasTokenArgs(IEnumerable<string> references)
        {
            References = references.ToArray();
        }

        public string[] References { get; }
    }
}