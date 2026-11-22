using backendProj.Shared.Models;

namespace backendProj.Services;

public interface IPaymentQueue
{
    Task EnqueueAsync(Payment payment, CancellationToken cancellationToken);
}