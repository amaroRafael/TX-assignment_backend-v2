namespace TX.RMC.UnitTests;

using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.UnitTests.Data;

public class NUnitTestUserService
{
    private UserService userService;
    private Guid userId;

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
        User? user = await this.userService.AddAsync("John Doe", username, password);
        this.userId = user?.Id ?? Guid.Empty;

        Assert.IsTrue(this.userId != Guid.Empty);

        user = await this.userService.GetAsync(this.userId);
        Assert.IsNotNull(user);

        var userId = await this.userService.ValidateCredentialsAsync(username, password);
        Assert.IsNotEmpty(userId);
    }
}
