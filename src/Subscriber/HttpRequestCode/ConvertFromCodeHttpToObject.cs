using ASyncFramework.Domain.Interface;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using HttpVersion = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpVersion;

namespace Subscriber.HttpRequestCode
{
    public class ConvertFromCodeHttpToObject : IHttpRequestLineHandler, IHttpHeadersHandler, IConvertFromCodeHttpToObject
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        string url = string.Empty;
        string httpMethod = string.Empty;
        public HttpRequestMessage Convert(string codeHttp)
        {

            byte[] requestRaw = Encoding.UTF8.GetBytes(codeHttp);
            ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(requestRaw);
            HttpParser<ConvertFromCodeHttpToObject> parser = new HttpParser<ConvertFromCodeHttpToObject>();
            ConvertFromCodeHttpToObject app = new ConvertFromCodeHttpToObject();

            parser.ParseRequestLine(app, buffer, out var consumed, out var examined);
            buffer = buffer.Slice(consumed);

            SequenceReader<byte> sequenceReader = new SequenceReader<byte>(buffer);
            parser.ParseHeaders(app, ref sequenceReader);
            buffer = sequenceReader.Sequence;
            buffer = buffer.Slice(consumed);
            string body = Encoding.UTF8.GetString(buffer.ToArray());

            
            HttpRequestMessage request = new HttpRequestMessage(new System.Net.Http.HttpMethod(httpMethod), url);
            request.Content = string.IsNullOrWhiteSpace(body) ? null : new StringContent(body, Encoding.UTF8, "application/json");
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            return request;
        }

        public void OnHeader(Span<byte> name, Span<byte> value)
        {
            var headerName = Encoding.UTF8.GetString(name);
            var headerValue = Encoding.UTF8.GetString(value);
            if (headerName == "Host")
            {
                url += headerValue;
            }
            else
            {
                headers.Add(headerName, headerValue);
            }

        }

        public void OnHeadersComplete()
        {

        }

        public void OnStartLine(Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod method, HttpVersion version, Span<byte> target, Span<byte> path, Span<byte> query, Span<byte> customMethod, bool pathEncoded)
        {
            httpMethod = method.ToString();
            url += $"{Encoding.UTF8.GetString(target)}";
        }
    }
}
