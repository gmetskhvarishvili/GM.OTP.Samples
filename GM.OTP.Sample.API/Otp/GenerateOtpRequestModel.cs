namespace GM.OTP.Sample.API.Otp;

/// <summary>
/// Request payload to generate a new OTP challenge.
/// </summary>
public sealed record GenerateOtpRequestModel
{
    public required string Subject { get; init; }
    public required string Destination { get; init; }
    public required string Purpose { get; init; }
}
