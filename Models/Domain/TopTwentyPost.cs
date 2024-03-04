using System;
namespace DataMining_API.Models.Domain
{
	public class TopTwentyPost
	{
        public Guid Id { get; set; }
        public String PostName { get; set; }
        public int votes { get; set; }
        public int? comments { get; set; }
        public String URL { get; set; }
        public DateTime TimeStamp { get; set; }
        public int SortIndex { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CombinationKey { get; set; }
    }
}

