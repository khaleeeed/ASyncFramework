using System.Threading.Tasks;

namespace UI.ConsumeApi
{
    public interface ICallService
    {
        Task<T> CallGet<T>(string url, string token = null);
        Task<T> CallPost<T>(string url,string body, string token = null);
    }
}