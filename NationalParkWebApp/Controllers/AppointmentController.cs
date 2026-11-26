using Microsoft.AspNetCore.Mvc;
using NationalParkWebApp.Models;
using NationalParkWebApp.Models.ViewModels;
using NationalParkWebApp.Repository.IRepository;
using NationalParkWebApp.Services;
using Stripe.Checkout;
using System.Net.Http.Json;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace NationalParkWebApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly ITrailRepository _trailRepository;
        private readonly EmailSender _emailSender;
        private readonly IConfiguration _configuration;
        //private readonly TwilioSender _twilioSender;

        public AppointmentController(
            IAppointmentRepository appointmentRepository,
            INationalParkRepository nationalParkRepository,
            ITrailRepository trailRepository,
            EmailSender emailSender, IConfiguration configuration)
            //TwilioSender twilioSender
        {
            _appointmentRepository = appointmentRepository;
            _nationalParkRepository = nationalParkRepository;
            _trailRepository = trailRepository;
            _emailSender = emailSender;
            _configuration = configuration;
            //_twilioSender = twilioSender;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int nationalParkId)
        {
            var nationalPark = await _nationalParkRepository.GetAsync(
                SD.NationalParkAPIPath, nationalParkId);

            if (nationalPark == null) return NotFound();

            var trails = await _trailRepository.GetAllAsync(SD.TrailAPIPath);

            var viewModel = new AppointmentVM
            {
                Appointment = new Appointment
                {
                    NationalParkId = nationalPark.Id,
                    VisitDate = DateTime.Today.AddDays(1)
                },
                NationalPark = nationalPark,
                TrailList = trails?.Where(t => t.NationalParkId == nationalPark.Id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentVM model)
        {
            if (model.Appointment == null)
                return BadRequest();

            var nationalPark = await _nationalParkRepository.GetAsync(
                SD.NationalParkAPIPath, model.Appointment.NationalParkId);

            if (nationalPark == null)
                return NotFound("National Park not found.");

            if (!ModelState.IsValid)
            {
                await PopulateAppointmentVM(model, nationalPark);
                return View(model);
            }
            try
            {
                var savedAppointment = await _appointmentRepository.CreateAppointmentAsync(
                    SD.AppointmentAPIPath, model.Appointment);

                if (savedAppointment != null)
                {
                    HttpContext.Session.SetInt32("AppointmentId", savedAppointment.Id);
                    HttpContext.Session.SetInt32("AdultCount", model.Appointment.AdultCount);
                    HttpContext.Session.SetInt32("ChildCount", model.Appointment.ChildCount);

                    TempData["success"] = "Appointment saved successfully!";
                    return RedirectToAction(nameof(Payment));
                }

                ModelState.AddModelError("", "Booking failed. Please try again.");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            await PopulateAppointmentVM(model, nationalPark);
            return View(model);
        }

        private async Task PopulateAppointmentVM(AppointmentVM model, NationalPark nationalPark)
        {
            var trails = await _trailRepository.GetAllAsync(SD.TrailAPIPath);
            model.NationalPark = nationalPark;
            model.TrailList = trails?.Where(t => t.NationalParkId == nationalPark.Id).ToList();
        }

        [HttpGet]
        public IActionResult Payment()
        {
            int adults = HttpContext.Session.GetInt32("AdultCount") ?? 0;
            int children = HttpContext.Session.GetInt32("ChildCount") ?? 0;

            ViewBag.AdultCount = adults;
            ViewBag.ChildCount = children;
            ViewBag.AdultTotal = adults * 200;
            ViewBag.ChildTotal = children * 100;
            ViewBag.TotalAmount = adults * 200 + children * 100;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            int adults = HttpContext.Session.GetInt32("AdultCount") ?? 0;
            int children = HttpContext.Session.GetInt32("ChildCount") ?? 0;

            if (adults < 0 || children < 0 || adults + children == 0)
                return BadRequest("Invalid ticket quantity.");

            var lineItems = new List<SessionLineItemOptions>();

            if (adults > 0)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "inr",
                        UnitAmount = 20000,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Adult National Park Ticket"
                        }
                    },
                    Quantity = adults
                });
            }

            if (children > 0)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "inr",
                        UnitAmount = 10000,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Child National Park Ticket"
                        }
                    },
                    Quantity = children
                });
            }

            var appointmentId = HttpContext.Session.GetInt32("AppointmentId");

            if (appointmentId == null)
                return BadRequest("Appointment not found.");

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                ClientReferenceId = appointmentId.Value.ToString(),
                LineItems = lineItems,
                SuccessUrl = Url.Action(nameof(Success), "Appointment", null, Request.Scheme)
                    + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action(nameof(Payment), "Appointment", null, Request.Scheme)
            };

            var service = new SessionService();
            var checkoutSession = await service.CreateAsync(options);

            return Redirect(checkoutSession.Url);
        }


        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            if (string.IsNullOrWhiteSpace(session_id))
                return BadRequest("Missing Stripe session ID.");

            var service = new SessionService();
            var checkoutSession = await service.GetAsync(session_id);

            if (checkoutSession.PaymentStatus != "paid")
                return Content("Payment has not been completed.");

            int appointmentId = int.Parse(checkoutSession.ClientReferenceId);
            string transactionId = checkoutSession.PaymentIntentId;
            string updateUrl = $"{SD.AppointmentAPIPath}/{appointmentId}/payment";

            using var client = new HttpClient();
            var response = await client.PutAsJsonAsync(updateUrl, transactionId);

            if (!response.IsSuccessStatusCode)
                return Content("Payment succeeded, but updating the appointment failed.");

            var appointment = await _appointmentRepository.GetAsync(
                SD.AppointmentAPIPath, appointmentId);

            if (appointment == null)
                return Content("Payment succeeded, but appointment details could not be retrieved.");

            // Send confirmation email
            await _emailSender.SendAppointmentConfirmationAsync(
                appointment.Email, appointment.Name);

            // Send confirmation SMS
            //await _twilioSender.SendSmsAsync(
            //    appointment.PhoneNumber, appointment.Name);

            //// Make confirmation call
            //await _twilioSender.MakeConfirmationCallAsync(
            //    appointment.PhoneNumber, appointment.Name);

            return View();
        }
[HttpGet]
public async Task<IActionResult> MyAppointments()
        {
            var appointments = await _appointmentRepository.GetAllAsync(
                SD.AppointmentAPIPath);

            return View(appointments);
        }
// GET: Show cancellation confirmation page
[HttpGet]
public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentRepository.GetAsync(
                SD.AppointmentAPIPath, id);

            if (appointment == null)
                return NotFound("Appointment not found.");

            return View(appointment);
        }

        // POST: Confirm cancellation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            // 1. Get the appointment before cancelling it
            var appointment = await _appointmentRepository.GetAsync(
                SD.AppointmentAPIPath, id);

            if (appointment == null)
            {
                TempData["error"] = "Appointment not found.";
                return RedirectToAction(nameof(Cancel), new { id });
            }

            if (appointment.Status == "Cancelled")
            {
                TempData["error"] = "This appointment is already cancelled.";
                return RedirectToAction(nameof(Cancel), new { id });
            }

            // 2. Only paid appointments can be refunded
            if (appointment.Status != "Paid" ||
                string.IsNullOrWhiteSpace(appointment.TransactionId))
            {
                TempData["error"] =
                    "A refund cannot be requested because payment was not confirmed.";
                return RedirectToAction(nameof(Cancel), new { id });
            }

            // 3. Request a full refund from Stripe
            try
            {
                Stripe.StripeConfiguration.ApiKey =
                    _configuration["Stripe:SecretKey"];

                var refundService = new Stripe.RefundService();

                var refund = await refundService.CreateAsync(
                    new Stripe.RefundCreateOptions
                    {
                        PaymentIntent = appointment.TransactionId,
                        Reason = Stripe.RefundReasons.RequestedByCustomer
                    });

                if (refund.Status != "succeeded" &&
                    refund.Status != "pending")
                {
                    TempData["error"] =
                        "The refund was not completed. Please contact support.";
                    return RedirectToAction(nameof(Cancel), new { id });
                }

                // 4. Cancel the appointment through your API
                using var client = new HttpClient();

                var response = await client.PutAsync(
                    $"{SD.AppointmentAPIPath}/{id}/cancel",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["error"] =
                        "The refund was requested, but appointment cancellation failed. Please contact support.";
                    return RedirectToAction(nameof(Cancel), new { id });
                }

                // 5. Send cancellation email
                await _emailSender.SendAppointmentCancellationAsync(
                    appointment.Email,
                    appointment.Name);

                TempData["success"] = refund.Status == "succeeded"
                    ? "Appointment cancelled and refund issued successfully."
                    : "Appointment cancelled. Your refund is pending processing.";
            }
            catch (Stripe.StripeException)
            {
                TempData["error"] =
                    "Stripe could not process the refund. Your appointment has not been cancelled.";
            }

            return RedirectToAction(nameof(Cancel), new { id });
        }
    }
}