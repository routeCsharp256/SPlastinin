using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Table;

namespace OzonEdu.MerchandiseService.Migrator.Extensions
{
    public static class MigrationExtensions
    {
        public static ICreateTableWithColumnOrSchemaOrDescriptionSyntax CreateTableIfNotExists(this Migration self,
            string tableName,
            string schemaName = "dbo")
        {
            if (!self.Schema.Schema(schemaName).Table(tableName).Exists())
            {
                return self.Create.Table(tableName);
            }

            return null;
        }

        public static IInSchemaSyntax DeleteTableIfExists(this Migration self,
            string tableName,
            string schemaName = "dbo")
        {
            if (self.Schema.Schema(schemaName).Table(tableName).Exists())
            {
                return self.Delete.Table("merch_orders");
            }

            return null;
        }
    }
}