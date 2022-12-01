using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OpenXML.Abstractions.Models
{
    public class EndnotesNode : BaseNode
    {
        public EndnotesNode(BaseNode baseNode)
           : base(baseNode)
        {

        }
        public string NodeType { get; set; }
        public int Order { get; set; }
        public string Content { get; set; }

        public WordprocessingDocument Render(WordprocessingDocument doc, MainDocumentPart mainPart)
        {
            new Document(new Body()).Save(mainPart);
            Body body = mainPart.Document.Body;

            body.Append(new Endnote(new Paragraph(new Run(new Text(Content)))));

            return doc;

        }

    }
}
