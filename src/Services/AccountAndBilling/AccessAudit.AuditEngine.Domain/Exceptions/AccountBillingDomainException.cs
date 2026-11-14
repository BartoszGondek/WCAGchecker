namespace AccessAudit.AccountAndBilling.Domain.Exceptions;

public abstract class AccountBillingDomainException : Exception
{
    public string Code { get; }
    protected AccountBillingDomainException(string code, string message) : base(message) => Code = code;
}