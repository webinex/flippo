using System.Collections.Generic;
using System.Linq;

namespace Webinex.Flippo
{
    public class FlippoVerifySasTokenArgs
    {
        public FlippoVerifySasTokenArgs(string token, IEnumerable<string> references)
        {
            Token = token;
            References = references.ToArray();
        }

        public string Token { get; }
        public string[] References { get; }
    }
}