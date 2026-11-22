namespace backendProj.Shared.Models;

public class Payment
{
    public string CorrelationId { get; set; } = String.Empty;
    public decimal Amount { get; set; }
}