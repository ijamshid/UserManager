using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManager.Application.UserMediator.DeleteUser;
using UserManager.Application.UserMediator.GetUsers;
using UserManager.Application.UserMediator.UpdateUserStatus;
using UserManager.Domain.Enums;

namespace UserManager.Presentation.Pages.Users
{
    public class UsersPageModel : PageModel
    {
        private readonly IMediator _mediator;
        public UsersPageModel(IMediator mediator) => _mediator = mediator;

        public IList<UserDto> Users { get; set; } = new List<UserDto>();

        public async Task OnGetAsync()
        {
            Users = await _mediator.Send(new GetUsersQuery());
        }

        public async Task<IActionResult> OnPostBlockSelectedAsync([FromForm] int[] selectedUserIds)
        {
            // Sessiondan userId ni olamiz
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
                return RedirectToPage("/Auth/Login");

            bool blockedSelf = false;
            foreach (var id in selectedUserIds ?? Array.Empty<int>())
            {
                await _mediator.Send(new UpdateUserStatusCommand(id, Status.Blocked));
                if (id == currentUserId.Value) blockedSelf = true;
            }

            if (blockedSelf)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Auth/Login");
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostUnblockSelectedAsync([FromForm] int[] selectedUserIds)
        {
            foreach (var id in selectedUserIds ?? Array.Empty<int>())
                await _mediator.Send(new UpdateUserStatusCommand(id, Status.Active));
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteSelectedAsync([FromForm] int[] selectedUserIds)
        {
            foreach (var id in selectedUserIds ?? Array.Empty<int>())
                await _mediator.Send(new DeleteUserCommand(id));
            return RedirectToPage();
        }

    }
}
