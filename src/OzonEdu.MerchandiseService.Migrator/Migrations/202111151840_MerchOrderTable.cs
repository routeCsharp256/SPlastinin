using System.Data.SqlServerCe;
using FluentMigrator;
using OzonEdu.MerchandiseService.Migrator.Extensions;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(202111151840)]
    public class MerchOrderTable : Migration
    {
        private const string merchOrderTableName = "merch_orders";
        public override void Up()
        {
            this.CreateTableIfNotExists(merchOrderTableName)?
                .WithColumn("id").AsInt32().Identity().PrimaryKey()
                .WithColumn("receiver_id").AsInt32()
                .WithColumn("manager_id").AsInt32()
                .WithColumn("status_id").AsInt32()
                .WithColumn("status_date").AsDateTime()
                .WithColumn("status_description").AsString();
        }

        public override void Down()
        {
            this.DeleteTableIfExists(merchOrderTableName);
        }
    }
}