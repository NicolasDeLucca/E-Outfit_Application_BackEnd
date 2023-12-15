using Blogs.Domain.BusinessEntities;

namespace Blogs.Models.Out
{
    public class OffenseDetail
    {
        public int Id { get; set; }
        public string Word { get; set; }

        public OffenseDetail(Offense offense)
        {
            Id = offense.Id;
            Word = offense.Word;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && obj is OffenseDetail detail && Id == detail.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

    }
}
