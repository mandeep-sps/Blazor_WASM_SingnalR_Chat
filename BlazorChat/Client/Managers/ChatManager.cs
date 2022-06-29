﻿using BlazorChat.Shared;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorChat.Client.Managers
{
    public class ChatManager : IChatManager
    {
        private readonly HttpClient _httpClient;

        public ChatManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<ChatMessage>> GetConversationAsync(string contactId)
        {
            return await _httpClient.GetFromJsonAsync<List<ChatMessage>>($"api/chat/{contactId}");
        }
        public async Task<ApplicationUser> GetUserDetailsAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<ApplicationUser>($"api/chat/users/{userId}");
        }
        public async Task<List<ApplicationUserResult>> GetUsersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<ApplicationUserResult>>("api/chat/users");
            return response;
        }
        public async Task<int> GetTotalUnreadCountAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<int>("api/chat/unread-count");
            return response;
        }
        public async Task SaveMessageAsync(ChatMessage message)
        {
            await _httpClient.PostAsJsonAsync("api/chat", message);
        }

        public async Task<bool> ReadMessages(string contactId)
        {
            var response = await _httpClient.GetAsync($"api/chat/read/{contactId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReadAllMessages()
        {
            var response = await _httpClient.GetAsync($"api/chat/read");
            return response.IsSuccessStatusCode;
        }
    }
}
