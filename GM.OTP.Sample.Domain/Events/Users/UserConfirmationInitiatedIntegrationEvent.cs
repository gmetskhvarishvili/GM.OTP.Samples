using GM.Messaging.Domain.Events;
using Wolverine.Attributes;

namespace GM.OTP.Sample.Domain.Events.Users;

// MessageIdentity must match the alias on the publisher (GM.Identity) so Wolverine resolves this
// local type from the incoming cross-service message-type header.
[MessageIdentity("user.confirmation.initiated")]
public sealed record UserConfirmationInitiatedIntegrationEvent(
    string Subject,
    int ConfirmationType) : IntegrationEvent;
