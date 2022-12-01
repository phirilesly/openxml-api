using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Newtonsoft.Json;
using OpenXML.Abstractions.Models;
using OpenXML.Abstractions.Repository;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.Net;


namespace OpenXML.API.Controllers
{
    [Route("api/docs-generator")]
    [ApiController]
    public class OpenXmlController : ControllerBase
    {
        private ISectionRepository _sectionRepository;
        private IReportTemplateRepository _reportTemplateRepository;
        private readonly IHttpClientFactory _clientFactory;
        private string _documentsURL;
        public OpenXmlController(ISectionRepository sectionRepository, IConfiguration configuration, IHttpClientFactory clientFactory, IReportTemplateRepository reportTemplateRepository)
        {
            _clientFactory = clientFactory;
            _sectionRepository = sectionRepository;
            _reportTemplateRepository = reportTemplateRepository;
         
        }


     
        [HttpPost]
        [Route("templates")]
        public async Task<ActionResult> SaveReportTemplate([FromBody] Template template)
        {
            template.Id = template.Id == Guid.Empty ? Guid.NewGuid() : template.Id;
            await _reportTemplateRepository.SaveModelAsync(template);
            return Ok();
        }

        [HttpGet]
        [Route("docs/{templateId}")]
        public async Task<ActionResult> GetReport(Guid templateId)
        {
            // load report template
            var template = await _reportTemplateRepository.LoadModelAsync(templateId);

            // generate report file
            var stream = new MemoryStream();
            WordprocessingDocument doc = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true);
            MainDocumentPart mainPart = doc.AddMainDocumentPart();
            foreach (var section in template.Sections)
            {
                doc = PrintSection(section, doc, mainPart);
            }
            doc.Close();
            stream.Seek(0, SeekOrigin.Begin);
            // upload report to storage
            var reportURL = await UploadFileAsync(stream, template.Id, "test.docx");

            return Ok(new SaveResult
            {
                EntityId = templateId,
                URL = reportURL
            });
        }

        [HttpPost]
        [Route("sections")]
        public async Task<ActionResult> AddSection([FromBody] Section section)
        {
            section.Id = section.Id == Guid.Empty ? Guid.NewGuid() : section.Id;
            await _sectionRepository.SaveModelAsync(section);
            var stream = new MemoryStream();
            using (WordprocessingDocument doc = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                PrintSection(section, doc, mainPart);
                doc.Close();

                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "test.docx");
            }
        }

        [HttpGet]
        [Route("sections/{Id}")]
        public async Task<IEnumerable<Section>> GetClientSections(Guid Id)
        {
            return await _sectionRepository.FindModelsAsync(new List<SearchParameter>
            {
                new SearchParameter{Name="ID",Value = Id.ToString()}
            });
        }

        [HttpGet]
        [Route("docs/{Id}")]
        public async Task<IEnumerable<Template>> GetClientReport(Guid Id)
        {
            return await _reportTemplateRepository.FindModelsAsync(new List<SearchParameter>
            {
                new SearchParameter{Name="ID",Value = Id.ToString()}
            });
        }



        private WordprocessingDocument PrintSection(Section section, WordprocessingDocument doc, MainDocumentPart mainPart)
        {

            var orderedNodes = section.Nodes.OrderBy(c => c.Order);
            foreach (var node in orderedNodes)
            {
                switch (node.NodeType.ToLower())
                {
                    case "header":
                        {
                            var headerNode = new HeaderNode(node);
                            headerNode.Render(doc, mainPart);
                            break;
                        }
                    case "paragraph":
                        {
                            var paragraphNode =new ParagraphNode(node);
                            paragraphNode.Render(doc, mainPart);
                            break;
                        }
                    case "footer":
                        {
                            var footerNode =new FooterNode(node);
                            footerNode.Render(doc, mainPart);
                            break;
                        }
                    case "footnotes":
                        {
                            var footerNode =new FootnotesNode(node);
                            footerNode.Render(doc, mainPart);
                            break;
                        }
                    case "endnotes":
                        {
                            var endNotes =new EndnotesNode(node);
                            endNotes.Render(doc, mainPart);
                            break;
                        }
                    case "image":
                        {
                            AddImage(doc, node.Content);
                            break;
                        }
                    default:
                        {
                            node.Render(doc, mainPart);
                            break;
                        }

                }
            }
            return doc;
        }

        private void AddImage(WordprocessingDocument doc, string content)
        {
            string fileName = @"C:\Users\dirk\Pictures\Saved Pictures\picture.png";
            // Add a main document part. 
            ImagePart imagePart = doc.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);

            WebRequest request = WebRequest.Create(content);
            WebResponse response = request.GetResponse();
            Stream returnStream = response.GetResponseStream();
            using (returnStream = response.GetResponseStream())
            {
                imagePart.FeedData(returnStream);
            }
            AddImageToBody(doc, doc.MainDocumentPart.GetIdOfPart(imagePart));
        }

        private void AddImageToBody(WordprocessingDocument doc, string relationshipId)
        {
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        private async Task<string> UploadFileAsync(MemoryStream ms, Guid Id, string fileName)
        {
            string resultData = string.Empty;
            //initialise client
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Clear();
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            //content type negotiation
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.1));

            //initialise
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _documentsURL))
            {
                using var form = new MultipartFormDataContent();
                using (var fileContent = new ByteArrayContent(ms.ToArray()))
                {
                    //add multi_part data
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    form.Add(fileContent, "file", fileName);
                    form.Add(new StringContent(fileName), "fileName");
                    form.Add(new StringContent(Convert.ToString(Id)), "entityId");
                    request.Content = form;

                    //send request
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        var stream = await response.Content.ReadAsStringAsync();
                        var saveResult = JsonConvert.DeserializeObject<CommandResult<UploadedDocument>>(stream);
                        resultData = saveResult.Resource.BlobUrl;
                    }
                }
            }
            return resultData;
        }
    }
}
