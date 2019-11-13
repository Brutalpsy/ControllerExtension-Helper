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
            var id = 5;
            return this.RedirectTo<DefaultController>(x => x.Index(id,"my test"));

           // return RedirectToAction("Index", "Default");
        }
    }
}
