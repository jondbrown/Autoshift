using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AutoShift
{
    class Shift
    {
        private static CookieContainer cookieContainer;
        private static HttpClientHandler clienthandler;
        private string cookies;
        private string LatestToken;
        private HttpClient client;
        private const string baseUrl = "https://shift.gearboxsoftware.com";
        public Shift()
        {
            cookieContainer = new CookieContainer();
            clienthandler = new HttpClientHandler { AllowAutoRedirect = true, UseCookies = true, CookieContainer = cookieContainer };
            client = new HttpClient(clienthandler);
        }

        private async Task<string> GetTokenAsync(string url)
        {
            var response = await client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = response.Content.ReadAsStreamAsync();
                var doc = new HtmlDocument();
                doc.Load(await content);
                var meta = doc.DocumentNode.SelectSingleNode("/html/head/meta[@name='csrf-token']");
                var token = meta.GetAttributeValue("content", "no");
                return token;
            }
            
            return "";
        }

        public async Task<bool> LoginAsync(string user, string password)
        {
            var uri = new Uri($"{baseUrl}/sessions");
            var values = new Dictionary<string, string>
            {
                { "token", await GetTokenAsync($"{baseUrl}/home") },
                { "user[email]", user},
                { "user[password]", password },
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(uri, content);
            var responseContent = response.Content.ReadAsStreamAsync();
            var doc = new HtmlDocument();
            doc.Load(await responseContent);
            return false;
        }

        public async Task<bool> GetRedeemForm(string code)
        {
            var token = await GetTokenAsync($"{baseUrl}/code_redemptions/new");
            client.DefaultRequestHeaders.Add("X-CSRF-Token", token);
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            var response = await client.GetAsync($"{baseUrl}/entitlement_offer_codes?code={code}");
            var responseContent =  response.Content.ReadAsStreamAsync();
            var doc = new HtmlDocument();
            doc.Load(await responseContent);
            return false;
        }
    }
}
