using System.ComponentModel.DataAnnotations;

namespace FinanceiroApi.Dtos;

record CriarUsuarioDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Nome {get; init;}

    [Required]
    [EmailAddress]
    [StringLength(150, MinimumLength = 5)]
    public required string Email {get; init;}
}