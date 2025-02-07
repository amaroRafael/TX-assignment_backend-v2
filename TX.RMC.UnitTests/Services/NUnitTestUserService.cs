namespace TX.RMC.UnitTests;

using TX.RMC.BusinessLogic;
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
    public async Task TestAddUser()
    {
        var user = await this.userService.AddAsync("John Doe", username, password);
        this.userId = user?.Id ?? Guid.Empty;

        Assert.IsTrue(this.userId != Guid.Empty);
    }

    [Test]
    public async Task TestGetById()
    {
        var user = await this.userService.GetAsync(this.userId);
        Assert.IsNotNull(user);
    }

    [Test]
    public async Task TestValidateCredentials()
    {
        var userId = await this.userService.ValidateCredentialsAsync(username, password);
        Assert.IsNotEmpty(userId);
    }
}
