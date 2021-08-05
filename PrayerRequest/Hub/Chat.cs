using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using PrayerRequest.Areas.Identity.Data;
using PrayerRequest.Models;
using PrayerRequest.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerRequest.Hub
{
    public class Chat : Microsoft.AspNetCore.SignalR.Hub
    {
        private IDBRepository _repository;
        private UserManager<PrayerRequestUser> _UserManager;
        public Chat(IDBRepository repo, UserManager<PrayerRequestUser> UserManager)
        {
            _repository = repo;
            _UserManager = UserManager;
        }
        public async Task SendMessage(string message, string group)
        {
            Post post = new Post
            {
                GroupID = int.Parse(group),
                OwnerID = _UserManager.GetUserId(Context.User),
                Postmessage = message
            };
            if (message.Trim() != "")
            {
                int temp = _repository.AddPost(post);
                post = _repository.GetPost(temp);
                var userid = post.OwnerID;
                post.OwnerID = _UserManager.FindByIdAsync(post.OwnerID).GetAwaiter().GetResult().DisplayName;
                await Clients.Group(group).SendAsync("ReceiveMessage", post.ID, post.OwnerID, post.Date.ToString(), post.Postmessage, userid); //postid, usename, date, message
            }
        }
        public Task Join(string room)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, room);
        }
        public Task Leave(string room)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        }
    }
}
