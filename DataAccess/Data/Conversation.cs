using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long? LastMessageId { get; set; }


        //ignored
        public int UnReadMessagesCount { get; set; }
        public Message LastMessage { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
