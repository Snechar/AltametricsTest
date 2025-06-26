using Altametrics_Backend_C__.NET.Data;
using Altametrics_Backend_C__.NET.Models.DTOs.Event;
using Altametrics_Backend_C__.NET.Models.DTOs.RSVP;
using Altametrics_Backend_C__.NET.Models.Entities;
using Altametrics_Backend_C__.NET.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using System.Security.Claims;
public class EventWithRsvpsDto
{
    public EventRespModel Event { get; set; } = default!;
    public List<RSVPRespModel> RSVPs { get; set; } = new();
}

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditLogger _auditLogger;

    public EventController(AppDBContext context, IMapper mapper, IAuditLogger auditLogger)
    {
        _context = context;
        _mapper = mapper;
        _auditLogger = auditLogger;
    }

    // POST: api/event

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public async Task<IActionResult> CreateEvent(CreateEventModel model)
    {
        var ev = _mapper.Map<Event>(model);
        ev.UserId = GetUserId(); 
        ev.EventCode = Guid.NewGuid();
        ev.CreatedAt = DateTime.UtcNow;
        if (model.EventDate < DateTime.UtcNow)
            return BadRequest("Event date cannot be in the past.");

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();
        await _auditLogger.LogAsync("Event Created", eventId: ev.EventId, userId: ev.UserId);
        var result = _mapper.Map<EventRespModel>(ev);
        return CreatedAtAction(nameof(GetEvent), new { eventCode = ev.EventCode }, result);
    }

    // GET event by GUID: api/event/{eventCode}, accessible to anyone
    [HttpGet("{eventCode:guid}")]
    [AllowAnonymous]

    public async Task<IActionResult> GetEvent(Guid eventCode)
    {
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventCode == eventCode);
        if (ev == null) return NotFound();

        return Ok(_mapper.Map<EventRespModel>(ev));
    }

    // PUT: api/event/{id}
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public async Task<IActionResult> UpdateEvent(int id, EventUpdateModel model)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();

        int userId;
        try
        {
            userId = GetUserId();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Missing user ID");
        }
        if (model.EventDate < DateTime.UtcNow)
            return BadRequest("Event date cannot be in the past.");

        if (ev.UserId != userId) return Forbid();

        _mapper.Map(model, ev);
        await _context.SaveChangesAsync();
        await _auditLogger.LogAsync("Event Updated", eventId: ev.EventId, userId: userId);
        return Ok(_mapper.Map<EventRespModel>(ev));
    }

    // DELETE: api/event/{id}
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.UserId != GetUserId()) return Forbid();

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        await _auditLogger.LogAsync("Event Updated", eventId: ev.EventId, userId: ev.UserId);
        return Ok();
    }

    [HttpGet("my")]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public async Task<IActionResult> GetMyEvents(
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20)
    {
        int userId = GetUserId();

        // Enforce max pageSize of 50
        pageSize = Math.Clamp(pageSize, 1, 50);
        page = Math.Max(page, 1);

        var events = await _context.Events
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EventDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = _mapper.Map<List<EventRespModel>>(events);

        var response = new
        {
            Page = page,
            TotalCount = events.Count,
            Events = result
        };

        return Ok(response);
    }

    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEventsByUser(int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
        )
    {
        var events = await _context.Events
             .Where(e => e.UserId == userId)
             .OrderByDescending(e => e.EventDate)
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();

        if (!events.Any())
            return NotFound($"No events found for user ID {userId}.");

        var result = _mapper.Map<List<EventRespModel>>(events);
        var response = new
        {
            Page = page,
            TotalCount = events.Count,
            Events = result
        };

        return Ok(response);
    }


    [HttpGet("all")]
    [AllowAnonymous]


    public async Task<IActionResult> GetAllEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        // Enforce max pageSize of 50
        pageSize = Math.Clamp(pageSize, 1, 50);
        page = Math.Max(page, 1);

        var eventsQuery = _context.Events
            .OrderByDescending(e => e.EventDate);

        var totalCount = await eventsQuery.CountAsync();
        var events = await eventsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = _mapper.Map<List<EventRespModel>>(events);
        var response = new
        {
            Page = page,
            TotalCount = totalCount,
            Events = result
        };

        return Ok(response);
    }
    [HttpGet("{eventCode:guid}/details")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetEventWithRsvps(Guid eventCode)
    {
        int userId;
        try
        {
            userId = GetUserId();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }

        var ev = await _context.Events
            .FirstOrDefaultAsync(e => e.EventCode == eventCode && e.UserId == userId);

        if (ev == null)
            return NotFound("Event not found or access denied.");

        var rsvps = await _context.RSVPs
            .Where(r => r.EventCode == eventCode)
            .ToListAsync();

        var eventDto = _mapper.Map<EventRespModel>(ev);
        var rsvpDtos = _mapper.Map<List<RSVPRespModel>>(rsvps);

        var result = new EventWithRsvpsDto
        {
            Event = eventDto,
            RSVPs = rsvpDtos
        };

        return Ok(result);
    }
  [HttpGet("{eventCode}/invite")]
[Authorize]

    public async Task<IActionResult> GetInviteLink(Guid eventCode)
    {
        var userId = GetUserId(); // your helper to get logged-in user ID
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventCode == eventCode);

        if (ev == null)
            return NotFound();

        if (ev.UserId != userId)
            return Forbid();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var inviteLink = $"{baseUrl}/rsvp?eventCode={eventCode}";
        return Ok(new { inviteLink });
    }


    //Quick helper to get the user ID from the claims
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? throw new UnauthorizedAccessException("Missing user ID"));
    }
}