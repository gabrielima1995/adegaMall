using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.DTO
{
    public class ApiTokenRequest
    {
        public string seller_id_or_email { get; set; }
        public string name { get; set; }
    }
}