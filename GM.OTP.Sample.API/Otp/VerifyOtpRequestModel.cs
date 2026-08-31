namespace GM.OTP.Sample.API.Otp;

/// <summary>
/// Request payload to verify a previously generated OTP challenge.
/// </summary>
public sealed record VerifyOtpRequestModel
{
    public required string Subject { get; init; }
    public required string Purpose { get; init; }
    public required string Code { get; init; }
}
