namespace Blogs.Importing.Parameters
{
    public class ParameterImplementation<T>
    {
        public bool IsRequired { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public T Type { get; set; }
    }
}
