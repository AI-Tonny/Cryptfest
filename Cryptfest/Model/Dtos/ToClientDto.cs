using Cryptfest.Enums;

namespace Cryptfest.Model.Dtos;

public class ToClientDto
{
    public string? Message { get; set; }
    public ResponseStatus Status {  get; set; }    
    public object? Data { get; set; }
}
