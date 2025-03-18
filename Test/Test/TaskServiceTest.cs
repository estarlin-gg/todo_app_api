using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Todo_api.Context;
using Todo_api.Hubs;
using Todo_api.Models;
using Todo_api.Services.Implementation;
using Xunit;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class TaskServiceTests
{
    private readonly TaskService _taskService;
    private readonly Mock<IHubContext<TaskHub>> _hubContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextMock;
    private readonly ToDoContext _dbContext;

    public TaskServiceTests()
    {
        
        var options = new DbContextOptionsBuilder<ToDoContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid()) 
            .Options;

        _dbContext = new ToDoContext(options);
        _hubContextMock = new Mock<IHubContext<TaskHub>>();
        _httpContextMock = new Mock<IHttpContextAccessor>();

        _taskService = new TaskService(
            _dbContext,
            new TaskQueueService(),
            new CacheService(),
            _httpContextMock.Object,
            _hubContextMock.Object
        );
    }

    [Fact]
    public async Task GetAllTasks_ReturnsTasksForUser()
    {
        string userId = "test-user";
        _httpContextMock.Setup(h => h.HttpContext.User.Identity.Name).Returns(userId);

  
        var task = new TodoTask<string>
        {
            Title = "Test Task",
            Description = "Description",
            FechaDeVencimiento = DateTime.Now.AddDays(3),
            Status = "Pending",
            Prioridad = "Alta",
            UserId = userId
        };

        
        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        var tasks = await _taskService.GetAllTasks();

        Assert.Single(tasks);
    }

    [Fact]
    public async Task GetTaskCompletionRate_ReturnsCorrectPercentage()
    {
        string userId = "test-user";
        _httpContextMock.Setup(h => h.HttpContext.User.Identity.Name).Returns(userId);

        
        var task1 = new TodoTask<string> { UserId = userId, Status = "Completed" };
        var task2 = new TodoTask<string> { UserId = userId, Status = "Pending" };

        _dbContext.Tasks.Add(task1);
        _dbContext.Tasks.Add(task2);
        await _dbContext.SaveChangesAsync();

        double completionRate = await _taskService.GetTaskCompletionRate();
        Assert.Equal(50, completionRate); 
    }
}
