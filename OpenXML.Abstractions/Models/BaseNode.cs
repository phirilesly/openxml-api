using DocumentFormat.OpenXml.Packaging;

namespace OpenXML.Abstractions.Models
{
    public class BaseNode : ITemplateNode
    {
        public BaseNode()
        {

        }
        public BaseNode(BaseNode baseNode)
        {
            NodeType = baseNode.NodeType;
            Order = baseNode.Order;
            Content = baseNode.Content;
        }

        public string NodeType { get; set; }
        public int Order { get; set; }
        public string Content { get; set; }

        public virtual WordprocessingDocument Render(WordprocessingDocument doc, MainDocumentPart mainPart)
        {
         return doc;
        }
    }
}
