using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FlashCard.Api.Data;
using FlashCard.Api.Models;
using System.Security.Claims;

namespace FlashCard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlashcardsController : ControllerBase
{
    private readonly FlashCardDbContext _context;

    public FlashcardsController(FlashCardDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlashcardDTO>>> GetUserFlashcards()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var flashcards = await _context.Flashcards
            .Where(f => f.UserId == userId)
            .Select(f => new FlashcardDTO
            {
                Id = f.Id,
                Front = f.Front,
                Back = f.Back,
                Source = f.Source
            })
            .ToListAsync();

        return Ok(flashcards);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFlashcard(int id, FlashcardDTO flashcardDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var flashcard = await _context.Flashcards
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

        if (flashcard == null)
        {
            return NotFound();
        }

        flashcard.Front = flashcardDto.Front;
        flashcard.Back = flashcardDto.Back;
        flashcard.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlashcard(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var flashcard = await _context.Flashcards
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

        if (flashcard == null)
        {
            return NotFound();
        }

        _context.Flashcards.Remove(flashcard);
        await _context.SaveChangesAsync();
        return NoContent();
    }
} 