using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace Core.Modules.PdfArticle.Infrastructure
{
    public class PdfPageEvent(int volume, int number, string header) : IEventHandler
    {
        private readonly string _header = header;
        private readonly int _volume = volume;
        private readonly int _number = number;
        private readonly PdfFont _headerFooterFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

        public void HandleEvent(Event currentEvent)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
            PdfPage page = docEvent.GetPage();

            // Adiciona cabeçalho e rodapé
            AddHeader(docEvent, page);
            AddFooter(docEvent, page);
        }

        private void AddHeader(PdfDocumentEvent docEvent, PdfPage page)
        {
            PdfCanvas canvas = new(page.NewContentStreamBefore(), page.GetResources(), docEvent.GetDocument());
            var pageSize = page.GetPageSize();
            float y = pageSize.GetTop() - 20;

            // Link à esquerda
            canvas.BeginText()
                  .SetFontAndSize(_headerFooterFont, 10)
                  .MoveText(pageSize.GetLeft() + 35, y)
                  .SetColor(ColorConstants.DARK_GRAY, true)
                  .ShowText(_header)
                  .EndText();

            // Volume e número à direita
            canvas.BeginText()
                  .SetFontAndSize(_headerFooterFont, 8)
                  .MoveText(pageSize.GetRight() - 110, y)
                  .SetColor(ColorConstants.GRAY, true)
                  .ShowText($"Volume {_volume}, Number {_number}")
                  .EndText();

            canvas.Release();
        }

        private void AddFooter(PdfDocumentEvent docEvent, PdfPage page)
        {
            PdfCanvas canvas = new(page.NewContentStreamBefore(), page.GetResources(), docEvent.GetDocument());
            var pageSize = page.GetPageSize();
            float x = (pageSize.GetWidth() / 2);
            float y = pageSize.GetBottom() + 20;

            int pageNumber = docEvent.GetDocument().GetPageNumber(page);

            // Número da página no centro
            canvas.BeginText()
                  .SetFontAndSize(_headerFooterFont, 10)
                  .MoveText(x, y)
                  .SetColor(ColorConstants.GRAY, true)
                  .ShowText(pageNumber.ToString())
                  .EndText();

            canvas.Release();
        }
    }
}