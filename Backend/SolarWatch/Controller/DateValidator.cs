using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SolarWatch.Controller;

public class DateValidator : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.ContainsKey("date") && context.ActionArguments["date"] is string dateString)
        {
            DateTime date;
            if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                context.Result = new BadRequestObjectResult("datevalidator! rossz dátum formátum");
            }
        }
        base.OnActionExecuting(context);
        base.OnActionExecuting(context);
    }
}