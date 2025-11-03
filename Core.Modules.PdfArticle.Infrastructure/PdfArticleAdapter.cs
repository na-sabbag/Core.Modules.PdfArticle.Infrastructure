using Core.Modules.PdfArticle.Domain.ViewModels;
using Core.Modules.PdfArticle.Infrastructure.Interfaces;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Core.Modules.PdfArticle.Infrastructure
{
    public class PdfArticleAdapter : IPdfArticleAdapter
    {
        private Document _document = null!;
        private PdfArticleViewModel _pdfArticleModel = null!;
        private MemoryStream _memoryStream = null!;
        private bool _disposed = false;
        public void AddTitle(string title)
        {
            var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Obtém o título e separa a primeira letra
            string firstLetter = title[..1];
            string remainingText = title[1..];

            // Cria um Text para a primeira letra com cor diferente
            var firstLetterText = new Text(firstLetter)
                .SetFont(titleFont)
                .SetFontSize(20)
                .SetFontColor(ColorConstants.ORANGE); // Cor diferente

            // Cria um Text para o restante do título
            var remainingTextObject = new Text(remainingText)
                .SetFont(titleFont)
                .SetFontSize(20)
                .SetFontColor(ColorConstants.WHITE);

            // Adiciona ambos ao Paragraph
            var titleParagraph = new Paragraph()
                .Add(firstLetterText)
                .Add(remainingTextObject)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingBottom(40)
                .SetPaddingTop(40)
                .SetPaddingLeft(60)
                .SetPaddingRight(60)
                .SetMarginLeft(-50)
                .SetMarginRight(-50)
                .SetBackgroundColor(new DeviceRgb(46, 54, 63));

            _document.Add(titleParagraph);
        }

        public void AddHeaderParagraphTop(string text)
        {
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            _document.Add(new Paragraph(text).SetFont(fontBold).SetFontSize(10).SetMultipliedLeading(0.8f).SetMarginTop(20));
        }

        public void AddHeaderParagraph(string text)
        {
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _document.Add(new Paragraph(text).SetFont(font).SetFontSize(8).SetMultipliedLeading(0.8f));
        }

        public void OpenDocument(PdfArticleViewModel articleModel, string siteName)
        {
            _memoryStream = new MemoryStream();
            _pdfArticleModel = articleModel ?? throw new ArgumentNullException(nameof(articleModel));

            var pdfDocument = new PdfDocument(new PdfWriter(_memoryStream));
            var eventHandler = new PdfPageEvent(_pdfArticleModel.PdfModel.Volume ?? 0, _pdfArticleModel.PdfModel.Number ?? 0, siteName);
            pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, eventHandler);
            _document = new Document(pdfDocument);
        }

        public byte[] Build()
        {
            // É crucial fechar o documento antes de obter os bytes
            // para garantir que o iText finalize a escrita do PDF
            _document.Close();
            
            var result = _memoryStream.ToArray();
            return result;
        }
        public void AddHeaderParagraphBottom(string text)
        {
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _document.Add(new Paragraph(text).SetFont(font).SetFontSize(10).SetMultipliedLeading(0.8f).SetMarginBottom(10));
        }

        public void AddLineTop()
        {
            var line = new SolidLine(1);
            line.SetColor(ColorConstants.GRAY);
            var lineSeparator = new LineSeparator(line);
            lineSeparator.SetMarginBottom(5).SetMarginTop(20);
            _document.Add(lineSeparator);
        }

        public void AddAbstract(string text)
        {
            var resumeFontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var resumeFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            _document.Add(new Paragraph("Abstract:").SetFont(resumeFontBold).SetFontSize(11));
            _document.Add(new Paragraph(text).SetFont(resumeFont).SetFontSize(11));
        }

        public void AddKeyWords(string text)
        {
            var resumeFontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var resumeFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            _document.Add(new Paragraph("Key words:").SetFont(resumeFontBold).SetFontSize(11));
            _document.Add(new Paragraph(text).SetFont(resumeFont).SetFontSize(11));
        }

        public void AddLineBottom()
        {
            var line = new SolidLine(1);
            line.SetColor(ColorConstants.GRAY);
            var lineSeparator = new LineSeparator(line);
            lineSeparator.SetMarginBottom(20).SetMarginTop(5);
            _document.Add(lineSeparator);
        }
        public void AddSubtitule(string text, string tag)
        {
            var fontSize = tag.Contains('3') ? 10 : 12;
            var subtitleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var subtitleParagraph = new Paragraph(text.Trim())
                .SetFont(subtitleFont)
                .SetFontSize(fontSize)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginTop(10);
            _document.Add(subtitleParagraph);
        }
        public void AddCode(string text)
        {
            var formatBody = text.Replace(" ", "\u00A0");
            var codeFont = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            var codeParagraph = new Paragraph(formatBody)
                .SetFont(codeFont)
                .SetFontSize(10)
                .SetFontColor(ColorConstants.WHITE)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginTop(10)
                .SetPadding(10)
                .SetBackgroundColor(new DeviceRgb(46, 54, 63));

            _document.Add(codeParagraph);
        }

        public void AddBody(string text)
        {
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            _document.Add(new Paragraph(text.Trim())
                .SetFont(font)
                .SetFontSize(10)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetMarginTop(10));
        }
        public void AddImage(byte[] imageBytes)
        {
            var imageData = ImageDataFactory.Create(imageBytes);
            var image = new iText.Layout.Element.Image(imageData);
            _document.Add(image);
        }

        public void AddAuthorAbout(string text)
        {
            var authorDescFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _document.Add(new Paragraph(text).SetFont(authorDescFont).SetFontSize(9));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _memoryStream?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}