using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BoC.Extensions;
using Xunit;
using Xunit.Extensions;

namespace BoC.Tests.Extensions
{
    public class ExpressionExtensionFixture
    {
        [Fact]
        public void And_Expression_Should_Tighten_Filter()
        {
            var items = new[] {"item1", "item2", "item3", "item4"};

            Expression<Func<string, bool>> filter = s => s.StartsWith("item");
            filter = filter.And(s1 => s1.EndsWith("2"));

            var result = items.Where(filter.Compile());

            Assert.Equal(1, result.Count());
            Assert.Equal("item2", result.First());
        }

        [Fact]
        public void Or_Expression_Should_Modify_Filter()
        {
            var items = new[] { "item1", "item2", "item3", "item4" };

            Expression<Func<string, bool>> filter = s => s == "item2";
            filter = filter.Or(s => s == "item3");

            var result = items.Where(filter.Compile());

            Assert.Equal(2, result.Count());
            Assert.Contains("item2", result);
            Assert.Contains("item3", result);
        }
    }
}