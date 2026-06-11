using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using toad.DTOs;
using toad.Repositories;

namespace toad.Pages;

public class ViewUser : PageModel
{
    private readonly IForumRepository _repo;

    public ViewUser(IForumRepository repo)
    {
        _repo = repo;
    }

    public UserDTO? SelectedUser { get; set; }
    public List<PostDTO> Posts { get; set; } = new();
    public List<CommentDTO> Comments { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        SelectedUser = await _repo.GetUserInfoAsync(id);

        if (SelectedUser == null)
        {
            return NotFound();
        }

        Posts = await _repo.GetPostsByUserIdAsync(id);
        Comments = await _repo.GetCommentsByUserIdAsync(id);

        return Page();
    }
}
