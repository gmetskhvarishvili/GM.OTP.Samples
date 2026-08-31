using GM.OTP.Sample.Application.Otp.Commands.GenerateOtp;
using GM.OTP.Sample.Application.Otp.Commands.VerifyOtp;
using Xunit;

namespace GM.OTP.Sample.Tests;

public class ValidatorTests
{
    [Fact]
    public void GenerateOtp_accepts_a_valid_command()
    {
        var result = new GenerateOtpCommandValidator().Validate(new GenerateOtpCommand
        {
            Subject = "user-1", Destination = "user@x.com", Purpose = "login", Channel = 2
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void GenerateOtp_rejects_blank_subject_destination_and_purpose()
    {
        // Subject/Destination/Purpose are `required init` members, so a blank string exercises the
        // NotEmpty() rule here; an entirely omitted value is already rejected at compile time.
        var result = new GenerateOtpCommandValidator().Validate(new GenerateOtpCommand
        {
            Subject = string.Empty, Destination = string.Empty, Purpose = string.Empty
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GenerateOtpCommand.Subject));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GenerateOtpCommand.Destination));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GenerateOtpCommand.Purpose));
    }

    [Fact]
    public void VerifyOtp_requires_subject_purpose_and_code()
    {
        var blank = new VerifyOtpCommand { Subject = string.Empty, Purpose = string.Empty, Code = string.Empty };
        Assert.False(new VerifyOtpCommandValidator().Validate(blank).IsValid);

        Assert.True(new VerifyOtpCommandValidator().Validate(new VerifyOtpCommand
        {
            Subject = "user-1", Purpose = "login", Code = "123456"
        }).IsValid);
    }
}
