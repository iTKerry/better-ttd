module Config

open IdentityServer4
open IdentityServer4.Models

let identityResources : IdentityResource list =
    [ IdentityResources.OpenId()
      IdentityResources.Profile()
      IdentityResources.Address() ]
    
let apiResources =
    [ ApiResource("openttdapi", "OpenTTD API") ]
    
let private betterttdClient =
    let client = Client()
    client.ClientName <- "BetterTTD"
    client.ClientId <- "mvc"
    client.AllowedGrantTypes <- GrantTypes.Hybrid
    client.AccessTokenType <- AccessTokenType.Reference
    client.UpdateAccessTokenClaimsOnRefresh <- true
    client.AllowOfflineAccess <- true
    client.AllowedScopes <-
        [| IdentityServerConstants.StandardScopes.OpenId
           IdentityServerConstants.StandardScopes.Profile
           "openttdapi" |]
    client.ClientSecrets <-
        [| Secret("secret".Sha256()) |]
    client.RedirectUris <-
        [| "https://localhost:6001/signin-oidc" |]
    client.PostLogoutRedirectUris <-
        [| "https://localhost:6001/signout-callback-oidc" |]
    client
        

let clients : Client list =
    [ betterttdClient ]