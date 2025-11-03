using Core.Modules.PdfArticle.Domain.ViewModels;

namespace Core.Modules.PdfArticle.Infrastructure.Interfaces
{
    public interface IPdfArticleAdapter : IDisposable
    {
        void AddTitle(string title);
        void OpenDocument(PdfArticleViewModel articleModel, string siteName);
        void AddHeaderParagraphTop(string text);
        void AddHeaderParagraph(string text);
        void AddHeaderParagraphBottom(string text);
        void AddLineTop();
        void AddLineBottom();
        void AddAbstract(string text);
        void AddKeyWords(string text);
        void AddSubtitule(string text, string tag);
        void AddCode(string text);
        void AddBody(string text);
        void AddImage(byte[] imageBytes);
        void AddAuthorAbout(string text);
        byte[] Build();
    }
}