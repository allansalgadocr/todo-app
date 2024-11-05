using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DTOs;
using TodoApp.Api.Exceptions;
using TodoApp.Api.Models;
using TodoApp.Api.Repositories;

namespace TodoApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TodosController> _logger;

        public TodosController(ITodoRepository todoRepository, IMapper mapper, ILogger<TodosController> logger)
        {
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all TODO items.
        /// </summary>
        /// <returns>List of TodoItemReadDto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemReadDto>>> GetTodos()
        {
            var todoItems = await _todoRepository.GetAllAsync();
            var todoItemDtos = _mapper.Map<IEnumerable<TodoItemReadDto>>(todoItems);
            return Ok(todoItemDtos);
        }

        /// <summary>
        /// Retrieves a specific TODO item by its unique ID.
        /// </summary>
        /// <param name="id">The GUID of the TODO item</param>
        /// <returns>TodoItemReadDto</returns>
        [HttpGet("{id}", Name = "GetTodo")]
        public async Task<ActionResult<TodoItemReadDto>> GetTodo(Guid id)
        {
            var todoItem = await _todoRepository.GetByIdAsync(id);

            if (todoItem == null)
            {
                _logger.LogWarning("TODO item with ID {TodoId} not found.", id);
                throw new UserFriendlyException($"TODO item with ID {id} not found.", StatusCodes.Status404NotFound);
            }

            var todoItemDto = _mapper.Map<TodoItemReadDto>(todoItem);
            return Ok(todoItemDto);
        }

        /// <summary>
        /// Creates a new TODO item.
        /// </summary>
        /// <param name="todoItemCreateDto">The DTO containing TODO item details</param>
        /// <returns>Created TodoItemReadDto</returns>
        [HttpPost]
        public async Task<ActionResult<TodoItemReadDto>> CreateTodo([FromBody] TodoItemCreateDto todoItemCreateDto)
        {
            if (todoItemCreateDto == null)
            {
                _logger.LogWarning("CreateTodo called with null DTO.");
                throw new UserFriendlyException("Invalid TODO item data.", StatusCodes.Status400BadRequest);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateTodo called with invalid model state.");
                throw new UserFriendlyException("Invalid TODO item data.", StatusCodes.Status400BadRequest);
            }

            var todoItem = _mapper.Map<TodoItem>(todoItemCreateDto);

            await _todoRepository.AddAsync(todoItem);
            await _todoRepository.SaveAsync();

            var todoItemReadDto = _mapper.Map<TodoItemReadDto>(todoItem);

            return CreatedAtRoute(nameof(GetTodo), new { id = todoItemReadDto.Id }, todoItemReadDto);
        }

        /// <summary>
        /// Updates an existing TODO item.
        /// </summary>
        /// <param name="id">The GUID of the TODO item to update</param>
        /// <param name="todoItemUpdateDto">The DTO containing updated TODO item details</param>
        /// <returns>NoContent if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] TodoItemUpdateDto todoItemUpdateDto)
        {
            if (todoItemUpdateDto == null)
            {
                _logger.LogWarning("UpdateTodo called with null DTO for ID {TodoId}.", id);
                throw new UserFriendlyException("Invalid TODO item data.", StatusCodes.Status400BadRequest);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateTodo called with invalid model state for ID {TodoId}.", id);
                throw new UserFriendlyException("Invalid TODO item data.", StatusCodes.Status400BadRequest);
            }

            var todoItemFromRepo = await _todoRepository.GetByIdAsync(id);

            if (todoItemFromRepo == null)
            {
                _logger.LogWarning("TODO item with ID {TodoId} not found for update.", id);
                throw new UserFriendlyException($"TODO item with ID {id} not found.", StatusCodes.Status404NotFound);
            }

            _mapper.Map(todoItemUpdateDto, todoItemFromRepo);

            _todoRepository.Update(todoItemFromRepo);

            await _todoRepository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific TODO item.
        /// </summary>
        /// <param name="id">The GUID of the TODO item to delete</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            var todoItemFromRepo = await _todoRepository.GetByIdAsync(id);

            if (todoItemFromRepo == null)
            {
                _logger.LogWarning("TODO item with ID {TodoId} not found for deletion.", id);
                throw new UserFriendlyException($"TODO item with ID {id} not found.", StatusCodes.Status404NotFound);
            }

            _todoRepository.Delete(todoItemFromRepo);
            await _todoRepository.SaveAsync();

            return NoContent();
        }
    }
}
