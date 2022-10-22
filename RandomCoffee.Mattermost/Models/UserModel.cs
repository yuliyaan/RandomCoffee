using Newtonsoft.Json;

namespace RandomCoffee.Mattermost.Models
{
    /// <summary>
    /// Mattermost user info model
    /// </summary>
    public class UserModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
}