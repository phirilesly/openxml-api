using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OpenXML.Abstractions.Models
{
    public class FooterNode :  BaseNode
    {
        public FooterNode(BaseNode baseNode)
         : base(baseNode)
        {

        }
        public virtual WordprocessingDocument Render(WordprocessingDocument doc, MainDocumentPart mainPart)
        {
            new Document(new Body()).Save(mainPart);
            Body body = mainPart.Document.Body;

            body.Append(new Footer(new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Header1" }),
                new Run(new Text(Content)))));
            return doc;
        }
    }
}
