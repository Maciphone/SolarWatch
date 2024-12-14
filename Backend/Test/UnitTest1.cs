using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Controller;
using SolarWatch.Model;
using SolarWatch.Service;
using SolarWatch.Service.Repository;


namespace Test;

[TestFixture]

public class Tests
{
   
    private Mock<ICoordinateProvider> _coordinateProviderMock;
    private Mock<ILogger<SolarWatchController>> _loggerMock;
    private Mock<ISunsetProvider> _sunsetProviderMock;
    private Mock<IJsonProcessor<SolarWatch.Model.SolarWatch>> _jsonProcessorMock;
    private Mock<IJsonProcessor<Coordinate>> _jsonProcessorCoordinateMock;
    private Mock<IJsonProcessor<City>> _jsonProcessorCityMock;
    private Mock<ICityRepository<City, CityDto>> _cityRepositoryMock;
    private Mock<ICityRepository<SunriseSunset, SunriseSunsetDto>> _sunriseSunsetRepositoryMock;

    private SolarWatchController _solarWatchController;

    [SetUp]
    public void SetUp()
    {
        _coordinateProviderMock = new Mock<ICoordinateProvider>();
        _loggerMock = new Mock<ILogger<SolarWatchController>>();
        _sunsetProviderMock = new Mock<ISunsetProvider>();
        _jsonProcessorMock = new Mock<IJsonProcessor<SolarWatch.Model.SolarWatch>>();
        _jsonProcessorCoordinateMock = new Mock<IJsonProcessor<Coordinate>>();
        _jsonProcessorCityMock = new Mock<IJsonProcessor<City>>();
        _cityRepositoryMock = new Mock<ICityRepository<City, CityDto>>();
        _sunriseSunsetRepositoryMock = new Mock<ICityRepository<SunriseSunset, SunriseSunsetDto>>();

        _solarWatchController = new SolarWatchController(
            _loggerMock.Object,
            _sunsetProviderMock.Object,
            _jsonProcessorMock.Object,
            _coordinateProviderMock.Object,
            _jsonProcessorCoordinateMock.Object,
            _cityRepositoryMock.Object,
            _sunriseSunsetRepositoryMock.Object,
            _jsonProcessorCityMock.Object
        );
    }


    [Test]
    public async Task GetAll_ShouldReturnListOfSolarWatch()
    {
        // Arrange
        var mockCityList = new List<City>
        {
            new City { CityId = 1, Name = "City1" },
            new City { CityId = 2, Name = "City2" }
        };

        var mockSunsetSunriseList = new List<SunriseSunset>
        {
            new SunriseSunset
            {
                CityId = 1,
                Sunrise = new DateTime(2024, 1, 1, 6, 0, 0),
                Sunset = new DateTime(2024, 1, 1, 18, 0, 0)
            },
            new SunriseSunset
            {
                CityId = 2,
                Sunrise = new DateTime(2024, 1, 2, 6, 10, 0),
                Sunset = new DateTime(2024, 1, 2, 17, 50, 0)
            }
        };

        _cityRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(mockCityList.AsEnumerable());

        _sunriseSunsetRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(mockSunsetSunriseList.AsEnumerable());

        // Act
        var result = await _solarWatchController.GetAll();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<IEnumerable<SolarWatch.Model.SolarWatch>>(result);

        var solarWatchList = result.ToList();
        Assert.AreEqual(2, solarWatchList.Count);

        Assert.AreEqual("City1", solarWatchList[0].City);
        Assert.AreEqual(new DateOnly(2024, 1, 1), solarWatchList[0].Year);
        Assert.AreEqual(TimeOnly.FromDateTime(mockSunsetSunriseList[0].Sunrise), solarWatchList[0].Sunrise);
        Assert.AreEqual(TimeOnly.FromDateTime(mockSunsetSunriseList[0].Sunset), solarWatchList[0].Sunset);

        Assert.AreEqual("City2", solarWatchList[1].City);
        Assert.AreEqual(new DateOnly(2024, 1, 2), solarWatchList[1].Year);
        Assert.AreEqual(TimeOnly.FromDateTime(mockSunsetSunriseList[1].Sunrise), solarWatchList[1].Sunrise);
        Assert.AreEqual(TimeOnly.FromDateTime(mockSunsetSunriseList[1].Sunset), solarWatchList[1].Sunset);
    }

    [Test]
    public async Task GetAsyncDatabase_ShouldReturnSolarWatchFromDb_WhenDataExists()
    {
        // Arrange
        var mockCity = new City { CityId = 1, Name = "TestCity", Latitude = 10.0, Longitude = 20.0 };
        var mockSunriseSunset = new SunriseSunset
        {
            CityId = 1,
            Sunrise = new DateTime(2024, 1, 1, 6, 0, 0),
            Sunset = new DateTime(2024, 1, 1, 18, 0, 0)
        };

        _cityRepositoryMock.Setup(repo => repo.GetByNameAsync(It.IsAny<string>(), It.IsAny<DateTime?>()))
            .ReturnsAsync(mockCity);
        _sunriseSunsetRepositoryMock.Setup(repo => repo.GetSunriseSunsetByCityAndDateAsync(1, It.IsAny<DateTime>()))
            .ReturnsAsync(mockSunriseSunset);

        // Act
        var result = await _solarWatchController.GetAsyncDatabase(DateTime.Now, "TestCity");

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var solarWatch = okResult.Value as SolarWatch.Model.SolarWatch;
        Assert.IsNotNull(solarWatch);
        Assert.AreEqual("TestCity", solarWatch.City);
        Assert.AreEqual(TimeOnly.FromDateTime(mockSunriseSunset.Sunrise), solarWatch.Sunrise);
        Assert.AreEqual(TimeOnly.FromDateTime(mockSunriseSunset.Sunset), solarWatch.Sunset);
    }

    [Test]
    public async Task UpdateCity_ShouldReturnOk_WhenCityIsUpdatedSuccessfully()
    {
        // Arrange
        var cityId = 1;
        var cityDto = new CityDto
        {

            Name = "Updated City",
            Latitude = 40.7128,
            Longitude = -74.0060,
            State = "New-York",
            Country = "USA"
        };

        var existingCity = new City
        {
            CityId = cityId,
            Name = "Original City",
            Latitude = 40.7128,
            Longitude = -74.0060,
            State = "New-York",
            Country = "USA"
        };

        _cityRepositoryMock.Setup(repo => repo.GetByIdAsync(cityId))
            .ReturnsAsync(existingCity);

        _cityRepositoryMock.Setup(repo => repo.UpdateAsync(cityId, cityDto))
            .Returns(Task.CompletedTask);

        _cityRepositoryMock.Setup(repo => repo.GetByIdAsync(cityId))
            .ReturnsAsync(new City
            {
                CityId = cityId,
                Name = "Updated City",
                Latitude = 40.7128,
                Longitude = -74.0060,
                Country = "USA"
            });

        // Act
        var result = await _solarWatchController.UpdateCity(cityId, cityDto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var updatedCity = okResult.Value as City;
        Assert.IsNotNull(updatedCity);
        Assert.AreEqual("Updated City", updatedCity.Name);
    }
}
