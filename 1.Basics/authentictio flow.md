

# Authentication

Authentication is the process of verifying the identity of a user or system before allowing access to resources or performing actions.

Authentication verifies whether the principal's credentials are valid and trustworthy.

The server ensures that credentials are trustworthy by verifying:
- Credentials against stored data (e.g., passwords, hashed values).
- Token signatures, issuer, and expiration.
- The validity of digital certificates.
- Multi-factor credentials through trusted channels.
- The authority of third-party identity providers in federated authentication.

These mechanisms help the server validate the claims, ensuring they are not tampered with and are issued by a legitimate, trusted source.


## Core concepts
### 1. Credentials vs. Claims

- **Credentials**: These are the pieces of information the user provides to prove their identity. Common examples include:
    - **Username/Password** (for basic authentication).
    - **Tokens** (like a JWT).
    - **Certificates**, **biometrics**, **SSO assertions**, etc.

  Credentials are **used during the authentication process** to verify a user, often at the beginning of a session.

- **Claims**: After a user is authenticated, a **claim** is a piece of information about the user that is carried within a **security token** (e.g., a JWT). Claims can describe various aspects of the user, such as their:
    - **Unique Identifier (e.g., User ID)**.
    - **Roles** (admin, user, etc.).
    - **Permissions**.
    - **Email Address**, **Phone Number**, etc.

  Claims are generally used after authentication is complete, **during the authorization process** to determine what the user is allowed to do.



**Key difference**:
Credentials are for proving identity during authentication
Claims represent what is known about the user after authentication is done.

the username as a credential is fixed and securely managed, while the username as a claim in a token can be more flexible, depending on how the system handles claims.

---

### 2. Authentication Process (Overview)

The core authentication process can be broken down into distinct steps, depending on the method used. Here’s a general breakdown:

#### Step 1: User Provides Credentials

- The user submits their credentials, such as a username and password.
- In federated authentication systems, this could involve submitting credentials to an external Identity Provider (IdP) like Google or Azure AD (for OAuth/OpenID Connect).

#### Step 2: System Validates Credentials

- The system or service (or an external IdP) verifies the credentials provided.
    - For username/password, the system checks the credentials against a stored hash in a database.
    - For token-based authentication, like OAuth or SAML, the external IdP issues a token (e.g., JWT) after successfully validating the credentials.

#### Step 3: Issue a Token or Session

- If the credentials are valid, the system generates an authentication token or session.
    - **Token-based systems (e.g., JWT)**: A token is generated and sent back to the user. The token may contain **claims** (like a user ID, roles, etc.).
    - **Session-based systems**: A session is created on the server, and a session ID (often in the form of a cookie) is returned to the user.

#### Step 4: Include the Token in Requests

- The user includes the authentication token (or session cookie) in subsequent HTTP requests to access resources or perform operations.
    - For token-based systems, the token is usually placed in the `Authorization` header (`Bearer <token>`).
    - For session-based systems, the session ID cookie is automatically included in requests.

#### Step 5: Validate the Token/Session on Each Request

- Each time the user sends a request, the system checks the token or session to ensure the user is still authenticated.
    - In token-based authentication, the system verifies the token (usually by checking its signature) and extracts any claims to determine the user’s identity and permissions.
    - In session-based systems, the server checks the session ID against its stored session information.

#### Step 6: Authorization

- After validating the user's identity, the system checks the **claims** or session data to authorize specific actions.
- Claims can include roles, permissions, or other relevant information that helps determine what the user can do within the system.

---

### 3. Types of Authentication

Let’s now clarify the differences between **session-based** and **token-based** authentication, as well as **federated** authentication.

#### a. Session-Based Authentication

- The server maintains a session for each authenticated user.
- After successful login, a session ID is stored in a cookie on the client’s side.
- On subsequent requests, the client sends the session cookie, and the server looks it up to identify the user.
- **Advantages**: Works well in traditional web apps.
- **Disadvantages**: Requires server-side session management, which can become inefficient at scale.

#### b. Token-Based Authentication (e.g., JWT)

- Upon successful authentication, the server issues a token (JWT) that contains user data and **claims**.
- The client stores the token (often in local storage or a cookie).
- On subsequent requests, the client sends the token in the `Authorization` header.
- The server does not need to store user sessions, as the token is self-contained and can be validated using a signature.
- **Advantages**: Scales well, especially in stateless, distributed environments (e.g., microservices).
- **Disadvantages**: Token expiration and security handling can be complex.

#### c. Federated Authentication (e.g., OAuth, SAML, OpenID Connect)

- The authentication process is delegated to an external provider (e.g., Google, Facebook, Azure AD).
- The external IdP validates the credentials and issues a token (e.g., an OAuth access token or SAML assertion).
- Your system consumes this token and uses the **claims** within it for authorization purposes.
- **Advantages**: Centralized authentication, easier integration with multiple services, reduced user management.
- **Disadvantages**: Reliance on third-party IdPs, added complexity for token exchanges.

---

### 4. Clarification of Terms

- **Credentials**: Information provided by the user to prove their identity (e.g., username/password, OAuth token).
- **Claims**: Data about the user (e.g., roles, permissions) included in a token after authentication is successful. These claims are used during authorization to determine what the user can access or perform.
- **Session**: A server-maintained record of the user’s state (common in session-based authentication).
- **Token**: A digital entity (e.g., JWT) that holds claims about the user. Used in token-based authentication.

---

### Conclusion

In summary:

- **Authentication** starts with credentials (e.g., password, token).
- Once authenticated, claims about the user (e.g., roles, permissions) are used in the **authorization** process to determine what they can do.
- Tokens (e.g., JWT) and sessions are different mechanisms for tracking an authenticated user.



## Authentication Lifecycle

- Registration (Sign Up):
    - User creates a new account by providing credentials and any required information.
    - The system stores the information, securely hashing passwords and verifying email addresses, if needed.
- Login (Sign In):
    - User provides credentials (e.g., username and password).
    - The system verifies the credentials. Upon successful verification, the system creates a session or issues a token (like a JWT) for future authenticated requests.
    - **Session IDs or tokens** are used to maintain the user’s session across requests in stateless systems like HTTP. They solve the problem of keeping the user logged in between requests without requiring re-authentication.
    - **Security aspect**: Tokens are often signed, encrypted, and short-lived to protect against session hijacking and tampering. While they manage sessions, they are not considered credentials themselves.
- Authenticated Session:
    - During the session, each request contains the session ID or token, which serves as proof of the user’s prior authentication.
    - Tokens may also include **authorization claims**, such as roles or permissions, which determine what actions the user can perform.
    - **Security aspect**: These tokens can be validated without exposing credentials, and their expiration or revocation helps mitigate security risks.
- Logout (Sign Out):
    - The user requests to end their session.
    - The system invalidates the session or token, effectively logging out the user. This ensures that the token or session ID can no longer be used to make authenticated requests.


### Registration/Sign up

Registration is a one-time action to create an account.

**Registration** is the process where a new user creates an account in the system. It's typically the first step for a user to access any service that requires authentication.

Credentials

**Steps in Registration:**
- **Step 1: User Provides Data**: The user enters the necessary data, which could include:
    - Username
    - Password
    - Email
    - Phone number
    - Other identifying information
- **Step 2: System Validates and Stores Data**: The system checks if the provided information is valid:
    - It might ensure that the username or email is unique.
    - Passwords are typically hashed and stored securely.
    - Verification steps like confirming email addresses (via a link sent to the user) may be included.
- **Step 3: Optional KYC (Know Your Customer)**: If the application requires additional identity verification, the user may go through a **KYC process**, providing documents or details to verify their identity.
- **Step 4: Account Creation**: After successful validation, the system stores the user’s details, creating an account for them.

**End Result**: The user has now registered an account and can proceed to the login/sign in process.



### Log-in/Sign-in

**Login** (or **Sign In**) is the process where an already registered user provides their credentials to authenticate and gain access to the system.

**Login** is repeated each time the user wishes to start a session.
**Session** – a period of time that is used to do a particular activity


The authentication process doesn’t end at login—it continues throughout the session with the server or application validating the user's credentials (either via session ID or token) on every request.

Tokens are kept smaller to ensure optimal performance by clients that request them.

Login/Sign In and **Logout/Sign Out** are synonymous terms.

Claims
**claims** refer to attributes or pieces of information (e.g., username, roles, permissions) that the principal asserts about themselves.

When a user logs in, the computer system keeps a record, or “log,” of that user’s **session** on the system. We can also use the single word “login” as a noun to describe such a session on a computer.

Signing
the action of writing one's signature on an official document.


**Steps in Login:**
- **Step 1: User Provides Credentials**:
    - The user submits their credentials (e.g., username and password).
    - In federated authentication systems, this could involve submitting credentials to an external Identity Provider (IdP) like Google or Azure AD (for OAuth/OpenID Connect).
- **Step 2: System Validates Credentials**:
    - The system compares the username and password (or other credentials) with the information stored during registration.
    - Passwords are typically hashed before comparing them with stored hashes.
- **Step 3: Successful Login**:
    - If the credentials match, the system considers the user **authenticated**.
    - The system either creates a session (session-based) or generates a token (token-based), as described in the previous explanation.
    - In federated authentication, the system receives a token from an external identity provider (e.g., OAuth or OpenID Connect).
- **Step 4: Access**: The user now has access to the application and can perform actions based on their role and permissions.

**End Result**: The user is successfully logged in and can now use the system.



### Log-out/Sign-out


**Logout** (or **Sign Out**) is the process where a user terminates their authenticated session, meaning the system no longer considers them authenticated.

When a user signs out in an authentication system using cookies, the typical behaviour is that the system will either **delete the authentication cookie** or **invalidate it** by sending back a new cookie with the same name but with no content or an expired timestamp.


**Steps in Logout:**
- **Step 1: User Requests Logout**:
    - The user explicitly initiates the logout process, typically by clicking a "Logout" button.
- **Step 2: Invalidate the Session or Token**:
    - For **session-based** systems, the server destroys the session on the backend, removing any link between the session ID and the user's account.
    - For **token-based** systems (e.g., JWT), the token on the client side is deleted or cleared from storage (e.g., local storage or cookies). The token can no longer be used to authenticate future requests.
    - In some cases, systems may also maintain a token blacklist to ensure that tokens are invalidated until their expiration time.
- **Step 3: Redirect or Confirmation**: After logging out, the user is often redirected to the login page or a confirmation message is displayed.

**End Result**: The user is no longer authenticated and must log in again to access the system.





https://www.descope.com/learn/post/authentication-types