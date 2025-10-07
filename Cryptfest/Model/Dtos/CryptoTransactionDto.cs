using API.Data.Entities.Wallet;
using Cryptfest.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptfest.Model.Dtos;

public class CryptoTransactionDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? FromAsset { get; set; } = "";
    public string? ToAsset { get; set; } = "";
}
