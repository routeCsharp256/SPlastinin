using FluentMigrator;
using OzonEdu.MerchandiseService.Migrator.Extensions;

namespace OzonEdu.MerchandiseService.Migrator.Migrations
{
    [Migration(202111151905)]
    public class EmployeeTable : Migration
    {
        private const string employeeTableName = "employees";
        public override void Up()
        {
            this.CreateTableIfNotExists(employeeTableName)?
                .WithColumn("id").AsInt32().PrimaryKey()
                .WithColumn("firstname").AsString()
                .WithColumn("lastname").AsString()
                .WithColumn("middlename").AsString()
                .WithColumn("email").AsString();
        }

        public override void Down()
        {
            this.DeleteTableIfExists(employeeTableName);
        }
    }
}