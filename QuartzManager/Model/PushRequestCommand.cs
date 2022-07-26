using ASyncFramework.Domain.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzManager.Model
{
    public class PushRequestCommand
    {
        /// <summary>
        /// send queues id sperated by , for queue description call api/message/queue          
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public string Queues { get; set; }

        /// <summary>
        /// Target request service
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public TargetRequestModel TargetRequest { get; set; }

        /// <summary>
        /// CallBack request service most number can send two callback 
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public List<CallBackRequestModel> CallBackRequest { get; set; }

        /// <summary>
        /// check if request send before
        /// </summary>
        public bool IsUniqueRequest { get; set; }


    }
}
