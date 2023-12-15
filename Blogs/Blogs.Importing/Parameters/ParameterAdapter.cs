using Blogs.Interfaces;

namespace Blogs.Importing.Parameters
{
    public class ParameterAdapter<T> : IParameter<T>
    {
        private readonly ParameterImplementation<T> _input;

        public ParameterAdapter(ParameterImplementation<T> input)
        {
            _input = input;
        }

        public bool IsRequired()
        {
            return _input.IsRequired;
        }

        public string Name()
        {
            return _input.Name;
        }

        public T ParameterType()
        {
            return _input.Type;
        }

        public object Value()
        {
            return _input.Value;
        }
    }
}
