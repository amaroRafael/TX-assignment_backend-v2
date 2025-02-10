namespace TX.RMC.UnitTests;

using System;
using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.UnitTests.Data;

public class NUnitTestCommandService
{
    private UserService userService;
    private RobotService robotService;
    private CommandService commandService;
    private string robot = "TX-01";
    private object? RobotId;
    private object? UserId;
    private object? CommandId;
    private EDirections Direction;
    private int PositionX;
    private int PositionY;

    public NUnitTestCommandService()
    {
        this.userService = new UserService(new UserDataRepository());
        var robotDataRepository = new RobotDataRepository();
        var commandDataRepository = new CommandDataRepository();
        this.robotService = new RobotService(robotDataRepository, commandDataRepository);
        this.commandService = new CommandService(commandDataRepository, robotDataRepository);
    }

    [SetUp]
    public async Task Setup()
    {
        if (this.RobotId is null)
        {
            var robotModel = await this.robotService.AddAsync(robot, CancellationToken.None);
            this.RobotId = robotModel.Id;
        }

        if (this.UserId is null)
        {
            var user = await userService.AddAsync("John Doe", "johndoe", "password", CancellationToken.None);
            this.UserId = user.Id;
        }
    }

    [Test]
    public async Task TestCommandService()
    {
        Command? command = await this.commandService.SendAsync(ECommands.MoveForward, this.robot, this.UserId, CancellationToken.None);
        UpdateRobotVariablePostionAndDirection(command);

        Assert.IsNotNull(command);
        Assert.IsNotNull(this.CommandId);
        Assert.IsTrue(this.Direction == EDirections.North);
        Assert.IsTrue(this.PositionX == 0);
        Assert.IsTrue(this.PositionY == 1);

        command = await this.commandService.GetAsync(this.CommandId, CancellationToken.None);
        Assert.IsNotNull(command);
        Assert.IsTrue((command?.Id ?? Guid.Empty) == this.CommandId);

        command = await this.commandService.UpdateAsync(ECommands.RotateLeft, this.robot, this.UserId, CancellationToken.None);
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
