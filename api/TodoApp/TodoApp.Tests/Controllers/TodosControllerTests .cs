using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Controllers;
using TodoApp.Api.DTOs;
using TodoApp.Api.Exceptions;
using TodoApp.Api.Models;
using TodoApp.Api.Repositories;

namespace TodoApp.Tests.Controllers
{
    public class TodosControllerTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<TodosController>> _mockLogger;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            // Setup AutoMapper
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TodoItem, TodoItemReadDto>();
                cfg.CreateMap<TodoItemCreateDto, TodoItem>();
                cfg.CreateMap<TodoItemUpdateDto, TodoItem>();
            });
            _mapper = configuration.CreateMapper();

            // Initialize Mock Repository
            _mockRepo = new Mock<ITodoRepository>();

            // Initialize Mock Logger
            _mockLogger = new Mock<ILogger<TodosController>>();

            // Initialize Controller with Mocks
            _controller = new TodosController(_mockRepo.Object, _mapper, _mockLogger.Object);
        }

        #region GetTodos Tests

        [Fact]
        public async Task GetTodos_ReturnsOkResult_WithListOfTodoItems()
        {
            // Arrange
            var todos = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 1", Description = "Description 1", IsCompleted = false },
                new TodoItem { Id = Guid.NewGuid(), Title = "Test Todo 2", Description = "Description 2", IsCompleted = true }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todos);

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<TodoItemReadDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        #endregion

        #region GetTodo Tests

        [Fact]
        public async Task GetTodo_ExistingId_ReturnsOkResult_WithTodoItem()
        {
            // Arrange
            var existingTodo = new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = "Sample Todo",
                Description = "Sample Description",
                IsCompleted = false
            };
            _mockRepo.Setup(repo => repo.GetByIdAsync(existingTodo.Id)).ReturnsAsync(existingTodo);

            // Act
            var result = await _controller.GetTodo(existingTodo.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<TodoItemReadDto>(okResult.Value);
            Assert.Equal(existingTodo.Title, returnValue.Title);
        }

        [Fact]
        public async Task GetTodo_NonExistingId_ThrowsUserFriendlyException()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetByIdAsync(nonExistingId)).ReturnsAsync((TodoItem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _controller.GetTodo(nonExistingId));
            Assert.Equal($"TODO item with ID {nonExistingId} not found.", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }

        #endregion

        #region CreateTodo Tests

        [Fact]
        public async Task CreateTodo_ValidDto_ReturnsCreatedAtRoute()
        {
            // Arrange
            var createDto = new TodoItemCreateDto
            {
                Title = "New Todo",
                Description = "New Description"
            };

            var todoItem = new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = createDto.Title,
                Description = createDto.Description
            };

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateTodo(createDto);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            var returnValue = Assert.IsType<TodoItemReadDto>(createdAtRouteResult.Value);
            Assert.Equal(createDto.Title, returnValue.Title);
            Assert.Equal(createDto.Description, returnValue.Description);
        }

        [Fact]
        public async Task CreateTodo_NullDto_ThrowsUserFriendlyException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _controller.CreateTodo(null));
            Assert.Equal("Invalid TODO item data.", exception.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact]
        public async Task CreateTodo_InvalidModel_ThrowsUserFriendlyException()
        {
            // Arrange
            var createDto = new TodoItemCreateDto
            {
                // Missing required Title
                Title = string.Empty,
                Description = "Description without empty title",
            };
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _controller.CreateTodo(createDto));
            Assert.Equal("Invalid TODO item data.", exception.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact]
        public async Task CreateTodo_DbUpdateException_ThrowsUserFriendlyException()
        {
            // Arrange
            var createDto = new TodoItemCreateDto
            {
                Title = "New Todo",
                Description = "New Description"
            };

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ThrowsAsync(new DbUpdateException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => _controller.CreateTodo(createDto));
            Assert.Equal("Database error", exception.Message);
        }

        #endregion

        #region UpdateTodo Tests

        [Fact]
        public async Task UpdateTodo_ExistingId_ValidDto_ReturnsNoContent()
        {
            // Arrange
            var existingId = Guid.NewGuid();
            var existingTodo = new TodoItem
            {
                Id = existingId,
                Title = "Original Todo",
                Description = "Original Description",
                IsCompleted = false
            };

            var updateDto = new TodoItemUpdateDto
            {
                Title = "Updated Todo",
                Description = "Updated Description",
                IsCompleted = true
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(existingId)).ReturnsAsync(existingTodo);
            _mockRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTodo(existingId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.Update(existingTodo), Times.Once);
            _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
            Assert.Equal(updateDto.Title, existingTodo.Title);
            Assert.Equal(updateDto.Description, existingTodo.Description);
            Assert.Equal(updateDto.IsCompleted, existingTodo.IsCompleted);
        }

        [Fact]
        public async Task UpdateTodo_NullDto_ThrowsUserFriendlyException()
        {
            // Arrange
            var existingId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _controller.UpdateTodo(existingId, null));
            Assert.Equal("Invalid TODO item data.", exception.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact]
        public async Task UpdateTodo_InvalidModel_ThrowsUserFriendlyException()
        {
            // Arrange
            var existingId = Guid.NewGuid();
            var updateDto = new TodoItemUpdateDto
            {
                // Missing required Title
                Title = string.Empty,
                Description = "Updated Description",
                IsCompleted = true
            };
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _controller.UpdateTodo(existingId, updateDto));
            Assert.Equal("Invalid TODO item data.", exception.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        }

        [Fact]
        public async Task UpdateTodo_NonExistingId_ThrowsUserFriendlyException()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var updateDto = new TodoItemUpdateDto
            {
                Title = "Updated Todo",
                Description = "Updated Description",
                IsCompleted = true
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(nonExistingId)).ReturnsAsync((TodoItem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _controller.UpdateTodo(nonExistingId, updateDto));
            Assert.Equal($"TODO item with ID {nonExistingId} not found.", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task UpdateTodo_DbUpdateConcurrencyException_ThrowsUserFriendlyException()
        {
            // Arrange
            var existingId = Guid.NewGuid();
            var existingTodo = new TodoItem
            {
                Id = existingId,
                Title = "Original Todo",
                Description = "Original Description",
                IsCompleted = false
            };

            var updateDto = new TodoItemUpdateDto
            {
                Title = "Updated Todo",
                Description = "Updated Description",
                IsCompleted = true
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(existingId)).ReturnsAsync(existingTodo);
            _mockRepo.Setup(repo => repo.SaveAsync()).ThrowsAsync(new DbUpdateConcurrencyException("Concurrency error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _controller.UpdateTodo(existingId, updateDto));
            Assert.Equal("Concurrency error", exception.Message);
        }

        #endregion

        #region DeleteTodo Tests

        [Fact]
        public async Task DeleteTodo_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var existingId = Guid.NewGuid();
            var existingTodo = new TodoItem
            {
                Id = existingId,
                Title = "Todo to Delete",
                Description = "Description",
                IsCompleted = false
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(existingId)).ReturnsAsync(existingTodo);
            _mockRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTodo(existingId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepo.Verify(repo => repo.Delete(existingTodo), Times.Once);
            _mockRepo.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTodo_NonExistingId_ThrowsUserFriendlyException()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetByIdAsync(nonExistingId)).ReturnsAsync((TodoItem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _controller.DeleteTodo(nonExistingId));
            Assert.Equal($"TODO item with ID {nonExistingId} not found.", exception.Message);
            Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task DeleteTodo_DbUpdateException_ThrowsUserFriendlyException()
        {
            // Arrange
            var existingId = Guid.NewGuid();
            var existingTodo = new TodoItem
            {
                Id = existingId,
                Title = "Todo to Delete",
                Description = "Description",
                IsCompleted = false
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(existingId)).ReturnsAsync(existingTodo);
            _mockRepo.Setup(repo => repo.Delete(existingTodo));
            _mockRepo.Setup(repo => repo.SaveAsync()).ThrowsAsync(new DbUpdateException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => _controller.DeleteTodo(existingId));
            Assert.Equal("Database error", exception.Message);
        }

        #endregion
    }
}