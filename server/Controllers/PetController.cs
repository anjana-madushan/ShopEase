// using System;
// using Microsoft.AspNetCore.Mvc;
// using server.Services;
// using server.Models;

// namespace server.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class PetController : ControllerBase
// {
//   private readonly MongoDBService _mongoDBPetService;

//   public PetController(MongoDBService mongoDBPetService)
//   {
//     _mongoDBPetService = mongoDBPetService;
//   }

//   [HttpGet]
//   public async Task<List<Pet>> Get()
//   {
//     return await _mongoDBPetService.GetPetsAsync();
//   }

//   [HttpGet("{id:length(24)}")]
//   public async Task<ActionResult<Pet>> Get(string id)
//   {
//     var pet = await _mongoDBPetService.GetPetAsync(id);

//     if (pet is null)
//     {
//       return NotFound();
//     }

//     return pet;
//   }

//   [HttpPost]
//   public async Task<IActionResult> Post([FromBody] Pet pet)
//   {
//     await _mongoDBPetService.CreatePetAsync(pet);
//     return CreatedAtAction(nameof(Get), new { id = pet.Id }, pet);
//   }

//   [HttpPut("{id:length(24)}")]
//   public async Task<IActionResult> Update(string id, Pet updatedPet)
//   {
//     var pet = await _mongoDBPetService.GetPetAsync(id);

//     if (pet is null)
//     {
//       return NotFound();
//     }

//     updatedPet.Id = pet.Id;

//     await _mongoDBPetService.UpdatePetAsync(id, updatedPet);

//     return NoContent();
//   }

//   [HttpDelete("{id}")]
//   public async Task<IActionResult> Delete(string id)
//   {
//     await _mongoDBPetService.DeletePetAsync(id);
//     return NoContent();
//   }
// }
