using myProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;


namespace myProject.Mvc.Helpers.TagHelpers;

public class PaginationTagHelper : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    public PaginationTagHelper(IUrlHelperFactory urlHelperFactory)
    {
        _urlHelperFactory = urlHelperFactory;
    }

    public PageInfo PageInfo { get; set; }
    public string PageAction { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }


    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
        var result = new TagBuilder("div");
        

        var currentPage = 0;

        if (ViewContext.HttpContext.Request.Query.ContainsKey("page") && int.TryParse(
                ViewContext.HttpContext.Request.Query["page"],
                out var actualPage))
        {
            currentPage = actualPage;
        }

        // Determine the starting and ending page numbers
        var startPage = currentPage - 2;
        var endPage = currentPage + 2;
        if (startPage <= 0)
        {
            endPage -= (startPage - 1);
            startPage = 1;
        }

        if (endPage > PageInfo.TotalPages)
        {
            endPage = PageInfo.TotalPages;
            if (endPage > 8)
            {
                startPage = endPage - 7;
            }
        }

        TagBuilder tag;
        string anchorInnerHtml;
        if (startPage > 1)
        {
            tag = new TagBuilder("a");
            anchorInnerHtml = "«";
            tag.AddCssClass("btn btn-outline-primary");
            tag.Attributes["href"] = urlHelper.Action(PageAction, new { page = 1 });
            tag.InnerHtml.Append(anchorInnerHtml);
            result.InnerHtml.AppendHtml(tag);
        }
        for (int i = startPage; i <= endPage; i++)
        {
            tag = new TagBuilder("a");
            anchorInnerHtml = i.ToString();
            tag.AddCssClass("btn btn-outline-primary");
            if (i == currentPage)
            {
                tag.AddCssClass("active");
            }
            tag.Attributes["href"] = urlHelper.Action(PageAction, new { page = i });
            tag.InnerHtml.Append(anchorInnerHtml);
            result.InnerHtml.AppendHtml(tag);
        }
        if (endPage < PageInfo.TotalPages)
        {
            tag = new TagBuilder("a");
            anchorInnerHtml = "»";
            tag.AddCssClass("btn btn-outline-primary");
            tag.Attributes["href"] = urlHelper.Action(PageAction, new { page = PageInfo.TotalPages });
            tag.InnerHtml.Append(anchorInnerHtml);
            result.InnerHtml.AppendHtml(tag);
        }
        output.TagName = "div";

        output.Attributes.SetAttribute("class", "pagination-btns");
        output.Content.AppendHtml(result.InnerHtml);
    }
}