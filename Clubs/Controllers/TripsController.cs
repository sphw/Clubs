using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clubs.Data;
using Clubs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace Clubs.Controllers
{
    [Authorize]
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public TripsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var trips = (await _context.Trip.Include(t => t.TripUsers).ToListAsync()).Select(t =>
            {
                var tripUser = t.TripUsers.FirstOrDefault(tu => tu.UserId == user.Id);
                return new TripViewModel
                {
                    TripId = t.TripId,
                    Name = t.Name,
                    BodyRendered = t.BodyRendered,
                    Date = t.Date,
                    Price = t.Price,
                    IsOrganizer = tripUser?.Role == Role.Organizer,
                    IsVisible = t.Visible || (tripUser != null),
                    RequiredQualifications = t.RequiredQualifications
                };
            });
            return View(trips);
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trip
                .FirstOrDefaultAsync(m => m.TripId == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // GET: Trips/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TripId,Name,Date,Body,Visible,Price")] Trip trip, [FromForm] string requiredQualifications)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                trip.TripId = Guid.NewGuid();
                var qualifications = JsonConvert.DeserializeObject<IList<string>>(requiredQualifications);
                trip.RequiredQualifications = qualifications;
                trip.TripUsers = new List<TripUser>();
                trip.TripUsers.Add(new TripUser
                {
                    User = user,
                    Role = Role.Organizer
                });
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trip.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TripId,Name,Date,Body,Visible,Price")] Trip trip, [FromForm] string requiredQualifications)
        {
            if (id != trip.TripId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var qualifications = JsonConvert.DeserializeObject<IList<string>>(requiredQualifications);
                    trip.RequiredQualifications = qualifications;
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.TripId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trip
                .FirstOrDefaultAsync(m => m.TripId == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var trip = await _context.Trip.FindAsync(id);
            _context.Trip.Remove(trip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Trips/Edit/5
        public async Task<IActionResult> SignUp(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trip.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            return View(trip);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSession(Guid id)
        {
            var trip = await _context.Trip.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"https://localhost:5001/Trips/PaymentSuccess/{trip.TripId}/",
                CancelUrl = $"https://localhost:5001/Trips/SignUp/{trip.TripId}",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Currency = "NZD",
                        Name = trip.Name,
                        Amount = (long)(trip.Price * 100),
                        Quantity = 1,
                    }
                },
                Mode = "payment",
            };
            var service = new SessionService();
            var session = service.Create(options);
            return new JsonResult(new {
                Id = session.Id,
            });
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSuccess(Guid id)
        {
             
        }


        private bool TripExists(Guid id)
        {
            return _context.Trip.Any(e => e.TripId == id);
        }
    }
}
