namespace bekokkonen.pro.Providers
{
    public interface IRequestProvider
    {
        Task<TResult?> GetAsync<TResult>(string clientName, string url);
        Task<string> GetStringAsync(string clientName, string url);
        Task PostAsync<TRequest>(string clientName, string url, TRequest data);
        Task PostAsync(string clientName, string url);
        Task<TResult?> PostAsync<TResult>(string clientName, string url);
        public Task<TResult?> PostAsync<TRequest, TResult>(string clientName, string url, TRequest data);

        Task<TResult?> DeleteAsync(string clientName, string url);
    }
}