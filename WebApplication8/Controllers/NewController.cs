using Microsoft.AspNetCore.Mvc;
using WebApplication8.Infrastructure;

namespace WebApplication8.Controllers
{
    public class NewController: Controller
    {
        public NewController()
        {

        }

        public IActionResult About() 
        {

            return this.RedirectTo<DefaultController>(x => x.Index());

           // return RedirectToAction("Index", "Default");
        }
    }
}
