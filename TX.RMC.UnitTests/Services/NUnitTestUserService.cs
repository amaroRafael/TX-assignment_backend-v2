namespace TX.RMC.UnitTests;

using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.UnitTests.Data;

public class NUnitTestUserService
{
    private UserService userService;
    private string? userId;

    private string username = "johndoe";
    private string password = "password";

    public NUnitTestUserService()
    {
        this.userService ??= new UserService(new UserDataRepository());
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestUserService()
    {
        User? user = await this.userService.AddAsync("John Doe", username, password, CancellationToken.None);
        this.userId = user?.Id;

        Assert.IsNotNull(this.userId);

        user = await this.userService.GetAsync(this.userId, CancellationToken.None);
        Assert.IsNotNull(user);

        var userId = await this.userService.ValidateCredentialsAsync(username, password, CancellationToken.None);
        Assert.IsNotEmpty(userId);
    }
}
