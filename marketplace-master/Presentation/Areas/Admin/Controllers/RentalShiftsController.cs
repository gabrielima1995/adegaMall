using Application.Modules.Products.Rental;
using Domain.StoreModule.Commands;
using Domain.StoreModule.Interfaces;
using Presentation.Areas.Admin.Models;
using System;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class RentalShiftsController : BaseController
    {
        private readonly RentalShiftsAppService appService;
        private readonly IRentalShiftQueries queries;

        public RentalShiftsController(
            RentalShiftsAppService appService,
            IRentalShiftQueries queries)
        {
            this.appService = appService;
            this.queries = queries;
        }

        public ActionResult Index()
        {
            var items = queries.ListRentalShifts();
            var model = new RentalShiftsIndexViewModel(items);
            return View(model);
        }

        [HttpPost]
        public JsonResult Create(RentalShiftsCreateOrEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var start = GetStartTime(model);
                var end = GetEndTime(model);

                var addCommand = new AddRentalShiftCommand(Guid.NewGuid(), model.Name, start.Hour, start.Minute, end.Hour, end.Minute);
                var result = appService.AddRentalShift(addCommand);

                if (result.IsFailure)
                    return JsonError(result.Errors.ToSimpleString());

                return JsonSuccess(new { redirectUrl = Url.Action("Index") });
            }

            return JsonErrorFromModelState();
        }

        [HttpPost]
        public JsonResult Edit(RentalShiftsCreateOrEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var start = GetStartTime(model);
                var end = GetEndTime(model);

                var updateCommand = new UpdateRentalShiftCommand(model.Id.Value, model.Name, start.Hour, start.Minute, end.Hour, end.Minute);
                var result = appService.UpdateRentalShift(updateCommand);

                if (result.IsFailure)
                    return JsonError(result.Errors.ToSimpleString());

                return JsonSuccess(new { redirectUrl = Url.Action("Index") });
            }

            return JsonErrorFromModelState();
        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            if (id == Guid.Empty)
                return JsonError("Id inválido");

            var removeCommand = new RemoveRentalShiftCommand(id);
            var result = appService.RemoveRentalShift(removeCommand);

            if (result.IsFailure)
                return JsonError(result.Errors.ToSimpleString());

            return JsonSuccess(new { redirectUrl = Url.Action("Index") });
        }

        private DateTime GetStartTime(RentalShiftsCreateOrEditViewModel model)
        {
            model.TryParseStartTime(out int hours, out int minutes);
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hours, minutes, 0);
        }

        private DateTime GetEndTime(RentalShiftsCreateOrEditViewModel model)
        {
            model.TryParseEndTime(out int hours, out int minutes);
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hours, minutes, 0);
        }
    }
}