using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KeepAlive.Controllers
{
    [Route("")]
    [ApiController]
    public class KeepAliveController : ControllerBase
    {
        private readonly IConfiguration _config;

        public KeepAliveController( IConfiguration config)
        {
            _config = config;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Alive()        
        {
            var urls = _config.GetSection("KeepAliveUrls").AsEnumerable().Where(a => !string.IsNullOrWhiteSpace(a.Value)).Select(a => a.Value).ToList();

            // we don't need to wait for all urls to return a value, we just fire and forget because our target is to hit the api
            await DoCalls(urls);


            return Ok("alive");
        }

        private async Task DoCalls(List<string> urls)
        {
            foreach (var url in urls)
            {
                try
                {
                    using HttpClient client = new HttpClient();

                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();

                    if (_config.GetValue<bool>("CheckIfSend"))
                        using (var fs = System.IO.File.Open($"log\\{DateTime.Now:yyyy-MM-dd}.txt", FileMode.Append))
                        using (var outputFile = new StreamWriter(fs))
                        {
                            outputFile.WriteLine(DateTime.Now + $" {url} {content}");
                        }
                }
                catch (Exception ex)
                {
                    if (_config.GetValue<bool>("CheckIfSend"))
                        using (var fs = System.IO.File.Open($"log\\{DateTime.Now:yyyy-MM-dd}Exception.txt", FileMode.Append))
                        using (var outputFile = new StreamWriter(fs))
                        {
                            outputFile.WriteLine("ex: " + ex.ToString() + "\n \n \n \n");
                        }
                }
            }
        }
    }
}