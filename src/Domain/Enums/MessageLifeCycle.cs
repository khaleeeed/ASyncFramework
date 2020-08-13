using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Enums
{
    public enum MessageLifeCycle
    {
        NewRequest,
        PublishingTarget,
        ReceivedTarget,
        SendTargetRequest,
        RetryTarget,
        FinishInTarget,
        PublishingCallBack,
        ReceivedCallBack,
        SendCallBackRequest,
        RetryCallBack,
        FinishInCallBackFailure,
        FinishInSuccessed
    }
}
