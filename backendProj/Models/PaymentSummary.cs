namespace backendProj.Models;

public class PaymentSummary
{
    public PaymentProcessorSummary Default { get; set; }
    public PaymentProcessorSummary Fallback { get; set; }
    
}

public class PaymentProcessorSummary
{
    public int TotalRequests { get; set; }
    public Decimal TotalAmount { get; set; }
}

