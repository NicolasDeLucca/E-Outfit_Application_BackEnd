using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.In
{
    public class OffenseModel
    {
        public string Word { get; set; }

        public Offense ToEntity()
        {
            return new Offense {Word = Word};
        }
    }
}
