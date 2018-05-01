using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.MessageApi
{
    public class BusinessMessagesRequest
    {
        public string Email { get; set; }

        public string Message { get; set; }
    }
}