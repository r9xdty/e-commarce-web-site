using CategoriesWithProducts.UI.Helpers;
using CategoriesWithProducts.UI.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace CategoriesWithProducts.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7287/api/auth/register"),
                Content = new StringContent(JsonConvert.SerializeObject(registerRequestDto), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error while registering";
                return RedirectToAction("Register");
            }

            var registerResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();


            if (registerResponseContent is not null)
            {
                TempData["SuccessMessage"] = "Successfully Registered!";
                return RedirectToAction("Login");
            }
            TempData["Error"] = responseContent;
            return RedirectToAction("Register");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl, CartDto cartDto)
            {
            var model = new LoginRequestDto
            {
                ReturnUrl = returnUrl
            };

            var items = new CartDto
            {
                Items = cartDto.Items
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {

            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7287/api/auth/login"),
                Content = new StringContent(JsonConvert.SerializeObject(loginRequestDto), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                TempData["Error"] = "Wrong Email or Password!";
                return RedirectToAction("Login");
            }

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(responseContent);
            Console.WriteLine(loginResponse.JwtToken);

            if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.JwtToken))
            {
                HttpContext.Session.SetString("token", loginResponse.JwtToken);
                HttpContext.Session.SetString("username", loginRequestDto.UserName);
                HttpContext.Session.SetString("id", loginResponse.Id);

                var tokenData = loginResponse.JwtToken;
                var parts = tokenData.Split('.');
                var payload = parts[1];
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonPayload = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                var claimsDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonPayload);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, claimsDict[ClaimTypes.NameIdentifier]?.ToString()),
                    new Claim(ClaimTypes.Email, claimsDict[ClaimTypes.Email]?.ToString()),
                    new Claim("token", loginResponse.JwtToken)
                };

                if (claimsDict.TryGetValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out var roleValue))
                {
                    if (roleValue is JArray rolesArray)
                    {
                        foreach (var role in rolesArray)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleValue.ToString()));
                    }
                }

                var identity = new ClaimsIdentity(claims, "cookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("cookieAuth", principal);

                TempData["SuccessMessage"] = "Successfully logged in!";

                
                var guestCart = CookieHelper.GetGuestCart(HttpContext);

                if (guestCart != null && guestCart.Items.Any())
                {
                    CookieHelper.MergeGuestCartToUserCart(HttpContext, loginResponse.Id);
                    CookieHelper.ClearGuestCart(HttpContext);
                }

                if (!string.IsNullOrWhiteSpace(loginRequestDto.ReturnUrl))
                {
                    return Redirect(loginRequestDto.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Sunucudan geçerli yanıt alınamadı!";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("cookieAuth");
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Successfully logged out!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
