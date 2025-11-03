using Core.Modules.PdfArticle.Domain.ViewModels;
using Core.Modules.PdfArticle.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace leaders.tec.br.Controllers
{
    public class PdfController : Controller
    {
        private readonly IPdfArticleAdapter _pdfAdapter;

        public PdfController(IPdfArticleAdapter pdfAdapter)
        {
            _pdfAdapter = pdfAdapter;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(string title, string author, string content)
        {
            try
            {
                var articleModel = new PdfArticleViewModel
                {
                    Title = title ?? "Título do Artigo",
                    Author = author ?? "Autor Desconhecido",
                    Subject = "Artigo Técnico",
                    Keywords = "PDF, Artigo, Tecnologia",
                    CreationDate = DateTime.Now
                };

                _pdfAdapter.OpenDocument(articleModel, "leaders.tec.br");
                _pdfAdapter.AddTitle(title ?? "Título do Artigo");
                _pdfAdapter.AddHeaderParagraph($"Por: {author ?? "Autor Desconhecido"}");
                _pdfAdapter.AddHeaderParagraph($"Data: {DateTime.Now:dd/MM/yyyy}");
                _pdfAdapter.AddLineTop();
                _pdfAdapter.AddAbstract("Este é um artigo de exemplo gerado pela plataforma leaders.tec.br, " +
                    "demonstrando o uso do módulo Core.Modules.PdfArticle.Infrastructure.");
                _pdfAdapter.AddKeyWords("PDF, Tecnologia, Open Source, C#, .NET");
                _pdfAdapter.AddSubtitule("Introdução", "h2");
                _pdfAdapter.AddBody(content ?? "Conteúdo do artigo não fornecido.");
                _pdfAdapter.AddLineBottom();
                _pdfAdapter.AddAuthorAbout($"Sobre o autor: {author ?? "Autor Desconhecido"}");

                var pdfBytes = _pdfAdapter.Build();

                return File(pdfBytes, "application/pdf", $"artigo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao gerar PDF: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}

