﻿using Backend.Services.PointsService;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class ErrorResponse
    {
        public string Message { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PointsController : ControllerBase
    {
        private readonly IPointsService _pointsService;

        public PointsController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        [HttpGet("user/{userId}/points-history")]
        public async Task<IActionResult> GetUserPointsHistory(int userId)
        {
            var history = await _pointsService.GetUserPointsHistoryAsync(userId);

            if (history == null || !history.Any())
            {
                return NotFound(new ErrorResponse { Message = "No points history found for this user." });
            }

            return Ok(history);
        }
    }
}
