using InventoriesExercise.API.Entities;
using InventoriesExercise.API.Models;
using InventoriesExercise.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsService _itemsService;

        public ItemsController(IItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        /// <summary>
        /// Get specific item
        /// </summary>
        /// <response code="200">Return the item</response>
        /// <response code="401">You must be logged in</response>
        /// <response code="403">You don't have the right to get this item</response>
        /// <returns></returns>
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Item>> GetItem(string id)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                var item = await _itemsService.GetAsync(currentUserId, id);

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Create an item
        /// </summary>
        /// <response code="200">Return the newly created item's id</response>
        /// <response code="400">You cant create this item, most probably beceause the inventory doesn't exist</response>
        /// <response code="401">You must be logged in</response>
        /// <response code="403">You don't have the rights to create this item</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateItem([FromBody] ItemModel itemModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                var itemId = await _itemsService.CreateAsync(currentUserId, itemModel);

                return Ok(itemId);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException e)
            {
                return BadRequest($"You can't create this item: {e.Message}");
            }

        }

        /// <summary>
        /// Update an item
        /// </summary>
        /// <response code="200">The item has successfuly been updated</response>
        /// <response code="401">You must be logged in</response>
        /// <response code="403">You don't have the right to update this item</response>
        /// <returns></returns>
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] ItemModel itemModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                await _itemsService.UpdateAsync(currentUserId, id, itemModel);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <response code="200">The item has successfuly been deleted</response>
        /// <response code="401">You must be logged in</response>
        /// <response code="403">You don't have the right to delete this item</response>
        /// <returns></returns>
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteInventory(string id)
        {
            var currentUserId = User.Identity.Name;
            try
            {
                await _itemsService.DeleteAsync(currentUserId, id);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
