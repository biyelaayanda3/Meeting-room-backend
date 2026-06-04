using MeetingRoomBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
   
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllRooms()
    {
        _logger.LogInformation("Getting all rooms");
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetRoom(int id)
    {
        _logger.LogInformation("Getting room with ID {Id}", id);
        var room = await _roomService.GetRoomByIdAsync(id);
        return Ok(room);
    }
 }