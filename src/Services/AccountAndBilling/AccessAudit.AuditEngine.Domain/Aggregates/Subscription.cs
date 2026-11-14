using AccessAudit.AccountAndBilling.Domain.Exceptions;

namespace AccessAudit.AccountAndBilling.Domain.Aggregates;

// Agregat odpowiadający żółtej karteczce "Subscription" z tablicy Event Storming
public sealed class Subscription
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public bool IsActive { get; private set; }

    public Subscription(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        IsActive = false;
    }

    // Komenda "Process payment" z tablicy Event Storming
    public void ProcessPayment(bool paymentGatewaySuccess)
    {
        // HOT SPOT (Bordowa karteczka): "Who is responsible for storing personal and payment sensitive data?"
        // ROZWIĄZANIE: Ten agregat domenowy celowo nie przyjmuje numeru karty kredytowej.
        // Polega wyłącznie na statusie (paymentGatewaySuccess) zwróconym przez zewnętrzny Payment System.

        if (!paymentGatewaySuccess)
        {
            // Realizacja różowej karteczki (Parallel Variant): "Card Declined"
            throw new CardDeclinedException(Id);
        }

        IsActive = true;
    }
}   