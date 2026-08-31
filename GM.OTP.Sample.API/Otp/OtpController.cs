using Asp.Versioning;
using GM.API.Controllers;
using GM.OTP.Sample.Application.Otp.Commands.GenerateOtp;
using GM.OTP.Sample.Application.Otp.Commands.VerifyOtp;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GM.OTP.Sample.API.Otp;

/// <summary>
/// OTP Controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class OtpController : BaseController
{
    /// <summary>
    /// Generate OTP
    /// </summary>
    /// <param name="request">Generate OTP request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Challenge Id and expiry</returns>
    [HttpPost("generate", Name = nameof(GenerateOtp))]
    [ProducesResponseType(typeof(GenerateOtpResultModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateOtp(
        [FromBody] GenerateOtpRequestModel request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<GenerateOtpCommand>();
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result.Adapt<GenerateOtpResultModel>());
    }

    /// <summary>
    /// Verify OTP
    /// </summary>
    /// <param name="request">Verify OTP request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Verification result</returns>
    [HttpPost("verify", Name = nameof(VerifyOtp))]
    [ProducesResponseType(typeof(VerifyOtpResultModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequestModel request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<VerifyOtpCommand>();
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result.Adapt<VerifyOtpResultModel>());
    }
}
