using GM.Messaging.Domain.Events;
using Wolverine.Attributes;

namespace GM.OTP.Sample.Domain.Events.Otp;

/// <summary>
/// Emitted after a one-time code is generated, for GM.Notifications to deliver.
/// Message is rendered from the OTP template with the plaintext code; Channel carries the
/// delivery channel (passed through from the OTP request).
/// </summary>
// UserId is inherited from IntegrationEvent; set it via object initializer, not positionally.
// MessageIdentity sets the cross-service wire name so the GM.Notifications consumer (its own copy
// of this contract, different namespace) resolves the same Wolverine message-type.
[MessageIdentity("otp.generated")]
public sealed record OtpGeneratedIntegrationEvent(
    string Destination,
    int Channel,
    string Purpose,
    string Message) : IntegrationEvent;
