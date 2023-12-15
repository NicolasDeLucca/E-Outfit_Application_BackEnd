using Blogs.Domain.BusinessEntities;
using Blogs.Importing.Parameters;
using Blogs.Interfaces;

namespace Blogs.Models.Out
{
    public class ArticleImporterDetail
    {
        public string Name { get; set; }
        public List<Tuple<string, string>> Parameters { get; set; }

        public ArticleImporterDetail(IImporter<Article, ParameterType> articleImporter)
        {
            Name = articleImporter.GetName();
            Parameters = new();
             
            foreach(IParameter<ParameterType> parameter in articleImporter.GetParameters())
            {
                Tuple<string, string> parameterValue = new(parameter.Name(), parameter.ParameterType().ToString());
                Parameters.Add(parameterValue);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is ArticleImporterDetail detail && Name == detail.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
