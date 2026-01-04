// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HastaneRandevuSistemi.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null, string role = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            
            // Role parametresini query string'den al veya parametre olarak al
            if (string.IsNullOrEmpty(role) && Request.Query.ContainsKey("role"))
            {
                role = Request.Query["role"].ToString();
            }
            
            // Role parametresini sakla (formda kullanılmak üzere)
            if (!string.IsNullOrEmpty(role))
            {
                ViewData["Role"] = role;
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, string role = null)
        {
            returnUrl ??= Url.Content("~/");

            // Role parametresini form'dan al
            if (string.IsNullOrEmpty(role) && Request.Form.ContainsKey("role"))
            {
                role = Request.Form["role"].ToString();
            }
            // Veya query string'den al
            else if (string.IsNullOrEmpty(role) && Request.Query.ContainsKey("role"))
            {
                role = Request.Query["role"].ToString();
            }

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Bu giriş hataları hesap kilitleme olarak sayılmaz
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı giriş yaptı.");
                    
                    // Kullanıcının gerçek rolünü kontrol et
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        
                        _logger.LogInformation($"Kullanıcı {Input.Email} roller: {string.Join(", ", userRoles)}");
                        
                        // Kullanıcının rolüne göre yönlendir
                        if (userRoles.Contains("Doctor"))
                        {
                            _logger.LogInformation("Doktor paneline yönlendiriliyor");
                            return Redirect("/Doctor/Panel");
                        }
                        else if (userRoles.Contains("Patient"))
                        {
                            _logger.LogInformation("Hasta paneline yönlendiriliyor");
                            return Redirect("/Patient/Panel");
                        }
                        else if (userRoles.Contains("Admin"))
                        {
                            _logger.LogInformation("Admin ana sayfaya yönlendiriliyor");
                            return RedirectToAction("Index", "Home");
                        }
                        
                        // Eğer hiçbir rol bulunamadıysa, kullanıcıyı hasta paneli olarak varsay
                        if (userRoles == null || userRoles.Count == 0)
                        {
                            _logger.LogWarning($"Kullanıcı {Input.Email} için rol bulunamadı, varsayılan olarak Hasta Paneli");
                            return Redirect("/Patient/Panel");
                        }
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Kullanıcı hesabı kilitlendi.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // Açıklayıcı Türkçe hata mesajları
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Bu e-posta adresiyle kayıtlı kullanıcı bulunamadı. Lütfen önce kayıt olun.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı. Lütfen tekrar deneyin.");
                        _logger.LogWarning($"Giriş başarısız - e-posta: {Input.Email}");
                    }
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
