namespace Scheduler
{
    public class Task_Info
    {
        public string name { get; set; }
        public ID task_id { get; set; }
        public ID agent_id { get; set; }
        public Executor executor { get; set; }

        public Resource[] resources { get; set; }
    }
}