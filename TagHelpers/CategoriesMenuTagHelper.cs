using Homework.Data;
using Homework.Data.Entities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace Homework.TagHelpers
{
    public class CategoriesMenuTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        private readonly ShopContext shopContext;
        private readonly IUrlHelperFactory urlHelperFactory;

        public CategoriesMenuTagHelper(ShopContext shopContext, IUrlHelperFactory urlHelperFactory)
        {
            this.shopContext = shopContext;
            this.urlHelperFactory = urlHelperFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.AddClass("main_nav", HtmlEncoder.Default);

            var ul = new TagBuilder("ul");
            var li = new TagBuilder("li");
            var a = new TagBuilder("a");
            a.MergeAttribute("id", "categories-container");
            a.MergeAttribute("class", "w-100 h-100");
            a.InnerHtml.Append("Product catalog");
            if (shopContext.Categories?.Any() is true)
            {
                var categories = await shopContext.Categories
                    .Where(category => !category.ParentCategoryId.HasValue)
                    .Include(category => category.ChildCategories)
                    .ToListAsync();
                
                li.InnerHtml.AppendHtml(a);
                li.InnerHtml.AppendHtml(await GenerateCategoriesAsync(categories));
            }
            ul.InnerHtml.AppendHtml(li); 
            output.Content.SetHtmlContent(ul);
        }

        private async Task<IHtmlContent> GenerateCategoriesAsync(IEnumerable<Category> categories)
        {
            var ul = new TagBuilder("ul");

            foreach (var category in categories)
            {
                var li = new TagBuilder("li");
                var a = new TagBuilder("a");
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                a.MergeAttribute("href", urlHelper.Action("List", "Shop", new { categoryName = category.Name }));
                a.InnerHtml.Append(category.Name);
                li.InnerHtml.AppendHtml(a);

                if (category.ChildCategories?.Any() is true)
                {
                    var i = new TagBuilder("i");
                    i.AddCssClass("fa");
                    i.AddCssClass("fa-caret-down");
                    li.InnerHtml.AppendHtml(i);
                    li.InnerHtml.AppendHtml(await GenerateCategoriesAsync(category.ChildCategories));
                }

                ul.InnerHtml.AppendHtml(li);
            }

            return ul;
        }
    }
}