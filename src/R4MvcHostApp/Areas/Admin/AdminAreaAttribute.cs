using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Areas.Admin
{
    public class AdminAreaAttribute : AreaAttribute
    {
        public AdminAreaAttribute() : base("Admin")
        { }
    }
}
