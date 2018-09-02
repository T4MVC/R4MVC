using Microsoft.AspNetCore.Mvc;

namespace AspNetFeatureFolders.Areas.Members
{
    public class MembersAreaAttribute : AreaAttribute
    {
        public MembersAreaAttribute() : base("Members")
        {
        }
    }
}
