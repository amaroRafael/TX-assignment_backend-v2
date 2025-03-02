namespace TX.RMC.UnitTests;

using Microsoft.EntityFrameworkCore;
using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.DataService.MongoDB;
using TX.RMC.UnitTests.Data;

public class NUnitTestUserService
{
    private UserService userService;
    private string? userId;

    private string username = "johndoe";
    private string password = "password";
    private UserDataRepository userRepository;

    [SetUp]
    public void Setup()
    {
        this.userRepository ??= UserDataRepository.Create();
        this.userService ??= new UserService(this.userRepository);
    }

    [Test]
    public async Task TestUserService()
    {
        User? user = await this.userRepository.GetByUsernameAsync(username, CancellationToken.None) ?? await this.userService.AddAsync("John Doe", username, password, CancellationToken.None);
        this.userId = user?.Id;

        Assert.IsNotNull(this.userId);

        user = await this.userService.GetAsync(this.userId, CancellationToken.None);
        Assert.IsNotNull(user);

        var userId = await this.userService.ValidateCredentialsAsync(username, password, CancellationToken.None);
        Assert.IsNotEmpty(userId);
    }
}
