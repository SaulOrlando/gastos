using FinanzApp.Web.Models;
using FinanzApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanzApp.Web.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Currency = model.Currency
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Dashboard");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, TranslateIdentityError(error.Code, error.Description));
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "El correo o la contraseña no son correctos. Inténtalo de nuevo.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        ModelState.AddModelError(string.Empty, "El correo o la contraseña no son correctos. Inténtalo de nuevo.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is not null)
        {
            var code = await _userManager.GenerateTwoFactorTokenAsync(
                user, TokenOptions.DefaultEmailProvider);

            Console.WriteLine($"[DEBUG OTP] El código para {model.Email} es: {code}");

            var body = $@"
                <p>Hola {user.FullName},</p>
                <p>Recibimos una solicitud para recuperar tu contraseña. Usa este código de verificación:</p>
                <p style=""font-size:32px;font-weight:bold;letter-spacing:8px;color:#14532d;"">{code}</p>
                <p>El código es válido por unos minutos. Si no fuiste tú, ignora este correo.</p>";

            try
            {
                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Tu código para recuperar la contraseña",
                    body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Auth] Fallo al enviar OTP a {model.Email}: {ex}");
                TempData["OtpSendError"] = true;
            }
        }

        return RedirectToAction("VerifyOtp", new { email = model.Email });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult VerifyOtp(string? email = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new VerifyOtpViewModel { Email = email });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            return RedirectToAction(nameof(ForgotPassword));
        }

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultEmailProvider, model.Code.Trim());

        if (!isValid)
        {
            ModelState.AddModelError(nameof(model.Code), "El código es incorrecto o ha expirado.");
            return View(model);
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        TempData["ResetToken"] = resetToken;

        return RedirectToAction("ResetPassword", new { email = user.Email });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? email = null)
    {
        var resetToken = TempData["ResetToken"] as string;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(resetToken))
        {
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new ResetPasswordViewModel
        {
            Email = email,
            Token = resetToken
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, TranslateIdentityError(error.Code, error.Description));
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private static string TranslateIdentityError(string code, string defaultDescription) => code switch
    {
        "DuplicateUserName" or "DuplicateEmail" => "Ya existe una cuenta con ese correo. Inicia sesión en lugar de registrarte.",
        "PasswordTooShort" => "Tu contraseña necesita al menos 6 caracteres.",
        "InvalidEmail" => "Ese correo no parece válido, revísalo.",
        _ => defaultDescription
    };
}
