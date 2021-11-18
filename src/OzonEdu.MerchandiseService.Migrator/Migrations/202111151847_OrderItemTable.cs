using FluentMigrator;
using OzonEdu.MerchandiseService.Migrator.Extensions;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(202111151847)]
    public class OrderItemTable : Migration
    {
        private const string orderItemTableName = "order_items";
        public override void Up()
        {
            this.CreateTableIfNotExists(orderItemTableName)?
                .WithColumn("id").AsInt32().Identity().PrimaryKey()
                .WithColumn("order_id").AsInt32()
                .WithColumn("sku").AsInt64()
                .WithColumn("sku_description").AsString()
                .WithColumn("quantity").AsInt32()
                .WithColumn("status_id").AsInt32()
                .WithColumn("status_date").AsDateTime()
                .WithColumn("status_description").AsString();
        }

        public override void Down()
        {
            this.DeleteTableIfExists(orderItemTableName);
        }
    }
}