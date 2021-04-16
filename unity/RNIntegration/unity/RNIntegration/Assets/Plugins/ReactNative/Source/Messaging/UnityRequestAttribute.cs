using System;
using UnityEngine.Scripting;

namespace ReactNative
{
    public class UnityMessageAttribute : PreserveAttribute
    {
        public UnityMessageAttribute(int messageID)
        {
            this.ID = messageID;
        }

        public UnityMessageAttribute(Enum messageID)
            : this(Convert.ToInt32(messageID)) { }

        public int ID { get; }
    }

    public sealed class UnityRequestAttribute : UnityMessageAttribute
    {
        public UnityRequestAttribute(int requestID, Type responseType)
            : base(requestID)
        {
            this.ResponseType = responseType;
        }

        public UnityRequestAttribute(Enum requestType, Type responseType)
            : this(Convert.ToInt32(requestType), responseType) { }

        public Type ResponseType { get; }
    }
}
