using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Blogs.Importing.DTOs;
using Blogs.Importing.Parameters;
using Blogs.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Blogs.Importing.ImportService
{
    public class ArticleImporterService : IImporterService<Article, ParameterType>
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<Article> _articleRepository;
        private string _importersPath;

        public ArticleImporterService(IConfiguration configuration, IRepository<Article> articleRepository)
        { 
            _configuration = configuration;
            _articleRepository = articleRepository;
            _importersPath = configuration.GetSection(@"ImportersAssemblys").Value;
        }

        public List<IImporter<Article, ParameterType>> GetImporters()
        {
            List<IImporter<Article, ParameterType>> availableImporters = new();
            string[] filePaths = Directory.GetFiles(_importersPath);

            foreach (string file in filePaths)
            {
                if (FileHasDllExtension(file))
                {
                    FileInfo dllFile = new(file);
                    Assembly myAssembly = Assembly.LoadFile(dllFile.FullName);

                    foreach (Type type in myAssembly.GetTypes())
                    {
                        if (ImplementsRequiredInterface(type))
                        {
                            var instance = (IImporter<ImportedArticle, ParameterType>) Activator.CreateInstance(type, _configuration);
                            var entityMapper = new EntityMapper();
                            availableImporters.Add(new ArticleImporterAdapter(instance, entityMapper));
                        }
                    }
                }
            }

            return availableImporters;
        }

        public List<Article> Import(string importerName, List<IParameter<ParameterType>> parameters)
        {
            IImporter<Article, ParameterType> desiredImporter = GetDesiredImporter(importerName);
            List<Article> articleToStore = desiredImporter.Import(parameters);
            List<Article> importedArticles = new();

            foreach (Article article in articleToStore)
            {
                TryArticleRegister(article);
            }

            return importedArticles;
        }

        private bool FileHasDllExtension(string file)
        {
            return file.EndsWith("dll");
        }

        private bool ImplementsRequiredInterface(Type type)
        {
            return typeof(IImporter<Article, ParameterType>).IsAssignableFrom(type) && !type.IsInterface;
        }

        private void TryArticleRegister(Article article)
        {
            try
            {
                article.ValidOrFail();
                _articleRepository.Store(article);
            }
            catch (InvalidRequestDataException){}
            catch (ResourceNotFoundException){}
        }

        private IImporter<Article, ParameterType> GetDesiredImporter(string importerName)
        {
            List<IImporter<Article, ParameterType>> availableImporters = GetImporters();
            IImporter<Article, ParameterType> desiredImporter = null;

            foreach (IImporter<Article, ParameterType> importer in availableImporters)
                if (importer.GetName() == importerName)
                    desiredImporter = importer;

            if (desiredImporter == null)
                throw new ResourceNotFoundException("Couldnt find specified importer");

            return desiredImporter;
        }
        
    }
}
