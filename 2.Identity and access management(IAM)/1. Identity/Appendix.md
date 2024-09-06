https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0
https://cheatsheetseries.owasp.org/cheatsheets/DotNet_Security_Cheat_Sheet.html

https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authentication

# Authentication

Authentication is responsible for providing the ClaimsPrincipal for authorization to make permission decisions against.
The authentication scheme can select !!!!which authentication handler(cookieHandler e.g.)!!! is responsible for generating the correct set of claims.

Authenticate examples include:

- A cookie authentication scheme constructing the user's identity from cookies.
- A JWT bearer scheme deserializing and validating a JWT bearer token to construct the user's identity.
  
Tokens are containers with metadata that are explicitly passed through the headers or body of HTTP requests. 
The main advantage of tokens over cookies is that they are not tied to a specific app or domain. 
Instead, tokens are usually signed with asymmetric cryptography.

token contents (claims) are used to create a ClaimsPrincipal
authentication steps (like validating the token’s signature or checking its metadata)

Authentication is about verifying that the entity presenting the token is who they claim to be. 
This involves validating the token’s signature, ensuring it was issued by a trusted authority, and checking other aspects like expiration and audience. 
Simply using the claims without these checks means you trust the claims without verifying their source or integrity, which is risky and not secure.

# Principal

доверитель - тот, кто выдал кому-либо доверенность

A principal in computer security is an entity that can be authenticated by a computer system or network.

Principals can be individual people, computers, services, computational entities such as processes and threads, or any group of such things.[1] 
They need to be identified and authenticated before they can be assigned rights and privileges over resources in the network. 
A principal typically has an associated identifier (such as a security identifier) that allows it to be referenced for identification or assignment of properties and permissions.


## Claim 
претензия, иск, утверждение

a claims principal 


1. Principal
    A Principal represents the security context under which code is running. 
    It typically includes the user's identity and any associated roles or claims. 
    A principal object encapsulates both the user identity and any roles they might have, allowing the system to make decisions about what actions the user is permitted to perform.

In .NET, the principal is often represented by the IPrincipal interface. There are various implementations, such as ClaimsPrincipal for claims-based authentication.

2. Identity
   Identity represents the user’s identity information, which is often obtained during the authentication process. It is an object that encapsulates details like the user's name, the authentication type, and any claims associated with the user.

In .NET, this is represented by the IIdentity interface. The IIdentity object is associated with a Principal, providing information about the authenticated user.

3. Claim
   A Claim is a statement about a user that can be used for authorization and identity purposes. Claims are key-value pairs that provide information about the user, such as their name, role, email, or any other attribute.

In a claims-based authentication system, each Claim is a piece of identity data, and the user’s identity is composed of a collection of such claims. For example, a claim might state that the user has an email address of "user@example.com" or that the user is part of the "Admin" role.

In .NET, claims are represented by the Claim class, and a collection of claims can be attached to an IIdentity object through the ClaimsIdentity class.