using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Matterhook.NET.MatterhookClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RandomCoffee.Core.Abstractions;
using RandomCoffee.Mattermost.Models;

namespace RandomCoffee.Mattermost
{
    /// <summary>
    /// Connector to Mattermost
    /// </summary>
    public class MessengerGateway : IMessengerGateway
    {
        private readonly string _botLogin;
        private readonly string _botPassword;
        
        private readonly string _botAvatarUrl;
        private readonly string _botAssignedName;
        
        private readonly string _randomCoffeeChannel;
        private readonly string _baseApiUrl;

        private readonly MatterhookClient _matterhookClient;

        public MessengerGateway(IConfiguration configuration)
        {
            _botLogin = configuration["Mattermost:Username"];
            _botPassword = configuration["Mattermost:Password"];
            
            _botAvatarUrl = configuration["Mattermost:BotAvatarUrl"];
            _botAssignedName = configuration["Mattermost:BotName"];
            
            _randomCoffeeChannel = configuration["Mattermost:RandomCoffeeChannel"];
            _baseApiUrl = configuration["Mattermost:BaseUrl"] + "/api/v4";
            
            var webhookUrl = configuration["Mattermost:WebhookUrl"];
            _matterhookClient = new MatterhookClient(webhookUrl);
        }
        
        /// <inheritdoc />
        public async Task<string[]> GetRandomCoffeeParticipantIds()
        {
            var channelMembers = await GetRandomCoffeeChannelMembers();
            var allMembers = await GetAllMattermostUsers();

            var result = new List<string>();
            foreach (var channelMember in channelMembers)
            {
                var memberInfo = allMembers.FirstOrDefault(x => x.Id == channelMember.Id);
                if (memberInfo != null && memberInfo.Username != _botLogin)
                {
                    result.Add(memberInfo.Username);
                }
            }

            return result.ToArray();
        }
        
        /// <inheritdoc />
        public async Task<bool> SendRandomCoffeeMessage(List<string[]> groups)
        {
            var mattermostMessage = new MattermostMessage
            {
                Text = BuildText(groups),
                Channel = _randomCoffeeChannel,
                Username = _botAssignedName,
                IconUrl = _botAvatarUrl
            };

            var result = await _matterhookClient.PostAsync(mattermostMessage);

            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Prepare text for message in random coffee channel
        /// </summary>
        private static string BuildText(List<string[]> groups)
        {
            var coffeeEmoji = " :coffee: ";
            var textBuilder = new StringBuilder(groups.Count);

            groups.ForEach(group =>
            {
                var lineBuilder = new StringBuilder(group.Length);
                for (var userNum = 1; userNum <= group.Length; userNum++)
                {
                    lineBuilder.Append('@');
                    lineBuilder.Append(group[userNum - 1]);

                    if (userNum != group.Length)
                    {
                        lineBuilder.Append(coffeeEmoji);
                    }
                    else
                    {
                        lineBuilder.Append(' ');
                    }
                }

                textBuilder.AppendLine(lineBuilder.ToString());
            });

            var text = "Hi! It's time for Random Coffee :tada:" + Environment.NewLine +
                       "Your coffeemate of the week:" + Environment.NewLine + Environment.NewLine +
                    
                       textBuilder;

            return text;
        }

        private async Task<List<ShortUserModel>> GetRandomCoffeeChannelMembers()
        {
            var client = await GetOrCreateHttpClient();
            
            var result = await client.GetAsync(_baseApiUrl + "/channels/ijyqyeapnbdwzb8788fjig531a/members");
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Couldn't get members of ~random-coffee channel");
            }
            
            var stringResponse = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ShortUserModel>>(stringResponse);
        }

        private async Task<List<UserModel>> GetAllMattermostUsers()
        {
            var client = await GetOrCreateHttpClient();
            
            // Should switch to paging if you have more than 200 users in Mattermost
            var result = await client.GetAsync(_baseApiUrl + "/users?active=true&include_bots=false&per_page=200");
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Couldn't get the list of Mattermost users");
            }
            
            var stringResponse = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<UserModel>>(stringResponse);
        }

        #region Get HttpClient

        private HttpClient _client;
        
        private async Task<HttpClient> GetOrCreateHttpClient()
        {
            if (_client == null)
            {
                var httpClient = new HttpClient();
            
                var token = await GetApiToken(httpClient);
                if (token == null)
                {
                    throw new Exception("Couldn't get auth token");
                }
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                _client = httpClient;
            }

            return _client;
        }

        /// <summary>
        /// Get MatterMost api token
        /// </summary>
        private async Task<string> GetApiToken(HttpClient httpClient)
        {
            var form = new
            {
                login_id = _botLogin,
                password = _botPassword
            };

            var content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(_baseApiUrl + "/users/login", content);
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }
            
            var accessToken = result.Headers.GetValues("Token").FirstOrDefault();

            return accessToken;
        }
        
        #endregion
    }
}