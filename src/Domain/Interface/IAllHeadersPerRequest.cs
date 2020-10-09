using System.Collections.Generic;

namespace ASyncFramework.Domain.Interface
{
    public interface IAllHeadersPerRequest
    {
        Dictionary<string, string> Headrs { get; }
    }
}