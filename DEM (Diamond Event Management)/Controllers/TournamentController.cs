using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEM__Diamond_Event_Management_.Controllers
{
    public class TournamentRegistrationController : Controller
    {
        private readonly DB _context;

        public TournamentRegistrationController(DB context)
        {
            _context = context;
        }

        // Show registration form
        public IActionResult Register(int tournamentId)
        {
            var tournament = _context.Tournament.Find(tournamentId);
            if (tournament == null) return NotFound();

            ViewBag.Tournament = tournament;
            return View(new TournamentRegistration { TournamentId = tournamentId });
        }

        [HttpPost]
        public IActionResult Register(TournamentRegistration model)
        {
            var tournament = _context.Tournament.Find(model.TournamentId);
            if (tournament == null) return NotFound();

            // Validation
            if (DateTime.Now.Date > tournament.RegistrationDeadline.ToDateTime(TimeOnly.MinValue))
            {
                ModelState.AddModelError("", "Registration deadline has passed.");
            }
            var team = _context.Team.Include(t => t.Players).FirstOrDefault(t => t.Id == model.TeamId);
            if (team == null)
            {
                ModelState.AddModelError("", "Team not found.");
            }
            else
            {
                if (team.Players.Count < tournament.MinPlayersPerTeam || team.Players.Count > tournament.MaxPlayersPerTeam)
                {
                    ModelState.AddModelError("", $"Team must have between {tournament.MinPlayersPerTeam} and {tournament.MaxPlayersPerTeam} players.");
                }
            }
            if (_context.TournamentRegistration.Count(r => r.TournamentId == tournament.Id) >= tournament.MaxTeams)
            {
                ModelState.AddModelError("", "No slots available.");
            }

            if (ModelState.IsValid)
            {
                model.RegisteredAt = DateTime.UtcNow;
                model.Status = RegistrationStatus.Pending;
                _context.TournamentRegistration.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Status", new { id = model.Id });
            }

            ViewBag.Tournament = tournament;
            return View(model);
        }

        // Admin: Approve registration
        public IActionResult Approve(int id)
        {
            var reg = _context.TournamentRegistration.Find(id);
            if (reg != null)
            {
                reg.Status = RegistrationStatus.Approved;
                reg.ReviewedAt = DateTime.UtcNow;
                reg.ReviewedByEmail = User.Identity?.Name;
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }

        // Admin: Reject registration
        public IActionResult Reject(int id, string note)
        {
            var reg = _context.TournamentRegistration.Find(id);
            if (reg != null)
            {
                reg.Status = RegistrationStatus.Rejected;
                reg.AdminNote = note;
                reg.ReviewedAt = DateTime.UtcNow;
                reg.ReviewedByEmail = User.Identity?.Name;
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }

        // Admin: List all registrations
        public IActionResult List()
        {
            var regs = _context.TournamentRegistration
                .Include(r => r.Tournament)
                .Include(r => r.Team)
                .ToList();
            return View(regs);
        }

        // Team captain: View status
        public IActionResult Status(int id)
        {
            var reg = _context.TournamentRegistration
                .Include(r => r.Tournament)
                .Include(r => r.Team)
                .FirstOrDefault(r => r.Id == id);

            if (reg == null) return NotFound();
            return View(reg);
        }
    }
}
