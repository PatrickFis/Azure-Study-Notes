- [Shared responsibility model](#shared-responsibility-model)
- [Defense in depth](#defense-in-depth)
    - [Layers](#layers)
- [Confidentiality, Integrity, Availability (CIA)](#confidentiality-integrity-availability-cia)
- [Zero Trust Model](#zero-trust-model)
- [Compliance concepts](#compliance-concepts)
- [Azure Active Directory (AAD)](#azure-active-directory-aad)
  - [Editions](#editions)
  - [AAD Identity Types](#aad-identity-types)
  - [External Identities](#external-identities)
  - [Hybrid Identity](#hybrid-identity)
  - [Authentication Methods](#authentication-methods)
    - [Passwords](#passwords)
    - [Phone](#phone)
    - [OATH (Open Authentication)](#oath-open-authentication)
    - [Passwordless Authentication](#passwordless-authentication)
    - [MFA in AAD](#mfa-in-aad)
    - [Security Defaults](#security-defaults)
    - [Self-Service Password Reset (SSPR)](#self-service-password-reset-sspr)
    - [Password Protection and Management Capabilities of AAD](#password-protection-and-management-capabilities-of-aad)
  - [Conditional Access](#conditional-access)
    - [Access Controls](#access-controls)
  - [AAD Roles and RBAC](#aad-roles-and-rbac)
    - [Built-in Roles](#built-in-roles)
    - [Custom Roles](#custom-roles)
    - [AAD RBAC vs Azure RBAC](#aad-rbac-vs-azure-rbac)
  - [Identity Governance in AAD](#identity-governance-in-aad)
    - [Identity Lifecycle](#identity-lifecycle)
    - [Access Lifecycle](#access-lifecycle)
    - [Privileged Access Lifecycle](#privileged-access-lifecycle)
  - [Entitlement Management](#entitlement-management)
  - [AAD Access Reviews](#aad-access-reviews)
  - [AAD Terms of User](#aad-terms-of-user)
  - [Privileged Identity Management (PIM)](#privileged-identity-management-pim)
  - [Azure Identity Protection](#azure-identity-protection)
- [Security Capabilities in Azure](#security-capabilities-in-azure)
  - [DDoS Protection](#ddos-protection)
  - [Azure Firewall](#azure-firewall)
  - [Web Application Firewall (WAF)](#web-application-firewall-waf)
  - [Azure Virtual Networks (VNets)](#azure-virtual-networks-vnets)
    - [Network Security Groups (NSGs)](#network-security-groups-nsgs)
  - [Azure Bastion](#azure-bastion)
  - [Just In Time (JIT) Access](#just-in-time-jit-access)
  - [Encryption](#encryption)
    - [Azure Key Vault](#azure-key-vault)
- [Security Management Capabilities in Azure](#security-management-capabilities-in-azure)
  - [Cloud Security Posture Management (CSPM)](#cloud-security-posture-management-cspm)
  - [Microsoft Defender for Cloud (formerly Azure Defender)](#microsoft-defender-for-cloud-formerly-azure-defender)
  - [Microsoft Sentinel](#microsoft-sentinel)
  - [Azure Security Benchmark and Security Baselines for Azure](#azure-security-benchmark-and-security-baselines-for-azure)
- [Microsoft 365 Defender](#microsoft-365-defender)
  - [Defender for Office 365](#defender-for-office-365)
    - [Defender for Office 365 plans:](#defender-for-office-365-plans)
  - [Microsoft Defender for Endpoint](#microsoft-defender-for-endpoint)
  - [Microsoft Defender for Cloud Apps](#microsoft-defender-for-cloud-apps)
  - [Microsoft Defender for Identity](#microsoft-defender-for-identity)
  - [Microsoft 365 Defender Portal](#microsoft-365-defender-portal)
- [Compliance](#compliance)
  - [Service Trust Portal](#service-trust-portal)
  - [Microsoft's Privacy Principles](#microsofts-privacy-principles)
  - [Microsoft 365 Compliance Center](#microsoft-365-compliance-center)
  - [Compliance Manager](#compliance-manager)
    - [Compliance Score](#compliance-score)
  - [Information Protection](#information-protection)
  - [Data Classification](#data-classification)
  - [Sensitivity Labels](#sensitivity-labels)
  - [Data Loss Prevention (DLP)](#data-loss-prevention-dlp)
    - [Endpoint DLP](#endpoint-dlp)
    - [DLP in MS Teams](#dlp-in-ms-teams)
  - [Retention Policies and Retention Labels](#retention-policies-and-retention-labels)
    - [Retention policies](#retention-policies)
    - [Retention labels](#retention-labels)
  - [Records Management](#records-management)
  - [Insider Risk](#insider-risk)
  - [Communication Compliance](#communication-compliance)
  - [Information Barriers](#information-barriers)
  - [eDiscovery and Audit](#ediscovery-and-audit)
    - [Auditing](#auditing)
  - [Resource Governance](#resource-governance)
    - [Azure Policy](#azure-policy)
    - [Azure Blueprints](#azure-blueprints)
    - [Azure Purview](#azure-purview)
- [Resources](#resources)

# Shared responsibility model
https://docs.microsoft.com/en-us/learn/wwl-sci/describe-security-concepts-methodologies/media/3-shared-responsibility-model.png

Some things are always the responsibility of the customer: information and data, devices (mobile and PCs), and accounts and identities.
Some responsibilities vary by service type: identity and directory infrastructure, applications, network controls, and operating systems.
The following are always responsibilities of the cloud provider: physical hosts, physical networks, and physical datacenters


# Defense in depth
Defense in depth uses a layered approach to security, rather than relying on a single perimeter. A defense in depth strategy uses a series of mechanisms to slow the advance of an attack.
### Layers
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


# Zero Trust Model
Zero Trust assumes everything is on an open and untrusted network, even resources behind the firewalls of the corporate network. This model operates on the principle of "trust no one, verify everything." Zero Trust has three principles:
- Verify explicitly - Always authenticate and authorize based on the available data points (user identity, location, device, service or workload, data classification, and anomalies).
- Least privileged access - Limit user access with just-in-time and just-enough access (JIT/JEA).
- Assume breach - Segment access by network, user, devices, and application. Use encryption to protect data, and use analytics to get visibility, detect threats, and improve your security.

The Zero Trust model also has six pillars which work together to provide end-to-end security:
- Identities - Users, services, or devices. Identities attempting to access a resource must be verified with strong authentication and  follow the least privilege access principles.
- Devices - Devices create a large attack surface as data flows from devices to on-prem workloads and the cloud. Monitoring devices for health and compliance is an important aspect of security.
- Applications - The way data is consumed. This includes discovering apps used by Shadow IT (ie, they aren't managed centrally). This pillar also includes managing permissions and access.
- Data - Should be classified, labeled, and encrypted based on its attributes. Security efforts are ultimately about protecting data.
- Infrastructure - Represents a threat vector on-prem or in the cloud. You should apply updates and use JIT access to block or flag risky behavior.
- Networks - Should be segmented and incorporate real-time threat protection, end-to-end encryption, monitoring, and analytics.


# Compliance concepts
- Data residency - Regulations which govern the physical locations where data can be stored and how and when it can be transferred, processed, or accessed internationally.
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
  - Apps must first be registered with AAD to delegate its identity and access functions to AAD. Once registered, a service principal is created in each AAD tenant where the app is used.
  - Enables core features such as authentication and authorization to resources secured by the AAD tenant.
- Managed Identity - A type of service principal that are automatically managed in AAD and don't require devs to manage credentials.
  - Provide an identity for apps to use when connecting to Azure resources that support AAD authentication.
  - Two types: System-assigned and User-assigned
    - System-assigned - Some Azure services allow you to enable a managed identity directly on a service instance. When used, an identity is created in AAD that's tied to the lifecycle of the service instance. When the resource is deleted Azure automatically deletes the identity for you.
    - User-assigned - A managed identity which is created as a standalone Azure resource. One user-assigned managed identity can be assigned to one or more instances of an Azure service. These identities are managed separately from the resources that use it.
- Device - A piece of hardware (mobile devices, laptops, servers, printers, etc).
  - AAD Registered Devices - Supports a bring your own device scenario. Users can access your org's resources using a personal device. AAD registered devices register to AAD without requiring an organization account to sign in to the device. Supported OSes: Windows 10 and above, iOS, Android, and macOS.
  - AAD Joined Devices - Device joined to AAD through an org account which is used to sign in to the device. AAD joined devices are generally owned by the org. These exist only in the cloud. Supported OSes: Windows 10 or greater (except Home edition) and Windows Server 2019 Virtual Machines running in Azure.
  - Hybrid AAD Joined Devices - Orgs with existing on prem AD can use this functionality. These devices are joined to your on-prem AD and AAD and require an organizational account to sign in to the device. Supported OSes: Windows 7, 8.1, 10, Windows Server 2008 or newer.
  - Registering and joining devices to AAD gives users SSO to cloud-based resources. It also allows SSO to on-prem resources.


## External Identities
- B2B Collaboration - Allows you to share your org's apps and services with guest users from other orgs while maintaining control over your own data.
- B2C Access Management - Customer identity access management (CIAM) solution. AAD B2C allows users to sign in with their preferred social, enterprise, or local account identities to SSO in to your applications. External users are managed in the AAD B2C directory (which is separate from the org's employee and partner directory).


## Hybrid Identity
Hybrid identity is a solution which spans on-prem and cloud-based capabilities. These allow for common user identity for authentication/authorization to all resources regardless of location. Microsoft offers several ways to authenticate:
- AAD Password Hash Sync - Simplest way to enable authentication for on-prem directory objects in AAD. Users can sign in to AAD services with their on-prem creds. AAD handles the user's sign-in process. This service extracts password hashes from on-prem AD using Azure AD Connect and syncs them with the AAD authentication service. This approach provides highly available cloud authentication.
- AAD Pass-through authentication - Allows users to sign in to both on-prem and cloud based apps using the same passwords (like password hash sync). The difference is that authentication occurs on your on-prem AD instead of in the cloud. This service also uses Azure AD Connect (alongside one or more authentication agents).
- Federated authentication - Recommended as an authentication for orgs that have advanced features not currently supported in AAD (like smart cards or certs, an on-prem MFA server, or sign-on using a third party authentication solution). AAD hands off the authentication process to a separate trusted authentication system (like Active Directory Federation Services).


## Authentication Methods
### Passwords
- Password access is the most common form of authentication, but they need to be augmented with more secure authentication methods available in AAD because of their many problems.

### Phone
- SMS-based authentication - Text messages can be sent to users for authentication. With SMS the user doesn't need to know a username or password to access applications and services. They enter their registered phone number, receive a text with a verification code, and then enter the code. SMS may also be used as a secondary form of authentication during self-service password reset (SSPR) or AAD MFA.
- Voice call verification - Users can receive a voice call as a secondary form of authentication. The number the user registered with is called and prompts the user to press the # key on their keypad to finish authentication. Voice calls are not supported as a primary form of authentication.

### OATH (Open Authentication)
OATH is an open password standard that specifies how time-based, one-time password (TOTP) codes are generated. They can be used to authenticate a user. OATH is only supported as a secondary form of authentication for use in SSPR or AAD MFA.
- Software OATH tokens - Typically applications. AAD generates a secret key (or seed) that's used by the app to generate each OTP.
- OATH TOTP hardware tokens - Public preview. Small hardware devices that display a code that refreshes every 30 or 60 seconds. Typically come with a secret key (or seed) pre-programmed in the token. These details must be input into AAD and then activated for end-users.

### Passwordless Authentication
- Windows Hello for Business - Replaces passwords with strong two-factor authentication on devices. This two-factor authentication is a combination of a key or cert tied to a device and something that the person knows (a PIN) or something that the person is (biometrics). Both options trigger the use of the private key to cryptographically sign data that is sent to the identity provider. The IDP verifies the user's identity and authenticates the user. Windows Hello for Business serves as a primary form of authentication and can also be used as a secondary form of authentication during MFA.
- FIDO2 - Fast Identity Online (FIDO) is an open standard for passwordless authentication. FIDO2 is the latest standard that incorporates the web authentication (WebAuthn) standard and is supported by AAD. FIDO2 security keys are an unphishable standards-based passwordless authentication method that can come in any form factor. They are typically USB devices but can also be Bluetooth or NFC based devices. Can be used to sign in to AAD or hybrid AAD joined Windows 10 devices to get SSO to the cloud and on-prem resources. This serves as a primary form of authentication and can also be used as a secondary form of authentication during MFA.
- Microsoft Authenticator App - Can be used to sign in to an AAD account or as an additional verification option during SSPR or MFA. Users must download the app to their phone and register their account. Available on Android and iOs.

### MFA in AAD
MFA requires more than one form of verification to prove that an identity is legitimate. This protects a user even when their password is compromised. AAD MFA requires the following:
- Something you know (password or PIN) and requires one of the following
  - Something you have (trusted device like a phone or hardware key)
  - Something you are (biometrics like a fingerprint or face scan)

The authentication methods detailed above can be used with AAD MFA.

### Security Defaults
Security defaults are a set of basic identity security mechanisms recommended by Microsoft. When enabled, these recommendations will be automatically enforced in your organization. The goal is to provide a basic level of security at no extra cost. Some of the common features are the following:
- Enforcing AAD MFA registration for all users
- Forcing admins to use MFA
- Requiring all users to complete MFA when needed

These options are good for organizations that want to increase their security posture but don't know where to start. They're also good for organizations using the free tier of AAD licensing. Security defaults may not be appropriate for organizations with AAD premium licenses or more complex security requirements.

### Self-Service Password Reset (SSPR)
SSPR is a feature in AAD that allows users to change or reset their password without admin/help desk involvement. SSPR works in the following scenarios:
- Password change (password known but user wants to change it to something new)
- Password reset - User can't sign in and wants to reset their password
- Account unlock - User can't sign in because their account is locked out

To use SSPR users must:
- Be assigned an AAD license
- Have SSPR be enabled by an admin
- Be registered with the authentication methods they want to use. 2+ methods are recommended in case one is unavailable.

SSPR supports the following authentication methods:
- Mobile app notification
- Mobile app code
- Email
- Mobile phone
- Office phone
- Security questions - note: users pick these from a set of questions and these can only be used during SSPR, they can't be used for authentication during a sign-in event. Admin accounts can't use them as a verification method with SSPR.

### Password Protection and Management Capabilities of AAD
Password Protection is an AAD feature that reduces the risk of users setting weak passwords by detecting and blocking known weak passwords and their variants. 
- Global banned password list - A list automatically updated and enforced by Microsoft. Blocks weak or compromised passwords and their variations. Automatically applied and can't be disabled. Sourced from real-world, actual password spray attacks.
- Custom banned password lists - Admins can create custom banned password lists that support their business. Feature of AAD P1 or P2. Should be focused on organizational-specific terms such as:
  - Brand names
  - Product names
  - Locations, such as company headquarters
  - Company-specific internal terms
  - Abbreviations that have specific company meaning
- Helps defend against password spray attacks[type of brute force dictionary attack] (which typically submit only a few of the known weakest passwords against each of the accounts in an enterprise). This attack allows an attacker to quickly search for an easily compromised account and avoid potential detection thresholds. AAD Password Protection blocks known weak passwords that are likely to be used in password spray attacks.
- AAD Password Protection can be enabled within an on-premises AD environment. A component on-prem receives the global and custom banned password lists from AAD and domain controllers use them to process password change events.

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
  - Require hybrid AAD joined device
  - Require approved client app
  - Require app protection policy
  - Require password change
- Control user access based on session controls to enable limited experiences within specific cloud applications. Example: Conditional Access App Control uses signals from Microsoft Defender for Cloud Apps to block the download, cut, copy, and print capabilities for sensitive documents or to require labeling of sensitive files.


## AAD Roles and RBAC
### Built-in Roles
AAD ships with many roles. A few of the common roles include:
- Global administrator - Users with this role have access to all admin features in AAD. The person who signs up for the AAD tenant automatically becomes a global administrator.
- User administrator - Users with this role can create and manage all aspects of users and groups. Includes the ability to manage support tickets and monitor service health.
- Billing administrator - Users with this role make purchases, manage subscriptions and support tickets, and monitor service health.

### Custom Roles
- Custom roles are collections of permissions that you choose from a preset list. The permissions are the same as those used by built-in roles.
- Requires an AAD Premium P1 or P2 license.

### AAD RBAC vs Azure RBAC
AAD roles are a form of RBAC referred to as Azure AD RBAC. AAD RBAC controls access to AAD resources. Similarly, Azure can control access to Azure resources using Azure RBAC. AAD RBAC controls access to AAD resources such as users, groups, and applications. Azure RBAC controls access to Azure resources such as VMs or storage using Azure Resource Management.


## Identity Governance in AAD
### Identity Lifecycle
- Orgs typically follow a "join, move, and leave" process:
  - Create a new digitial identity if one doesn't exist
  - Give different access authorizations when they move
  - Remove their access when they leave
- AAD Premium can integrate with cloud based HR systems and sync updates from the HR system to ensure consistency.
- AAD Premium offers Microsoft Identity Manager which can import records from on-prem HR systems.

### Access Lifecycle
Access lifecycle is the process of managing access throughout the user's organizational life. Orgs can automate the access lifecycle process using things like dynamic groups. Dynamic groups enable admins to create attribute-based rules to determine membership of groups. When attributes of a user or devices changes, the system evaluates all dynamic group rules in a directory to see if the change would trigger any users to be added or removed from a group.

### Privileged Access Lifecycle
AAD Privileged Identity Management (PIM) provides extra controls tailored to securing access rights. PIM helps minimize the number of people who have access to resources across AAD, Azure, and other Microsoft online services. PIM is a feature of AAD Premium P2.


## Entitlement Management
Entitlement management is an identity governance feature that enables orgs to manage the identity and access lifecycle at scale. It automates access request workflows, access assignments, reviews, and expiration. It is used when orgs face the following challenges:
- Users don't know what access they should have, or if they do know they can't find the right person to approve it.
- Users may hold on to access for longer than is required for business purposes.
- Managing access for external users.

Entitlement management addresses these challenges with the following capabilities:
- Delegate the creation of access packages to non-admins. These access packages contain resources that users can request. The delegated access package managers then define policies that include rules such as which users can request access, who must approve their access, and when access expires.
- Managing external users. When a user who isn't yet in your directory requests access, and is approved, they're automatically invited into your directory and assigned access. When their access expires, if they have no other access package assignments, their B2B account in your directory can be automatically removed.

Entitlement management is a feature of AAD Premium P2 and uses access packages to manage access to resources.

## AAD Access Reviews
- Allows orgs to manage group memberships, access to enterprise apps, and role assignment
- Access reviews can be created through AAD access reviews or AAD PIM.
- Can be used to review and manage access for both users and guests.
- Can be set up to require users to review their own access.

Access reviews are helpful when:
- You have too many users in privileged roles (like global administrator)
- When automation isn't possible (like when HR data isn't in AAD)
- You want to control business critical data access
- Your governance policies require periodic reviews of access permissions

## AAD Terms of User
AAD terms of use allows orgs to present terms to a user that they must accept before accessing data or an application. Conditional Access policies are used to require terms to be displayed and ensure that users agree to them before accessing an app.


## Privileged Identity Management (PIM)
PIM is a service in AAD that enables you to manage, control, and monitor access to important resources in your organization. PIM is:
- JIT - Provides privileged access only when needed and not before
- Time-bound - Roles can be assigned for specific start and end dates
- Approval-based - Requires specific users or groups to provide approval to activate packages
- Visible - Sends notifications when privileged roles are activated
- Auditable - Allows a full access history to be downloaded
- A feature of AAD Premium P2
- Used to reduce the chance of a malicious actor getting access by minimizing the amount of people with access to sensitive resources.
- Reduces risk of authorized users accidentally affecting sensitive resources by time-limiting their roles.
- Provides oversight into what users are doing with their administrator privileges.


## Azure Identity Protection
Identity protection is a tool that allows orgs to accomplish the following: automate detection and remediation of identity-based risks, investigate risks using data in the portal, and export risk detection data to third-party utilities for further analysis. Identity Protection is a feature of AAD Premium P2.

Identity Protection can calculate sign-in risk (see previous definition). The following sign-in risks are identifiable by Identity Protection:
- Anonymous IP address. This risk detection type indicates a sign-in from an anonymous IP address; for example, a Tor browser or anonymized VPNs.
- Atypical travel. This risk detection type identifies two sign-ins originating from geographically distant locations, where at least one of the locations may also be atypical for the user, given past behavior.
- Malware linked IP address. This risk detection type indicates sign-ins from IP addresses infected with malware that is known to actively communicate with a bot server.
- Unfamiliar sign-in properties. This risk detection type considers past sign-in history to look for anomalous sign-ins. The system stores information about previous locations used by a user, and considers these "familiar" locations. The risk detection is triggered when the sign-in occurs from a location that's not already in the list of familiar locations.
- Password spray. This risk detection is triggered when a password spray attack has been performed.
- AAD threat intelligence. This risk detection type indicates sign-in activity that is unusual for the given user or is consistent with known attack patterns based on Microsoft's internal and external threat intelligence sources.

The following user risks can be identified:
- Leaked credentials. This risk detection type indicates that the user's valid credentials have been leaked. When cybercriminals compromise valid passwords of legitimate users, they often share those credentials. This sharing is typically done by posting publicly on the dark web, paste sites, or by trading and selling the credentials on the black market. When the Microsoft leaked credentials service acquires user credentials from the dark web, paste sites, or other sources, they're checked against Azure AD users' current valid credentials to find valid matches.
- Azure AD threat intelligence. This risk detection type indicates user activity that is unusual for the given user or is consistent with known attack patterns based on Microsoft's internal and external threat intelligence sources.

Identity Protection only generates risk detections when correct creds are used in the authentication request. Incorrect creds will not be flagged by Identity Protection since there isn't a risk of credential compromise unless a bad actor uses the correct creds. Risk detections can trigger actions like requiring MFA, resetting passwords, or blocking access until an admin takes action.

Identity Protection provides three reports: risky users, risky sign-ins, and risk detections.

# Security Capabilities in Azure
## DDoS Protection
Types of attacks:
- Volumetric - Volume-based attacks that flood the network with seemingly legit traffic to overwhelm available bandwidth. Legit traffic can't get through. Measured in bits per second.
- Protocol - Render target inaccessible by exhausting server resources with false protocol requests that exploit weaknesses in layer 3 (network) and layer 4 (transport) protocols. Measured in packets per second.
- Resource (application) layer attacks - Target web application packets to disrupt the transmission of data between hosts.

Azure DDoS Protection
- Designed to help protect apps and servers by analyzing network traffic and discarding anything that looks like a DDoS attack.
- Two tiers
  - Basic - Enabled automatically for every property in Azure at no extra cost. Has always-on traffic monitoring and real-time mitigation of common network-level attacks.
  - Standard - Provides extra features tuned for VNet resources. Requires no application changes. Protection policies are applied to public IP addresses (which are associated with resources deployed in VNets such as Azure Load Balancer and Application Gateway). This tier has a fixed monthly charge that includes protection for 100 resources. Additional resources are charged on a monthly per-resource basis.

## Azure Firewall
Managed, cloud-based network security service that protects Azure VNets from attackers.
- Can be deployed on any VNet, but recommended use is on a centralized VNet that routes to other VNets so that control can be managed centrally.
- Has built-in high availability and availability zones.
- Allows network and application level filtering (using IP address, port, protocol).
- Outbound SNAT and inbound DNAT used to communicate and for IP address translation.
- Multiple public IP addresses can be associated with Azure Firewall.
- Threat intelligence - This filtering can be used to alert and deny traffic from/to known malicious IP addresses and domains.
- Integration with Azure Monitor for handling logging.

## Web Application Firewall (WAF)
WAF provides centralized protection for web apps from common exploits and vulns.

## Azure Virtual Networks (VNets)
- VNets are the fundamental building block for your org's private network in Azure.
- VNets allow network segmentation. Multiple VNets can be created per region in a subscription, and smaller networks (subnets) can be created within each VNet.
- VNets provide network level containment of resources. No traffic is allowed across VNets or inbound to the VNet by default. Communication has to be set up explicitly.

### Network Security Groups (NSGs)
- NSGs are used to filter network traffic to and from Azure resources in a VNet.
- Only one NSG can be associated with a particular VNet subnet and network interface in a VM.
  - The same NSG can be associated with multiple subnets and network interfaces though.
- NSGs are made up of inbound and outbound rules that are evaluated in priority order (with lower numbers being higher priority). The following properties are specified by each rule:
  - Name - NSG rules must be named uniquely
  - Priority - Processing order for the rules. Once traffic finds a matching rule processing stops. The priority is a number between 100 and 4096 (with lower numbers meaning higher priority).
  - Source or destination: Specify either with an IP address (or range), service tags (group of IP address prefixes from a given Azure service), or application security group.
  - Protocol: The network protocol checked by the rule (TCP, UDP, ICMP, Any).
  - Direction: Inbound or outbound.
  - Port range: Individual or range of ports.
  - Action: What happens when the rule is triggered.
- NSGs come with default rules that cannot be removed, but they can be overridden.
- NSGs and Azure Firewall complement each other for better defense-in-depth network security. NSGs provide distributed network layer ***within*** VNets in each subscription. Azure Firewall provides network and application-level protection ***across*** different subscriptions and VNets.


## Azure Bastion
- Bastion is a PaaS service which lets you connect to virtual machines using a browser and the Azure portal instead of exposing ports for RDP or SSH. Bastion provides a secure way to use RDP or SSH over TLS. VMs using Bastion do not need a public IP, agent, or special client software.
- Bastion provides access to VMs in a VNet or a peered VNet.
- Bastion has the following features:
  - RDP and SSH from the Azure portal
  - The remote session uses TLS (to secure the connection) and an HTML5 web client to stream the session to you.
  - No public IP required for the VM
  - Doesn't require modifying NSGs
  - Provides protection against port scanning because ports aren't made available to internet traffic
  - Bastion is hardened and protects against zero-day exploits


## Just In Time (JIT) Access
- Used to restrict port access to VMs for selected ports.
- Uses Microsoft Defender for Cloud to ensure that "deny all inbound traffic" rules exist for the ports you've selected. If other rules already exist for those ports then they will take priority over these new rules.
- When users need access to a VM, Microsoft Defender for Cloud checks if a user has Azure RBAC permissions for the VM. If approved, Microsoft Defender for Cloud modifies NSGs and Azure Firewall to allow inbound traffic to the selected ports from the relevant IP address (or range) for a specified amount of time. Afterwards, Microsoft Defender for Cloud reverts the changes without interrupting established connections.
- JIT requires Microsoft Defender for servers to be enabled on the subscription.


## Encryption
Azure provides various options for encrypting data:
- Azure Storage Service Encryption - Encrypts data at rest before it is persisted to Azure-managed disks, Blob Storage, Azure Files, or Azure Queue Storage. Also handles decrypting the data before retrieval.
- Azure Disk Encryption - Encrypts Windows/Linux IaaS VM disks using BitLocker (Windows) or dm-crypt (Linux).
- Transparent data encryption (TDE) - Real-time encryption/decryption of Azure SQL Database and Azure Data Warehouse. Includes the DB, backups, and transaction log files.

### Azure Key Vault
Key Vault is a centralized cloud service for storing application secrets with the following features:
- Secrets management - Control access to tokens, passwords, certs, API keys, etc.
- Key management - Allows control of encryption keys.
- Certificate management - SSL/TLS certs can be provisioned, managed, and deployed through Key Vault.
- Support for secrets backed by HSMs.


# Security Management Capabilities in Azure
## Cloud Security Posture Management (CSPM)
CSPM is a new class of tools designed to improve cloud security management. Assess systems and alerts IT when vulns are found. CSPM uses a combination of the following tools and services:
- Zero Trust based access control - Considers the active threat level during access control decisions
- Real time risk scoring - Provides visibility into top risks
- Threat and vulnerability management (TVM) - View of org's attack surface and risk and integrates it into operations and engineering decision making
- Discover risks to guard against data exposure
- Technical policy to apply guardrails to enforce standards
- Threat modeling

## Microsoft Defender for Cloud (formerly Azure Defender)
Microsoft Defender for Cloud is a CSPM tool. It allows you to continuously assess your security posture, secure your resources, and defend against threats. It provides visibility into your current security and provides guidance for hardening.
- Visibility is enabled by the Secure Score feature in Defender for Cloud. It allows you to tell how secure your applications are at a glance.
- Hardening recommendations are shown in Defender for Cloud. The recommendations will improve your security score when all recommendations for a single resource within a control have been resolved.
- Defender for Cloud is offered in two modes:
  - Microsoft Defender for Cloud (free) - Enables secure score and related features: security policy, continuous security assessment, and actionable security recommendations.
  - Microsoft Defender for Cloud with enhanced security features - Extends free mode to workloads in Azure, hybrid, and other cloud platforms. Provides cloud workload protection through various plans.
- Defender for Cloud offers cloud workload protection (CWP) that provide protection specific to resources and subscriptions. The following plans are available:
  - Defender for servers - Windows/Linux machines
  - Defender for App Service - Specific to apps using App Service
  - Defender for Storage
  - Defender for SQL
  - Defender for Kubernetes
  - Defender for container registries
  - Defender for Key Vault
  - Defender for Resource Manager
  - Defender for DNS
  - Defender for open-source relational protection - Used for open-source relational databases
- Defender for Cloud plans include the following enhanced security features:
  - Endpoint detection and response - included in Defender for servers
  - Vuln scanning for VMs
  - Security for accounts in AWS and GCP
  - Security for on-prem and hybrid workloads
  - Threat protection alerts for incoming attacks and post-breach activity
  - Compliance tracking for standards
  - Access and application controls
- Defender for Cloud uses Azure Security Benchmarks to assess an org's cloud environment. Azure Security Benchmarks also provide security baselines for services.


## Microsoft Sentinel
Microsoft Sentinel is a security information event management (SIEM) and security orchestration automated response (SOAR) solution.
- SIEM - A tool used to collection data from infrastructure, software, and resources. Does analysis and looks for anomalies to generate alerts and incidents.
- SOAR - Takes alerts for systems (like SIEM) which trigger automated workflows and processes to run security tasks to mitigate issues.

Sentinel enables the following functionality:
- Collecting data from all users, devices, apps, and infrastructure on-prem and in multiple clouds.
- Detects threats using analytics and threat intel
- Investigates threats using AI
- Responds to incidents with built-in orchestration and automation

Sentinel's key features:
- Built-in connectors to connect to your data
- Azure Monitor Workbooks - Used for data analysis and reporting in the Azure portal
- Analytics - Correlates alerts into incidents (groups of related alerts) that need to be investigated and resolved. Provides machine learning rules to map your network behavior and look for anomalies across resources.
- Incident management
- Automated workflows (playbooks) that are used for repeatable tasks
- Investigation to understand the scope of a threat and find the root cause
- Hunting - Provides search-and-query tools to proactively hunt for threats across your org
- Notebooks - Support for Jupyter Notebook to extend the scope of what you can do with your data from Sentinel
- Community - Analysts create new workbooks, playbooks, hunting queries, etc. that are given to the community for your use

Cost
- Sentinel is available using capacity reservations (fixed fee for predictable costs) or pay as you go (billed per GB of data ingested for analysis in Sentinel and stored in the Azure Monitor Log Analytics workspace).

## Azure Security Benchmark and Security Baselines for Azure
Azure Security Benchmark (ASB) provides best practices and recommendations to help improve the security of workloads, data, and services on Azure. ASB is an Excel spreadsheet with each ASB containing the following:
- ASB ID
- Control domain - High-level feature or activity that isn't specific to a technology or implementation (network security, data protection, etc)
- Mapping to industry frameworks such as CIS, NIST, or PCI DSS
- Recommendations
- Security principle - Each recommendation has a security principle that explains the "what" for the control at the technology-agnostic level
- Azure Guidance - Focused on how controls should be implemented in Azure

Security baselines apply ASBs to specific services. Security baselines help orgs strengthen their security through improved tooling, tracking, and security features. They provide orgs a consistent experience when securing their environment. Each security baseline includes the following information:
- Azure ID - ASB ID that corresponds to the recommendation
- Azure control - Content is grouped by control domain
- Benchmark recommendation - Maps to the recommendation for the ASB ID
- Customer guidance - The rationale for the recommendation and links on guidance on how to implement it
- Responsibility - Who is responsible for implementing the control (customer, Microsoft, or shared)
- Microsoft Defender for Cloud monitoring - Indicates if Microsoft Defender for Cloud monitors the control

# Microsoft 365 Defender
365 Defender is an enterprise defense suite which protects the following:
- Identities with Microsoft Defender for Identity and Azure AD Identity Protection
- Endpoints with Microsoft Defender for Endpoint
- Applications with Microsoft Defender for Cloud Apps
- Email and collaboration with Microsoft Defender for Office 365 - Protects against emails, links, and protects collaboration tools (Teams, SharePoint Online, OneDrive for Business, other Office clients)

## Defender for Office 365 
Covers the following areas:
- Threat protection policies
- Reports - real time reports for Defender for Office 365
- Threat investigation and response (also simulations)
- Automated investigation and response

### Defender for Office 365 plans:
- Microsoft Defender for Office 365 Plan 1
  - Safe attachments - Checks email attachments for malicious content
  - Safe links - Scans link before they're clicked so that malicious content is blocked
  - Safe Attachments for SharePoint, OneDrive, and Microsoft Teams - Protects from malicious files in collaboration tools
  - Anti-phishing protection
  - Real-time detections for analyzing recent events
- Microsoft Defender for Office 365 Plan 2
  - Everything from Plan 1
  - Threat trackers for prevailing cybersecurity issues
  - Threat explorer - Real-time report for analyzing recent threats
  - Automated investigation and response (AIR) - Security playbooks which are launched by alerts (or manually) that can automatically investigate and provide results and recommendations for your security team to approve or reject.
  - Attack simulation
  - Threat hunting with query based tools
  - Alert and incident investigation
- Availability
  - Included in certain subscriptions or purchased as an add-on.

## Microsoft Defender for Endpoint
- Platform designed to protect endpoints for enterprise networks.
- Prevents, detects, investigates, and responds to advanced threats.

Includes the following features:
- Threat and vulnerability management
- Attack surface reduction by providing network and web protection to regulate access to malicious IPs, domains, and URLs
- Next generation protection using machine learning
- Endpoint detection and response in near real time
- Automated investigation and remediation
- A managed threat hunting service
- APIs to integrate with other stuff
- Includes a Secure Score for Devices

## Microsoft Defender for Cloud Apps
- Defender for Cloud Apps is a Cloud Access Security Broker (CASB). It provides visibility into cloud app usage and helps identify shadow IT. It allows you to control and protect data in sanctioned apps.
- CASB is a gatekeeper between enterprise user's and the cloud resources they use. It is composed of the following pillars:
  - Visibility into cloud services and apps and shadow IT
  - Threat protection and monitoring
  - Data security - allows classification and control of sensitive info
  - Compliance for cloud services
  
Microsoft Defender for Cloud Apps is built on the following framework:
- Discover and control the use of Shadow IT
- Protect against cyberthreats and anomalies
- Protect your sensitive information anywhere in the cloud
- Assess your cloud app's compliance

Microsoft Defender for Cloud Apps has the following features:
- Cloud Discovery - Maps cloud apps your org uses (note: Cloud App Discovery is included in AAD Premium P1 at no extra cost)
- Sanctioning and unsanctioning apps
- App connectors to integrate cloud apps with Microsoft Defender for Cloud Apps
- Conditional Access to cloud apps
- Policies to detect risky behavior, violations, or suspicious activities

## Microsoft Defender for Identity
Uses on-prem AD data (called signals) to detect and investigate threats and malicious insider actions. It provides the following functions:
- Monitor and profile user behavior and activities.
- Protect user identities and reduce the attack surface.
- Identify and investigate suspicious activities and advanced attacks across the cyberattack kill-chain.
- Provide clear incident information on a simple timeline for fast triage

## Microsoft 365 Defender Portal
The portal home page has content grouped into common cards in the following categories:
- Identities - Monitor the identities in your organization and keep track of suspicious or risky behaviors.
- Data - Help track user activity that could lead to unauthorized data disclosure.
- Devices - Get up-to-date information on alerts, breach activity, and other threats on your devices.
- Apps - Gain insight into how cloud apps are being used in your organization.

The portal home page can be tweaked to meet your org's needs.

Functions in the left navigation pane include:
- Email and collaboration tools
- Incident and alerts
- Hunting
- Threat analytics
- Secure Score
- Learning Hub
- Reports
- Permissions & roles


# Compliance

## Service Trust Portal
The Service Trust Portal provides information, tools, and other resources about Microsoft security, privacy, and compliance practices. The following features are available from the main menu:
- Service Trust Portal - Link to home page
- Compliance Manager - Links to Compliance Manager in Microsoft 365 compliance center.
- Trust Documents -  Info about security implementation and design information for Microsoft cloud services. The goal here is to make it easier for orgs to meet regulatory compliance objectives. The following options are available:
  - Audit Reports for Microsoft's cloud services
  - Data Protection - info on audited controls, white papers, FAQs, pen tests, risk assessment tools, and compliance guides
  - Azure Stack - Contains security and compliance solutions specifically for Azure Stack customers
- Industries & Regions - Provides compliance info about Microsoft cloud services organized by industry and region. The following options are available:
  - Industry Solutions - Specifically for the Financial Services industry. Contains info like compliance offerings, FAQs, and success stories.
  - Regional Solutions - Provides documentation on compliance with the laws of various countries/regions.
- Trust Center - Links to Microsoft Trust Center (contains info about privacy, security, and compliance)
- Resources - Links to the Office 365 Security & Compliance Center
- My Library - Lets you save documents so that you can quickly access them again. Also allows for email notifications to be set up so that you can be notified when documents are updated.
- More - Provides an admin selection which is available only to Global Administrators and relates to options associated with Compliance Manager.

## Microsoft's Privacy Principles
- Control - Putting the customer in control of their data and privacy. Microsoft will not use your data without your permission.
- Transparency - Transparency around data collection and how it's used.
- Security - Protecting data that is entrusted to Microsoft using strong security and encryption.
- Strong legal protections - Respecting local privacy laws and fighting for legal protection of privacy as a fundamental human right.
- No content-based targeting: Your data (like email, chat, files, other personal content) is not sold or used for targeted advertising.
- Benefits to you: Data that Microsoft collects is used to benefit you and make your experience better.
  - Troubleshooting
  - Feature improvement
  - Personalized customer experience

## Microsoft 365 Compliance Center
- Used to bring together tools and data to manage an org's compliance needs.
- Available to the following:
  - Global administrator
  - Compliance administrator
  - Compliance data administrator
- Has a dashboard which shows how your org is doing with data compliance and what solutions are available to you. Also displays a summary of active alerts.
- The default home page contains the following cards:
  - Compliance Manager - This takes you to the Compliance Manager solution which shows a risk-based compliance score that measures progress toward completing recommended actions to reduce risk associated with data protection and regulatory standards.
  - Solution Catalog - Links to collections of integrated solutions to help manage end-to-end compliance scenarios. Includes the following:
    - Information protection & governance - Help classify, protect, and retain data. Includes data loss prevention (DLP), information governance, information protection, and records management.
    - Privacy
    - Insider risk management
    - Discovery & respond - Tools to find, investigate, and respond with relevant data. Includes audit, data subject requests, and eDiscovery.
  - Active Alerts - Includes a summary of the most active alerts and a link to find more detailed information.
- In addition to cards a left navigation pane is shown that gives you access to alerts, reports, policies, the solutions catalog, data connectors, and the ability to customize the navigation.

## Compliance Manager
Compliance Manager is a feature in the Microsoft 365 compliance center that helps admins to manage an org's compliance requirements. It provides the following:
- Prebuilt assessments for common regs and standards.
- Workflow capabilities to complete risk assessments.
- Step-by-step improvement actions to help meet regs and standards.
- Compliance score to understand overall compliance posture.

Compliance Manager is composed of the following key elements:
- Controls: A requirement of a reg, standard, or policy. Compliance Manager tracks the following types of controls:
  - Microsoft-managed controls - Controls which Microsoft is responsible for implementing
  - Your controls (AKA customer-managed controls) - implemented by you
  - Shared controls - Responsibility is shared between you and Microsoft for implementing
- Assessments: Grouping of controls from a specific reg, standard, or policy.
- Templates: Templates provide a way to help admins quickly create assessments.
- Improvement Actions: Each action provides recommended guidance that helps align with regs and standards.

### Compliance Score
- Measures progress in completing recommended improvement actions within controls.
- Helps orgs understand their current compliance postures.
- Calculated using scores assigned to actions. There are two types of actions:
  - Your improved actions - actions that your org is expected to manage.
  - Microsoft actions - actions that Microsoft manages for the org.
- Actions are categorized the following ways:
  - Mandatory - Actions which shouldn't be bypassed
  - Discretionary - Depend on users understanding and following a policy
  - Actions have the following subcategories:
    - Preventative - Designed to handle specific risks 
    - Detective - Monitor systems to identify irregularities
    - Corrective - Minimize the impact of security incidents

## Information Protection
- Microsoft Information Protection (MIP) discovers, classifies, and protects sensitive and business-critical content.
- Microsoft Information Governance (MIG) manges your content lifecycle using solutions to import, store, and classify business-critical data.

Both systems work together and have the following principles:
- Know your data - Organizations can understand their data landscape and identify important data across on-premises, cloud, and hybrid environments. Capabilities and tools such as trainable classifiers, activity explorer, and content explorer allow organizations to know their data.
- Protect your data - Organizations can apply flexible protection actions including encryption, access restrictions, and visual markings.
- Prevent data loss - Organizations can detect risky behavior and prevent accidental oversharing of sensitive information. Capabilities such as data loss prevention policies and endpoint data loss prevention enable organizations to avoid data loss.
- Govern your data - Organizations can automatically keep, delete, and store data and records in a compliant manner. Capabilities like retention policies, retention labels, and records management enable organizations to govern their data.

## Data Classification
Data can be classified using tools in the Microsoft 365 compliance center so that data is handled in line with compliance requirements. It provides the following tools:
- Sensitive Information Types - Pattern based classifier (for example, credit card numbers or bank account numbers)
- Trainable Classifiers - AI and ML used to classify your data.
  - Pre-trained classifiers - Microsoft offers pretrained classifiers that can classify things like resumes, source code, harassment, etc.
  - Custom trainable classifiers - Orgs can create and train their own classifiers for data unique to an org.
- Content Explorer - Allows admins to gain visibility into content and get a snapshot of individual items that have been classified. Access is highly restricted.
- Activity Explorer - Provides visibility into what content has been discovered and labeled and where it is. It shows what happens to content across the org and shows label changes and downgrades.

## Sensitivity Labels
Orgs can label their content to protect it. Labels are:
- Customizable
- Clear text (stored in metadata)
- Persistent

Sensitivity labels can be configured to do the following:
- Encrypt emails or documents
- Mark content when Office apps are used (add watermarks, headers, or footers)
- Apply labels automatically in Office apps or recommend a label
- Protect content in containers such as sites and groups
- Extend sensitivity labels to third-party apps and services through the Microsoft Information Protection SDK
- Classify content without using any protection settings

Label policies enable admins to:
- Choose who can see specific labels
- Apply a default label to things that specific users and groups create
- Require justifications for label changes
- Require users to apply a label (mandatory labeling)
- Link users to custom help pages

## Data Loss Prevention (DLP)
DLP is a way to protect sensitive info and prevent its disclosure. With DLP admins can do the following:
- Identify, monitor, and automatically protect sensitive info across Microsoft 365
- Help users learn how compliance works
- View DLP reports

DLP policies protect content through rule enforcement. The rules consist of:
- Conditions that the content must match
- Actions that the admin wants the rule to do automatically
- Locations where the policy will be applied

### Endpoint DLP
Endpoint DLP is extending DLP to Windows 10 devices. Endpoint DLP enables admins to audit activities that users perform on sensitive content.

### DLP in MS Teams
DLP can be used in Teams to prevent users from sharing sensitive information through messages or files.

## Retention Policies and Retention Labels
Retention policies and labels help orgs manage and govern info and keep it for a specified time before being permanently deleted.
- Helps orgs comply with regs and internal policies
- Reduces risk when there's litigation or a breach by deleting old data that you aren't required to keep
- Ensures users work only with content that's current and relevant to them

### Retention policies 
- Used to assign retention settings to content at a site or mailbox level
- Can be assigned to multiple locations or specific locations or users
- Items inherit retention settings from their container

### Retention labels
- Used to assign retention settings at an item level (folder, document, email)
- Email or document can only have one retention label assigned at a time
- Labels travel with content when it's moved to a different location

## Records Management
Helps orgs look after legal obligations and demonstrate compliance with regulation. Records management includes many features:
- Labeling content as a record
- Establishing retention and deletion policies for record labels
- Triggering event-based retention
- Reviewing and validating disposition
- Proof of records deletion
- Exporting information about disposed items

Items can be marked as records or as regulatory records. Regulatory records provide other controls and restrictions (they can't be removed and their retention period can't be made shorter after the label has been applied). Regulatory labels aren't available by default as no one, not even a global administrator, can remove them. They must be enabled by the administrator using PowerShell.

## Insider Risk
Insider risk management helps minimize internal risks. Internal risks include:
- Data leaks
- Confidentiality violations
- IP theft
- Fraud
- Insider trading
- Regulatory compliance violations

Insider risk management is centered around the following principles:
- Transparency: Balance user privacy versus organization risk with privacy-by-design architecture.
- Configurable: Configurable policies based on industry, geographical, and business groups.
- Integrated: Integrated workflow across Microsoft 365 compliance solutions.
- Actionable: Provides insights to enable user notifications, data investigations, and user investigations.

Insider risk management has the following workflow:
- Policies - Define risk indicators and how they're used for alerts, who is affected by the policy, what services are prioritized, and what the monitoring time period is.
- Alerts - Generated by risk indicators that match policy conditions. Displayed in the Alerts dashboard.
- Triage - Activities that need investigation generate alerts and are assigned a needs review status.
- Investigate - Cases are created for alerts that require deeper review and investigation around the policy match.
- Action - After investigation, reviewers can take actions to resolve the case or collaborate with other risk stakeholders in the org.

## Communication Compliance
Communication compliance helps minimize risks by detecting, capturing, and taking actions for inappropriate messages. It uses the following workflow:
- Configure
- Investigate
- Remediate
- Monitor

## Information Barriers
Information barriers can be used when an org wants to restrict communications between some groups to avoid a conflict of interest. In MS Teams it prevents the following:
- Searching for a user
- Adding a member to a team
- Starting a chat session with someone
- Starting a group chat
- Inviting someone to join a meeting
- Sharing a screen
- Placing a call
- Sharing a file with another user
- Access to file through sharing link

## eDiscovery and Audit
eDiscovery is the process of identifying and delivering electronic info that can be used as evidence in legal cases. Microsoft 365 provides three eDiscovery solutions:
- Content Search - Used to find content across Microsoft 365 data sources
- Core eDiscovery - Builds on content search by letting you make eDiscovery cases and assign eDiscovery managers to specific cases. Lets you associate searches and exports with a case and place eDiscovery holds on content locations relevant to the case.
- Advanced eDiscovery - Builds on core eDiscovery and provides an end-to-end workflow for handling internal and external investigations. Allows legal teams to manage custodians (people that you've identified as people of interest in the case). Provides analytics and machine learning based predictive coding models to further narrow the scope of your investigation to the most relevant content.

### Auditing
Microsoft 365 provides two auditing solutions:
- Basic Audit - Provides the ability to log and search for audited activities. Can export audit records to a CSV file to allow for further analysis. Records are retained for 90 days.
- Advanced Audit - Builds on Basic Audit and provides audit log retention policies and longer retention of audit records. Provides records of high-value events that can help organizations investigate possible security or compliance breaches and determine the scope of compromise.
- It can take up to 30 minutes or up to 24 hours after an event occurs for the corresponding audit log record to be returned in the results of an audit log search.

## Resource Governance
### Azure Policy
- See AZ-900.
- Designed to enforce standards and assess compliance across an org. Allows for bulk remediation for existing resources and automatic remediation for new resources.

### Azure Blueprints
- See AZ-900.
- Provides a way to define a repeatable set of Azure resources and lets development teams rapidly provision and run new environments.
- Relationship between the blueprint definition (what should be deployed) and the blueprint assignment (what was deployed).
- Supports improved tracking and auditing of deployments.

### Azure Purview
Azure Purview is designed to address the issues of rapid growth of data and to help enterprises get the most value from their existing information assets.
- Purview is a unified data governance service that helps manage and govern on-prem, multi-cloud, and SaaS data.
- Allows orgs to create maps of their data landscape with automated data discovery
- Supports sensitive data classification
- Automates data discovery by providing data scanning and classification as a service
- Provides a Data Map that serves as the foundation for data discovery and data governance.
- Provides a Data Catalog that lets business and technical users quickly find data by searching for it.
- Provides Data Insights that allows data officers and security officers to get information on data that is actively scanned and where sensitive data is and how it moves.


# Resources
https://docs.microsoft.com/en-us/learn/certifications/exams/sc-900

https://www.youtube.com/watch?v=LLKza5oULAA