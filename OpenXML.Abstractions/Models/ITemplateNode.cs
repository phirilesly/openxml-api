using DocumentFormat.OpenXml.Packaging;

namespace OpenXML.Abstractions.Models
{
  
    public interface ITemplateNode
    {
        string NodeType { get; set; }
         int Order { get; set; }
         string Content { get; set; }

        WordprocessingDocument Render(WordprocessingDocument doc, MainDocumentPart mainPart);

    }
}
