using Microsoft.AspNetCore.Mvc.RazorPages;
using toad.DTOs;
using toad.Repositories;

namespace toad.Pages;

public class ViewPosts : PageModel
{
    private readonly IForumRepository _repo;

    public List<PostDTO> Posts { get; set; } = new();

    public ViewPosts(IForumRepository repo)
    {
        _repo = repo;
    }

    public async Task OnGetAsync()
    {
        Posts = await _repo.GetAllPostsAsync();
    }
}
