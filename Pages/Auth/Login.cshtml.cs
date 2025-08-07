using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManager.Application.Auth.SignIn;

namespace UserManager.Presentation.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IMediator _mediator;
        public LoginModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public SignInRequestDto Dto { get; set; }



        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var command = new SignInCommand(Dto);
                var result = await _mediator.Send(command);

                // Misol uchun result.UserId bo'lsa:
                HttpContext.Session.SetInt32("UserId", result.UserId);

                // Agar JWT token ishlatilsa:
                // Response.Cookies.Append("jwtToken", result.Token, new CookieOptions { HttpOnly = true, Secure = true });

                return RedirectToPage("/Users/UsersPage");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }



    }
}