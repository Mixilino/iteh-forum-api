﻿using ForumWebApi.DataTransferObject.PostDto;
using ForumWebApi.DataTransferObject.UserDto;
using ForumWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ForumWebApi.Data.PostRepo
{
    public class PostRepository : IPostRepository
    {

        private readonly DataContext _context;

        public PostRepository(DataContext context)
        {
            _context = context;
        }

        public Post Add(PostCreateDto postDto, UserResponseDto userDto)
        {
            User u = _context.Users.SingleOrDefault(u => u.UserId == userDto.UserId);
            if (u == null)
            {
                throw new Exception("User shouldnt be null!");
            }
            Post post = new Post
            {
                PostTitle = postDto.PostTitle,
                UserId = userDto.UserId,
                DatePosted = DateTime.Now,
                PostCategories = _context.PostCategories.Where(pc => postDto.PostCategoryIds.Contains(pc.PcId)).ToList(),
                PostText = postDto.PostText,
                User = u,
            };
            _context.Posts.Add(post);
            return post;
        }

        public Post? Change(PostChangeDto postDto, UserResponseDto userDto)
        {

            Post post = _context.Posts.Include(p => p.User)
                                      .Include(p => p.Votes)
                                      .ThenInclude(v => v.User)
                                      .Include(p => p.Comments)
                                      .ThenInclude(c=>c.User)
                                      .Include(p => p.PostCategories)
                                      .SingleOrDefault(p => p.PostId == postDto.PostId);
            if (post == null)
            {
                return null;
            }
            post.PostCategories = _context.PostCategories.Where(pc => postDto.PostCategoryIds.Contains(pc.PcId)).ToList();
            post.PostText = postDto.PostText;
            post.PostTitle = postDto.PostTitle;
            return post;
        }


        public void Delete(int PostId, UserResponseDto userDto)
        {

            Post post = _context.Posts.SingleOrDefault(p => p.PostId == PostId && p.UserId == userDto.UserId);
            if (post == null)
            {
                return;
            }
            _context.Posts.Remove(post);
        }

        public List<Post> GetAll()
        {
            return _context.Posts.Include(p => p.User)
                                      .Include(p => p.Votes)
                                      .ThenInclude(v => v.User)
                                      .Include(p => p.Comments)
                                      .ThenInclude(c=>c.User)
                                      .Include(p => p.PostCategories).ToList();
        }

        public Post? Vote(int PostId, bool UpVote, UserResponseDto userDto)
        {
            Post post = _context.Posts.Include(p => p.User)
                                      .Include(p => p.Votes)
                                      .ThenInclude(v => v.User)
                                      .Include(p => p.Comments)
                                      .ThenInclude(c=>c.User)
                                      .Include(p => p.PostCategories)
                                      .SingleOrDefault(p => p.PostId == PostId);
            if (post == null)
            {
                return null;
            }
            Vote vote = post.Votes.FirstOrDefault(v => v.PostId == PostId && v.UserId == userDto.UserId);
            if (vote == null)
            {
                vote = new Vote
                {
                    UserId = userDto.UserId,
                    DateLiked = DateTime.Now,
                    PostId = PostId,
                    UpVote = UpVote,
                    User = _context.Users.First(u=>u.UserId == userDto.UserId),
                };
                post.Votes.Add(vote);
                return post;
            }
            if (vote.UpVote == UpVote)
            {
                post.Votes.Remove(vote);
                return post;
            }
            if (vote.UpVote != UpVote)
            {
                    vote.UpVote = UpVote;
                
            }
            return post;
        }
    }
}
