using System.Linq;
using System.Reflection;
using FluentMigrator;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Migrator.Extensions;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(4)]
    public class OrderItemStatusTable : Migration
    {
        private const string orderItemStatusesTableName = "order_item_statuses";

        public override void Up()
        {
            this.CreateTableIfNotExists(orderItemStatusesTableName)?
                .WithColumn("id").AsInt32().PrimaryKey()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("description").AsString().NotNullable();

            var allPredefinedStatuses = OrderItemStatus.GetAll<OrderItemStatus>();

            foreach (var status in allPredefinedStatuses)
            {
                Insert.IntoTable(orderItemStatusesTableName)
                    .Row(new {id = status.Id, name = status.Name, description = status.DefaultDescription});
            }
        }

        public override void Down()
        {
            this.DeleteTableIfExists(orderItemStatusesTableName);
        }
    }
}