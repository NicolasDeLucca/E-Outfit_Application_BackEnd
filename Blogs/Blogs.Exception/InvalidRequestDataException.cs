using System.Diagnostics.CodeAnalysis;

namespace Blogs.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidRequestDataException : Exception
    {
        public InvalidRequestDataException(string message) : base(message) { }
    }
}
