namespace GM.OTP.Sample.Domain.Constants;

public static class OtpPurpose
{
    public const string ConfirmUser = "ConfirmUser";

    // Must match the purpose GM.Identity validates the second factor against (grant_type=two_factor).
    public const string TwoFactor = "TwoFactor";
}
