using FluentValidation.Results;

namespace APIAttachedDocument.Application.Validation.Result
{
    public class MessageResult
    {
        public static string GetMessage(ValidationResult result)
        {
           return string.Join("; ", result.Errors.Select(x => x.ErrorMessage));
        }
    }
}
