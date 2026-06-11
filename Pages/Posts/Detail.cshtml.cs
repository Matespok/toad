using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using toad.DTOs;
using toad.Repositories;

namespace toad.Pages;

public class FocusPost : PageModel
{
    private readonly IForumRepository _repo;

    public FocusPost(IForumRepository repo)
    {
        _repo = repo;
    }

    public PostDTO? FocusedPost { get; set; }
    public List<CommentDTO> CommentList { get; set; } = new();

    [BindProperty]
    public string Reply { get; set; } = string.Empty;

    [BindProperty]
    public int? ParentCommentId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        FocusedPost = await _repo.GetPostByIdAsync(id);

        if (FocusedPost == null)
        {
            return NotFound();
        }

        CommentList = await _repo.GetCommentsForPostAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");

        if (currentUserId.HasValue && !string.IsNullOrWhiteSpace(Reply))
        {
            await _repo.AddCommentAsync(currentUserId.Value, id, ParentCommentId, Reply);
        }

        return RedirectToPage(new { id = id });
    }
}
