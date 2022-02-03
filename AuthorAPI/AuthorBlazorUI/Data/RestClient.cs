using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthorBlazorUI.Data {
    public class RestClient :IRestClient {
        private string requestUrl = "https://localhost:5003";

        public async Task<IList<T>> GetAsync<T>() {
            using HttpClient client = new HttpClient();
            string url = "";
            switch (typeof(T).Name) {
                case "Author":
                    url = "authors";
                    break;
                case "Book":
                    url = "books";
                    break;
            }

            HttpResponseMessage responseMessage = await client.GetAsync($"{requestUrl}/{url}");
            
            string result = await responseMessage.Content.ReadAsStringAsync();
            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception(result);

            List<T> items = JsonSerializer.Deserialize<List<T>>(result, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return items;
        } 
  
        public async Task<T> PostAsync<T>(T item) {
            using HttpClient client = new HttpClient();

            string itemAsJson = JsonSerializer.Serialize(item);
            StringContent content = new StringContent(
                itemAsJson, Encoding.UTF8, "application/Json");
            HttpResponseMessage responseMessage = await client.PostAsync($"{requestUrl}/authors", content);
            string result = await responseMessage.Content.ReadAsStringAsync();
            
            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception(result);

            T newItem = JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return newItem;
        }

        public async Task<T> PostAsync<T>(T item, int? id) {
            if (id == null) throw new Exception("Please input an id");
            using HttpClient client = new HttpClient();
            string itemAsJson = JsonSerializer.Serialize(item);
            StringContent content = new StringContent(
                itemAsJson, Encoding.UTF8, "application/Json");
            HttpResponseMessage responseMessage =
                await client.PostAsync($"{requestUrl}/books/{id}",
                    content);
            string result = await responseMessage.Content.ReadAsStringAsync();
            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception(result);

            T newItem = JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return newItem;
        }

        public async Task DeleteAsync<T>(int? isbn) {
            using HttpClient client = new HttpClient();
            if (isbn == null) throw new Exception("Please input an id");
            HttpResponseMessage responseMessage = await client.DeleteAsync($"{requestUrl}/books/{isbn}");
            if (!responseMessage.IsSuccessStatusCode) {
                string error = await responseMessage.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }
    }
}