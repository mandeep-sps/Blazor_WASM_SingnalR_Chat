using BlazorChat.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChat.Client.Pages
{
    public partial class Chat
    {
        [CascadingParameter] public HubConnection hubConnection { get; set; }
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public string CurrentUserId { get; set; }
        [Parameter] public string CurrentUserEmail { get; set; }
        [Parameter] public string CurrentUserName { get; set; }
        public bool Theme { get; set; }
        public bool IsSendDisabled { get; set; } = true;

        bool isVisible = false;
        bool isVisibleChat = false;
        bool isVisibleContact = false;

        private List<ChatMessage> messages = new List<ChatMessage>();

        [CascadingParameter]
        public Task<AuthenticationState> AuthStat { get; set; }

        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(ContactId))
            {
                //Save Message to DB
                var chatHistory = new ChatMessage()
                {
                    Message = CurrentMessage,
                    ToUserId = ContactId,
                    CreatedDate = DateTime.Now

                };
                await _chatManager.SaveMessageAsync(chatHistory);
                chatHistory.FromUserId = CurrentUserId;
                await hubConnection.SendAsync("SendMessageAsync", chatHistory, CurrentUserEmail);
                CurrentMessage = string.Empty;
                IsSendDisabled = true;
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
        }
        protected override async Task OnInitializedAsync()
        {
            isVisible = true;
            var state = await ((ApiAuthenticationStateProvider)_stateProvider).GetAuthenticationStateAsync();
            var user = state.User;

            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder().WithUrl(_navigationManager.ToAbsoluteUri("/signalRHub")).Build();
            }
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
            hubConnection.On<ChatMessage, string>("ReceiveMessage", async (message, userName) =>
            {
                if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId) || (ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
                {

                    if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId))
                    {
                        messages.Add(new ChatMessage { Message = message.Message, CreatedDate = message.CreatedDate.AddHours(5).AddMinutes(30), FromUserId = CurrentUserId, FromUser = new ApplicationUser() { Id = CurrentUserId, Email = CurrentUserEmail, Name = CurrentUserName } });
                        await hubConnection.SendAsync("ChatNotificationAsync", $"New Message From {userName}", ContactId, CurrentUserId);
                    }
                    else if ((ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
                    {
                        messages.Add(new ChatMessage { Message = message.Message, CreatedDate = message.CreatedDate.AddHours(5).AddMinutes(30), FromUser = new ApplicationUser() { Id = ContactId, Email = ContactEmail, Name = ContactName } });
                    }
                    await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
                    StateHasChanged();
                }
            });

            await GetUsersAsync();


            CurrentUserId = user.Claims.Where(a => a.Type == "Id").Select(a => a.Value).FirstOrDefault();
            CurrentUserEmail = user.Claims.Where(a => a.Type == "Email").Select(a => a.Value).FirstOrDefault();
            CurrentUserName = user.Claims.Where(a => a.Type == "Name").Select(a => a.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(ContactId))
            {
                await LoadUserChat(ContactId);
            }

            isVisible = false;
        }
        public List<ApplicationUserResult> ChatUsers = new List<ApplicationUserResult>();
        [Parameter] public string ContactEmail { get; set; }
        [Parameter] public string ContactName { get; set; }
        [Parameter] public string ContactId { get; set; }
        public string abc { get; set; }

        public async Task LoadUserChat(string userId)
        {
            isVisibleChat = true;
            var contact = await _chatManager.GetUserDetailsAsync(userId);
            ContactId = contact.Id;
            ContactEmail = contact.Email;
            ContactName = contact.Name;
            _navigationManager.NavigateTo($"chat/{ContactId}");
            await _chatManager.ReadAllMessages(ContactId);
            messages = new List<ChatMessage>();
            messages = await _chatManager.GetConversationAsync(ContactId);
            ChatUsers = await _chatManager.GetUsersAsync();
            isVisibleChat = false;
        }
        private async Task GetUsersAsync(bool stateChanged = false)
        {
            isVisibleContact = true;
            ChatUsers = await _chatManager.GetUsersAsync();
            if (stateChanged)
            {
                StateHasChanged();
            }
            isVisibleContact = false;
        }

        public async Task Enter(KeyboardEventArgs e)
        {
            IsSendDisabled = string.IsNullOrEmpty(CurrentMessage) ? true : false;

            if ((e.Code == "Enter" || e.Code == "NumpadEnter") && !string.IsNullOrEmpty(CurrentMessage))
            {
                await SubmitAsync();
            }

        }

    }
}
