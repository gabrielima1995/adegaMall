using Framework;
using System.Linq;

namespace System.Data.Entity.Validation
{
    public static class DbEntityValidationExceptionHelper
    {
        public static ErrorDictionary GetErrors(this DbEntityValidationException exception)
        {
            var items = exception.EntityValidationErrors;
            var errorStrings = items.SelectMany(item =>
                item.ValidationErrors.Select(error =>
                    $"Error '{error.ErrorMessage}' in {item.Entry.Entity.GetType().Name}.{error.PropertyName}."));
            return new ErrorDictionary("", errorStrings.ToArray());
        }
    }
}
