using BeCoreApp.Data.Enums;
using Core.Application.Interfaces;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Extensions;
using Core.Models.AccountViewModels;
using Core.Services;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Core.Utilities.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Core.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserService _userService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IBlockChainService _blockChainService;
        private readonly ILogger<AccountController> _logger;
        private readonly IViewRenderService _viewRenderService;

        public AccountController(
            IBlockChainService blockChainService,
            IUserService userService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailSender emailSender,
            RoleManager<AppRole> roleManager,
            IViewRenderService viewRenderService,
            ILogger<AccountController> logger)
        {
            _blockChainService = blockChainService;
            _userService = userService;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login(string returnUrl, int c = 0)
        {
            if (c == 1)
                ViewBag.ConfirmEmail = true;

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateRecaptcha]
        [ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginVm)
        {
            _logger.LogInformation($"Login start {loginVm.Email}");
            try
            {
                _logger.LogInformation($"Login Model State {ModelState.IsValid}");
                if (ModelState.IsValid)
                {

                    var currentUser = await _userManager.FindByNameAsync(loginVm.Email);

                    if (currentUser == null)
                    {
                        ModelState.AddModelError(string.Empty, "Wrong login");
                        return View(loginVm);
                    }

                    if (currentUser.EmailConfirmed == false)
                    {
                        ModelState.AddModelError(string.Empty, "Email has not been confirmed");
                        return View(loginVm);
                    }

                    if (currentUser.Status != Status.Active)
                    {
                        ModelState.AddModelError(string.Empty, "The account is locked");
                        return View(loginVm);
                    }

                    var result = await _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Login in success.");
                        return Redirect("/home");
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return Redirect($"/LoginWith2fa?rememberMe={loginVm.RememberMe}&returnUrl={loginVm.ReturnUrl}");
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("The account is locked.");

                        ModelState.AddModelError(string.Empty, "The account is locked");
                        return View(loginVm);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Wrong login");
                        return View(loginVm);
                    }
                }

                return View(loginVm);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Login Exception {e.Message}");
                throw;
            }

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register")]
        public IActionResult Register(string sponsor)
        {
            if (string.IsNullOrWhiteSpace(sponsor))
                sponsor = $"{CommonConstants.DEFAULT_SPONSOR}";

            return View(new RegisterViewModel
            {
                Sponsor = sponsor
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateRecaptcha]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerVm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new OkObjectResult(new GenericResult(false,
                            string.Join(',', ModelState.Values.SelectMany(v => v.Errors)
                            .Select(x => x.ErrorMessage))));

                if (registerVm.Sponsor.Length <= 10)
                    return new OkObjectResult(new GenericResult(false,
                            "Sponsor does not exists."));


                var userSponsor = _userManager.Users
                    .FirstOrDefault(x => x.Sponsor == registerVm.Sponsor.GetRawSponsor());
                if (userSponsor == null)
                    return new OkObjectResult(new GenericResult(false,
                            "Sponsor does not exists."));

                var currentUserName = await _userManager.FindByNameAsync(registerVm.Email);
                if (currentUserName != null)
                    return new OkObjectResult(new GenericResult(false,
                            "Email already exists."));

                var currentUserEmail = await _userManager.FindByEmailAsync(registerVm.Email);
                if (currentUserEmail != null)
                    return new OkObjectResult(new GenericResult(false, "Email already exists."));


                string newSponsor = _userService.GenerateReferralCode();

                var customer = new AppUser
                {
                    Sponsor = newSponsor,
                    ReferralId = userSponsor.Id,
                    UserName = registerVm.Email,
                    Email = registerVm.Email,
                    IsSystem = false,
                    Status = Status.Active,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    USDTAmount = 0,
                    HBTAmount = 0,
                    StakingLevel = StakingLevel.Member,
                    StakingAmount = 0,
                    StakingAffiliateAmount = 0,
                    IsShowOff = false
                };

                var result = await _userManager.CreateAsync(customer, registerVm.Password);
                if (result.Succeeded)
                {
                    var appRole = await _roleManager.FindByNameAsync("Customer");
                    if (appRole == null)
                    {
                        await _roleManager.CreateAsync(new AppRole
                        {
                            Name = "Customer",
                            NormalizedName = "Customer",
                            Description = "Customer is role use for member"
                        });
                    }
                    await _userManager.AddToRoleAsync(customer, "Customer");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(customer);
                    var callbackUrl = Url.EmailConfirmationLink(customer.Id, code, Request.Scheme);

                    var content = await _viewRenderService
                        .RenderToStringAsync("EmailTemplate/_VerifyAccount", callbackUrl);

                    await _emailSender.SendEmailAsync(registerVm.Email,
                        $"{CommonConstants.PROJECT_NAME}: Verify Email", content);

                    return new OkObjectResult(new GenericResult(true,
                        "The account has been registered successfully," +
                        " Please check your email to confirm the account."));
                }
                else
                {
                    return new OkObjectResult(new GenericResult(false,
                        string.Join(',', result.Errors.Select(x => x.Description))));
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("resendemailconfirm")]
        public IActionResult ResendEmailConfirm()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirm(ResendEmailConfirmViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new OkObjectResult(new GenericResult(false,
                        string.Join(',', ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage))));
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new OkObjectResult(new GenericResult(false, "Email does not exist."));
                }

                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (isEmailConfirmed == true)
                {
                    return new OkObjectResult(new GenericResult(false, "Email has been verified."));
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                var content = await _viewRenderService.RenderToStringAsync("EmailTemplate/_VerifyAccount", callbackUrl);

                await _emailSender.SendEmailAsync(model.Email, $"{CommonConstants.PROJECT_NAME}: Resend Email Confirm", content);

                return new OkObjectResult(new GenericResult(true, "Resend email confirm successfully, Please check your email to confirm the account."));
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("forgotpassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new OkObjectResult(new GenericResult(false, string.Join(',', ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage))));
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new OkObjectResult(new GenericResult(false, "Email does not exist."));
                }

                if (user.IsShowOff)
                    return new OkObjectResult(new GenericResult(false, "Account does not allow to forgot password"));

                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return new OkObjectResult(new GenericResult(false, "Email has not been verified."));
                }

                if (user.IsShowOff)
                    return new OkObjectResult(new GenericResult(false, "Reset password failed , contact support"));

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);

                var content = await _viewRenderService.RenderToStringAsync("EmailTemplate/_ForgotPassword", callbackUrl);

                await _emailSender.SendEmailAsync(model.Email, $"{CommonConstants.PROJECT_NAME}: Forgot password", content);

                return new OkObjectResult(new GenericResult(true));
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return Redirect("/home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Redirect("/home");

            if (user.EmailConfirmed)
                return Redirect("/login?c=1");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                return Redirect("/home");

            var wallet = _blockChainService.CreateAccount();

            user.PublishKey = wallet.Address;
            user.PrivateKey = wallet.PrivateKey;

            await _userManager.UpdateAsync(user);

            return Redirect("/login?c=1");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string code = null, string userId = null)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId))
                return Redirect("/home");

            var user = await _userManager.FindByIdAsync(userId);

            if (user.IsShowOff)
                return Redirect("/home");

            var model = new ResetPasswordViewModel { Code = code, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new OkObjectResult(new GenericResult(false, string.Join(',', ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage))));
                }

                if (model.Email.IsMissing())
                {
                    return new OkObjectResult(new GenericResult(false, "Email is required."));
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new OkObjectResult(new GenericResult(false, "Email does not exists"));
                }

                if (user.IsShowOff)
                    return new OkObjectResult(new GenericResult(false, "Email does not exists"));

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {

                    await _userManager.SetLockoutEnabledAsync(user, false);
                    await _userManager.SetLockoutEndDateAsync(user, null);


                    return new OkObjectResult(new GenericResult(true));
                }
                else
                {
                    return new OkObjectResult(new GenericResult(false, string.Join(',', result.Errors.Select(x => x.Description))));
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("resetpasswordconfirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [Route("Logout")]
        public async Task<ActionResult> Signout()
        {
            await _signInManager.SignOutAsync();

            return Redirect("/");
        }

        public async Task<IActionResult> Profile()
        {
            var model = await _userService.GetById(CurrentUserId.ToString());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordManual([FromBody] ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new OkObjectResult(new GenericResult(false, string.Join(',', ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage))));
                }


                var user = await _userManager.FindByIdAsync(CurrentUserId.ToString());

                var correctedPassword = await _userManager.CheckPasswordAsync(user, model.OldPassword);

                if (!correctedPassword)
                {
                    return new OkObjectResult(new GenericResult(false, "Incorrect Old Password"));
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

                if (result.Succeeded)
                {
                    return new OkObjectResult(new GenericResult(true));
                }
                else
                {
                    return new OkObjectResult(new GenericResult(false, string.Join(',', result.Errors.Select(x => x.Description))));
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new GenericResult(false, ex.Message));
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("LoginWith2fa")]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            _logger.LogInformation($"LoginWith2fa begin");
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                _logger.LogInformation($"LoginWith2fa user not found");

                return RedirectToAction("login");
            }

            _logger.LogInformation($"LoginWith2fa success {user.Email}");

            var model = new LoginWith2faViewModel { RememberMe = rememberMe, ReturnUrl = returnUrl };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("LoginWith2fa")]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation($"LoginWith2fa Submit");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogInformation($"LoginWith2fa Submit User not found");

                return RedirectToAction("login");
                //throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            _logger.LogWarning($"User authenticator code {model.TwoFactorCode} - {authenticatorCode} entered for user with ID {user.Id}. {user.Email}");

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return Redirect("/home");
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }
    }
}
