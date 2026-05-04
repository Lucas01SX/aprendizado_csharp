namespace FinanceiroApi.Dtos;

record TransacaoResponseDto(Guid Id, string Descricao, decimal Valor, DateTime Data);
