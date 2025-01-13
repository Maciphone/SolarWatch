using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text;
using System.Text.Json;
using IntegrationTest;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SolarWatch.Data;
using SolarWatch.Service.Authentication;
using SolarWatch.Service.Converter;
using Xunit.Abstractions;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SW.IntegratonTes;

[Collection("IntegrationTests")] //this is to avoid problems with tests running in parallel
public class MyControllerIntegrationTestForDatabase : IClassFixture<SolarWatchWebApplicationFactoryInMemory>
{
  //  private readonly SolarWatchWebApplicationFactory _factory;
    private readonly SolarWatchWebApplicationFactoryInMemory _factoryInMemory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper output;

    public MyControllerIntegrationTestForDatabase(ITestOutputHelper output,
        SolarWatchWebApplicationFactoryInMemory factoryInMemory)
    {
        this.output = output;
        _factoryInMemory = factoryInMemory;
        //_factory = new SolarWatchWebApplicationFactory();
        _client = _factoryInMemory.CreateClient();
    }
    
    [Fact]
    public async Task TestUserLoginPassword()
    {
        var LoginRequest = new AuthRequest("a@a.hu", "password");
        output.WriteLine($"login req: {LoginRequest.Email}");
            var LoginResponse = await _client.PostAsync(requestUri: "api/Auth/Login",
            new StringContent(JsonConvert.SerializeObject(LoginRequest),
                Encoding.UTF8, mediaType: "application/json")); // Task<HttpResponseMessage>
        
        var response = await LoginResponse.Content.ReadAsStringAsync();
        var authRespones = JsonConvert.DeserializeObject<AuthResponse>(response);
        output.WriteLine(response);
        output.WriteLine(authRespones.Email);
        Assert.Equal("a@a.hu", authRespones.Email);

    }
    
    [Fact]
    public async Task TestDatabaseSeed()
    {
        using var scope = _factoryInMemory.Services.CreateScope();
        var services = scope.ServiceProvider;

        var solarContext = services.GetRequiredService<ApplicationDbContext>();
        var userContext = services.GetRequiredService<IdentityUserContext>();

        // Check if the City is seeded correctly
        var city = solarContext.Cities.FirstOrDefault(c => c.Name == "TestCity");
        Assert.NotNull(city);

        // Check if the User is seeded correctly
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync("a@a.hu");
        Assert.NotNull(user);
        var roles = await userManager.GetRolesAsync(user);
        Assert.Contains("User", roles);
    }

    [Fact]
    public async Task TestEndPointAuthenticated()
    {
        var token = await GetAuthTokenAsync();
        output.WriteLine($"testendpointAuthenticate, token: {token}");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("api/SolarWatch/GetByCityDateAsyncDatabase?city=TestCity");
        response.EnsureSuccessStatusCode();
        output.WriteLine(response.ToString());

        var responseData = await response.Content.ReadFromJsonAsync<SolarWatch.Model.SolarWatch>();

        output.WriteLine("Response Data:");
        output.WriteLine(responseData.City ?? "Response data is null");
        
        
        
        Assert.Equal("TestCity", responseData.City);
        }
    
    [Fact]
    public async Task TestEndPointNotFound()
    {
        var token = await GetAuthTokenAsync();
        output.WriteLine($"testendpointAuthenticate, token: {token}");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        string wrongCity = "dfkljdflgkdgdkgjds";
        var response = await _client.GetAsync($"api/SolarWatch/GetByCityDateAsyncDatabase?city={wrongCity}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    }


    public async Task<string> GetAuthTokenAsync()
    {
        var loginData = new  { Email = "a@a.hu", Password = "password" };

        output.WriteLine("Sending login request with data:");
        output.WriteLine(JsonSerializer.Serialize(loginData));
        
        var loginResponse = await _client.PostAsJsonAsync("api/auth/login", loginData);
       // var responseContent = await loginResponse.Content.ReadAsStringAsync();
       var responseContent = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        output.WriteLine("Login response:");
        output.WriteLine(responseContent.Token);
        
        loginResponse.EnsureSuccessStatusCode();
        return responseContent.Token;
        /*

        AuthResponse loginResult = null;
        try
        {
            loginResult = JsonSerializer.Deserialize<AuthResponse>(responseContent);
            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                throw new Exception("Deserialization failed or token is missing.");
            }
        }
        catch (Exception ex)
        {
            output.WriteLine($"Deserialization error: {ex.Message}");
            throw;
        }

        output.WriteLine($"loginresult a getauthtokenben: {loginResult.Token}");
        return loginResult.Token;

       */
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
    
     
}