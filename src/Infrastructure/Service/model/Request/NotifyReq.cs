using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Service.model.Request
{
    public class NotifyReq
    {
        public NotifyReq()
        {
            SendTo = new SendTo();
            EmailModel = new EmailModel();
        }
        /// <summary>
        /// رمز القالب
        /// </summary>
        public int? TemplateId { get; set; }


        /// <summary>
        /// نوع الوسيط المستخدم للإرسال 
        /// </summary>
        public MediumTypes SendByMedium { get; set; }


        /// <summary>
        /// مستلم الرسالة
        /// </summary>
        public SendTo SendTo { get; set; }

        /// <summary>
        /// قائمة البياانات المرجعية  
        /// </summary>
        public Dictionary<string, string> RefrenceData { get; set; }

        /// <summary>
        /// بيانات ارسال الايميل
        /// </summary>
        public EmailModel EmailModel { get; set; }

    }
    /// <summary>
    /// أنواع الوسيط 
    /// </summary>
    public enum MediumTypes
    {
        /// <summary>
        /// رسالة على البريد الإلكتروني 
        /// </summary>
        Email = 3
    }


    public class SendTo
    {
        /// <summary>
        /// بريد الكتروني
        /// </summary>
        public string EmailAddress { get; set; }
    }

    /// <summary>
    /// بيانات البريد المرسل
    /// </summary>
    public class EmailModel
    {
 
        /// <summary>
        /// عنوان البريد الالكتروني
        /// </summary>
        public string Subject { get; set; }
        public string[] CCTo { get;  set; }
        public bool IsBodyHTML { get;  set; }
    }
}
