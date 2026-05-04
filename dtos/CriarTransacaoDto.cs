using System.ComponentModel.DataAnnotations;

namespace FinanceiroApi.Dtos;

record CriarTransacaoDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string Descricao { get; init; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public required decimal Valor { get; init; }

    [Required]
    public required DateTime Data { get; init; }
}
