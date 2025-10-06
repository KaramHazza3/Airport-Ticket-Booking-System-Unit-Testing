using System.ComponentModel.DataAnnotations;
using AirportTicketBookingSystem.Common.Models;

namespace AirportTicketBookingSystem.Common;

public static class AttributeConstraintsGenerator
{

    public static IReadOnlyList<AttributeConstraint> GetValidationConstraints<T>()
    {
        var properties = typeof(T).GetProperties();
        var constraints = new List<AttributeConstraint>();

        foreach (var property in properties)
        {
            var propertyConstraint = new AttributeConstraint
            {
                PropertyName = property.Name,
                PropertyType = property.PropertyType.Name,
                Constraints = new List<string>()
            };

            var validationAttributes = property.GetCustomAttributes(true)
                .OfType<ValidationAttribute>();

            foreach (var attribute in validationAttributes)
            {
                switch (attribute)
                {
                    case RequiredAttribute:
                        propertyConstraint.Constraints.Add(ValidationConstants.Required);
                        break;
                }
                
                if (!string.IsNullOrWhiteSpace(attribute.ErrorMessage))
                {
                    propertyConstraint.Constraints.Add(attribute.ErrorMessage);
                }
            }
            
            constraints.Add(propertyConstraint);
        }
        
        return constraints.AsReadOnly();
    }
}