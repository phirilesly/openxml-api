using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace OpenXML.Abstractions.Models
{
    [DataContract]
    public class SaveResult:ICommandData
    {
        [DataMember]
        public Guid EntityId { get; set; }
        [DataMember(Name ="url")]
        public string URL { get; set; }
    }
}
