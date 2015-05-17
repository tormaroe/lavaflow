using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LavaFlow
{
    public class LavaFlowClient : IDisposable
    {
        private readonly string _baseUrl;
        private readonly string _eventsUrl;
        private readonly HttpClient _client;

        public LavaFlowClient(string address, int port = 3002)
        {
            _baseUrl = string.Format("http://{0}:{1}/", address, port);
            _eventsUrl = _baseUrl + "events";
            _client = new HttpClient();
        }

        private string EventsResource(string aggregate = null, string key = null)
        {
            if (aggregate == null) 
                return _eventsUrl;
            
            if (key == null) 
                return string.Format("{0}/{1}", _eventsUrl, aggregate);
            
            return string.Format("{0}/{1}/{2}", _eventsUrl, aggregate, key);
        }

        public async Task<IEnumerable<string>> GetAggregatesAsync()
        {
            var response = await _client.GetAsync(EventsResource());

            return JsonConvert.DeserializeObject<string[]>(
                await response
                    .EnsureSuccessStatusCode()
                    .Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string aggregate)
        {
            var response = await _client.GetAsync(
                EventsResource(aggregate: aggregate));

            return JsonConvert.DeserializeObject<string[]>(
                await response
                    .EnsureSuccessStatusCode()
                    .Content.ReadAsStringAsync());
        }

        public async Task<bool> PostEventAsync(string aggregate, string key, string eventData)
        {
            var response = await _client.PostAsync(
                EventsResource(aggregate: aggregate, key: key),
                new StringContent(eventData));

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<string>> GetEventsAsync(string aggregate, string key)
        {
            var response = await _client.GetAsync(
                EventsResource(aggregate: aggregate, key: key));

            return (await response.EnsureSuccessStatusCode()
                .Content.ReadAsStringAsync())
                .Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return ReadLines(await response
                .EnsureSuccessStatusCode()
                .Content.ReadAsStreamAsync());
        }

        private IEnumerable<string> ReadLines(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
