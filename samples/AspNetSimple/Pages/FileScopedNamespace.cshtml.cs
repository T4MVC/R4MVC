namespace AspNetSimple.Pages;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public partial class FileScopedNamespaceModel : PageModel
{
    public virtual IActionResult OnGet()
    {
        return Page();
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
