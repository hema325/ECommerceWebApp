using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class UserConversation
    {
        public int UserId { get; set; }
        public int ConversationId { get; set; }
        public DateTime JoinDateTime { get; set; }
        public DateTime? LeftDateTime { get; set; }

    }
}
