using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string UserName { get; set; }

        public string SenderUsername { get; set; }
        
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime SentTime { get; set; }
        public int seen { get; set; } //1 for unseen and 0 for seen
    }
}