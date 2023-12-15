using Blogs.Exceptions;
using Blogs.Importing.DTOs;
using Blogs.Importing.Parameters;
using Blogs.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Blogs.Importing.Importers.JSON
{
    public class ArticleImporter : IImporter<ImportedArticle, ParameterType>
    {
        private readonly string _jsonFilePath;
        private readonly string _importerName;
        private readonly IParameter<ParameterType> _defaultParameter;
        private readonly List<IParameter<ParameterType>> _requiredParameters;
        private readonly ParameterImplementation<ParameterType> _parameterInput;

        public ArticleImporter(IConfiguration configuration)
        {
            _parameterInput = new() {IsRequired = true, Name = "File to parse", Type = ParameterType.File};
            _defaultParameter = new ParameterAdapter<ParameterType>(_parameterInput);
            _requiredParameters = new() {_defaultParameter};

            _jsonFilePath = configuration.GetSection("JSONFilesToImport").Value;
            _importerName = "JSONImporter";
        }

        public string GetName()
        {
            return _importerName;
        }

        public List<IParameter<ParameterType>> GetParameters()
        {
            return new List<IParameter<ParameterType>>(_requiredParameters);
        }

        public List<ImportedArticle> Import(List<IParameter<ParameterType>> parameters)
        {
            List<ImportedArticle> parsedArticles = new();
            ValidateParameters(parameters);

            var fileName = (string) parameters.Find(p => p.Name() == _defaultParameter.Name()).Value();
            try
            { 
                var JSONtext = File.ReadAllText(_jsonFilePath + '/' + fileName);

                if (JSONtext.StartsWith('['))
                    parsedArticles = JsonConvert.DeserializeObject<List<ImportedArticle>>(JSONtext);
                else
                    parsedArticles.Add(JsonConvert.DeserializeObject<ImportedArticle>(JSONtext));

                return parsedArticles;
            }
            catch(FileNotFoundException)
            {
                throw new InvalidRequestDataException("Couldnt find specified file");
            }
        }

        #region Validators

        private void ValidateParameters(List<IParameter<ParameterType>> parameters)
        {
            if (parameters.Count != _requiredParameters.Count)
                throw new InvalidRequestDataException("Wrong number of parameters");

            if (!_requiredParameters.TrueForAll(rp => parameters.Any(p => p.Name == rp.Name)))
                throw new InvalidRequestDataException("Wrong parameters");
        }

        #endregion
    }
}
