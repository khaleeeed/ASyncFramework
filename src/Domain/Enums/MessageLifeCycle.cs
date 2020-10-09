using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Enums
{
    public enum MessageLifeCycle
    {        
        NewRequest,     
        PushToQueue,     
        ReceiveFromQueue,        
        SendRequestToTarget,     
        RetrySendingTarget,
        TargetUsedAllRetries,
        PushCallBackToQueue,
        ReceiveCallBackFromQueue,
        SendRequestToCallBack,
        RetrySendingCallBack,
        CallBackUsedAllRetries,
        Succeeded,
        ExceptoinWhenProcessNewRequest,
        ExceptoinWhenProcessMessage,
        ExceptoinWhenSendTargetRequest,
        ExceptoinWhenSendCallBackRequest,
        ValidationError
    }
}