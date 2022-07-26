using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class RetryFailureCommand
    {
        public List<string> referenceNumbers { get; set; }
        public List<byte[]> timeStampChecks { get; set; }
    }
}
