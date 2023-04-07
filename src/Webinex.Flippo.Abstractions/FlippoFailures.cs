using Webinex.Coded;

namespace Webinex.Flippo
{
    public static class FlippoFailures
    {
        public static CodedFailure<( string reference, object _ )> Exists(string reference) =>
            new CodedFailure<(string reference, object _)>(FlippoCodes.EXISTS, (reference, _: null));

        public static CodedFailure<( string reference, object _ )> NotFound(string reference)
            => new CodedFailure<(string reference, object _)>(FlippoCodes.NOT_FOUND, (reference, _: null));

        public static CodedFailure<( long value, object _ )> MaxSize(long value)
            => new CodedFailure<(long value, object _)>(FlippoCodes.MAX_SIZE, (value, _: null));
    }
}