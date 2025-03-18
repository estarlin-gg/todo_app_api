using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Todo_api.Controllers;
using Todo_api.Dtos;
using Todo_api.Services.Abstraction;
using Xunit;

public class TodoControllerTests
{
    private readonly TodoController _controller;
    private readonly Mock<ITaskService> _taskServiceMock;

    public TodoControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _controller = new TodoController(_taskServiceMock.Object);
    }

    [Fact]
    public async Task GetTasks_ReturnsOkResult()
    {
        var result = await _controller.GetTasks();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void CreateTask_ValidData_ReturnsOk()
    {
        var taskDto = new TodoTaskDto<string> { Title = "New Task", Description = "Task Desc" };
        var result = _controller.CreateTask(taskDto);
        Assert.IsType<OkObjectResult>(result);
    }
}
