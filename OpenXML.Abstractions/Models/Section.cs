using System.Runtime.Serialization;

namespace OpenXML.Abstractions.Models
{
    [DataContract]
    public class Section : BaseQueryModel
    {
        [DataMember]
        public string Title { get; set; }

       
        [DataMember]
        public List<BaseNode> Nodes { get; set; }

        [DataMember]
        public List<Section>? SubSection { get; set; }

        public Section()
        {
            Nodes = new List<BaseNode>();
        }
    }
}
