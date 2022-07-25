using System;

namespace Domain.DTO
{
    public class ApiTokenResponse
    {
        public string token { get; set; }

        public ApiTokenResponse() { } 

        public ApiTokenResponse(string _token)
        {
            token = _token;
        }
    }
    
}
