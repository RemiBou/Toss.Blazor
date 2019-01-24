using System.Web;
using Markdig;
using Microsoft.AspNetCore.Blazor;
public interface IMarkdownService
{
    MarkupString ToHtml(string content);
}
public class MarkdownService : IMarkdownService
{
    private MarkdownPipeline pipeline;

    public MarkdownService()
    {
        this.pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }
    public MarkupString ToHtml(string content)
    {
        return (MarkupString)Markdown.ToHtml(HttpUtility.HtmlEncode(content), this.pipeline);
    }
}