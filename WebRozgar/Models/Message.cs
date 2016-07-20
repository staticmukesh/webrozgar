using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRozgar.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string SenderUsername { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentTime { get; set; }
        public int seen { get; set; } //1 for unseen and 0 for seen
    }
}