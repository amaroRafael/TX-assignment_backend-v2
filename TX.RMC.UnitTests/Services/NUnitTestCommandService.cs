namespace TX.RMC.UnitTests;

using System;
using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.UnitTests.Data;

public class NUnitTestCommandService
{
    private const string RobotNameIdentifier = "TX-01";
    private const string username = "johndoe";
    private UserService userService;
    private RobotService robotService;
    private CommandService commandService;
    private string? RobotId;
    private string? UserId;
    private string? CommandId;
    private EDirections Direction;
    private int PositionX;
    private int PositionY;
    private RobotDataRepository robotRepository;

    [SetUp]
    public async Task Setup()
    {
        var userRepository = UserDataRepository.Create();
        this.robotRepository ??= RobotDataRepository.Create();
        this.userService ??= new UserService(userRepository);
        this.robotService ??= new RobotService(this.robotRepository, CommandDataRepository.Create());
        this.commandService ??= new CommandService(CommandDataRepository.Create(), this.robotRepository);

        if (this.RobotId is null)
        {
            var robotModel = await this.robotRepository.GetByNameIdentityAsync(RobotNameIdentifier, CancellationToken.None);
            robotModel ??= await this.robotService.AddAsync(RobotNameIdentifier, CancellationToken.None);
            this.RobotId = robotModel.Id;
        }

        if (this.UserId is null)
        {
            var user = await userRepository.GetByUsernameAsync(username, CancellationToken.None) ?? await userService.AddAsync("John Doe", username, "password", CancellationToken.None);

            this.UserId = user.Id;
        }
    }

    [Test]
    public async Task TestCommandService()
    {
        Command? command = await this.commandService.SendAsync(ECommands.MoveForward, RobotNameIdentifier, this.UserId!, CancellationToken.None);
        UpdateRobotVariablePostionAndDirection(command);

        Assert.IsNotNull(command);
        Assert.IsNotNull(this.CommandId);
        Assert.IsTrue(this.Direction == EDirections.North);
        Assert.IsTrue(this.PositionX == 0);
        Assert.IsTrue(this.PositionY == 1);

        command = await this.commandService.GetAsync(RobotNameIdentifier, this.CommandId, CancellationToken.None);
        Assert.IsNotNull(command);
        Assert.IsTrue((command?.Id ?? Guid.Empty.ToString()) == this.CommandId);

        command = await this.commandService.UpdateAsync(ECommands.RotateLeft, RobotNameIdentifier, this.UserId!, CancellationToken.None);
        UpdateRobotVariablePostionAndDirection(command);

        Assert.IsNotNull(command);
        Assert.IsNotNull(this.CommandId);
        Assert.IsTrue(this.Direction == EDirections.Northwest);
        Assert.IsTrue(this.PositionX == 0);
        Assert.IsTrue(this.PositionY == 0);
    }

    private void UpdateRobotVariablePostionAndDirection(DataAccess.Core.Models.Command? command)
    {
        this.CommandId = command?.Id;
        this.Direction = command?.Direction ?? EDirections.North;
        this.PositionX = command?.PositionX ?? 0;
        this.PositionY = command?.PositionY ?? 0;
    }
}
