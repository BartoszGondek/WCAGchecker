# Tactical Design: Value Objects & Identifiers
**Domain:** WCAG AccessAudit System

## 1. Domain Value Objects (Primitive Obsession Elimination)

| Value Object Name | Encapsulated Data | Guard Clauses (Creation) | Immutability Proof |
| :--- | :--- | :--- | :--- |
| **WebsiteUrl** | `string Value` | The input must not be empty and must contain a valid `http://` or `https://` prefix. Otherwise, `InvalidWebsiteUrlException` is thrown. | `public WebsiteUrl ToHttps()`<br><br>Generates and returns a completely new instance of `WebsiteUrl` with a secure scheme, without mutating the current object. |
| **WcagVersion** | `string Version` | The provided string must match the strictly supported standards: `"2.0"`, `"2.1"`, or `"2.2"`. Rejects other inputs via `UnsupportedWcagVersionException`. | `public WcagVersion UpgradeToLatest()`<br><br>Yields a new `WcagVersion` pointing to the latest standard (e.g., 2.2). |
| **AuditScore** | `int Score` | Checks if the integer is within the `0 - 100` range. Exceeding these bounds triggers `InvalidAuditScoreException`. | `public AuditScore ApplyPenalty(int penalty)`<br><br>Calculates the new score and returns a fresh `AuditScore` object. |
| **MoneyAmount** | `decimal Value`<br>`string Currency` | Validates that the amount is `>= 0` and the currency is a 3-character ISO string. Throws `InvalidCurrencyException` on failure. | `public MoneyAmount Add(MoneyAmount other)`<br><br>Throws `CurrencyMismatchException` if currencies do not match, or returns a new `MoneyAmount` with updated totals. |

## 2. Strongly Typed Identity Wrappers

To enforce type safety at compile time and prevent accidental swapping of raw `Guid` parameters, our bounded contexts utilize explicit ID records:

* **`ProjectId`** – secures the Project aggregate root.
* **`AuditScopeId`** – secures the AuditScope aggregate root.
* **`SubscriptionId`** – secures the Subscription aggregate root.
* **`ComplianceReportId`** – secures the ComplianceReport aggregate root.