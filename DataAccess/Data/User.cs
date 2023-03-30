using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public DateTime LastSeen { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime LockOutEnd { get; set; }
        public bool IsBlocked { get; set; }
        public string ImgUrl { get; set; }
        public bool IsOnline { get; set; }


        //ignored

        public int ConvsHasUnReadMsgsCount { get; set; }
        public Conversation Conversation { get; set; }
        public List<Role> Roles { get; set; }
        public List<Address> Addresses { get; set; }
    }
}
