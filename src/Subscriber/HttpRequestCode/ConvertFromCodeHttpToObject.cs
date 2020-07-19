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
        private Dictionary<string, string> headers { get; set; }=new Dictionary<string, string>();
        private string url { get; set; }
        private string httpMethod { get; set; }
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

            
            HttpRequestMessage request = new HttpRequestMessage(new System.Net.Http.HttpMethod(app.httpMethod), app.url);
            request.Content = string.IsNullOrWhiteSpace(body) ? null : new StringContent(body, Encoding.UTF8, "application/json");
            foreach (var header in app.headers)
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
                url = "https://"+headerValue+url;
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
