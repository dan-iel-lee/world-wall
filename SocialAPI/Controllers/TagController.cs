using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialAPI.Controllers
{
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private SocialContext _context;

        public TagController(SocialContext c)
        {
            _context = c;
        }

        // GET: api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            string[] output = new string[tags.Count];
            for (int ii = 0; ii < tags.Count; ii++)
            {
                output[ii] = tags[ii].TagId;
            }

            return output;
        }

        /*[HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostTags(long postId)
        {
            var p = await _context.Posts.Include(x => x.PostTags).ThenInclude(x => x.Tag).ToArrayAsync();
            return p;
        }*/

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetTag(string id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag.TagId;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<Post>> PostTag([FromQuery] long postId, [FromBody]Tag tag)
        {
            Post post = await _context.Posts.FindAsync(postId);

            // add the many to many relation
            PostTag pt = new PostTag
            {
                PostId = postId,
                Post = post,

                TagId = tag.TagId,
                Tag = tag,
            };

            // add PostTags to tag and post
            tag.PostTags.Add(pt);
            post.PostTags.Add(pt);

            // save to db
            await _context.Tags.AddAsync(tag);
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // let client know we created this tag
            return CreatedAtAction(nameof(GetTag), new { id = tag.TagId }, tag);
        }
    }
}
