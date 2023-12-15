using Blogs.Importing.Parameters;
using Blogs.Interfaces;

namespace Blogs.Models.In
{
    public class ArticleImporterInputModel
    {
        public string Name { get; set; }
        public List<Tuple<string, object>> Parameters { get; set; }

        public Tuple<string, List<IParameter<ParameterType>>> ToEntity()
        {
            List<IParameter<ParameterType>> mappedParameters = new();

            foreach(var parameter in Parameters)
            {
                ParameterImplementation<ParameterType> parameterInput = new() {IsRequired = true, Name = parameter.Item1,
                    Type = ParameterType.File, Value = parameter.Item2};

                IParameter<ParameterType> mappedParameter = new ParameterAdapter<ParameterType>(parameterInput); 
                mappedParameters.Add(mappedParameter);
            }
            
            return new(Name, mappedParameters);
        }
    }
}
