namespace TX.RMC.UnitTests;

using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.UnitTests.Data;

public class NUnitTestRobotService
{
    private const string RobotNameIdentifier = "TX-01";
    private CommandDataRepository commandDataRepository;
    private RobotService robotService;
    private string robotId = null!;

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
    public async Task TestRobotService()
    {
        Robot? robot = await this.robotService.AddAsync(RobotNameIdentifier, CancellationToken.None);
        this.robotId = robot.Id;
        Assert.IsNotNull(this.robotId);

        robot = await this.robotService.GetAsync(this.robotId, CancellationToken.None);
        Assert.IsNotNull(robot);

        string status = await this.robotService.GetStatusAsync(RobotNameIdentifier, CancellationToken.None);
        Assert.IsNotEmpty(status);

        IEnumerable<(string Id, string Command, DateTime ExecutedAt)> commandHistory = await this.robotService.GetCommandHistoryAsync(RobotNameIdentifier);
        Assert.IsTrue(!commandHistory.Any());

        await this.commandDataRepository.AddAsync(
            new Command
            {
                RobotId = this.robotId,
                Action = ECommands.MoveForward,
                CreatedAt = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Direction = EDirections.North,
                PositionX = 1,
                PositionY = 1,
                UserId = Guid.NewGuid().ToString()
            });

        commandHistory = await this.robotService.GetCommandHistoryAsync(RobotNameIdentifier);
        Assert.IsTrue(commandHistory.Any());
    }
}
