using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Dynamic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly SocialContext _context;

        public PostController(SocialContext context)
        {
            _context = context;

            DbSet<Post> posts = _context.Posts;

            if (posts.Count() == 0)
            {
                posts.Add(new Post {
                    Title = "Post 1",
                    Content = "Lorem Ipsum",
                    PublishDate = DateTime.Now,
                });
                posts.Add(new Post
                {
                    Title = "Post 2",
                    Content = "Lorem Ipsum",
                    PublishDate = DateTime.Now,
                });
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IDictionary>>> GetPosts()
        {
            var posts = await _context.Posts.Include(x => x.PostTags).ThenInclude(x => x.Tag).ToArrayAsync();

            LinkedList<IDictionary> dicts = new LinkedList<IDictionary>();
            foreach (var post in posts)
            {
                dicts.AddLast(HelperAddTagsToPost(post));
            }

            return dicts;
        }

        private IDictionary HelperAddTagsToPost(Post post)
        {
            // initialize dict with post's properties
            IDictionary dict = new Dictionary<string, Object>();
            foreach (var property in post.GetType().GetProperties())
            {
                dict.Add(FirstLetterLowercase(property.Name), property.GetValue(post));
            }

            string FirstLetterLowercase(string s)
            {
                return Char.ToLowerInvariant(s[0]) + s.Substring(1);
            }

            // get tags
            LinkedList<string> tags = new LinkedList<string>();
            foreach (var postTag in post.PostTags)
            {
                var tag = postTag.Tag;
                tags.AddLast(tag.TagId);
            }

            dict.Add("tags", tags);

            return dict;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Post>> GetPost(long id)
        {
            var post = await _context.Posts.FindAsync(id);

            // check if post exists
            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPost]
        public async Task<ActionResult<Post>> PostPost()
        {
            // default publication date
            //p.PublishDate = DateTime.Now;

            // handle tags
            string s;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                s = await reader.ReadToEndAsync();
            }

            JObject jObject = JObject.Parse(s);
            var tags = jObject["tags"].ToObject<string[]>(); // jObject of the tags)

            Post p = JsonConvert.DeserializeObject<Post>(s);
            _context.Posts.Add(p);
            await _context.SaveChangesAsync();

            // add the tags to the db
            helperAddTags(p, tags);

            return CreatedAtAction(nameof(GetPost), new { id = p.PostId }, p);
        }

        private async void helperAddTags(Post post, string[] tags)
        {
            foreach (string tagString in tags)
            {
                // get the tag if it exists
                Tag tag = await _context.Tags.FindAsync(tagString);
                // create one if it doesn't exist
                if (tag == null)
                {
                    tag = new Tag { TagId = tagString };
                    await _context.Tags.AddAsync(tag);
                }
                // add the many to many relation
                PostTag pt = new PostTag
                {
                    PostId = post.PostId,
                    Post = post,

                    TagId = tagString,
                    Tag = tag,
                };

                // add the PostTag to the tag and the post
                tag.PostTags.Add(pt);
                post.PostTags.Add(pt);

                // send to database
                await _context.SaveChangesAsync();
            }

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutPost(long id, [FromBody]Post p)
        {
            if (id != p.PostId)
            {
                return BadRequest();
            }

            // check to make sure the id exists
            Post origPost = await _context.Posts.FindAsync(id);
            if (origPost == null)
            {
                return NotFound();
            }

            // make sure we don't change the publish date
            p.PublishDate = origPost.PublishDate;

            _context.Entry(origPost).State = EntityState.Deleted;
            _context.Entry(p).State = EntityState.Modified; // tells the context that 'p' is already in the database, but has some fields modified
            await _context.SaveChangesAsync(); // tells context to save Modified entities, sets all modified entities to Unchanged

            /*// alternative since calling DbSet.FindAsync(id) attaches the post
            origPost.Title = p.Title;
            origPost.Content = p.Content;
            await _context.SaveChangesAsync();*/

            return NoContent();
        }

    }
}
