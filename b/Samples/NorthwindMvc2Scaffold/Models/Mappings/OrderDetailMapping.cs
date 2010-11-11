using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace NorthwindMvcScaffold.Models.Mappings
{
    public class OrderDetailMapping: IAutoMappingOverride<OrderDetail>
    {
        public void Override(AutoMapping<OrderDetail> mapping)
        {
            mapping.Table("`Order Details`");
            mapping.IgnoreProperty(o => o.Id);
            mapping.CompositeId()
                .KeyReference(o => o.Product, "ProductID")
                .KeyReference(o => o.Order, "OrderID");
        }
    }
}
