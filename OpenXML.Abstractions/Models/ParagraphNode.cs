using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OpenXML.Abstractions.Models
{
    public class ParagraphNode : BaseNode
    {
        public ParagraphNode(BaseNode baseNode)
  : base(baseNode)
        {

        }
        public override WordprocessingDocument Render(WordprocessingDocument doc, MainDocumentPart mainPart)
        {

            new Document(new Body()).Save(mainPart);
            Body body = mainPart.Document.Body;

            body.Append(new Paragraph(
                new Run(
                    new Text(Content))));

            return doc;
        }
    }
}
