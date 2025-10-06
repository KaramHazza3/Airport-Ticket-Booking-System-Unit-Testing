using System.ComponentModel.DataAnnotations;

namespace AirportTicketBookingSystem.Common.CustomAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class FutureAttribute : ValidationAttribute
{
    public FutureAttribute(string? errorMessage = null)
    {
        ErrorMessage = errorMessage ?? "Allowed Range (today -> future).";
    }
    
    protected override ValidationResult IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is not DateTime dateValue)
        {
            return new ValidationResult("Invalid date format.");
        }
        return dateValue >= DateTime.UtcNow ? ValidationResult.Success! : new ValidationResult(ErrorMessage);
    }
}