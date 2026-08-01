namespace GM.OTP.Sample.API.Otp;

public class GenerateOtpRequestModel
{
    public string? Subject { get; set; }
    public string? Destination { get; set; }
    public string? Purpose { get; set; }
}
