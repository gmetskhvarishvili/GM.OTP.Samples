namespace GM.OTP.Sample.API.Otp;

public class GenerateOtpResultModel
{
    public Guid ChallengeId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string? Message { get; set; }
}

public class VerifyOtpResultModel
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public Guid? ChallengeId { get; set; }
}
