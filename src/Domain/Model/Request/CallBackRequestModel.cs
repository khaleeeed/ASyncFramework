using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Request
{
    public class CallBackRequestModel
    {
        /// <summary>
        /// call back api
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public CallBackRequest CallBackServiceRequest { get; set; }

    }
}
