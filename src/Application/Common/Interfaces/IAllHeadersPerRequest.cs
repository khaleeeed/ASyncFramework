using System.Collections.Generic;

namespace ASyncFramework.Application.Common.Interfaces
{
    public interface IAllHeadersPerRequest
    {
        Dictionary<string, string> Headrs { get; }
    }
}