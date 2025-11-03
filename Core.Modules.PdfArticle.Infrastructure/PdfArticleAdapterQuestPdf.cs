using Core.Modules.PdfArticle.Domain.ViewModels;
using Core.Modules.PdfArticle.Infrastructure.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Core.Modules.PdfArticle.Infrastructure
{
    public class PdfArticleAdapterQuestPdf : IPdfArticleAdapter
    {
        private PdfArticleViewModel _pdfArticleModel = null!;
        private string _siteName = string.Empty;
        private bool _disposed = false;
        private readonly List<Action<ColumnDescriptor>> _contentActions = new();

        public void OpenDocument(PdfArticleViewModel articleModel, string siteName)
        {
            _pdfArticleModel = articleModel ?? throw new ArgumentNullException(nameof(articleModel));
            _siteName = siteName;
            _contentActions.Clear();
        }

        public void AddTitle(string title)
        {
            _contentActions.Add(column =>
            {
                string firstLetter = title[..1];
                string remainingText = title[1..];

                column.Item()
                    .Background("#2E363F")
                    .PaddingVertical(40)
                    .PaddingHorizontal(40)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span(firstLetter).FontColor(Colors.Orange.Medium).Bold().FontSize(20);
                        text.Span(remainingText).FontColor(Colors.White).Bold().FontSize(20);
                    });
            });
        }

        public void AddHeaderParagraphTop(string text)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .PaddingTop(20)
                    .Text(txt =>
                    {
                        txt.Span(text).Bold().FontSize(10).LineHeight(0.8f);
                    });
            });
        }

        public void AddHeaderParagraph(string text)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .Text(txt =>
                    {
                        txt.Span(text).FontSize(8).LineHeight(0.8f);
                    });
            });
        }

        public void AddHeaderParagraphBottom(string text)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .PaddingBottom(10)
                    .Text(txt =>
                    {
                        txt.Span(text).FontSize(10).LineHeight(0.8f);
                    });
            });
        }

        public void AddLineTop()
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .PaddingTop(20)
                    .PaddingBottom(5)
                    .LineHorizontal(1)
                    .LineColor(Colors.Grey.Medium);
            });
        }

        public void AddLineBottom()
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .PaddingTop(5)
                    .PaddingBottom(20)
                    .LineHorizontal(1)
                    .LineColor(Colors.Grey.Medium);
            });
        }

        public void AddAbstract(string text)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .Text(txt =>
                    {
                        txt.Span("Abstract:").Bold().FontSize(11);
                    });

                column.Item()
                    .Text(txt =>
                    {
                        txt.Span(text).FontSize(11);
                    });
            });
        }

        public void AddKeyWords(string text)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .Text(txt =>
                    {
                        txt.Span("Key words:").Bold().FontSize(11);
                    });

                column.Item()
                    .Text(txt =>
                    {
                        txt.Span(text).FontSize(11);
                    });
            });
        }

        public void AddSubtitule(string text, string tag)
        {
            _contentActions.Add(column =>
            {
                var fontSize = tag.Contains('3') ? 10 : 12;

                column.Item()
                    .PaddingTop(10)
                    .Text(txt =>
                    {
                        txt.Span(text.Trim()).Bold().FontSize(fontSize).FontColor(Colors.Grey.Darken3);
                    });
            });
        }

        public void AddCode(string text)
        {
            _contentActions.Add(column =>
            {
                var formatBody = text.Replace(" ", "\u00A0");

                column.Item()
                    .PaddingTop(10)
                    .Padding(10)
                    .Background("#2E363F")
                    .Text(txt =>
                    {
                        txt.Span(formatBody).FontFamily("Courier").FontSize(10).FontColor(Colors.White);
                    });
            });
        }

        public void AddBody(string text)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .PaddingTop(10)
                    .Text(txt =>
                    {
                        txt.Span(text.Trim()).FontSize(10).FontColor(Colors.Grey.Darken3);
                    });
            });
        }

        public void AddImage(byte[] imageBytes)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .Image(imageBytes);
            });
        }

        public void AddAuthorAbout(string text)
        {
            _contentActions.Add(column =>
            {
                column.Item()
                    .Text(txt =>
                    {
                        txt.Span(text).FontSize(9);
                    });
            });
        }

        public byte[] Build()
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken4));

                    page.Header()
                        .Height(30)
                        .AlignMiddle()
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .AlignLeft()
                                .Text(_siteName)
                                .FontSize(10)
                                .Italic()
                                .FontColor(Colors.Grey.Darken2);

                            row.RelativeItem()
                                .AlignRight()
                                .Text($"Volume {_pdfArticleModel?.PdfModel?.Volume ?? 0}, Number {_pdfArticleModel?.PdfModel?.Number ?? 0}")
                                .FontSize(8)
                                .Italic()
                                .FontColor(Colors.Grey.Medium);
                        });

                    page.Content()
                        .Column(column =>
                        {
                            foreach (var action in _contentActions)
                            {
                                action(column);
                            }
                        });

                    page.Footer()
                        .Height(30)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text(text =>
                        {
                            text.CurrentPageNumber().FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                });
            });

            return document.GeneratePdf();
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
                    _contentActions.Clear();
                }
                _disposed = true;
            }
        }
    }
}

