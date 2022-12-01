using System.Runtime.Serialization;

namespace OpenXML.Abstractions.Models
{
    [DataContract]
    public class Template: BaseQueryModel
    {     


        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<Section> Sections { get; set; }
    }
}
