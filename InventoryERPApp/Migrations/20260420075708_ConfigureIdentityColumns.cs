using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryERPApp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureIdentityColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reset identity columns to match existing data
            migrationBuilder.Sql(@"
                DECLARE @MaxUsersId INT;
                SELECT @MaxUsersId = ISNULL(MAX(Id), 0) FROM Users;
                DBCC CHECKIDENT ('Users', RESEED, @MaxUsersId);
                
                DECLARE @MaxRolesId INT;
                SELECT @MaxRolesId = ISNULL(MAX(Id), 0) FROM Roles;
                DBCC CHECKIDENT ('Roles', RESEED, @MaxRolesId);
                
                DECLARE @MaxTenantsId INT;
                SELECT @MaxTenantsId = ISNULL(MAX(Id), 0) FROM Tenants;
                DBCC CHECKIDENT ('Tenants', RESEED, @MaxTenantsId);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
