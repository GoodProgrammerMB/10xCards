using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FlashCard.Api.Services;
using FlashCard.Api.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FlashCard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LearningController : ControllerBase
{
    private readonly ILearningService _learningService;

    public LearningController(ILearningService learningService)
    {
        _learningService = learningService;
    }

    [HttpGet("session")]
    public async Task<ActionResult<IEnumerable<FlashcardDTO>>> GetLearningSession()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var flashcards = await _learningService.GetFlashcardsForSessionAsync(userId);
        return Ok(flashcards);
    }

    [HttpPost("answer")]
    public async Task<IActionResult> ProcessAnswer([FromBody] ProcessAnswerRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var success = await _learningService.ProcessAnswerAsync(userId, request.FlashcardId, request.WasCorrect);

        if (!success)
        {
            // Could be NotFound or BadRequest depending on why ProcessAnswerAsync failed
            return NotFound(new { message = "Flashcard not found or update failed." }); 
        }

        return Ok(); // Indicate success
    }
} 