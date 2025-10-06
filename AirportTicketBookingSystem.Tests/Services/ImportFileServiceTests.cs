using AirportTicketBookingSystem.Common.Validators.CsvValidators.Models;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Models.DTOs;
using AirportTicketBookingSystem.Services.FlightService;
using AirportTicketBookingSystem.Services.ImportFileService;
using AirportTicketBookingSystem.Tests.Helpers;
using AirportTicketBookingSystem.Wrappers;
using AutoFixture;
using Moq;

namespace AirportTicketBookingSystem.Tests.Services;

public class ImportFileServiceTests
{
    private readonly Mock<IFlightService<Guid>> _flightServiceMock;
    private readonly Mock<ICSVReader> _csvReaderMock;
    private readonly IFileImportService _fileImportService;
    private readonly Flight _flight;
    private readonly Func<FlightCsvDto, Flight> _mapper;
    private readonly Func<FlightCsvDto, int, List<CsvValidationError>> _validator;
    private readonly string _filePath;
    private readonly TestDataBuilder _builder;
    
    public ImportFileServiceTests()
    {
        var fixture = new Fixture();
        _builder = new TestDataBuilder(fixture);
        _flight = _builder.BuildFlight();
        _csvReaderMock = new Mock<ICSVReader>();
        _fileImportService = new ImportCsvFileService(_csvReaderMock.Object);
        _csvReaderMock.Setup(x => x.Read(It.IsAny<string>(),
                It.IsAny<Func<FlightCsvDto, Flight>>(),
                It.IsAny<Func<FlightCsvDto, int, List<CsvValidationError>>>()))
            .ReturnsAsync(new ValueTuple<List<Flight>, List<CsvValidationError>>(new List<Flight>()
            {
                _flight
            }, new List<CsvValidationError>()));
        _flightServiceMock = new Mock<IFlightService<Guid>>();
        _flightServiceMock.Setup(x => x.AddAsync(It.IsAny<Flight>())).ReturnsAsync(_flight);
        _filePath = fixture.Create<string>();
        _mapper = dto => _builder.BuildFlight();
        _validator = (dto, lineNumber) => new List<CsvValidationError>();
    }

    [Fact]
    public async Task ImportFileAsync_WithValidData_ShouldReturnSuccessfulResult()
    {
        // Arrange
        
        
        // Act
        var result = await _fileImportService.ImportFileAsync(_filePath, _mapper, _flightServiceMock.Object, _validator);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.First(), _flight);
    }
    
    [Fact]
    public async Task ImportFileAsync_WithInvalidData_ShouldReturnFailureResult()
    {
        // Arrange
        _csvReaderMock.Setup(x => x.Read(It.IsAny<string>(),
                It.IsAny<Func<FlightCsvDto, Flight>>(),
                It.IsAny<Func<FlightCsvDto, int, List<CsvValidationError>>>()))
            .ReturnsAsync(new ValueTuple<List<Flight>, List<CsvValidationError>>(new List<Flight>(), new List<CsvValidationError>()
            {
                new(2, "Id", "Id is required")
            }));
        
        // Act
        var result = await _fileImportService.ImportFileAsync(_filePath, _mapper, _flightServiceMock.Object, _validator);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Csv.ValidationError", result.Error.Code);
    }
}