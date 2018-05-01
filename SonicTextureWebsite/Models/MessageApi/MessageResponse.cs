using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.MessageApi
{
    public class MessageResponse
    {
        public string Email { get; set; }
        public bool HasMessageBeenSent { get; set; }
        public string FailureReason { get; set; }
    }
}