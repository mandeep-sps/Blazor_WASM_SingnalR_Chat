using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BlazorChat.Shared
{
    public class ApplicationUser
    {
        public virtual ICollection<ChatMessage> ChatMessagesFromUsers { get; set; }
        public virtual ICollection<ChatMessage> ChatMessagesToUsers { get; set; }
        public ApplicationUser()
        {
            ChatMessagesFromUsers = new HashSet<ChatMessage>();
            ChatMessagesToUsers = new HashSet<ChatMessage>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Hash { get; set; }

        public DateTime AuditedOn { get; set; } = DateTime.Now;

    }
}
