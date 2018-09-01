using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Areas.Members
{
    public class MembersAreaAttribute : AreaAttribute
    {
        public MembersAreaAttribute() : base("Members")
        {
        }
    }
}
