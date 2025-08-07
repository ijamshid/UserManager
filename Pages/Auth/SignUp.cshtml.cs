using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Threading.Tasks;
using UserManager.Application.Auth.SignUp;

namespace UserManager.Presentation.Pages.Auth;

public class SignUpModel : PageModel
{
    private readonly IMediator _mediator;

    public SignUpModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public SignUpRequestDto user { get; set; }

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
            var result = await _mediator.Send(new SignUpCommand(user));

            HttpContext.Session.SetString("Username", result.Username);

            return RedirectToPage("/Auth/Login");
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            // Catching unique constraint violation error from PostgreSQL wrapped in EF Core exception
            ModelState.AddModelError("user.Email", "This email is already registered!");
            return Page();
        }
        catch (PostgresException ex) when (ex.SqlState == "23505" && ex.ConstraintName == "ix_users_email")
        {
            // Directly catching Postgres unique constraint violation
            ModelState.AddModelError("user.Email", "This email is already registered!");
            return Page();
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
            return Page();
        }
    }

}
