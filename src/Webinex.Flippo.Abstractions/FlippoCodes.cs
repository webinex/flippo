using Webinex.Coded;

namespace Webinex.Flippo
{
    public static class FlippoCodes
    {
        public static readonly Code EXISTS = Code.INVALID.Child("FLIPPO.EXISTS");
        public static readonly Code NOT_FOUND = Code.NOT_FOUND.Child("FLIPPO");
        public static readonly Code MAX_SIZE = Code.INVALID.Child("FLIPPO.MAX_SIZE");
    }
}