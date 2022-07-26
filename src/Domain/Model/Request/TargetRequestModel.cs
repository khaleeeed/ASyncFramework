using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Request
{
    public class TargetRequestModel
    {
        /// <summary>
        /// target api 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public PushRequest TargetServiceRequest { get; set; }
    }
}
