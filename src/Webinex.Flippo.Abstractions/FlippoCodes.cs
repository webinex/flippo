using Webinex.Coded;

namespace Webinex.Flippo
{
    public static class FlippoCodes
    {
        public static readonly Code EXISTS = Code.INVALID.Child("FLIPPO.EXISTS");
        public static readonly Code NOT_FOUND = Code.NOT_FOUND.Child("FLIPPO");
        public static readonly Code MAX_SIZE = Code.INVALID.Child("FLIPPO.MAX_SIZE");
        public static readonly Code TOKEN = Code.INVALID.Child("FLIPPO.TOKEN");
        public static readonly Code TOKEN_REFERENCE = Code.INVALID.Child("FLIPPO.TOKEN_REFERENCE");
    }
}