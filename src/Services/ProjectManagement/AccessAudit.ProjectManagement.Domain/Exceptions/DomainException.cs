namespace AccessAudit.ProjectManagement.Domain.Exceptions;

// Domain knows business rules, never HTTP status codes or database details.
public abstract class DomainException : Exception
{
    public string Code { get; }

    protected DomainException(string code, string message) : base(message) => Code = code;
}
