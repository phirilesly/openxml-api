using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OpenXML.Abstractions.Models
{
    public class FootnotesNode :BaseNode
    {
        public FootnotesNode(BaseNode baseNode)
      : base(baseNode)
        {

        }

        public string NodeType { get; set; }
        public int Order { get; set; }
        public string Content { get; set; }

        public override WordprocessingDocument Render(WordprocessingDocument doc, MainDocumentPart mainPart)
        {
            mainPart = doc.AddMainDocumentPart();
            var footPart = mainPart.AddNewPart<FootnotesPart>();
            footPart.Footnotes = new Footnotes();

            var footnote = new Footnote();
            footnote.Id = 1;
            var p2 = new Paragraph();
            var r2 = new Run();
            var t2 = new Text();
            t2.Text = Content;
            r2.Append(t2);
            p2.Append(r2);
            footnote.Append(p2);
            footPart.Footnotes.Append(footnote);

            var fref = new FootnoteReference();
            fref.Id = 1;
            var r3 = new Run();
            r3.RunProperties = new RunProperties();
            var s3 = new VerticalTextAlignment();
            s3.Val = VerticalPositionValues.Superscript;
            r3.RunProperties.Append(s3);
            r3.Append(fref);
            p2.Append(r3);

            return doc;
        }

    }
}
