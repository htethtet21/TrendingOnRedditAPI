using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMining_API.Data;
using DataMining_API.Models.Domain;
using DataMining_API.Models.DTO;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DataMining_API.Controllers
{
    [Route("api/[controller]")]
    public class MemesController : ControllerBase
    {
        private readonly DataMiningDbContext dbContext;

        public MemesController(DataMiningDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private const string RedditApiUrl = "https://www.reddit.com/r/memes/top.json?limit=20&t=day";

        [HttpGet("GetTopTwentyMemes")]
        public async Task<IActionResult> GetMemes()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DataMining"); // Reddit API requires a User-Agent header
                    var response = await client.GetAsync(RedditApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                      
                        var redditResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<RedditResponse>(content);

                        var posts = redditResponse.Data.Children;

                        var sortedPosts = posts.OrderByDescending(p => p.Data.Ups);


                        var results = new List<Result>();
                        int i = 1;
                        foreach (var post in sortedPosts)
                        {
                            var postData = post.Data;


                            // Create  with the extracted fields for each post
                            var result = new Result()
                            {
                                Id = i,
                                Url = postData.Url,
                                Title = postData.Title,
                                Ups = postData.Ups,
                                Commnets = postData.num_comments,
                                timestamp = DateTimeOffset.FromUnixTimeSeconds((long)postData.created_utc).DateTime,
                           
                            };
                             i++;
                            
                            results.Add(result);
                        }

                        InsertingTop20Data(results);

                        return Ok(results);
                    }
                    else
                    {
                        return BadRequest("No post cannot be retieved");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getTopTrendingMeme")]
        public IActionResult GetTrendingMemes()
          {
            var trendingPosts = dbContext.TrendingPosts.GroupBy(g => new { g.TimeStamp, g.PostName })
                                .Select(g=> g.First())
                                .ToList();
            var reult=trendingPosts.OrderByDescending(post => post.votes).ToList();
           
            return Ok(reult);
        }

        private void InsertingTop20Data(List<Result> results)
        {
            Guid keyGuid= Guid.NewGuid();
            var trendingPost = new TrendingPost();
            trendingPost.Id = keyGuid;
            trendingPost.PostName = results[0].Title;
            trendingPost.votes = results[0].Ups;
            trendingPost.comments = results[0].Commnets;
            trendingPost.url = results[0].Url;
            trendingPost.TimeStamp = results[0].timestamp;
            var totalCount = dbContext.TrendingPosts.ToList().Count;
            if (totalCount == 0)
            {
                trendingPost.SortIndex = 1;
            }
            else
            {
                trendingPost.SortIndex = totalCount + 1;
            }
            trendingPost.CreatedDate = DateTime.Now;
            dbContext.TrendingPosts.Add(trendingPost);
            dbContext.SaveChanges();


            for (int i=0;i<20;i++)
            {
                var newPost = new TopTwentyPost();
                newPost.Id = Guid.NewGuid();
                newPost.PostName = results[i].Title;
                newPost.votes = results[i].Ups;
                newPost.comments = results[i].Commnets;
                newPost.URL = results[i].Url;
                newPost.TimeStamp = results[i].timestamp;
                newPost.SortIndex = 1 + i;
                newPost.CreatedDate = DateTime.Now;
                newPost.CombinationKey = keyGuid;

                dbContext.TopTwentyPosts.Add(newPost);
                dbContext.SaveChanges();
            }
            
        }
    }
}

