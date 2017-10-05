using Scheduler.MesosOperators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Task subCmd = new Task(ExecuteSubscribeCommand);
            subCmd.Start();

            Task parseEvent = new Task(ParseIncomeEvent);
            parseEvent.Start();

            Console.WriteLine("Started ...");
            while (true)
            {
                Console.ReadLine();
                //File.WriteAllText("E:\\log\\1.txt", sb.ToString());
                Console.WriteLine("DUMPED");
            }
        }

        private static StringBuilder sb = new StringBuilder();

        private static string mesos_stream_Id = string.Empty;
        private static string framework_id = string.Empty;
        private static async void ExecuteSubscribeCommand()
        {
            HttpContent content = new StringContent(@"{
                                                       ""type""       : ""SUBSCRIBE"",
                                                       ""subscribe""  : {
                                                                            ""framework_info""  : {
                                                                                                    ""user"" :  ""root"",
                                                                                                    ""name"" :  ""McKay Framework"",
                                                                                                    ""roles"":  [""*""],
                                                                                                    ""capabilities"" : [{""type"": ""MULTI_ROLE""}]
                                                                                                    }
                                                                          }
                                                      }")
            {
                Headers = {
                    ContentType = new MediaTypeHeaderValue("application/json"),
                }
            };


            var url = "http://"+ Config.MesosIp + "/api/v1/scheduler";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                using (var response = await client.SendAsync(request,HttpCompletionOption.ResponseHeadersRead))
                {
                    mesos_stream_Id = response.Headers.GetValues("Mesos-Stream-Id").First();

                    using (var body = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(body))
                    {
                        while (!reader.EndOfStream)
                        {
                            sb.Append(reader.ReadLine());
                        }
                    }
                }
            }
        }

        private static int currentPosition = 0;
        private static string lengthBytes=string.Empty;
        private static void ParseIncomeEvent()
        {
            while (true)
            {
                do
                {
                    lengthBytes = readline();
                    Thread.Sleep(200);
                }while (lengthBytes.Length < 1);

                int messageLength = int.Parse(lengthBytes);

                string messages = string.Empty;

                while (messages.Length == 0)
                {
                    messages = read(messageLength);
                    Thread.Sleep(200);
                }

                process(messages);
            }
        }

        private static void process(string messages)
        {
            Event evt = messages.ToObject<Event>();
            Console.WriteLine(evt.type);
            switch (evt.type)
            {
                case "SUBSCRIBED":
                    framework_id = evt.subscribed.framework_id.value;
                    break;
                case "OFFERS":
                    Console.WriteLine("OFFERS RECEIVED...");
                    AcceptProcessor processor = new AcceptProcessor(framework_id, mesos_stream_Id, evt.offers);
                    processor.process();
                    break;
            }

        }

        private static string read(int messageLength)
        {
            if (sb.Length == 0)
                return string.Empty;

            string remains = sb.ToString().Substring(currentPosition);

            if (remains.Length == 0)
                return string.Empty;

            if (remains.Length < messageLength)
                return string.Empty;

            currentPosition += messageLength;

            return remains.Substring(0, messageLength);
        }

        private static string readline()
        {
            //必须找到了回车才能返回字符，否则返回string.Empty
            if (sb.Length == 0)
                return string.Empty;

            if(sb.Length<=currentPosition)
                return string.Empty;

            string remains = sb.ToString().Substring(currentPosition);

            var digitalCount = 0;
            var nonDigitalCount = 0;

            foreach (var c in remains.ToArray())
            {
                if (c.IsDigital())
                { 
                    digitalCount+=1;
                }
                else
                {
                    nonDigitalCount += 1;
                    break;
                }
            }

            if (digitalCount == 0)
                return string.Empty;

            if (nonDigitalCount == 0)
                return string.Empty;

            StringBuilder tmp_sb = new StringBuilder();
            remains.Take(digitalCount).ToList().ForEach(c=>{
                tmp_sb.Append(c);
            });

            currentPosition += digitalCount;

            return tmp_sb.ToString();
        }
    }
}
