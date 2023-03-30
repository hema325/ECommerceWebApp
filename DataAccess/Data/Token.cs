using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Token
    {
        public enum Purposes
        {
            ConfirmEmail,ConfirmPhoneNumber,ChangePassword
        }

        public string Value { get; set; }
        public Purposes Purpose { get;set; }
        public DateTime ValidTill { get; set; }
        public int UserId { get; set; }

    }
}
