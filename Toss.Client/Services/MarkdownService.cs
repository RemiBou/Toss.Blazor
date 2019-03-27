using System.Web;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace Toss.Client.Services
{
    public interface IMarkdownService
    {
        MarkupString ToHtml(string content);
    }
    public class MarkdownService : IMarkdownService
    {
        private readonly MarkdownPipeline pipeline;

        public MarkdownService()
        {
            this.pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Use<TargetLinkExtension>().Build();
        }
        public MarkupString ToHtml(string content)
        {
            return (MarkupString)Markdown.ToHtml(HttpUtility.HtmlEncode(content), this.pipeline);
        }
    }
}