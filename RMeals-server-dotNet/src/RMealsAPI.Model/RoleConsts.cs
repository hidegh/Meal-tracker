namespace RMealsAPI.Model
{
    public class RoleConsts
    {
        public const string Admin = "admin";
        public const string Manager = "manager";

        public static string[] RoleOrder = new string[] { RoleConsts.Manager, RoleConsts.Admin };
    }
}
