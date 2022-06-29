using BlazorChat.Server.Data;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{contactId}")]
        public async Task<IActionResult> GetConversationAsync(string contactId)
        {
            var userId = User.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
            var messages = await _context.ChatMessages
                    .Where(h => (h.FromUserId == contactId && h.ToUserId == userId) || (h.FromUserId == userId && h.ToUserId == contactId))
                    .OrderBy(a => a.CreatedDate)
                    .Include(a => a.FromUser)
                    .Include(a => a.ToUser)
                    .Select(x => new ChatMessage
                    {
                        FromUserId = x.FromUserId,
                        Message = x.Message,
                        CreatedDate = x.CreatedDate,
                        Id = x.Id,
                        ToUserId = x.ToUserId,
                        ToUser = x.ToUser,
                        FromUser = x.FromUser
                    }).AsNoTracking().ToListAsync();
            return Ok(messages);
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var userId = User.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
            var allUsers = await _context.ApplicationUsers.Where(user => user.Id != userId)
                .Include(i => i.ChatMessagesFromUsers).AsNoTracking().ToListAsync();

            List<ApplicationUserResult> applicationUsers = allUsers.Select(x => new ApplicationUserResult
            {
                AuditedOn = x.AuditedOn,
                Email = x.Email,
                Hash = x.Hash,
                Id = x.Id,
                IsDark = x.IsDark,
                Name = x.Name,
                Password = x.Password,
                SenderDate = x.ChatMessagesFromUsers.Count > 0 ? x.ChatMessagesFromUsers.LastOrDefault().CreatedDate : null,
                UnreadCount = x.ChatMessagesFromUsers.Count(h => (h.FromUserId == x.Id && h.ToUserId == userId) && (!h.IsRead))
            }).ToList();

            return Ok(applicationUsers.OrderByDescending(x => x.SenderDate).ToList());
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetTotalUnreadCountAsync()
        {
            var userId = User.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
            var allUsers = await _context.ApplicationUsers.Where(user => user.Id != userId)
                .Include(i => i.ChatMessagesFromUsers).AsNoTracking().ToListAsync();

            var count = 0;

            foreach (var item in allUsers)
            {
                count += item.ChatMessagesFromUsers.Count(h => (h.FromUserId == item.Id && h.ToUserId == userId) && (!h.IsRead));
            }
            return Ok(count);
        }


        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetailsAsync(string userId)
        {
            var user = await _context.ApplicationUsers.Where(user => user.Id == userId)
                .AsNoTracking().FirstOrDefaultAsync();
            return Ok(user);
        }

        [HttpGet("read/{contactId}")]
        public async Task<IActionResult> ReadMessages(string contactId)
        {
            var userId = User.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
            var messages = await _context.ChatMessages
                    .Where(h => (h.FromUserId == contactId && h.ToUserId == userId) && !h.IsRead).AsNoTracking().ToListAsync();

            messages.ForEach(x => x.IsRead = true);

            _context.UpdateRange(messages);

            return Ok(await _context.SaveChangesAsync());
        }

        [HttpGet("read")]
        public async Task<IActionResult> ReadAllMessages()
        {
            var userId = User.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
            var allUsers = await _context.ApplicationUsers.Where(user => user.Id != userId)
                .Include(i => i.ChatMessagesFromUsers).AsNoTracking().ToListAsync();

            foreach (var item in allUsers)
            {
                var messages = await _context.ChatMessages
                    .Where(h => (h.FromUserId == item.Id && h.ToUserId == userId) && !h.IsRead).AsNoTracking().ToListAsync();
                if (messages.Any())
                {
                    messages.ForEach(x => x.IsRead = true);

                    _context.UpdateRange(messages);
                }
            }

            return Ok(await _context.SaveChangesAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessageAsync(ChatMessage message)
        {
            var now = DateTime.Now;
            var userId = User.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
            message.FromUserId = userId;
            message.CreatedDate = now;
            message.ToUserId = message.ToUserId; //await _context.ApplicationUsers.Where(user => user.Id == message.ToUserId).FirstOrDefaultAsync();
            await _context.ChatMessages.AddAsync(message);
            return Ok(await _context.SaveChangesAsync());
        }
    }
}
