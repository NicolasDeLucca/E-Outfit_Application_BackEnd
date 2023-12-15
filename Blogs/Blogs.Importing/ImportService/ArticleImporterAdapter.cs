using Blogs.Domain.BusinessEntities;
using Blogs.Importing.DTOs;
using Blogs.Importing.Parameters;
using Blogs.Interfaces;

namespace Blogs.Importing.ImportService
{
    public class ArticleImporterAdapter: IImporter<Article, ParameterType>
    {
        private IImporter<ImportedArticle, ParameterType> _importer;
        private EntityMapper _mapper;

        public ArticleImporterAdapter(IImporter<ImportedArticle, ParameterType> importer, EntityMapper mapper)
        {
            _importer = importer;
            _mapper = mapper;
        }

        public string GetName()
        {
            return _importer.GetName();
        }

        public List<IParameter<ParameterType>> GetParameters()
        {
            return _importer.GetParameters();
        }

        public List<Article> Import(List<IParameter<ParameterType>> parameters)
        {
            return _importer.Import(parameters).Select(a => _mapper.MapArticle(a)).ToList();
        }
    }
}
