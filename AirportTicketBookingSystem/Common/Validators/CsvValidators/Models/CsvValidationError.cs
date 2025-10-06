namespace AirportTicketBookingSystem.Common.Validators.CsvValidators.Models;

public class CsvValidationError
{
    public int RowNumber { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    
    public CsvValidationError() { } // required for deserialization or model binding

    public CsvValidationError(int rowNumber, string propertyName, string errorMessage)
    {
        RowNumber = rowNumber;
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}