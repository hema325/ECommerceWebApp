using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Message
    {
        public long Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Value { get; set; }
        public int SenderId { get; set; }
        public bool IsRead { get; set; }
        public int ConversationId { get; set; }


        //ignored
        public User Sender { get; set; }
    }
}
