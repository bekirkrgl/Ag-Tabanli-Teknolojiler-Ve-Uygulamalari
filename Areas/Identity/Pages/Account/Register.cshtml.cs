// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;

namespace HastaneRandevuSistemi.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null, string role = "Patient")
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["SelectedRole"] = role;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, string role = "Patient")
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Email'i otomatik onayla (geliştirme için)
                    await _emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);
                    await _userManager.UpdateAsync(user);
                    
                    _logger.LogInformation("Yeni hesap şifre ile oluşturuldu.");

                    // Kullanıcıya rol ataması yap
                    if (!string.IsNullOrEmpty(role) && (role == "Patient" || role == "Doctor"))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        _logger.LogInformation($"Kullanıcıya rol atandı: {role}");
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "E-postanızı onaylayın",
                        $"Hesabınızı onaylamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>buraya tıklayın</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        
                        // Role göre yönlendirme ve entity oluşturma yap
                        if (role == "Doctor")
                        {
                            // Doktor entity oluştur
                            var doctor = new Doctor
                            {
                                FirstName = Input.Email.Split('@')[0], // Email'in başını al
                                LastName = "",
                                Email = Input.Email,
                                PhoneNumber = "00000000000", // Geçici telefon numarası
                                IsActive = true,
                                CreatedDate = DateTime.Now,
                                UserId = user.Id
                            };
                            _context.Doctors.Add(doctor);
                            await _context.SaveChangesAsync();
                            
                            return RedirectToAction("Panel", "Doctor");
                        }
                        else if (role == "Patient")
                        {
                            // Hasta entity oluştur
                            var patient = new Patient
                            {
                                FirstName = Input.Email.Split('@')[0], // Email'in başını al
                                LastName = "",
                                Email = Input.Email,
                                PhoneNumber = "0000000000", // Geçici telefon numarası
                                TcNumber = DateTime.Now.Ticks.ToString().Substring(0, 11), // Unique TcNumber oluştur
                                IsActive = true,
                                CreatedDate = DateTime.Now,
                                UserId = user.Id
                            };
                            _context.Patients.Add(patient);
                            await _context.SaveChangesAsync();
                            
                            return RedirectToAction("Panel", "Patient");
                        }
                        
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    // Hata mesajlarını Türkçeleştir
                    string turkishMessage = error.Code switch
                    {
                        "DuplicateUserName" => "Bu e-posta adresi zaten kullanılıyor. Lütfen farklı bir e-posta adresi deneyin.",
                        "DuplicateEmail" => "Bu e-posta adresi zaten kullanılıyor. Lütfen farklı bir e-posta adresi deneyin.",
                        "InvalidEmail" => "Geçersiz e-posta adresi. Lütfen geçerli bir e-posta adresi girin.",
                        "PasswordTooShort" => "Şifre çok kısa. En az 6 karakter olmalı.",
                        "PasswordRequiresDigit" => "Şifre en az bir rakam içermelidir.",
                        "PasswordRequiresLower" => "Şifre en az bir küçük harf içermelidir.",
                        "PasswordRequiresUpper" => "Şifre en az bir büyük harf içermelidir.",
                        "PasswordRequiresNonAlphanumeric" => "Şifre en az bir özel karakter içermelidir.",
                        _ => $"Kayıt olurken bir hata oluştu: {error.Description}"
                    };
                    ModelState.AddModelError(string.Empty, turkishMessage);
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["SelectedRole"] = role;
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
