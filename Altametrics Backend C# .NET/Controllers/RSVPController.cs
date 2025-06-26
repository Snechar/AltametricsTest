using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Altametrics_Backend_C__.NET.Data;
using Altametrics_Backend_C__.NET.Models.DTOs.RSVP;
using Altametrics_Backend_C__.NET.Models.Entities;
using Altametrics_Backend_C__.NET.Services;

namespace Altametrics_Backend_C__.NET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RSVPController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        private readonly IAuditLogger _auditLogger;

        public RSVPController(AppDBContext context, IMapper mapper, IAuditLogger auditLogger)
        {
            _context = context;
            _mapper = mapper;
            _auditLogger = auditLogger;
        }

        // POST: api/rsvp
        [HttpPost]
        public async Task<IActionResult> CreateRSVP(RSVPReqModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventCode == model.EventCode);
            if (ev == null)
                return NotFound("Event with that code does not exist.");

            var rsvp = _mapper.Map<RSVP>(model);
            rsvp.EventCode = model.EventCode;
            rsvp.EventId = ev.EventId;
            rsvp.CreatedAt = DateTime.UtcNow;

            _context.RSVPs.Add(rsvp);
            await _context.SaveChangesAsync();
            await _auditLogger.LogAsync("RSVP Created", eventId: ev.EventId, email: model.Email);

            var result = _mapper.Map<RSVPRespModel>(rsvp);
            return CreatedAtAction(nameof(GetRSVP), new { id = rsvp.RsvpId }, result);
        }

        // GET: api/rsvp/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRSVP(int id)
        {
            var rsvp = await _context.RSVPs.FindAsync(id);
            if (rsvp == null)
                return NotFound();

            var result = _mapper.Map<RSVPRespModel>(rsvp);
            return Ok(result);
        }

        // GET: api/rsvp/event/{eventCode}
        [HttpGet("event/{eventCode}")]
        public async Task<IActionResult> GetRSVPsForEvent(Guid eventCode)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventCode == eventCode);
            if (ev == null)
                return NotFound("Event not found.");

            var rsvps = await _context.RSVPs
                .Where(r => r.EventCode == ev.EventCode)
                .ToListAsync();

            var result = _mapper.Map<List<RSVPRespModel>>(rsvps);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRSVP([FromBody] RSVPReqModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.RSVPs
                .FirstOrDefaultAsync(r => r.EventCode == model.EventCode && r.Email == model.Email);

            if (existing == null)
                return NotFound("RSVP not found.");

            // Update fields
            existing.GuestName = model.GuestName;
            existing.GuestCount = model.GuestCount;
            existing.ResponseStatus = model.ResponseStatus;
            existing.ReminderRequested = model.ReminderRequested;

            await _context.SaveChangesAsync();
            await _auditLogger.LogAsync("RSVP Updated", eventId: existing.EventId, email: model.Email);
            return Ok("RSVP updated successfully.");
        }

        // DELETE: api/rsvp
        [HttpDelete]
        public async Task<IActionResult> DeleteRSVP([FromQuery] Guid eventCode, [FromQuery] string email)
        {
            var rsvp = await _context.RSVPs
                .FirstOrDefaultAsync(r => r.EventCode == eventCode && r.Email == email);

            if (rsvp == null)
                return NotFound("RSVP not found.");

            _context.RSVPs.Remove(rsvp);
            await _context.SaveChangesAsync();
            await _auditLogger.LogAsync("RSVP Deleted", eventId: rsvp.EventId, email: rsvp.Email);
            return Ok("RSVP deleted successfully.");
        }
      
    }
}
