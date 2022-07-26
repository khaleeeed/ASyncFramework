using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Enums
{
    public enum MessageLifeCycle
    {        
        NewRequest=1,     
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
        ValidationError,
        SucceededWithoutCallBack,
        FailurePushToQueue,
        MessageDisposed,
        ExceptoinWhenUpdateStatus,
        ReSendRequestAfterTargetUsedAllRetries,
        ReSendRequestAfterCallBackUsedAllRetries,
        RePushRequest
    }
}