using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.MesosOperators
{
    public class AcceptProcessor
    {
        private Offers offers;
        private string mesosId;
        private string framework_id;

        public AcceptProcessor(string framework_id, string mesosId, Offers offers)
        {
            this.framework_id = framework_id;
            this.offers = offers;
            this.mesosId = mesosId;
        }

        public void process()
        {
            Event evt = new Event();
            evt.framework_id = new ID() { value=this.framework_id };
            evt.type = "ACCEPT";
            evt.accept = new Accept();
            evt.accept.filters = new Filters() {  refuse_seconds= 31536000 };
            evt.accept = new Accept();

            List<ID> offer_ids = new List<ID>();
            List<Operation> operations = new List<Operation>();
            foreach (var offer in this.offers.offers)
            {
                ID offerId = new ID();

                offerId.value = offer.id.value;

                offer_ids.Add(offerId);


                Operation operation = new Operation();

                operation.type = "LAUNCH";
                operation.launch = new Launch();

                List<Task_Info> taskInfos = new List<Task_Info>();
                Task_Info taskInfo = new Task_Info();

                Guid taskId = Guid.NewGuid();
                taskInfo.name = "mckay task"+ taskId.ToString();
                taskInfo.task_id = new ID {value= "mckay-task-" + taskId.ToString() };
                taskInfo.agent_id = offer.agent_id;
                taskInfo.executor = new Executor();
                taskInfo.executor.executor_id=new ID { value = "mckay-executor-" + Guid.NewGuid().ToString() };
                taskInfo.executor.command = new Command();
                taskInfo.executor.command.shell = true;
                //taskInfo.executor.command.value = "echo 'hello222';echo 'hello333';exit 1";
                taskInfo.executor.command.value = "echo 'before';sleep 10s;echo 'done';";

                //List<Resource> resources = new List<Resource>();

                //Resource resource = new Resource();
                //resource.
                //resources.Add(resource);

                //taskInfo.resources = resources.ToArray();
                //taskInfo.resources =new Resource[] { offer.resources.First() };
                taskInfo.resources = new Resource[] {
                    new Resource(){ name="mem", type="SCALAR", scalar=new Scalar(){ value=32 } },
                    new Resource(){ name="cpus", type="SCALAR", scalar=new Scalar(){ value=0.1f } },
                };

                taskInfos.Add(taskInfo);

                operation.launch.task_infos = taskInfos.ToArray();

                operations.Add(operation);

                break;
            }
            evt.accept.offer_ids = offer_ids.ToArray();
            evt.accept.operations = operations.ToArray();



            string json = evt.ToJson();

            //File.WriteAllText("E:\\log\\accept_cmd.txt", json);

            HttpContent content = new StringContent(json)
            {
                Headers = {
                    ContentType = new MediaTypeHeaderValue("application/json"),
                }
            };
            content.Headers.Add("Mesos-Stream-Id", this.mesosId);


            var url = "http://"+ Config.MesosIp + "/api/v1/scheduler";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine("ACCEPT完成，返回：");
                Console.WriteLine(response);
            }
        }
    }
}
