using System.Net.Http;

namespace ASyncFramework.Domain.Interface
{
    public interface IConvertFromCodeHttpToObject
    {
        HttpRequestMessage Convert(string codeHttp);
    }
}