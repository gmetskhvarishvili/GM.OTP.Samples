using System.Resources;

// The embedded .resx resources ship only an invariant/English fallback; declaring the neutral
// culture lets the runtime skip satellite-assembly probing for "en" requests. See CA1824.
[assembly: NeutralResourcesLanguage("en")]
