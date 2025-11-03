using Core.Modules.PdfArticle.Domain.Models;

namespace Core.Modules.PdfArticle.Domain.ViewModels
{
    public class PdfArticleViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public PdfModel PdfModel { get; set; } = new PdfModel { Volume = 1, Number = 1 };
    }
}

