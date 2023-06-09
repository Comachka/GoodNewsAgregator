using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using myProject.Models;
using myProject.Data.Entities;

namespace myProject.Mvc.SignalR
{
    public class CommentsHub : Hub
    {
        public async Task SendMessage(string content, string avatar, string user, string dateCreated, int raiting, int userId)
        {
            await Clients.All.SendAsync("ReceiveMessage", content, avatar, user, dateCreated, raiting, userId);
        }
    }
}
