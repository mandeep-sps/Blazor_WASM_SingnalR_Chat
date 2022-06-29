using BlazorChat.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorChat.Client.Managers
{
    public interface IChatManager
    {
        Task<List<ApplicationUserResult>> GetUsersAsync();
        Task SaveMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetConversationAsync(string contactId);
        Task<ApplicationUser> GetUserDetailsAsync(string userId);
        Task<bool> ReadMessages(string contactId);
        Task<bool> ReadAllMessages();

        Task<int> GetTotalUnreadCountAsync();
    }
}
