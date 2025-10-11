# .NET Probe

WPF app for trying out various scenarios in .NET.

## Scenarios

- SSO with WAM/WebView
- Embedded HTTP server
- Azure Monitoring SDK

## Keycloak Setup

Start keycloak using Podman or Docker using `podman compose up` or `docker compose up`.

1. In Keycloak, create a new realm named `sso-probe`.
2. On realm settings -> User Profile remove first name and last name required attributes. (This is optional but simplifies user creation.)
3. Create test user in the `sso-probe` realm for example, username: `test`, password: `test`.

### Entra ID OpenID Connect Identity Provider

TODO: can it be just App Registration instead of Enterprise Application?

1. Create a new Entra ID Enterprise Application in the Azure portal.
2. Configure OpenID Connect-based SSO for the application.
3. Copy the `OpenID Connect metadata document` URL from the `Single sign-on` OpenID Connect configuration.
4. In Keycloak, navigate to the `sso-probe` realm.
5. Go to Identity Providers and select `OpenID Connect v1.0`.
6. Set the Alias to `entra-id-token-exchange`.
7. Paste the `OpenID Connect metadata document` URL into the `Import from URL` (you should see green dot-check icon).
8. Click on `Add` and then `Save` to create the identity provider.
9. Add section to appsettings.json:

```json
{
  "sso": {
    "wam": {
      "keycloak": {
        "url": "http://localhost:8080",
        "realm": "dotnet-probe",
        "clientId": "dotnet-probe",
        "clientSecret": "your-client-secret"
      }
    }
  }
}
```

### Entra ID SAML Identity Provider

1. Create a new Entra ID Enterprise Application in the Azure portal.
2. Configure SAML-based SSO for the application.
3. Set the `Identifier (Entity ID)` to `http://localhost:8080/realms/sso-probe`
4. Set the `Reply URL (Assertion Consumer Service URL)` to `http://localhost:8080/realms/sso-probe/broker/entraid-saml/endpoint`
5. Copy the `API Federation Metadata URL` from the `Single sign-on` SAML configuration.
6. In Keycloak, navigate to the `sso-probe` realm.
7. Go to Identity Providers and select `SAML v2.0`.
8. Set the Alias to `entraid-saml`.
9. Paste the `API Federation Metadata URL` into the `Import from URL` (you should see green dot-check icon).
10. Click on `Add` and then `Save` to create the identity provider.
11. Add section to appsettings.json:

```json
"Keycloak": {
  "TODO": "Add SAML config here"
}
```
