using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskManager. Models
{
    public class Token
    {
        public string token { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("userid")]
        public string UserId { get; set; }
        
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }
    }
}
