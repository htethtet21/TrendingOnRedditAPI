using System;
using DataMining_API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace DataMining_API.Data
{
	public class DataMiningDbContext: DbContext
	{

		public DataMiningDbContext(DbContextOptions dbContextOptions) :base(dbContextOptions)
		{

		}
		
		public DbSet<TrendingPost> TrendingPosts { get; set; }
		public DbSet<TopTwentyPost> TopTwentyPosts { get; set; }
	}
}

