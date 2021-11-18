using System.Linq;
using System.Reflection;
using FluentMigrator;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.SeedWork;
using OzonEdu.MerchandiseService.Migrator.Extensions;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(202111151854)]
    public class OrderStatusTable : Migration
    {
        private const string orderStatusesTableName = "order_statuses";
        public override void Up()
        {
            this.CreateTableIfNotExists(orderStatusesTableName)?
                .WithColumn("id").AsInt32().PrimaryKey()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("description").AsString().NotNullable();

            var allPredefinedStatuses = OrderStatus.GetAll<OrderStatus>();

            foreach (var status in allPredefinedStatuses)
            {
                Insert.IntoTable(orderStatusesTableName)
                    .Row(new {id = status.Id, name = status.Name, description = status.DefaultDescription});
            }
        }

        public override void Down()
        {
            this.DeleteTableIfExists(orderStatusesTableName);
        }
    }
}