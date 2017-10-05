namespace Scheduler
{
    public class Accept
    {
        public ID[] offer_ids { get; set; }
        public Operation[] operations { get; set; }
        public Filters filters { get; set; }
    }
}