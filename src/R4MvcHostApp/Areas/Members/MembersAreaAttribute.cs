using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Areas.Members
{
    public class MembersAreaAttribute : AreaAttribute
    {
        public MembersAreaAttribute() : base("Members")
        { }
    }
}
