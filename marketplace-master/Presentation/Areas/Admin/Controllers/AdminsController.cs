using Domain.Interfaces.Repositories;
using MediatR;
using Presentation.Areas.Admin.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using V2.Application.Modules.UserAccounts;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeMaster]
    public class AdminsController : Controller
    {
        private readonly IPersonRepository personRepository;
        private readonly IMediator mediator;

        public AdminsController(IPersonRepository personRepository, IMediator mediator)
        {
            this.personRepository = personRepository;
            this.mediator = mediator;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var admins = personRepository.ListAdminPersons();
            return View(admins);
        }

        public async Task<ActionResult> Create(AdminCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var profileAndAuth = model.Create();
                await mediator.Send(new RegisterUserAndProfile(profileAndAuth, model.Password));
            }

            return RedirectToAction("Index");
        }
    }
}