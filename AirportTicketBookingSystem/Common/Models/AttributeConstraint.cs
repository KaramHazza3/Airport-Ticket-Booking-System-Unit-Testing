namespace AirportTicketBookingSystem.Common.Models;

public class AttributeConstraint
{
    public string PropertyName { get; set; }
    public string PropertyType { get; set; }
    public List<string> Constraints { get; set; }

    public AttributeConstraint()
    {
        this.Constraints = [];
    }
    public AttributeConstraint(string propertyName, string propertyType, List<string> constraints)
    {
        this.PropertyName = propertyName;
        this.PropertyType = propertyType;
        this.Constraints = constraints;
    }
    
    public override string ToString()
    {
        var builder = new System.Text.StringBuilder();
        builder.AppendLine($"{PropertyName}:");
        builder.AppendLine($"Type: {PropertyType}");
        builder.AppendLine($"Constraints: {string.Join(", ", Constraints)}");
        return builder.ToString();
    }
}