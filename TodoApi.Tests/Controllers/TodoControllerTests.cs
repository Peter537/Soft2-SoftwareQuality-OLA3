using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Repositories;
using Xunit;

namespace TodoApi.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockRepo = new Mock<ITodoRepository>();
            _controller = new TodoController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithItems()
        {
            // Arrange
            var items = new List<TodoItem> { new() { Id = 1, Name = "Test" } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(items, okResult.Value);
            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Get_NotFound_WhenItemNotExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAsync(It.IsAny<long>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.Get(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenItemExists()
        {
            // Arrange
            var item = new TodoItem { Id = 1, Name = "Test" };
            _mockRepo.Setup(r => r.GetAsync(1)).ReturnsAsync(item);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(item, okResult.Value);
        }

        [Fact]
        public async Task Create_ValidItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var item = new TodoItem { Name = "Valid Title", Description = "Valid desc with length >10" };
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<TodoItem>())).ReturnsAsync(item);

            // Act
            var result = await _controller.Create(item);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.Get), createdResult.ActionName);
            _mockRepo.Verify(r => r.CreateAsync(It.Is<TodoItem>(i => i.Name == "Valid Title")), Times.Once);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var item = new TodoItem { Name = "" }; // Empty title
            _controller.ModelState.AddModelError("Name", "Title required");

            // Act
            var result = await _controller.Create(item);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_InvalidIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var item = new TodoItem { Id = 2, Name = "Updated" };
            var result = await _controller.Update(1, item); // Id mismatch

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_NotFound_WhenItemNotExists()
        {
            // Arrange
            var item = new TodoItem { Id = 999, Name = "Updated" };
            _mockRepo.Setup(r => r.UpdateAsync(999, It.IsAny<TodoItem>())).ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.Update(999, item);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Valid_ReturnsNoContent()
        {
            // Arrange
            var item = new TodoItem { Id = 1, Name = "Updated" };
            _mockRepo.Setup(r => r.UpdateAsync(1, It.IsAny<TodoItem>())).ReturnsAsync(item);

            // Act
            var result = await _controller.Update(1, item);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NotFound_WhenItemNotExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Success_ReturnsNoContent()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact] // BVA Description min
        public async Task Create_DescriptionTooShort_ReturnsBadRequest()
        {
            // Arrange
            var item = new TodoItem { Name = "Valid", Description = new string('a', 9) };
            _controller.ModelState.AddModelError("Description", "Min length 10");
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<TodoItem>())).ReturnsAsync(item);

            // Act
            var result = await _controller.Create(item);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact] // EP Valid State
        public async Task Update_ValidStateTransition_Succeeds()
        {
            // Arrange
            var item = new TodoItem { Id = 1, Name = "Test", Status = Models.TaskStatus.Completed };
            _mockRepo.Setup(r => r.UpdateAsync(1, It.IsAny<TodoItem>())).ReturnsAsync(item);

            // Act
            var result = await _controller.Update(1, item);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_InvalidState_Rejects()
        {
            // Arrange
            var item = new TodoItem { Id = 1, Name = "Test", Status = Models.TaskStatus.Invalid };

            // Act
            var result = await _controller.Update(1, item);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("Status"), "ModelState should contain an error for 'Status'");
            var errors = Assert.IsType<string[]>(modelState["Status"]);
            Assert.Contains("Invalid status value", errors);
        }
    }
}