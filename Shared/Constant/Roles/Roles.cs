namespace Shared.Constant.Roles
{
    public class Categories
    {
        public const string Page = "6c0a68cd-35d8-4148-9614-09922371b6e9";
        public const string Add = "725e669a-f59f-48b0-a1dc-ea38b2e1af4a"; // Example GUID for Add
        public const string Update = "29131444-034c-45fb-b50e-776a35010f89"; // Example GUID for Update
        public const string Delete = "33bf898c-e43a-4b49-b635-ffbb2a41816e"; // Example GUID for Delete
        public const string Print = "a21a98d0-01f3-41ed-ae54-77e7a6b2e195";

        public static Dictionary<string, string> GetRoles() => new()
        {
            { "Categories Page", Page },
            { "Add Categories", Add },
            { "Update Categories", Update },
            { "Delete Categories", Delete },
            { "Print Categories", Print }
        };
    }

    public class Accounts
    {
        public const string Page = "7cf25898-d61e-4a7d-b187-e10ef51793af"   ;
        public const string Add = "a070bca5-c563-481a-8f7e-5b5cfc82c3aa";
        public const string Update = "24c23b8f-7bd5-430b-b450-706203c55039";
        public const string Delete = "ba52a426-1778-4dc3-a440-fae6d263896d";
        public const string Print = "77f16bfb-3f04-4d03-8fa3-0abf0e2dbc9d";

        public static Dictionary<string, string> GetRoles() => new()
        {
            { "Accounts Page", Page },
            { "Add Accounts", Add },
            { "Update Accounts", Update },
            { "Delete Accounts", Delete },
            { "Print Accounts", Print }
        };
    }

    public class Items
    {
        public const string Page = "64c43e7a-5fec-4cd9-bfce-15812dff3950";
        public const string Add = "d0f327f7-279e-4fb0-81d7-28602b0c1ade"                ;
        public const string Update = "ef9088f1-9712-4092-a3dc-c6fa160e6829";
        public const string Delete = "aad6f62d-d4ab-42f5-a217-118262224bf3";
        public const string Print = "347cedeb-5978-4107-8588-ffdbc12711c5";

        public static Dictionary<string, string> GetRoles() => new()
        {
            { "Items Page", Page },
            { "Add Items", Add },
            { "Update Items", Update },
            { "Delete Items", Delete },
            { "Print Items", Print }
        };
    }

    public class UserRoles
    {
        public const string Page = "4eed1947-a5b8-4798-b7df-864b77d8ddbb";
        public const string Add = "fd34cef0-0511-4a5f-8c66-e23066c7ee62";
        public const string Update = "5e289d63-e692-40ce-bb63-52873250efa0";
        public const string Delete = "f8b970a1-0772-4852-ab45-647be737eb57";
        public const string Print = "02c00e53-3797-4695-90a4-f56810a53556";

        public static Dictionary<string, string> GetRoles() => new()
        {
            { "UserRoles Page", Page },
            { "Add UserRoles", Add },
            { "Update UserRoles", Update },
            { "Delete UserRoles", Delete },
            { "Print UserRoles", Print }
        };
    }

    public class Documents
    {
        public const string Page = "d5e22a6f-e5d6-40e8-841c-8c9094de3ad5";
        public const string Add = "7fbfd13a-ddcf-4dfd-a5b1-c3ca6495bbf9";
        public const string Update = "e0528918-2f1e-4c54-9cd4-e952f07b3018";
        public const string Delete = "3d885ec8-228d-49cc-afc8-48e7a6d2a304";

        public static Dictionary<string, string> GetRoles() => new()
        {
            { "Documents Page", Page },
            { "Add Documents", Add },
            { "Update Documents", Update },
            { "Delete Documents", Delete }
        };
    }

    public class Currencies
    {
        public const string Page = "dd815a85-c667-424c-b3c1-b118b18514c5";
        public const string Add = "14076ec2-fde4-4143-9b57-616c31868689";
        public const string Update = "86e07bf5-a8f5-4bc5-8da3-4fe8ee74ad62";
        public const string Delete = "c0b52466-417a-48da-a4d5-ce46f1c32605";
        public const string Print = "d86a5d44-1c80-4e61-ba96-39e98aff163e";

        public static Dictionary<string, string> GetRoles() => new()
        {
            { "Currencies Page", Page },
            { "Add Currencies", Add },
            { "Update Currencies", Update },
            { "Delete Currencies", Delete },
            { "Print Currencies", Print }
        };
    }

    public class Users
    {
        public const string Page = "bdc947b2-caa7-456f-bdc3-63d09a63ceab";
        public const string Add = "6e38b409-696c-4bea-8f62-92ae4c1ea5f2";
        public const string Update = "cb0e2e89-0166-43c3-bd0a-738df8cbb707";
        public const string Delete = "d82cbff5-fa7a-48b7-8660-54bddbb026c4";
        public const string Print = "3e929652-c45f-46b3-8885-aad2b5a829f1";

        public static Dictionary<string, string> GetRoles() => new()
        {
            { "Users Page", Page },
            { "Add Users", Add },
            { "Update Users", Update },
            { "Delete Users", Delete },
            { "Print Users", Print }
        };
    }

    public static class RolesManager
    {
        public static List<string> GetAllRolesIds()
        {
            var allRoleIds = new List<string>();

            void AddRoleIds(Dictionary<string, string> roles)
            {
                foreach (var role in roles)
                {
                    allRoleIds.Add(role.Value); // Add the GUID (role ID) to the list
                }
            }

            AddRoleIds(Currencies.GetRoles());
            AddRoleIds(Users.GetRoles());
            AddRoleIds(Accounts.GetRoles());
            AddRoleIds(Categories.GetRoles());
            AddRoleIds(Items.GetRoles());
            AddRoleIds(UserRoles.GetRoles());
            AddRoleIds(Documents.GetRoles());

            return allRoleIds;
        }
    }
}