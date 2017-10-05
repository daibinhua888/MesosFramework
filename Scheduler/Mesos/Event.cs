using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class Event
    {
        public string type { get; set; }

        public ID framework_id { get; set; }

        public Subscribe subscribe { get; set; }

        public Subscribed subscribed { get; set; }

        public Offers offers { get; set; }

        public Accept accept { get; set; }
    }
}
