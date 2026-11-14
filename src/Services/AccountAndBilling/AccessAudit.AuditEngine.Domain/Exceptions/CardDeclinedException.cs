namespace AccessAudit.AccountAndBilling.Domain.Exceptions;

public sealed class CardDeclinedException(Guid subscriptionId)
    : AccountBillingDomainException("billing.card_declined", "The credit card was declined by the payment gateway.")
{
    public Guid SubscriptionId { get; } = subscriptionId;
}