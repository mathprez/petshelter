using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace petshelterApi.Services
{
    //services.AddHttpClient<MailGunHttpClient>(client =>
    //        {
    //            var base64Token = Convert.ToBase64String(Encoding.UTF8.GetBytes("api" + ":" + Configuration["Mailgun:Token"]));
    //            client.BaseAddress = new Uri(Configuration["Mailgun:Audience"] + Configuration["Mailgun:Domain"]);
    //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Token);
    //        });

    public class MailGunHttpClient
    {
        private readonly HttpClient _client;
        public MailGunHttpClient(HttpClient client)
        {

            _client = client;
        }
        public async Task<HttpResponseMessage> SendSimpleMessage()
        {
            var form = new Dictionary<string, string>
            {
                ["from"] = "test@testmail.org",
                ["to"] = "test@testmail.com",
                ["subject"] = "Test",
                ["text"] = "testing testing"
            };

            return await _client.PostAsync("messages", new FormUrlEncodedContent(form));
        }

    }
    public class MailGunMail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }

}
