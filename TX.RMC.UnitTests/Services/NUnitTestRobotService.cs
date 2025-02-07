﻿namespace TX.RMC.UnitTests;

using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.UnitTests.Data;

public class NUnitTestRobotService
{
    private const string RobotNameIdentifier = "TX-01";
    private CommandDataRepository commandDataRepository;
    private RobotService robotService;
    private Guid robotId;

    public NUnitTestRobotService()
    {
        this.commandDataRepository = new CommandDataRepository();
        this.robotService = new RobotService(new RobotDataRepository(), this.commandDataRepository);
    }

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task TestAddRobot()
    {
        Robot robot = await this.robotService.AddAsync(RobotNameIdentifier);
        this.robotId = robot.Id;
        Assert.IsTrue(this.robotId != Guid.Empty);
    }

    [Test]
    public async Task TestGetRobotById()
    {
        Robot? robot = await this.robotService.GetAsync(this.robotId);
        Assert.IsNotNull(robot);
    }

    [Test]
    public async Task TestGetRobotByNameIdentity()
    {
        string status = await this.robotService.GetStatusAsync(RobotNameIdentifier);
        Assert.IsNotEmpty(status);
    }

    [Test]
    public async Task TestGetRobotCommandHistory()
    {
        IEnumerable<(Guid Id, string Command, DateTime ExecutedAt)> commandHistory = await this.robotService.GetCommandHistoryAsync(RobotNameIdentifier);
        Assert.IsTrue(!commandHistory.Any());

        await this.commandDataRepository.AddAsync(
            new Command
            {
                RobotId = this.robotId,
                Action = ECommands.MoveForward,
                CreatedAt = DateTime.Now,
                Id = Guid.NewGuid(),
                Direction = EDirections.North,
                PositionX = 1,
                PositionY = 1,
                UserId = Guid.NewGuid()
            });

        commandHistory = await this.robotService.GetCommandHistoryAsync(RobotNameIdentifier);
        Assert.IsTrue(commandHistory.Any());
    }
}
