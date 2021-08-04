﻿using InventoriesExercice.API.Entities;
using InventoriesExercice.API.Models;
using InventoriesExercice.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoriesService _inventoriesService;

        public InventoriesController(IInventoriesService inventoriesService)
        {
            _inventoriesService = inventoriesService;
        }

        /// <summary>
        /// Get all inventories of a user
        /// </summary>
        /// <response code="200">Return the list of the user's inventories</response>
        /// <response code="401">You must be logged in</response>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Inventory>>> GetAllInventories()
        {
            var currentUserId = User.Identity.Name;
            var inventories = await _inventoriesService.GetAllAsync(currentUserId);

            return Ok(inventories);
        } 

        /// <summary>
        /// Get specific inventory
        /// </summary>
        /// <response code="200">Return the inventory</response>
        /// <response code="401">You must be logged in</response>
        /// <response code="403">You don't have the right to get this inventory</response>
        /// <returns></returns>
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Inventory>> GetInventory(string id)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                var inventory = await _inventoriesService.GetAsync(currentUserId, id);

                if (inventory == null)
                {
                    return NotFound();
                }

                return Ok(inventory);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Create an inventory
        /// </summary>
        /// <response code="200">Return the newly created inventory's id</response>
        /// <response code="401">You must be logged in</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateInventory([FromBody] InventoryModel inventoryModel)
        {
            var currentUserId = User.Identity.Name;

            string inventoryId = await _inventoriesService.CreateAsync(currentUserId, inventoryModel);

            return Ok(inventoryId);
        }

        /// <summary>
        /// Update an inventory
        /// </summary>
        /// <response code="200">The inventory has successfuly been updated</response>
        /// <response code="401">You must be logged in</response>
        /// <response code="403">You don't have the right to update this inventory</response>
        /// <returns></returns>
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateInventory(string id, [FromBody] InventoryModel inventoryModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                await _inventoriesService.UpdateAsync(currentUserId, id, inventoryModel);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Delete an inventory
        /// </summary>
        /// <response code="200">The inventory has successfuly been deleted</response>
        /// <response code="401">You must be logged in</response>
        /// <response code="403">You don't have the right to delete this inventory</response>
        /// <returns></returns>
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteInventory(string id)
        {
            var currentUserId = User.Identity.Name;
            try
            {
                await _inventoriesService.DeleteAsync(currentUserId, id);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}