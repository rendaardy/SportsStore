using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;
using Moq;
using SportsStore.Infrastructure;
using SportsStore.Models.ViewModels;

namespace SportsStore.Tests
{
    public class PageLinkTagHelperTests
    {
        [Fact]
        public void CanGeneratePageLinks()
        {
            Mock<IUrlHelper> urlHelper = new();
            urlHelper.SetupSequence(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("Test/Page1")
                .Returns("Test/Page2")
                .Returns("Test/Page3");

            Mock<IUrlHelperFactory> urlHelperFactory = new();
            urlHelperFactory.Setup(f =>
                f.GetUrlHelper(It.IsAny<ActionContext>()))
                    .Returns(urlHelper.Object);

            PageLinkTagHelper helper = new(urlHelperFactory.Object)
            {
                PageModel = new PagingInfo {
                    CurrentPage = 2,
                    TotalItems = 28,
                    ItemsPerPage = 10,
                },
                PageAction = "Test",
            };

            TagHelperContext ctx = new(new TagHelperAttributeList(), new Dictionary<object, object>(), "");

            Mock<TagHelperContent> content = new();
            TagHelperOutput output = new("div", new TagHelperAttributeList(),
                (cache, encoder) => Task.FromResult(content.Object));

            helper.Process(ctx, output);

            Assert.Equal(@"<a href=""Test/Page1"">1</a>"
                + @"<a href=""Test/Page2"">2</a>"
                + @"<a href=""Test/Page3"">3</a>",
                output.Content.GetContent());
        }
    }
}
