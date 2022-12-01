using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenXML.Abstractions.Models
{
    [DataContract]
    public class ResultMessage
    {
        private readonly string _code;
        private readonly string _message;
        private ResultMessageType _messageType;
        public ResultMessage(ResultMessageType messageType, string code, string message)
        {
            _messageType = messageType;
            _code = code;
            _message = message;
        }

        [DataMember(Name = "messageType")]
        public ResultMessageType MessageType => _messageType;

        [DataMember(Name = "message")]
        public string Message => _message;

        [DataMember(Name = "code")]
        public string Code => _code;
    }

    public enum ResultMessageType
    {
        Information = 1,
        Warning = 2,
        Error = 3
    }
}
