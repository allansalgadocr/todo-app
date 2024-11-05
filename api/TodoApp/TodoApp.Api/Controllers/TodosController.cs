using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.DTOs;
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
            try
            {
                var todoItems = await _todoRepository.GetAllAsync();
                var todoItemDtos = _mapper.Map<IEnumerable<TodoItemReadDto>>(todoItems);
                return Ok(todoItemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving TODO items.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while retrieving TODO items." });
            }
        }

        /// <summary>
        /// Retrieves a specific TODO item by its unique ID.
        /// </summary>
        /// <param name="id">The GUID of the TODO item</param>
        /// <returns>TodoItemReadDto</returns>
        [HttpGet("{id}", Name = "GetTodo")]
        public async Task<ActionResult<TodoItemReadDto>> GetTodo(Guid id)
        {
            try
            {
                var todoItem = await _todoRepository.GetByIdAsync(id);

                if (todoItem == null)
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found.", id);
                    return NotFound(new { Message = $"TODO item with ID {id} not found." });
                }

                var todoItemDto = _mapper.Map<TodoItemReadDto>(todoItem);
                return Ok(todoItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving TODO item with ID {TodoId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while retrieving the TODO item." });
            }
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
                return BadRequest(new { Message = "Invalid TODO item data." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateTodo called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var todoItem = _mapper.Map<TodoItem>(todoItemCreateDto);

                await _todoRepository.AddAsync(todoItem);
                await _todoRepository.SaveAsync();

                var todoItemReadDto = _mapper.Map<TodoItemReadDto>(todoItem);

                return CreatedAtRoute(nameof(GetTodo), new { id = todoItemReadDto.Id }, todoItemReadDto);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while creating TODO item.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "A database error occurred while creating the TODO item." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating TODO item.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while creating the TODO item." });
            }
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
                return BadRequest(new { Message = "Invalid TODO item data." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateTodo called with invalid model state for ID {TodoId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var todoItemFromRepo = await _todoRepository.GetByIdAsync(id);

                if (todoItemFromRepo == null)
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found for update.", id);
                    return NotFound(new { Message = $"TODO item with ID {id} not found." });
                }

                _mapper.Map(todoItemUpdateDto, todoItemFromRepo);

                _todoRepository.Update(todoItemFromRepo);

                await _todoRepository.SaveAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                _logger.LogError(dbEx, "Concurrency error while updating TODO item with ID {TodoId}.", id);
                return StatusCode(StatusCodes.Status409Conflict, new { Message = "A concurrency error occurred while updating the TODO item." });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while updating TODO item with ID {TodoId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "A database error occurred while updating the TODO item." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating TODO item with ID {TodoId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while updating the TODO item." });
            }
        }

        /// <summary>
        /// Deletes a specific TODO item.
        /// </summary>
        /// <param name="id">The GUID of the TODO item to delete</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            try
            {
                var todoItemFromRepo = await _todoRepository.GetByIdAsync(id);

                if (todoItemFromRepo == null)
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found for deletion.", id);
                    return NotFound(new { Message = $"TODO item with ID {id} not found." });
                }

                _todoRepository.Delete(todoItemFromRepo);
                await _todoRepository.SaveAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while deleting TODO item with ID {TodoId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "A database error occurred while deleting the TODO item." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting TODO item with ID {TodoId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while deleting the TODO item." });
            }
        }
    }
}
