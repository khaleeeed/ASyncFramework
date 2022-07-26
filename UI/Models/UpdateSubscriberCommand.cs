using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class UpdateSubscriberCommand
    {
        public string[] Url { get; set; }
        public List<byte[]> TimeStampCheck { get; set; }
    }
}
