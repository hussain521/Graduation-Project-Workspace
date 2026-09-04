namespace Infrastructure.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            // Ensure the database is created
            context.Database.EnsureCreated();

            // Check if the roles already exist
            if (context.Roles.Any())
            {
                return; // Already seeded
            }

            // Insert the roles into the database
            var roles = new List<Role>
        {
            new Role { Id = Guid.Parse("6c0a68cd-35d8-4148-9614-09922371b6e9"), Name = "Categories Page" },
            new Role { Id = Guid.Parse("77f16bfb-3f04-4d03-8fa3-0abf0e2dbc9d"), Name = "Print Accounts" },
            new Role { Id = Guid.Parse("aad6f62d-d4ab-42f5-a217-118262224bf3"), Name = "Delete Items" },
            new Role { Id = Guid.Parse("64c43e7a-5fec-4cd9-bfce-15812dff3950"), Name = "Items Page" },
            new Role { Id = Guid.Parse("d0f327f7-279e-4fb0-81d7-28602b0c1ade"), Name = "Add Items" },
            new Role { Id = Guid.Parse("05e0a54d-867d-48c7-9c73-38b9e2848255"), Name = "Print Documents" },
            new Role { Id = Guid.Parse("d86a5d44-1c80-4e61-ba96-39e98aff163e"), Name = "Print Currencies" },
            new Role { Id = Guid.Parse("3d885ec8-228d-49cc-afc8-48e7a6d2a304"), Name = "Delete Documents" },
            new Role { Id = Guid.Parse("86e07bf5-a8f5-4bc5-8da3-4fe8ee74ad62"), Name = "Update Currencies" },
            new Role { Id = Guid.Parse("5e289d63-e692-40ce-bb63-52873250efa0"), Name = "Update UserRoles" },
            new Role { Id = Guid.Parse("d82cbff5-fa7a-48b7-8660-54bddbb026c4"), Name = "Delete Users" },
            new Role { Id = Guid.Parse("a070bca5-c563-481a-8f7e-5b5cfc82c3aa"), Name = "Add Accounts" },
            new Role { Id = Guid.Parse("14076ec2-fde4-4143-9b57-616c31868689"), Name = "Add Currencies" },
            new Role { Id = Guid.Parse("bdc947b2-caa7-456f-bdc3-63d09a63ceab"), Name = "Users Page" },
            new Role { Id = Guid.Parse("f8b970a1-0772-4852-ab45-647be737eb57"), Name = "Delete UserRoles" },
            new Role { Id = Guid.Parse("24c23b8f-7bd5-430b-b450-706203c55039"), Name = "Update Accounts" },
            new Role { Id = Guid.Parse("cb0e2e89-0166-43c3-bd0a-738df8cbb707"), Name = "Update Users" },
            new Role { Id = Guid.Parse("29131444-034c-45fb-b50e-776a35010f89"), Name = "Update Categories" },
            new Role { Id = Guid.Parse("a21a98d0-01f3-41ed-ae54-77e7a6b2e195"), Name = "Print Categories" },
            new Role { Id = Guid.Parse("4eed1947-a5b8-4798-b7df-864b77d8ddbb"), Name = "UserRoles Page" },
            new Role { Id = Guid.Parse("d5e22a6f-e5d6-40e8-841c-8c9094de3ad5"), Name = "Documents Page" },
            new Role { Id = Guid.Parse("6e38b409-696c-4bea-8f62-92ae4c1ea5f2"), Name = "Add Users" },
            new Role { Id = Guid.Parse("3e929652-c45f-46b3-8885-aad2b5a829f1"), Name = "Print Users" },
            new Role { Id = Guid.Parse("dd815a85-c667-424c-b3c1-b118b18514c5"), Name = "Currencies Page" },
            new Role { Id = Guid.Parse("7fbfd13a-ddcf-4dfd-a5b1-c3ca6495bbf9"), Name = "Add Documents" },
            new Role { Id = Guid.Parse("ef9088f1-9712-4092-a3dc-c6fa160e6829"), Name = "Update Items" },
            new Role { Id = Guid.Parse("c0b52466-417a-48da-a4d5-ce46f1c32605"), Name = "Delete Currencies" },
            new Role { Id = Guid.Parse("7cf25898-d61e-4a7d-b187-e10ef51793af"), Name = "Accounts Page" },
            new Role { Id = Guid.Parse("fd34cef0-0511-4a5f-8c66-e23066c7ee62"), Name = "Add UserRoles" },
            new Role { Id = Guid.Parse("e0528918-2f1e-4c54-9cd4-e952f07b3018"), Name = "Update Documents" },
            new Role { Id = Guid.Parse("725e669a-f59f-48b0-a1dc-ea38b2e1af4a"), Name = "Add Categories" },
            new Role { Id = Guid.Parse("02c00e53-3797-4695-90a4-f56810a53556"), Name = "Print UserRoles" },
            new Role { Id = Guid.Parse("ba52a426-1778-4dc3-a440-fae6d263896d"), Name = "Delete Accounts" },
            new Role { Id = Guid.Parse("33bf898c-e43a-4b49-b635-ffbb2a41816e"), Name = "Delete Categories" },
            new Role { Id = Guid.Parse("347cedeb-5978-4107-8588-ffdbc12711c5"), Name = "Print Items" },
        };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }
    }

}
