using System.Diagnostics.CodeAnalysis;

namespace Blogs.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message) { }
    }
}
