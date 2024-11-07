// File: Models/ReviewViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace OnlineMarketplace.Models;

public class ReviewViewModel
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    public string? ApplicationUserId { get; set; }

    public string? ProductName { get; set; }

    [Required]
    [Range(1,5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
    public string Comment { get; set; }
}
