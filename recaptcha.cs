using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private const string RecaptchaSecretKey = "your_secret_key";
        private const string RecaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] string userId, [FromForm] string password, [FromForm] string recaptchaToken)
        {
            if (string.IsNullOrEmpty(recaptchaToken))
            {
                return BadRequest(new { success = false, message = "reCAPTCHA token is missing" });
            }

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"{RecaptchaVerifyUrl}?secret={RecaptchaSecretKey}&response={recaptchaToken}", null);
                var responseContent = await response.Content.ReadAsStringAsync();
                var recaptchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(responseContent);

                if (recaptchaResponse != null && recaptchaResponse.Success && recaptchaResponse.Score > 0.5)
                {
                    // Proceed with your login/registration logic here
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "reCAPTCHA verification failed" });
                }
            }
        }

        private class RecaptchaResponse
        {
            public bool Success { get; set; }
            public double Score { get; set; }
            public string Action { get; set; }
        }
    }
}
