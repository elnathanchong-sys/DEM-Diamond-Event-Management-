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

    // READ: Display all payments
    [HttpGet]
    public IActionResult Index()
    {
        var payments = db.Payment.ToList();

        return View(payments);
    }

    // CREATE: GET
    [HttpGet]
    public IActionResult PayCreate()
    {
        return View();
    }

    // CREATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayCreate(Payment model)
    {
        if (ModelState.IsValid)
        {
            db.Payment.Add(model);
            await db.SaveChangesAsync();

            TempData["Info"] = "Payment created successfully.";

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // UPDATE: GET
    [HttpGet]
    public IActionResult PayUpdate(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var payment = db.Payment.Find(id);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // UPDATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayUpdate(Payment model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var payment = db.Payment.Find(model.Id);

        if (payment == null)
        {
            return NotFound();
        }

        payment.PaymentMethod = model.PaymentMethod;
        payment.AmountPaid = model.AmountPaid;
        payment.PaymentDate = model.PaymentDate;
        payment.RegistrationId = model.RegistrationId;

        await db.SaveChangesAsync();

        TempData["Info"] = "Payment updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    // DELETE: GET
    [HttpGet]
    public IActionResult PayDelete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var payment = db.Payment.Find(id);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // DELETE: POST
    [HttpPost, ActionName("PayDelete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayDeleteConfirmed(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var payment = db.Payment.Find(id);

        if (payment != null)
        {
            db.Payment.Remove(payment);
            await db.SaveChangesAsync();

            TempData["Info"] = "Payment deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // SEARCH
    [HttpGet]
    public IActionResult PayRead(string? search)
    {
        var payments = db.Payment.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            payments = payments.Where(p =>
                p.Id.Contains(search) ||
                p.PaymentMethod.Contains(search) ||
                p.RegistrationId.Contains(search));
        }

        return View("Index", payments.ToList());
    }
}