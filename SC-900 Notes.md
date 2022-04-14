# Shared responsibility model
https://docs.microsoft.com/en-us/learn/wwl-sci/describe-security-concepts-methodologies/media/3-shared-responsibility-model.png

Some things are always the responsibility of the customer: information and data, devices (mobile and PCs), and accounts and identities
Some responsibilities vary by service type: identity and directory infrastructure, applications, network controls, and operating systems
The following are always responsibilities of the cloud provider: physical hosts, physical networks, and physical datacenters


# Defense in depth
Defense in depth uses a layered approach to security, rather than relying on a single perimeter. A defense in depth strategy uses a series of mechanisms to slow the advance of an attack.
### Layers:
- Physical security: Limiting access to datacenters
- Identity & Access: MFA or condition-based access to control access to infra and change control
- Perimeter: Securing your corporate network from attacks like DDoS to prevent denial of service to users
- Network: Network segmentation and network access controls to limit communication between resources
- Compute: Securing access to VMs by closing ports
- Application: Ensure apps are secure and free of vulnerabilities
- Data: Controls to manage access to business and customer data and encryption to protect data


# Confidentiality, Integrity, Availability (CIA)
- Confidentiality - The need to keep confidential sensitive data (customer information, passwords, financial data, etc). 
- Integrity - Keeping data or messages correct. Having confidence that data hasn't been tampered with or altered.
- Availability - Making data available to those who need it when they need it. Data must be kept secure, but at the same time it must be available to employees to help customers.


# Zero Trust Model:
Zero Trust assumes everything is on an open and untrusted network, even resources behind the firewalls of the corporate network. This model operates on the principle of "trust no one, verify everything." Zero Trust has three principles:

Verify explicitly - Always authenticate and authorize based on the available data points (user identity, location, device, service or workload, data classification, and anomalies).

Least privileged access - Limit user access with just-in-time and just-enough access (JIT/JEA).

Assume breach - Segment access by network, user, devices, and application. Use encryption to protect data, and use analytics to get visibility, detect threats, and improve your security.

The Zero Trust model also has six pillars which work together to provide end-to-end security:
- Identities - Users, services, or devices. Identities attempting to access a resource must be verified with strong authentication and  follow the least privilege access principles.
- Devices - Devices create a large attack surface as data flows from devices to on-prem workloads and the cloud. Monitoring devices for health and compliance is an important aspect of security.
- Applications - The way data is consumed. This includes discovering apps used by Shadow IT (ie, they aren't managed centrally). This pillar also includes managing permissions and access.
- Data - Should be classified, labeled, and encrypted based on its attributes. Security efforts are ultimately about protecting data.
- Infrastructure - Represents a threat vector on-prem or in the cloud. You should apply updates and use JIT access to block or flag risky behavior.
- Networks - Should be segmented and incorporate real-time threat protection, end-to-end encryption, monitoring, and analytics.


# Compliance concepts:
- Data residency - Regulations which govern the physical locations where data can be stored and how and when it can be transferred, processed, or acecssed internationally.
- Data sovereignty - The concept that data, particularly personal data, is subject to the laws and regulations of the country/region in which it's physically collected, held, or processed.
- Data privacy - Providing notice and being transparent about the collection, processing, use, and sharing of personal data are fundamental principles of privacy laws and regulations. 


# Azure Active Directory (AAD)
## Editions
- AAD Free - Allows you to administer users and create groups, synch with on-prem AD, create basic reports, configure self-service password change for cloud users, and enable SSO across Azure, Microsoft 365, and popular SaaS apps.
- Office 365 Apps - Includes everything in the free version plus self-service password reset for cloud users and device write-back (two-way synch between on-prem directories and AAD).
- AAD Premium P1 - Includes everything in the free and Office 365 apps editions. Supports advanced administration (dynamic groups and self-service group management), Microsoft Identity Manager (an on-prem identity and access management suite), and cloud write-back capabilities (allows self-service password reset for on-prem users).
- AAD Premium P2 - Includes everything in the P1 tier and AAD Identity Protection (provides risk-based conditional access to apps and critical company data). Also includes Privileged Identity Management to help discover, restrict, and monitor admins and their access to resources. Also provides just-in-time access.


## AAD Identity Types
- User - A representation of something that's managed by AAD.
  - Employees and guests are represented as users in AAD.
  - Several users with the same access needs can be organized into groups (which can give access permissions to all members of the group instead of having to assign individual permissions).
  - AAD business-to-business (B2B) collaboration (a feature within External Identities) includes the capability to add guest users. This allows orgs to securely share apps and services with guest users from another org.
- Service Principal - An identity for an application.
  - Apps must first be registered with AAD to delegate its identity and acecss functions to AAD. Once registered, a service prinipal is created in each AAD tenant where the app is used.
  - Enables core features such as authentication and authorization to resources secured by the AAD tenant.
- Managed Identity - A type of service principal that are automatically managed in AAD and don't require devs to manage credentials.
  - Provide an identity for apps to use when connecting to Azure resources that support AAD authentication.
  - Two types: System-assigned and User-assigned
    - System-assigned - Some Azure services allow you to enable a managed identity directly on a service instance. When used, an identity is created in AAD that's tied to the lifecycle of the service instance. When the resource is deleted Azure automatically deletes the identity for you.
    - User-assigned - A managed identity which is created as a standalone Azure resource. One user-assigned managed identity can be assigned to one or more instances of an Azure service. These identities are managed separately from the resources that use it.
- Device - A piece of hardware (mobile devices, laptops, servers, printers, etc).
  - AAD Registered Devices - Supports a bring your own device scenario. Users can access your org's resources using a personal device. AAD registered devices register to AAD without requiring an organization account to sign in to the device. Supported OSes: Windows 10 and above, iOS, Android, and macOS.
  - AAD Joined Devices - Device joined to AAD through an org account which is used to sign in to the device. AAD joined devices are generally owned by the org. Supported OSes: Windows 10 or greater (except Home edition) and Windows Server 2019 Virtual Machines running in Azure.
  - Hybrid AAD Joined Devices - Orgs with existing on prem AD can use this functionality. These devices are joined to your on-prem AD and AAD and require an organizational account to sign in to the device.
  - Registering and joining devices to AAD gives users SSO to cloud-based resources. It also allows SSO to on-perm resources.


## External Identities
- B2B Collaboration - Allows you to share your org's apps and services with guest users from other orgs while maintaining control over your own data.
- B2C Access Management - Customer identity access management (CIAM) solution. AAD B2C allows users to sign in with their preferred social, enterprise, or local account identities to SSO in to your applications. External users are managed in the AAD B2C directory (which is separate from the org's employee and partner directory).


## Hybrid Identity
Hybrid identity is a solution which spans on-prem and cloud-based capabilities. These allow for common user identity for authentication/authorization to all resources regardless of location. Microsoft offers several ways to authenticate:
- AAD Password Hash Sync - Simplest way to enable authentication for on-prem directory objects in AAD. Users can sign in to AAD services with their on-prem creds. AAD handles the user's sign-in process. This service extracts password hashes from on-prem AD using Azure AD Connect and syncs them with the AAD authentication service. This approach provides highly available cloud authentication.
- AAD Pass-through authentication - Allows users to sign in to both on-prem and cloud based apps using the same passwords (like password hash sync). The difference is that authentication occurs on your on-prem AD instead of in the cloud. This service also uses Azure AD Connect (alongside one or more authentication agents).
- Federated authentication - Recommended as an authentication for orgs that have advanced features not currently supported in AAD (like smart cards or certs, an on-prem MFA server, or sign-on using a third party authentication solution). AAD hands off the authentication process to a separate trusted authentication system (like Active Directory Federation Services).


## Conditional Access
Conditional Access is a feature of AAD that provides an extra layer of security before allowing authenticated users to access data or other assets. It is implemented through policies created and managed in AAD. A Conditional Access policy analyses signals including user, location, device, application, and risk to automate authorization decisions.
- User or group membership - Policies can be targeted to all users, specific groups of users, directory roles, or external guest users
- Location - Can use IP address ranges or allow/block an entire country/region's IP range.
- Device - Specific platforms or specific states can be used.
- Applications - Accessing specific applications can trigger different Conditional Access policies.
- Real-time sign-in risk detection - Integration with AAD Identity Protection allows the identification of risky sign-in behavior (the probability that a given sign-in isn't authorized by the identity owner).
- Cloud apps or actions - Can include or exclude cloud apps or user actions for a policy.
- User risk - For customers with Identity Protection user risk can be evaluated as part of a Conditional Access policy. User risk represents the probability that a given identity or account is compromised.


### Access Controls
Access Controls refers to the decision made after a Conditional Access policy has been applied. Common decisions include:
- Block access
- Grant access
- Require one or more conditions:
  - Require MFA
  - Require a device to be marked as compliant
  - Require hybrid AAD joiend device
  - Require approved client app
  - Require app protection policy
  - Require password change
- Control user access based on session controls to enable limited experiences within specific cloud applications. Example: Conditional Access App Control uses signals from Microsoft Defender for Cloud Apps to block the download, cut, copy, and print capabilities for senstive documents or to require labeling of sensitive files.


## AAD Roles and RBAC
