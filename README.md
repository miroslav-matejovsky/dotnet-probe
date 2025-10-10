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

## Links

- [Credential Management API](https://www.developerfusion.com/code/4693/using-the-credential-management-api/)
