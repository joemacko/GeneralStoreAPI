namespace GeneralStoreAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "ProductSKU", "dbo.Products");
            DropIndex("dbo.Transactions", new[] { "ProductSKU" });
            AlterColumn("dbo.Customers", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Transactions", "ProductSKU", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Transactions", "ProductSKU");
            AddForeignKey("dbo.Transactions", "ProductSKU", "dbo.Products", "SKU", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "ProductSKU", "dbo.Products");
            DropIndex("dbo.Transactions", new[] { "ProductSKU" });
            AlterColumn("dbo.Transactions", "ProductSKU", c => c.String(maxLength: 128));
            AlterColumn("dbo.Products", "Name", c => c.String());
            AlterColumn("dbo.Customers", "LastName", c => c.String());
            AlterColumn("dbo.Customers", "FirstName", c => c.String());
            CreateIndex("dbo.Transactions", "ProductSKU");
            AddForeignKey("dbo.Transactions", "ProductSKU", "dbo.Products", "SKU");
        }
    }
}
