using System.ComponentModel.DataAnnotations;

namespace FinanceiroApi.Dtos;

record LoginDto
{
    [Required]
    [EmailAddress]
    [StringLength(150, MinimumLength = 5)]
    public required string Email {get; init;}

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public required string Senha {get; init;}
}