using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoProjectSample.Models;

namespace TodoProjectSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems([FromQuery] TodoPagination todoPagination)
        {
            // get all item count to determine if we need to show more items
            int itemCount = await _context.TodoItems.CountAsync();
            int skippedItemCount = todoPagination.NUM_VISIBLE_ITEMS * todoPagination.Page;
            int remainingItems = itemCount - skippedItemCount;

            // we will show 5 more items but we still have more after showing them
            bool hasMore = (remainingItems > todoPagination.NUM_VISIBLE_ITEMS);

            // if we have less than 5 items, show all of them. otherwise, only show 5 more.
            int showItemCount = remainingItems > 0 ? (Math.Min(remainingItems, todoPagination.NUM_VISIBLE_ITEMS)) : 0;


            IEnumerable<TodoItemDTO> todoItems = await _context.TodoItems.
                Select(x => ItemToDTO(x)).
                Skip(todoPagination.NUM_VISIBLE_ITEMS * todoPagination.Page).
                Take(showItemCount).
                ToListAsync();

            return Ok(new
            {
                itemCount,
                hasMore,
                todoItems
            });
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoDTO)
        {
            if (id != todoDTO.Id)
            {
                return BadRequest();
            }

            TodoItem? updatedItem = await _context.TodoItems
                .Where(item => item.Id == id)
                .FirstOrDefaultAsync();

            if (updatedItem == null)
            {
                return NotFound();
            }

            // found item. update some attributes
            updatedItem.IsComplete = todoDTO.IsComplete;
            updatedItem.Name = todoDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO)
        {
            TodoItem newTodoItem = new TodoItem()
            {
                Name = todoDTO.Name,
                IsComplete = todoDTO.IsComplete,
                Secret = $"secret of {todoDTO.Name}"
            };
            _context.TodoItems.Add(newTodoItem);
            await _context.SaveChangesAsync();

            // redirect to GetTodoItem action method to return the newly created to do item
            // todoItem can have different attributes for example system-generated ID
            return CreatedAtAction(nameof(GetTodoItem), new { id = newTodoItem.Id }, ItemToDTO(newTodoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        private static TodoItemDTO ItemToDTO(TodoItem item)
        {
            return new TodoItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                IsComplete = item.IsComplete,
            };
        }
    }
}
