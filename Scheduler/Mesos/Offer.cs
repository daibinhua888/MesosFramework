namespace Scheduler
{
    public class Offer
    {
        public Allocation_Info allocation_info { get; set; }
        public ID id { get; set; }
        public ID framework_id { get; set; }
        public ID agent_id { get; set; }
        public string hostname { get; set; }
        public Resource[] resources { get; set; }
        public Attribute[] attributes { get; set; }
        public ID[] executor_ids { get; set; }
    }
}