using GM.Messaging.Domain.Events;
using Wolverine.Attributes;

namespace GM.OTP.Sample.Domain.Events.Users;

// MessageIdentity must match the alias on the publisher (GM.Identity's TwoFactorChallengeIssuedIntegrationEvent)
// so Wolverine resolves this local type from the incoming cross-service message-type header. Raised when a
// password login hits a 2FA-enrolled user: generate + deliver the second-factor code for Subject.
[MessageIdentity("user.twofactor.challenge")]
public sealed record TwoFactorChallengeIssuedIntegrationEvent(
    string Subject) : IntegrationEvent;
