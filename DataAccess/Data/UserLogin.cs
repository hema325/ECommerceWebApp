using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class UserLogin
    {
        public string ProviderKey { get; set; }
        public string LoginName { get; set; }
        public int UserId { get; set; }
    }
}
