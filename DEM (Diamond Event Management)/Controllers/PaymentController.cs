using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEM__Diamond_Event_Management_.Controllers;

public class PaymentController : Controller
{
    private readonly DB db;

    public PaymentController(DB db)
    {
        this.db = db;
    }

    // PAYMENT PAGE
    [HttpGet]
    public IActionResult Index(string registrationId)
    {
        if (string.IsNullOrEmpty(registrationId))
        {
            return NotFound("Registration ID is missing.");
        }

        var registration = db.Registration
            .Include(r => r.Event)
            .FirstOrDefault(r => r.Id == registrationId);

        if (registration == null)
        {
            return NotFound("Registration not found.");
        }

        // Check if this registration already has a payment
        var existingPayment = db.Payment
            .FirstOrDefault(p => p.RegistrationId == registrationId);

        if (existingPayment != null)
        {
            return View("Success", existingPayment);
        }

        return View(registration);
    }


    // PROCESS PAYMENT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(
        string registrationId,
        string paymentMethod)
    {
        if (string.IsNullOrEmpty(registrationId))
        {
            return NotFound("Registration ID is missing.");
        }

        if (string.IsNullOrEmpty(paymentMethod))
        {
            TempData["Error"] = "Please select a payment method.";
            return RedirectToAction(nameof(Index),
                new { registrationId });
        }

        var registration = db.Registration
            .Include(r => r.Event)
            .FirstOrDefault(r => r.Id == registrationId);

        if (registration == null)
        {
            return NotFound("Registration not found.");
        }

        // Prevent duplicate payment
        var existingPayment = db.Payment
            .FirstOrDefault(p => p.RegistrationId == registrationId);

        if (existingPayment != null)
        {
            return View("Success", existingPayment);
        }

        // Create payment record
        var payment = new Payment
        {
            Id = "P" + Random.Shared.Next(100, 999),
            PaymentMethod = paymentMethod,
            AmountPaid = registration.Event.Price,
            PaymentDate = DateOnly.FromDateTime(DateTime.Now),
            RegistrationId = registrationId
        };

        db.Payment.Add(payment);

        await db.SaveChangesAsync();

        return View("Success", payment);
    }
}