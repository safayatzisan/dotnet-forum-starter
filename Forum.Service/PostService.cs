﻿using Forum.Data;
using System;
using System.Collections.Generic;
using Forum.Data.Models;
using System.Threading.Tasks;
using forum_app_demo.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Forum.Service
{
    public class PostService : IPost
    {
        private ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task Archive(int id)
        {
            var post = GetById(id);
            post.IsArchived = true;
            _context.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var post = GetById(id);
            _context.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task EditPostContent(int id, string content)
        {
            var post = GetById(id);
            post.Content = content;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Posts
                .Include(post=>post.User)
                .Include(post=>post.Replies)
                .ThenInclude(reply => reply.User);
        }

        public IEnumerable<ApplicationUser> GetAllUsers(IEnumerable<Post> posts)
        {
            var users = new List<ApplicationUser>();

            foreach(var post in posts)
            {
                users.Add(post.User);

                if (post.Replies.Any())
                {
                    foreach(var reply in post.Replies)
                    {
                        users.Add(reply.User);
                    }
                }
            }

            return users.Distinct();
        }

        public Post GetById(int id)
        {
            return _context.Posts.Find(id);
        }

        public string GetForumImageUrl(int id)
        {
            var post = GetById(id);
            return post.Forum.ImageUrl;
        }

        public IEnumerable<Post> GetLatestPosts(int count)
        {
            return GetAll().Take(count);
        }

        public IEnumerable<Post> GetPostsBetween(DateTime start, DateTime end)
        {
            return _context.Posts.Where(post => post.Created >= start && post.Created <= end);
        }

        public IEnumerable<Post> GetPostsByForumId(int id)
        {
            return _context.Forums.First(forum => forum.Id == id).Posts;
        }

        public IEnumerable<Post> GetPostsByUserId(int id)
        {
            return _context.Posts.Where(post => post.User.Id == id.ToString());
        }

        public int GetReplyCount(int id)
        {
            return GetById(id).Replies.Count();
        }
    }
}
