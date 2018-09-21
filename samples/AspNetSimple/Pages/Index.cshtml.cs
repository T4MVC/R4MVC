using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetSimple.Pages
{
    public partial class IndexModel : PageModel
    {
        public virtual void OnGet()
        {
        }

        public virtual void OnPost(int id)
        {
        }

        public virtual void OnPostTest(int id)
        {
        }

        public virtual Task OnPostDeleteAsync()
        {
            return Task.CompletedTask;
        }
    }
}
