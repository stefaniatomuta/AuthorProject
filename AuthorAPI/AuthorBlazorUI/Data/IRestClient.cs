using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthorBlazorUI.Data {
    public interface IRestClient {
        public Task<IList<T>> GetAsync<T>();
        public Task<T> PostAsync<T>(T item);
        public Task<T> PostAsync<T>(T item, int? id);
        public Task DeleteAsync<T>(int? isbn);

    }
}