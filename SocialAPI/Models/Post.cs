using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SocialAPI.Models
{
    public class Post
    {
        public long PostId { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public String Content { get; set; }
        public int Score { get; set; }
        public DateTime PublishDate { get; set; }

        // many to many relationship between posts and tags
        public List<PostTag> PostTags { get; set; }

        public Post()
        {
            // default values
            Score = 0;
            PublishDate = DateTime.Now;
            PostTags = new List<PostTag>();
        }
    }

    public class Tag
    {
        public string TagId { get; set; }

        // many to many relationship between posts and tags
        public List<PostTag> PostTags { get; set; }

        // default values
        public Tag()
        {
           PostTags = new List<PostTag>();
        }
    }

    // join table to allow Many-To-Many between Post and Tag
    public class PostTag
    {
        public long PostId { get; set; }
        public Post Post { get; set; }

        public string TagId { get; set; }
        public Tag Tag {get; set; }
    }
}
