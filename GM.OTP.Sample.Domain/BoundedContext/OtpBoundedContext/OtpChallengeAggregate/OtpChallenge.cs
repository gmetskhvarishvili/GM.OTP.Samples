using GM.EntityFramework.Domain.Abstractions;

namespace GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate;

public sealed class OtpChallenge : GM.OTP.Domain.Entities.OtpChallenge, IAggregateRoot
{
    // For EF materialization; delegates to the protected base parameterless constructor.
    private OtpChallenge()
    {
    }

    // Delegates to the base constructor so all invariants (validation, CreatedAtUtc, initial flags) run.
    private OtpChallenge(
        string subject,
        string destination,
        string codeHash,
        string salt,
        string purpose,
        DateTime expiresAtUtc,
        int maxAttempts,
        Guid? userId)
        : base(subject, destination, codeHash, salt, purpose, expiresAtUtc, maxAttempts, userId)
    {
    }

    public static OtpChallenge Create(
        string subject,
        string destination,
        string codeHash,
        string salt,
        string purpose,
        DateTime expiresAtUtc,
        int maxAttempts,
        Guid? userId = null) =>
        new(subject, destination, codeHash, salt, purpose, expiresAtUtc, maxAttempts, userId);
}
