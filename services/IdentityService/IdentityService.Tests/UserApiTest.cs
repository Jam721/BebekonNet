using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace IdentityService.Tests;

public class UserApiTest
{
    private readonly HttpClient _client;

    public UserApiTest(HttpClient client)
    {
        var server = new TestServer(new WebHostBuilder()
            .UseEnvironment("Development")
            .UseStartup<StartupBase>());
        _client = server.CreateClient();
    }
}