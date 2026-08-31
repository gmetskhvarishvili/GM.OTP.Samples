namespace GM.OTP.Sample.API.Otp;

/// <summary>
/// Result of generating an OTP challenge.
/// </summary>
public sealed record GenerateOtpResultModel
{
    public required Guid ChallengeId { get; init; }
    public required DateTime ExpiresAtUtc { get; init; }
    public string? Message { get; init; }
}

/// <summary>
/// Result of verifying an OTP challenge.
/// </summary>
public sealed record VerifyOtpResultModel
{
    public required bool IsValid { get; init; }
    public string? Message { get; init; }
    public Guid? ChallengeId { get; init; }
}
