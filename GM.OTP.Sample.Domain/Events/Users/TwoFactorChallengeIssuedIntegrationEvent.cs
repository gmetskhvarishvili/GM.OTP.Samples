using GM.Messaging.Domain.Events;
using Wolverine.Attributes;

namespace GM.OTP.Sample.Domain.Events.Users;

// MessageIdentity must match the alias on the publisher (GM.Identity's TwoFactorChallengeIssuedIntegrationEvent)
// so Wolverine resolves this local type from the incoming cross-service message-type header. Raised when a 2FA
// login needs a code: generate it under Subject (what GM.Identity verifies the code against) and deliver it to
// Destination over Channel (0 = Email, 1 = SMS, 2 = WhatsApp — the user's choice at login). Destination and
// Channel default so messages from a publisher that predates them still go to Subject by email.
[MessageIdentity("user.twofactor.challenge")]
public sealed record TwoFactorChallengeIssuedIntegrationEvent(
    string Subject,
    string? Destination = null,
    int Channel = 0) : IntegrationEvent;
