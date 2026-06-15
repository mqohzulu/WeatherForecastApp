using ChinaImportPlatform.Api.Common;
using Xunit;

namespace ChinaImportPlatform.Api.Tests;

public class PagedResultTests
{
    [Fact]
    public void Create_ReturnsRequestedPage()
    {
        var source = Enumerable.Range(1, 25).ToList();

        var page2 = PagedResult<int>.Create(source, page: 2, pageSize: 10);

        Assert.Equal(2, page2.Page);
        Assert.Equal(10, page2.PageSize);
        Assert.Equal(25, page2.TotalItems);
        Assert.Equal(3, page2.TotalPages);
        Assert.Equal(Enumerable.Range(11, 10), page2.Items);
    }

    [Fact]
    public void Create_NormalisesInvalidPaging()
    {
        var source = Enumerable.Range(1, 5).ToList();

        var result = PagedResult<int>.Create(source, page: 0, pageSize: 0);

        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(5, result.Items.Count);
    }
}
