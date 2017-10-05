namespace Scheduler
{
    public class Resource
    {
        public Allocation_Info allocation_info { get; set; }
        public string name { get; set; }
        public string type { get; set; }

        public Scalar scalar { get; set; }
        public string role { get; set; }

        public Ranges ranges { get; set; }
    }
}