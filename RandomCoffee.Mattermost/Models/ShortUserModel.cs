using Newtonsoft.Json;

namespace RandomCoffee.Mattermost.Models
{
    /// <summary>
    /// Mattermost user short info model
    /// </summary>
    public class ShortUserModel
    {
        [JsonProperty("user_id")]
        public string Id { get; set; }
    }
}