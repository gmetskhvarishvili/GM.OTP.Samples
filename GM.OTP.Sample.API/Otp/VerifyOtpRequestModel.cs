namespace GM.OTP.Sample.API.Otp;

public class VerifyOtpRequestModel
{
    public string? Subject { get; set; }
    public string? Purpose { get; set; }
    public string? Code { get; set; }
}
