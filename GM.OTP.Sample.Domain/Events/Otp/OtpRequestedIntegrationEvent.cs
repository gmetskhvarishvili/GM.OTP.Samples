using GM.Messaging.Domain.Events;
using Wolverine.Attributes;

namespace GM.OTP.Sample.Domain.Events.Otp;

/// <summary>
/// GM.OTP's own inbound contract: a request to issue a one-time code. Framed in OTP terms
/// (Subject = what the code is bound to, Destination = where it is delivered, Channel = how),
/// independent of any producing service's domain vocabulary.
/// </summary>
// UserId is inherited from IntegrationEvent; set it via object initializer, not positionally
// (a positional parameter matching the inherited init-only property is silently dropped, and
// System.Text.Json would likewise drop it on deserialize).
// MessageIdentity sets the cross-service wire name so a publishing service with its own copy of
// this contract (different namespace) resolves the same Wolverine message-type here.
[MessageIdentity("otp.requested")]
public sealed record OtpRequestedIntegrationEvent(
    string Subject,
    string Destination,
    string Purpose,
    int Channel) : IntegrationEvent;
