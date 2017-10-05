using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Executor
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = new Task(BeginRegistAndListen);
            t.Start();
            Console.WriteLine("Started ...");
            Console.ReadLine();
        }

        private static StringBuilder sb = new StringBuilder();
        static async void BeginRegistAndListen()
        {
            HttpContent content = new StringContent(@"{
                                                       ""type""       : ""SUBSCRIBE"",
                                                       ""subscribe""  : {
                                                                            ""framework_info""  : {
                                                                                                    ""id"" :  ""mckay.framework.id.001"",
                                                                                                    ""user"" :  ""root"",
                                                                                                    ""name"" :  ""McKay Framework"",
                                                                                                    ""roles"":  [""McKay""],
                                                                                                    ""capabilities"" : [{""type"": ""MULTI_ROLE""}]
                                                                                                    }
                                                                          }
                                                      }")
            {
                Headers = {
                    ContentType = new MediaTypeHeaderValue("application/json"),
                }
            };

            var url = "http://120.27.18.218:5050/api/v1/scheduler";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    using (var body = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(body))
                    {
                        while (!reader.EndOfStream)
                        {
                            sb.Append(reader.ReadLine());

                            Console.WriteLine("================");
                            Console.Write(sb.ToString());
                        }
                    }
                }
            }
        }
    }
}
