namespace BlazorTwUI.Services;

using BlazorTwUI.Models;

public class FormCatalogService
{
    private static readonly List<FormSchema> _allForms = new();

    static FormCatalogService()
    {
        // Form 1: User Login & SSO Form
        _allForms.Add(new FormSchema
        {
            Id = "login",
            Title = "User Login & SSO Form",
            Category = "Authentication & Identity",
            Description = "Standard email/password authentication with remember-me and social SSO options.",
            Icon = "🔑",
            Columns = 2,
            SubmitButtonText = "Submit User",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "email",
                    Label = "Email Address",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "user@example.com",
                    Options = null
                },
                new()
                {
                    Name = "password",
                    Label = "Password",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = "••••••••",
                    Options = null
                },
                new()
                {
                    Name = "remember_me",
                    Label = "Remember Me for 30 Days",
                    Type = FormFieldType.Checkbox,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 2: New Account Registration
        _allForms.Add(new FormSchema
        {
            Id = "registration",
            Title = "New Account Registration",
            Category = "Authentication & Identity",
            Description = "Self-service user registration with corporate domain matching and password confirmation.",
            Icon = "📝",
            Columns = 2,
            SubmitButtonText = "Submit New",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "full_name",
                    Label = "Full Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Alex Morgan",
                    Options = null
                },
                new()
                {
                    Name = "email",
                    Label = "Corporate Email",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "alex@company.com",
                    Options = null
                },
                new()
                {
                    Name = "password",
                    Label = "Secure Password",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
                new()
                {
                    Name = "confirm_password",
                    Label = "Confirm Password",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
                new()
                {
                    Name = "accept_terms",
                    Label = "I accept the Terms & Conditions",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = false,
                    Options = null
                },
            }
        });

        // Form 3: Multi-Factor Authentication (MFA) Form
        _allForms.Add(new FormSchema
        {
            Id = "mfa_otp",
            Title = "Multi-Factor Authentication (MFA) Form",
            Category = "Authentication & Identity",
            Description = "Time-based one-time password (TOTP) or SMS security verification code entry.",
            Icon = "🛡️",
            Columns = 2,
            SubmitButtonText = "Submit Multi-Factor",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "otp_code",
                    Label = "6-Digit Verification Code",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "123456",
                    Options = null
                },
                new()
                {
                    Name = "trust_device",
                    Label = "Trust this browser for 14 days",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 4: Password Recovery Request
        _allForms.Add(new FormSchema
        {
            Id = "password_reset",
            Title = "Password Recovery Request",
            Category = "Authentication & Identity",
            Description = "Request password reset magic link or token via registered email address.",
            Icon = "🔄",
            Columns = 2,
            SubmitButtonText = "Submit Password",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "email",
                    Label = "Registered Email Address",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "dev@example.com",
                    Options = null
                },
            }
        });

        // Form 5: Mandatory Password Rotation Form
        _allForms.Add(new FormSchema
        {
            Id = "force_password_change",
            Title = "Mandatory Password Rotation Form",
            Category = "Authentication & Identity",
            Description = "Compliance-enforced credential renewal with old password validation.",
            Icon = "🔒",
            Columns = 2,
            SubmitButtonText = "Submit Mandatory",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "current_password",
                    Label = "Current Password",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
                new()
                {
                    Name = "new_password",
                    Label = "New Password (min 12 chars)",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
                new()
                {
                    Name = "confirm_new_password",
                    Label = "Confirm New Password",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
            }
        });

        // Form 6: Passwordless Magic Link Request
        _allForms.Add(new FormSchema
        {
            Id = "magic_link",
            Title = "Passwordless Magic Link Request",
            Category = "Authentication & Identity",
            Description = "Zero-friction passwordless authentication via cryptographic single-use URL.",
            Icon = "✨",
            Columns = 2,
            SubmitButtonText = "Submit Passwordless",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "email",
                    Label = "Work Email",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "founder@startup.io",
                    Options = null
                },
            }
        });

        // Form 7: Enterprise SSO Domain Verification
        _allForms.Add(new FormSchema
        {
            Id = "sso_domain_claim",
            Title = "Enterprise SSO Domain Verification",
            Category = "Authentication & Identity",
            Description = "Register and verify corporate SAML 2.0 / OIDC identity provider domains.",
            Icon = "🏢",
            Columns = 2,
            SubmitButtonText = "Submit Enterprise",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "domain",
                    Label = "Corporate Domain",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "enterprise.com",
                    Options = null
                },
                new()
                {
                    Name = "idp_provider",
                    Label = "Identity Provider (IdP)",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Okta",
                    Options = new List<string> { "Okta", "Azure AD / Entra ID", "Google Workspace", "Ping Identity" }
                },
                new()
                {
                    Name = "metadata_url",
                    Label = "SAML Metadata XML URL",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "https://idp.enterprise.com/metadata",
                    Options = null
                },
            }
        });

        // Form 8: Session Timeout Re-Authentication
        _allForms.Add(new FormSchema
        {
            Id = "session_unlock",
            Title = "Session Timeout Re-Authentication",
            Category = "Authentication & Identity",
            Description = "Quick PIN or biometrics re-entry after idle screen lock.",
            Icon = "⏱️",
            Columns = 2,
            SubmitButtonText = "Submit Session",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "pin",
                    Label = "4-Digit Quick PIN",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = "••••",
                    Options = null
                },
            }
        });

        // Form 9: API Key & Access Token Generator
        _allForms.Add(new FormSchema
        {
            Id = "api_key_provisioning",
            Title = "API Key & Access Token Generator",
            Category = "Authentication & Identity",
            Description = "Developer form for creating scoped REST and GraphQL programmatic access tokens.",
            Icon = "⚡",
            Columns = 2,
            SubmitButtonText = "Submit API",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "key_name",
                    Label = "Key Description / Label",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Production Webhook Dispatcher",
                    Options = null
                },
                new()
                {
                    Name = "scope",
                    Label = "Permission Scope",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Read & Write",
                    Options = new List<string> { "Read-Only", "Read & Write", "Admin Superuser" }
                },
                new()
                {
                    Name = "expiration_days",
                    Label = "Expires In (Days)",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 90,
                    Options = null
                },
            }
        });

        // Form 10: New Device OAuth Authorization
        _allForms.Add(new FormSchema
        {
            Id = "device_authorization",
            Title = "New Device OAuth Authorization",
            Category = "Authentication & Identity",
            Description = "Authorize secondary devices, CLI tools, or Smart TVs via 8-character user code.",
            Icon = "📱",
            Columns = 2,
            SubmitButtonText = "Submit New",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "user_code",
                    Label = "User Code Displayed on Screen",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "ABCD-EFGH",
                    Options = null
                },
            }
        });

        // Form 11: Personal Profile Information
        _allForms.Add(new FormSchema
        {
            Id = "profile_edit",
            Title = "Personal Profile Information",
            Category = "User Profiles & Account Settings",
            Description = "Update legal name, display handle, job role, and biography.",
            Icon = "👤",
            Columns = 2,
            SubmitButtonText = "Submit Personal",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "first_name",
                    Label = "First Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Alex",
                    Options = null
                },
                new()
                {
                    Name = "last_name",
                    Label = "Last Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Morgan",
                    Options = null
                },
                new()
                {
                    Name = "handle",
                    Label = "Public Handle (@username)",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "alexm",
                    Options = null
                },
                new()
                {
                    Name = "bio",
                    Label = "Short Biography",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Staff Infrastructure Engineer & WASM aficionado.",
                    Options = null
                },
            }
        });

        // Form 12: Avatar & Profile Media Selector
        _allForms.Add(new FormSchema
        {
            Id = "avatar_upload",
            Title = "Avatar & Profile Media Selector",
            Category = "User Profiles & Account Settings",
            Description = "Upload or select custom image avatars with crop and focal positioning.",
            Icon = "🖼️",
            Columns = 2,
            SubmitButtonText = "Submit Avatar",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "avatar_url",
                    Label = "Image Asset URL",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "https://images.unsplash.com/photo-1534528741775-53994a69daeb",
                    Options = null
                },
                new()
                {
                    Name = "alt_text",
                    Label = "Accessibility Alt Text",
                    Type = FormFieldType.Text,
                    Required = false,
                    DefaultValue = "Profile portrait",
                    Options = null
                },
            }
        });

        // Form 13: Primary Email Address Change
        _allForms.Add(new FormSchema
        {
            Id = "email_change",
            Title = "Primary Email Address Change",
            Category = "User Profiles & Account Settings",
            Description = "Update notification address with two-step validation on both old and new inboxes.",
            Icon = "✉️",
            Columns = 2,
            SubmitButtonText = "Submit Primary",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "new_email",
                    Label = "New Email Address",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "new.email@company.com",
                    Options = null
                },
                new()
                {
                    Name = "current_password",
                    Label = "Current Password Confirmation",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
            }
        });

        // Form 14: Two-Factor Authentication Setup
        _allForms.Add(new FormSchema
        {
            Id = "security_2fa_setup",
            Title = "Two-Factor Authentication Setup",
            Category = "User Profiles & Account Settings",
            Description = "Configure Authenticator App (Google/Microsoft Authenticator) with secret key.",
            Icon = "🔐",
            Columns = 2,
            SubmitButtonText = "Submit Two-Factor",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "secret_key",
                    Label = "Secret Key (Scan QR or enter manually)",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "JBSWY3DPEHPK3PXP",
                    Options = null
                },
                new()
                {
                    Name = "confirmation_code",
                    Label = "Verification Code from App",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
            }
        });

        // Form 15: Notification Channel Preferences
        _allForms.Add(new FormSchema
        {
            Id = "notification_preferences",
            Title = "Notification Channel Preferences",
            Category = "User Profiles & Account Settings",
            Description = "Granular control over email digests, push notifications, and SMS alerts.",
            Icon = "🔔",
            Columns = 2,
            SubmitButtonText = "Submit Notification",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "email_digest",
                    Label = "Weekly Executive Email Digest",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "slack_alerts",
                    Label = "Instant Slack Incident Alerts",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "marketing_updates",
                    Label = "Product Announcements & Release Notes",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = false,
                    Options = null
                },
            }
        });

        // Form 16: Localization, Currency & Timezone
        _allForms.Add(new FormSchema
        {
            Id = "localization_settings",
            Title = "Localization, Currency & Timezone",
            Category = "User Profiles & Account Settings",
            Description = "Configure regional display formatting, locale language, and default currency.",
            Icon = "🌍",
            Columns = 2,
            SubmitButtonText = "Submit Localization,",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "language",
                    Label = "Display Language",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "English (US)",
                    Options = new List<string> { "English (US)", "German (DE)", "French (FR)", "Japanese (JA)", "Spanish (ES)" }
                },
                new()
                {
                    Name = "timezone",
                    Label = "Local Timezone",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "UTC -08:00 (Pacific Time)",
                    Options = new List<string> { "UTC -08:00 (Pacific Time)", "UTC -05:00 (Eastern Time)", "UTC +00:00 (London)", "UTC +01:00 (Berlin)", "UTC +09:00 (Tokyo)" }
                },
                new()
                {
                    Name = "currency",
                    Label = "Reporting Currency",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "USD ($)",
                    Options = new List<string> { "USD ($)", "EUR (€)", "GBP (£)", "JPY (¥)", "CAD ($)" }
                },
            }
        });

        // Form 17: Federated Social Accounts Linking
        _allForms.Add(new FormSchema
        {
            Id = "linked_social_accounts",
            Title = "Federated Social Accounts Linking",
            Category = "User Profiles & Account Settings",
            Description = "Connect GitHub, Google, LinkedIn, and GitLab accounts for single-click sign-in.",
            Icon = "🔗",
            Columns = 2,
            SubmitButtonText = "Submit Federated",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "link_github",
                    Label = "Connect GitHub Organization Account",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "link_google",
                    Label = "Connect Google Workspace",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "link_microsoft",
                    Label = "Connect Microsoft 365",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = false,
                    Options = null
                },
            }
        });

        // Form 18: Accessibility & Display Preferences
        _allForms.Add(new FormSchema
        {
            Id = "accessibility_preferences",
            Title = "Accessibility & Display Preferences",
            Category = "User Profiles & Account Settings",
            Description = "Adjust contrast modes, motion preferences, and font scaling.",
            Icon = "👓",
            Columns = 2,
            SubmitButtonText = "Submit Accessibility",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "high_contrast",
                    Label = "High Contrast WCAG AAA Mode",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = false,
                    Options = null
                },
                new()
                {
                    Name = "reduced_motion",
                    Label = "Reduce Animation Motion",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = false,
                    Options = null
                },
                new()
                {
                    Name = "font_scale",
                    Label = "Interface Text Scaling (100% - 150%)",
                    Type = FormFieldType.Slider,
                    Required = false,
                    DefaultValue = 100,
                    Options = null
                },
            }
        });

        // Form 19: Active Session Revocation Form
        _allForms.Add(new FormSchema
        {
            Id = "session_revocation",
            Title = "Active Session Revocation Form",
            Category = "User Profiles & Account Settings",
            Description = "Audit active browser sessions, IP addresses, and terminate unauthorized logins.",
            Icon = "🖥️",
            Columns = 2,
            SubmitButtonText = "Submit Active",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "revoke_all_other",
                    Label = "Log out of all other devices and locations",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "password",
                    Label = "Re-enter Password to confirm",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
            }
        });

        // Form 20: Account Deletion / Erasure Request
        _allForms.Add(new FormSchema
        {
            Id = "delete_account",
            Title = "Account Deletion / Erasure Request",
            Category = "User Profiles & Account Settings",
            Description = "Irreversible account termination in compliance with GDPR right to be forgotten.",
            Icon = "⚠️",
            Columns = 2,
            SubmitButtonText = "Submit Account",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "confirmation_phrase",
                    Label = "Type 'DELETE MY ACCOUNT' to confirm",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = null,
                    Options = null
                },
                new()
                {
                    Name = "reason",
                    Label = "Reason for Leaving",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Consolidating software stack.",
                    Options = null
                },
            }
        });

        // Form 21: Credit / Debit Card Checkout
        _allForms.Add(new FormSchema
        {
            Id = "credit_card_payment",
            Title = "Credit / Debit Card Checkout",
            Category = "Billing, Checkout & Payments",
            Description = "PCI-compliant card payment form with expiry, CVC, and cardholder name.",
            Icon = "💳",
            Columns = 2,
            SubmitButtonText = "Submit Credit",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "cardholder_name",
                    Label = "Cardholder Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Alex Morgan",
                    Options = null
                },
                new()
                {
                    Name = "card_number",
                    Label = "Card Number",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "4242 •••• •••• 4242",
                    Options = null
                },
                new()
                {
                    Name = "exp_date",
                    Label = "Expiration Date (MM/YY)",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "12/28",
                    Options = null
                },
                new()
                {
                    Name = "cvc",
                    Label = "CVC / CVV",
                    Type = FormFieldType.Password,
                    Required = true,
                    DefaultValue = "•••",
                    Options = null
                },
            }
        });

        // Form 22: Billing Address Information
        _allForms.Add(new FormSchema
        {
            Id = "billing_address",
            Title = "Billing Address Information",
            Category = "Billing, Checkout & Payments",
            Description = "Tax jurisdiction address for invoice calculation and VAT compliance.",
            Icon = "📍",
            Columns = 2,
            SubmitButtonText = "Submit Billing",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "street_address",
                    Label = "Street Address",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "100 Montgomery St, Suite 400",
                    Options = null
                },
                new()
                {
                    Name = "city",
                    Label = "City",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "San Francisco",
                    Options = null
                },
                new()
                {
                    Name = "state",
                    Label = "State / Province",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "California",
                    Options = null
                },
                new()
                {
                    Name = "postal_code",
                    Label = "Postal / ZIP Code",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "94104",
                    Options = null
                },
                new()
                {
                    Name = "country",
                    Label = "Country",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "United States",
                    Options = new List<string> { "United States", "Canada", "United Kingdom", "Germany", "Australia" }
                },
            }
        });

        // Form 23: SaaS Subscription Plan Upgrade
        _allForms.Add(new FormSchema
        {
            Id = "plan_upgrade",
            Title = "SaaS Subscription Plan Upgrade",
            Category = "Billing, Checkout & Payments",
            Description = "Select tier (Starter, Pro, Enterprise) and billing frequency (Monthly vs Annual).",
            Icon = "🚀",
            Columns = 2,
            SubmitButtonText = "Submit SaaS",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "target_plan",
                    Label = "Select Plan Tier",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Enterprise Ultra",
                    Options = new List<string> { "Starter ($49/mo)", "Pro ($199/mo)", "Enterprise Ultra ($999/mo)" }
                },
                new()
                {
                    Name = "billing_cycle",
                    Label = "Billing Frequency",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Annual (20% Savings)",
                    Options = new List<string> { "Monthly", "Annual (20% Savings)" }
                },
                new()
                {
                    Name = "seats",
                    Label = "Additional User Seats",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 25,
                    Options = null
                },
            }
        });

        // Form 24: Tax & VAT Identification Number
        _allForms.Add(new FormSchema
        {
            Id = "tax_vat_id",
            Title = "Tax & VAT Identification Number",
            Category = "Billing, Checkout & Payments",
            Description = "Enter EU VAT, GST, or US EIN for B2B reverse charge tax exemptions.",
            Icon = "🧾",
            Columns = 2,
            SubmitButtonText = "Submit Tax",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "tax_id_type",
                    Label = "Tax ID Standard",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "EU VAT Number",
                    Options = new List<string> { "EU VAT Number", "US EIN (Employer ID)", "UK VAT", "Australian ABN" }
                },
                new()
                {
                    Name = "tax_id_number",
                    Label = "Tax Identification Number",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "DE123456789",
                    Options = null
                },
            }
        });

        // Form 25: Purchase Order (PO) Invoice Request
        _allForms.Add(new FormSchema
        {
            Id = "purchase_order_request",
            Title = "Purchase Order (PO) Invoice Request",
            Category = "Billing, Checkout & Payments",
            Description = "Enterprise procurement request for Net-30 or Net-60 invoiced billing.",
            Icon = "📄",
            Columns = 2,
            SubmitButtonText = "Submit Purchase",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "po_number",
                    Label = "Official PO Reference Number",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "PO-2026-8941",
                    Options = null
                },
                new()
                {
                    Name = "payment_terms",
                    Label = "Payment Terms",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Net 30 Days",
                    Options = new List<string> { "Net 30 Days", "Net 60 Days", "Due Upon Receipt" }
                },
                new()
                {
                    Name = "accounts_payable_email",
                    Label = "Accounts Payable Department Email",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "ap@enterprise.com",
                    Options = null
                },
            }
        });

        // Form 26: Payment Refund / Credit Request
        _allForms.Add(new FormSchema
        {
            Id = "refund_request",
            Title = "Payment Refund / Credit Request",
            Category = "Billing, Checkout & Payments",
            Description = "Submit dispute or refund request with transaction reference and reason.",
            Icon = "💸",
            Columns = 2,
            SubmitButtonText = "Submit Payment",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "invoice_number",
                    Label = "Invoice / Charge Number",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "INV-84920",
                    Options = null
                },
                new()
                {
                    Name = "refund_reason",
                    Label = "Reason for Refund Request",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Accidental Duplicate Charge",
                    Options = new List<string> { "Accidental Duplicate Charge", "Service Non-Performance", "Billing Discrepancy", "Other" }
                },
                new()
                {
                    Name = "explanation",
                    Label = "Additional Context",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Charged twice during network hiccup.",
                    Options = null
                },
            }
        });

        // Form 27: Billing Discrepancy & Dispute Form
        _allForms.Add(new FormSchema
        {
            Id = "invoice_dispute",
            Title = "Billing Discrepancy & Dispute Form",
            Category = "Billing, Checkout & Payments",
            Description = "Formal line-item dispute form with supporting document attachment.",
            Icon = "⚖️",
            Columns = 2,
            SubmitButtonText = "Submit Billing",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "disputed_amount",
                    Label = "Disputed Amount ($ USD)",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 1250.0,
                    Options = null
                },
                new()
                {
                    Name = "dispute_details",
                    Label = "Specific Line Item Discrepancy",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Tier discount was not applied to seat add-on.",
                    Options = null
                },
            }
        });

        // Form 28: Wire Transfer / ACH Inward Notice
        _allForms.Add(new FormSchema
        {
            Id = "wire_transfer_notification",
            Title = "Wire Transfer / ACH Inward Notice",
            Category = "Billing, Checkout & Payments",
            Description = "Notify accounting of pending SWIFT/SEPA incoming wire payments.",
            Icon = "🏦",
            Columns = 2,
            SubmitButtonText = "Submit Wire",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "remittance_bank",
                    Label = "Originating Bank Institution",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "JPMorgan Chase",
                    Options = null
                },
                new()
                {
                    Name = "transfer_reference",
                    Label = "Bank Wire Reference / MT103",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "WT-992014881",
                    Options = null
                },
                new()
                {
                    Name = "amount_transferred",
                    Label = "Amount Transferred ($ USD)",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 45000.0,
                    Options = null
                },
            }
        });

        // Form 29: Promotional Code & Voucher Redeem
        _allForms.Add(new FormSchema
        {
            Id = "coupon_redeem",
            Title = "Promotional Code & Voucher Redeem",
            Category = "Billing, Checkout & Payments",
            Description = "Redeem promotional discount voucher or investor discount credit.",
            Icon = "🏷️",
            Columns = 2,
            SubmitButtonText = "Submit Promotional",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "promo_code",
                    Label = "Coupon / Voucher Code",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "LAUNCH-WASM-2026",
                    Options = null
                },
            }
        });

        // Form 30: Bank Account Micro-Deposit Verification
        _allForms.Add(new FormSchema
        {
            Id = "ach_verification",
            Title = "Bank Account Micro-Deposit Verification",
            Category = "Billing, Checkout & Payments",
            Description = "Confirm two small bank deposit amounts to verify ACH direct debit account.",
            Icon = "🔍",
            Columns = 2,
            SubmitButtonText = "Submit Bank",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "deposit_1",
                    Label = "First Deposit Amount ($0.xx)",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 0.14,
                    Options = null
                },
                new()
                {
                    Name = "deposit_2",
                    Label = "Second Deposit Amount ($0.xx)",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 0.38,
                    Options = null
                },
            }
        });

        // Form 31: Corporate Strategic Operating Plan
        _allForms.Add(new FormSchema
        {
            Id = "company_plan",
            Title = "Corporate Strategic Operating Plan",
            Category = "Enterprise Business & Governance",
            Description = "Formulate executive objectives, OKRs, capital budget, and milestones.",
            Icon = "🎯",
            Columns = 2,
            SubmitButtonText = "Submit Corporate",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "company_name",
                    Label = "Company Legal Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Global Frontier Systems Inc.",
                    Options = null
                },
                new()
                {
                    Name = "fiscal_year",
                    Label = "Planning Fiscal Year",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "2026",
                    Options = new List<string> { "2026", "2027", "2028" }
                },
                new()
                {
                    Name = "executive_sponsor",
                    Label = "Executive Sponsor",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Helena Vance, Executive Chair",
                    Options = null
                },
                new()
                {
                    Name = "budget_millions",
                    Label = "Total Operating Budget ($M)",
                    Type = FormFieldType.Slider,
                    Required = true,
                    DefaultValue = 28.5,
                    Options = null
                },
                new()
                {
                    Name = "headcount_target",
                    Label = "New Headcount Target",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 40,
                    Options = null
                },
            }
        });

        // Form 32: Corporate Profit & Loss (P&L) Statement
        _allForms.Add(new FormSchema
        {
            Id = "profit_loss",
            Title = "Corporate Profit & Loss (P&L) Statement",
            Category = "Enterprise Business & Governance",
            Description = "Model revenue streams, direct cost of goods sold, and operating margins.",
            Icon = "📊",
            Columns = 2,
            SubmitButtonText = "Submit Corporate",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "reporting_period",
                    Label = "Reporting Period",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "FY2026 Q3",
                    Options = null
                },
                new()
                {
                    Name = "rev_arr",
                    Label = "Software Subscriptions (ARR)",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 8450000,
                    Options = null
                },
                new()
                {
                    Name = "cogs_cloud",
                    Label = "Cloud & Infrastructure COGS",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 820000,
                    Options = null
                },
                new()
                {
                    Name = "opex_rnd",
                    Label = "R&D Engineering OPEX",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 3200000,
                    Options = null
                },
                new()
                {
                    Name = "tax_rate",
                    Label = "Effective Tax Rate (%)",
                    Type = FormFieldType.Slider,
                    Required = true,
                    DefaultValue = 21,
                    Options = null
                },
            }
        });

        // Form 33: Vendor Security & Due Diligence Questionnaire
        _allForms.Add(new FormSchema
        {
            Id = "vendor_due_diligence",
            Title = "Vendor Security & Due Diligence Questionnaire",
            Category = "Enterprise Business & Governance",
            Description = "Assess vendor compliance with ISO 27001, SOC2, and data safety.",
            Icon = "🛡️",
            Columns = 2,
            SubmitButtonText = "Submit Vendor",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "vendor_name",
                    Label = "Vendor / Supplier Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "CloudGrid Infrastructure Ltd",
                    Options = null
                },
                new()
                {
                    Name = "has_soc2",
                    Label = "Maintains Valid SOC 2 Type II Certification",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "data_encryption",
                    Label = "Data Encrypted at Rest (AES-256) & Transit (TLS 1.3)",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "incident_response_sla",
                    Label = "Critical Incident Notification SLA",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "< 24 Hours",
                    Options = new List<string> { "< 1 Hour", "< 4 Hours", "< 24 Hours", "< 72 Hours" }
                },
            }
        });

        // Form 34: Board of Directors Meeting Minutes
        _allForms.Add(new FormSchema
        {
            Id = "board_minutes",
            Title = "Board of Directors Meeting Minutes",
            Category = "Enterprise Business & Governance",
            Description = "Official recording of board quorum, resolutions passed, and voting records.",
            Icon = "📑",
            Columns = 2,
            SubmitButtonText = "Submit Board",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "meeting_date",
                    Label = "Board Meeting Date",
                    Type = FormFieldType.Date,
                    Required = true,
                    DefaultValue = "2026-10-22",
                    Options = null
                },
                new()
                {
                    Name = "quorum_present",
                    Label = "Quorum Formed by Certified Majority",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "resolutions_passed",
                    Label = "Key Corporate Resolutions Passed",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Approved FY2026 capital expansion and executive option grants.",
                    Options = null
                },
            }
        });

        // Form 35: Mutual Non-Disclosure Agreement (NDA)
        _allForms.Add(new FormSchema
        {
            Id = "nda_agreement",
            Title = "Mutual Non-Disclosure Agreement (NDA)",
            Category = "Enterprise Business & Governance",
            Description = "Confidentiality undertaking binding signatory parties prior to diligence.",
            Icon = "🤝",
            Columns = 2,
            SubmitButtonText = "Submit Mutual",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "counterparty_name",
                    Label = "Counterparty Legal Entity",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Apex Capital Partners LLC",
                    Options = null
                },
                new()
                {
                    Name = "term_years",
                    Label = "Confidentiality Duration (Years)",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "3 Years",
                    Options = new List<string> { "1 Year", "2 Years", "3 Years", "5 Years", "Perpetual" }
                },
                new()
                {
                    Name = "jurisdiction",
                    Label = "Governing Law Jurisdiction",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "State of Delaware",
                    Options = new List<string> { "State of Delaware", "State of California", "State of New York", "England & Wales" }
                },
            }
        });

        // Form 36: Shareholder Annual Proxy Voting Form
        _allForms.Add(new FormSchema
        {
            Id = "shareholder_proxy",
            Title = "Shareholder Annual Proxy Voting Form",
            Category = "Enterprise Business & Governance",
            Description = "Elect board members, ratify independent auditors, and vote on bylaws.",
            Icon = "🗳️",
            Columns = 2,
            SubmitButtonText = "Submit Shareholder",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "shares_held",
                    Label = "Total Common Shares Held",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 125000,
                    Options = null
                },
                new()
                {
                    Name = "elect_auditors",
                    Label = "Ratify Ernst & Young LLP as Independent Auditors",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "FOR",
                    Options = new List<string> { "FOR", "AGAINST", "ABSTAIN" }
                },
                new()
                {
                    Name = "bylaw_amendment",
                    Label = "Approve Amendment to 2026 Equity Incentive Plan",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "FOR",
                    Options = new List<string> { "FOR", "AGAINST", "ABSTAIN" }
                },
            }
        });

        // Form 37: Enterprise Risk Assessment Register
        _allForms.Add(new FormSchema
        {
            Id = "risk_register",
            Title = "Enterprise Risk Assessment Register",
            Category = "Enterprise Business & Governance",
            Description = "Document operational, cyber, market, and reputational risk factors.",
            Icon = "⚠️",
            Columns = 2,
            SubmitButtonText = "Submit Enterprise",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "risk_title",
                    Label = "Risk Title / Threat Description",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Critical Third-Party API Outage",
                    Options = null
                },
                new()
                {
                    Name = "severity",
                    Label = "Impact Severity",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "High",
                    Options = new List<string> { "Critical", "High", "Medium", "Low" }
                },
                new()
                {
                    Name = "likelihood",
                    Label = "Occurrence Likelihood",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Moderate",
                    Options = new List<string> { "Almost Certain", "Likely", "Moderate", "Unlikely" }
                },
                new()
                {
                    Name = "mitigation_strategy",
                    Label = "Mitigation Strategy & Redundancy",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Multi-region cloud failover with automated circuit breakers.",
                    Options = null
                },
            }
        });

        // Form 38: Business Continuity & Disaster Plan
        _allForms.Add(new FormSchema
        {
            Id = "business_continuity",
            Title = "Business Continuity & Disaster Plan",
            Category = "Enterprise Business & Governance",
            Description = "Disaster recovery RTO/RPO targets and emergency escalation chains.",
            Icon = "🚨",
            Columns = 2,
            SubmitButtonText = "Submit Business",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "rto_hours",
                    Label = "Recovery Time Objective (RTO in Hours)",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 2,
                    Options = null
                },
                new()
                {
                    Name = "rpo_hours",
                    Label = "Recovery Point Objective (RPO in Hours)",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 0.5,
                    Options = null
                },
                new()
                {
                    Name = "emergency_liaison",
                    Label = "Designated Emergency Response Commander",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "David Kim, VP Engineering",
                    Options = null
                },
            }
        });

        // Form 39: Fixed Corporate Asset Audit Intake
        _allForms.Add(new FormSchema
        {
            Id = "asset_audit",
            Title = "Fixed Corporate Asset Audit Intake",
            Category = "Enterprise Business & Governance",
            Description = "Log enterprise laptops, lab servers, test hardware, and lease contracts.",
            Icon = "💻",
            Columns = 2,
            SubmitButtonText = "Submit Fixed",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "asset_tag",
                    Label = "Barcode Asset Tag Number",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "BLZR-HW-9941",
                    Options = null
                },
                new()
                {
                    Name = "asset_type",
                    Label = "Hardware Category",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "MacBook Pro M3 Max",
                    Options = new List<string> { "MacBook Pro M3 Max", "Dell Precision Workstation", "Rack Server Unit", "Testing Tablet" }
                },
                new()
                {
                    Name = "assigned_to",
                    Label = "Assigned Employee",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Sophia Chen",
                    Options = null
                },
            }
        });

        // Form 40: M&A Target Diligence Initial Intake
        _allForms.Add(new FormSchema
        {
            Id = "ma_intake",
            Title = "M&A Target Diligence Initial Intake",
            Category = "Enterprise Business & Governance",
            Description = "Initial exploratory questionnaire for prospective acquisition targets.",
            Icon = "📈",
            Columns = 2,
            SubmitButtonText = "Submit M&A",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "target_name",
                    Label = "Target Enterprise Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "VectorCloud AI Technologies",
                    Options = null
                },
                new()
                {
                    Name = "valuation_expectation",
                    Label = "Estimated Enterprise Valuation ($M)",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 65.0,
                    Options = null
                },
                new()
                {
                    Name = "strategic_rationale",
                    Label = "Synergies & Strategic Fit",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Complementary client-side vector database engine and 14 enterprise patents.",
                    Options = null
                },
            }
        });

        // Form 41: Employee Excellence & Spot Bonus Nomination
        _allForms.Add(new FormSchema
        {
            Id = "employee_award",
            Title = "Employee Excellence & Spot Bonus Nomination",
            Category = "Human Resources & People Operations",
            Description = "Nominate exceptional team members for peer awards and cash grants.",
            Icon = "🏆",
            Columns = 2,
            SubmitButtonText = "Submit Employee",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "nominee_name",
                    Label = "Nominee Full Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Elena Rostova",
                    Options = null
                },
                new()
                {
                    Name = "award_category",
                    Label = "Award Category",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Engineering Velocity Master",
                    Options = new List<string> { "Founders' Innovation Award", "Customer Obsession Champion", "Engineering Velocity Master", "Zero-Downtime Guardian", "Cultural Torchbearer" }
                },
                new()
                {
                    Name = "grant_amount",
                    Label = "Spot Cash Bonus ($ USD)",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "$3,000 USD",
                    Options = new List<string> { "$500 USD", "$1,500 USD", "$3,000 USD", "$5,000 USD" }
                },
                new()
                {
                    Name = "inscription_quote",
                    Label = "Certificate Plaque Inscription",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "For legendary code craftsmanship and unwavering commitment to sub-millisecond latency.",
                    Options = null
                },
            }
        });

        // Form 42: Careers / Candidate Job Application
        _allForms.Add(new FormSchema
        {
            Id = "job_application",
            Title = "Careers / Candidate Job Application",
            Category = "Human Resources & People Operations",
            Description = "Standard job applicant intake with resume link, portfolio, and work authorization.",
            Icon = "💼",
            Columns = 2,
            SubmitButtonText = "Submit Careers",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "candidate_name",
                    Label = "Applicant Full Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Jordan Lee",
                    Options = null
                },
                new()
                {
                    Name = "email",
                    Label = "Email Address",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "jordan.lee@example.com",
                    Options = null
                },
                new()
                {
                    Name = "linkedin_url",
                    Label = "LinkedIn / GitHub Portfolio URL",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "https://github.com/jordanlee",
                    Options = null
                },
                new()
                {
                    Name = "work_authorization",
                    Label = "Legally Authorized to Work in Country",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "cover_letter",
                    Label = "Why are you interested in this mission?",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Passionate about building client-side WebAssembly solutions with zero runtime dependencies.",
                    Options = null
                },
            }
        });

        // Form 43: New Hire Employee Onboarding Intake
        _allForms.Add(new FormSchema
        {
            Id = "onboarding_checklist",
            Title = "New Hire Employee Onboarding Intake",
            Category = "Human Resources & People Operations",
            Description = "Capture emergency contacts, payroll bank routing, and equipment sizing.",
            Icon = "🚀",
            Columns = 2,
            SubmitButtonText = "Submit New",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "preferred_name",
                    Label = "Preferred First Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Alex",
                    Options = null
                },
                new()
                {
                    Name = "emergency_contact",
                    Label = "Emergency Contact Name & Phone",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Morgan Vance (555-0192)",
                    Options = null
                },
                new()
                {
                    Name = "tshirt_size",
                    Label = "Company Swag Size",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Medium",
                    Options = new List<string> { "Small", "Medium", "Large", "XL", "2XL" }
                },
                new()
                {
                    Name = "laptop_choice",
                    Label = "Development Workstation Choice",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "MacBook Pro 16-inch M3 Max",
                    Options = new List<string> { "MacBook Pro 16-inch M3 Max", "ThinkPad P16 Gen 2 Linux", "Dell XPS 15 Windows" }
                },
            }
        });

        // Form 44: Paid Time Off (PTO) & Leave Request
        _allForms.Add(new FormSchema
        {
            Id = "pto_request",
            Title = "Paid Time Off (PTO) & Leave Request",
            Category = "Human Resources & People Operations",
            Description = "Submit vacation, medical, or bereavement leave request for manager approval.",
            Icon = "🏖️",
            Columns = 2,
            SubmitButtonText = "Submit Paid",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "leave_type",
                    Label = "Leave Classification",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Vacation / Personal Leave",
                    Options = new List<string> { "Vacation / Personal Leave", "Sick / Medical Leave", "Parental Leave", "Jury Duty / Civic" }
                },
                new()
                {
                    Name = "start_date",
                    Label = "Leave Start Date",
                    Type = FormFieldType.Date,
                    Required = true,
                    DefaultValue = "2026-11-20",
                    Options = null
                },
                new()
                {
                    Name = "end_date",
                    Label = "Leave Return Date",
                    Type = FormFieldType.Date,
                    Required = true,
                    DefaultValue = "2026-11-27",
                    Options = null
                },
                new()
                {
                    Name = "coverage_handover",
                    Label = "Team Coverage Handover Notes",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Marcus Vance covering core deployment shifts.",
                    Options = null
                },
            }
        });

        // Form 45: Annual 360 Performance Appraisal
        _allForms.Add(new FormSchema
        {
            Id = "performance_review",
            Title = "Annual 360 Performance Appraisal",
            Category = "Human Resources & People Operations",
            Description = "Evaluate goals achieved, core competency ratings, and professional growth areas.",
            Icon = "⭐",
            Columns = 2,
            SubmitButtonText = "Submit Annual",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "review_period",
                    Label = "Appraisal Cycle",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "FY2026 Annual Review",
                    Options = null
                },
                new()
                {
                    Name = "overall_rating",
                    Label = "Overall Performance Rating (1 to 5 Stars)",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 5,
                    Options = null
                },
                new()
                {
                    Name = "key_achievements",
                    Label = "Top Achievements & Milestones Delivered",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Led the migration to .NET 10+ WASM resulting in 40% memory reduction.",
                    Options = null
                },
                new()
                {
                    Name = "growth_goals",
                    Label = "Goals for Upcoming Performance Year",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Author advanced financial vector charting modules.",
                    Options = null
                },
            }
        });

        // Form 46: Corporate Expense Reimbursement Report
        _allForms.Add(new FormSchema
        {
            Id = "expense_reimbursement",
            Title = "Corporate Expense Reimbursement Report",
            Category = "Human Resources & People Operations",
            Description = "Itemize business travel, conference tickets, and client dining expenses.",
            Icon = "🧾",
            Columns = 2,
            SubmitButtonText = "Submit Corporate",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "expense_category",
                    Label = "Expense Category",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Travel & Lodging",
                    Options = new List<string> { "Travel & Lodging", "Meals & Entertainment", "Software & Cloud Tools", "Conference & Education" }
                },
                new()
                {
                    Name = "total_amount",
                    Label = "Total Expense Amount ($ USD)",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 842.5,
                    Options = null
                },
                new()
                {
                    Name = "merchant_name",
                    Label = "Merchant / Vendor",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Hyatt Regency Tech Conference",
                    Options = null
                },
                new()
                {
                    Name = "receipt_attached",
                    Label = "Receipt / Proof of Payment Attached",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 47: Confidential Employee Exit Interview
        _allForms.Add(new FormSchema
        {
            Id = "exit_interview",
            Title = "Confidential Employee Exit Interview",
            Category = "Human Resources & People Operations",
            Description = "Gather honest feedback on company culture, leadership, and reasons for transition.",
            Icon = "🚪",
            Columns = 2,
            SubmitButtonText = "Submit Confidential",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "departure_reason",
                    Label = "Primary Reason for Departure",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "New Career Opportunity",
                    Options = new List<string> { "New Career Opportunity", "Relocation", "Compensation", "Career Pivot", "Retirement" }
                },
                new()
                {
                    Name = "culture_rating",
                    Label = "Company Culture Satisfaction (1-5)",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 4,
                    Options = null
                },
                new()
                {
                    Name = "constructive_feedback",
                    Label = "Constructive Feedback for Executive Leadership",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Increase budget for open-source component initiatives.",
                    Options = null
                },
            }
        });

        // Form 48: Remote & Hybrid Work Telecommuting Policy
        _allForms.Add(new FormSchema
        {
            Id = "remote_work_agreement",
            Title = "Remote & Hybrid Work Telecommuting Policy",
            Category = "Human Resources & People Operations",
            Description = "Agreement specifying home office ergonomic compliance and security standards.",
            Icon = "🏡",
            Columns = 2,
            SubmitButtonText = "Submit Remote",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "work_location",
                    Label = "Designated Home Office City & State",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Seattle, WA",
                    Options = null
                },
                new()
                {
                    Name = "has_secure_wifi",
                    Label = "Secured WPA3 Wi-Fi & Corporate VPN Active",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "acknowledges_hours",
                    Label = "Acknowledges Core Collaboration Overlap Hours",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 49: Annual Health & Benefits Election Form
        _allForms.Add(new FormSchema
        {
            Id = "benefits_enrollment",
            Title = "Annual Health & Benefits Election Form",
            Category = "Human Resources & People Operations",
            Description = "Select health insurance tier, dental, 401(k) matching, and HSA contributions.",
            Icon = "🏥",
            Columns = 2,
            SubmitButtonText = "Submit Annual",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "medical_plan",
                    Label = "Health Insurance Plan Option",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "PPO Platinum (Zero Deductible)",
                    Options = new List<string> { "PPO Platinum (Zero Deductible)", "HDHP with Health Savings Account", "Kaiser HMO" }
                },
                new()
                {
                    Name = "401k_contribution",
                    Label = "401(k) Salary Deferral Percentage",
                    Type = FormFieldType.Slider,
                    Required = true,
                    DefaultValue = 8,
                    Options = null
                },
                new()
                {
                    Name = "hsa_annual_pledge",
                    Label = "HSA Annual Contribution ($ USD)",
                    Type = FormFieldType.Number,
                    Required = false,
                    DefaultValue = 3500,
                    Options = null
                },
            }
        });

        // Form 50: Workplace Incident & Ethics Grievance Report
        _allForms.Add(new FormSchema
        {
            Id = "incident_grievance",
            Title = "Workplace Incident & Ethics Grievance Report",
            Category = "Human Resources & People Operations",
            Description = "Confidential intake for reporting harassment, discrimination, or safety hazards.",
            Icon = "🛡️",
            Columns = 2,
            SubmitButtonText = "Submit Workplace",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "is_anonymous",
                    Label = "Submit Anonymously",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = false,
                    Options = null
                },
                new()
                {
                    Name = "incident_type",
                    Label = "Incident Classification",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Workplace Safety Concern",
                    Options = new List<string> { "Harassment / Discrimination", "Workplace Safety Concern", "Unethical Accounting Practice", "Retaliation" }
                },
                new()
                {
                    Name = "detailed_report",
                    Label = "Description of Incident & Witnesses",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Documented report of server room cooling safety threshold violation.",
                    Options = null
                },
            }
        });

        // Form 51: Corporate Gala & Party Planner
        _allForms.Add(new FormSchema
        {
            Id = "company_party",
            Title = "Corporate Gala & Party Planner",
            Category = "Events, Parties & Hospitality",
            Description = "Coordinate venue logistics, entertainment, catering menus, and employee RSVPs.",
            Icon = "🎉",
            Columns = 2,
            SubmitButtonText = "Submit Corporate",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "event_name",
                    Label = "Celebration Title",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Annual Enterprise Gala & High-Performance Celebration",
                    Options = null
                },
                new()
                {
                    Name = "venue_style",
                    Label = "Venue Style",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Skyline Rooftop Terrace",
                    Options = new List<string> { "Skyline Rooftop Terrace", "Grand Crystal Ballroom", "Coastal Beach Club", "Modern Tech Lounge" }
                },
                new()
                {
                    Name = "event_datetime",
                    Label = "Date & Schedule",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Friday, Dec 18, 2026 · 6:30 PM - Midnight",
                    Options = null
                },
                new()
                {
                    Name = "budget",
                    Label = "Total Event Budget ($ USD)",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 85000,
                    Options = null
                },
                new()
                {
                    Name = "plus_one_allowed",
                    Label = "Allow Plus-One Guest Registration",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 52: Tech Conference Ticket Registration
        _allForms.Add(new FormSchema
        {
            Id = "conference_ticket",
            Title = "Tech Conference Ticket Registration",
            Category = "Events, Parties & Hospitality",
            Description = "Attendee registration with badge printing name, workshop pass, and dinner ticket.",
            Icon = "🎟️",
            Columns = 2,
            SubmitButtonText = "Submit Tech",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "badge_name",
                    Label = "Full Name for Conference Badge",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Dr. Marcus Vance",
                    Options = null
                },
                new()
                {
                    Name = "company_affiliation",
                    Label = "Company / Affiliation",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Global Frontier Systems",
                    Options = null
                },
                new()
                {
                    Name = "ticket_type",
                    Label = "Ticket Tier",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "All-Access Pass + VIP Workshops",
                    Options = new List<string> { "General Admission", "All-Access Pass + VIP Workshops", "Virtual Livestream Only" }
                },
                new()
                {
                    Name = "attend_hackathon",
                    Label = "Register for Weekend Hackathon",
                    Type = FormFieldType.Checkbox,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 53: Live Developer Webinar Sign-Up
        _allForms.Add(new FormSchema
        {
            Id = "webinar_signup",
            Title = "Live Developer Webinar Sign-Up",
            Category = "Events, Parties & Hospitality",
            Description = "Register for technical keynote with email calendar reminder and recording link.",
            Icon = "🎥",
            Columns = 2,
            SubmitButtonText = "Submit Live",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "webinar_topic",
                    Label = "Webinar Session",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = ".NET 10+ WASM & High-Performance Tailwind Mastery",
                    Options = new List<string> { ".NET 10+ WASM & High-Performance Tailwind Mastery", "Zero-CDN Security Architectures", "High-Frequency SVG Financial Charting" }
                },
                new()
                {
                    Name = "email",
                    Label = "Work Email for Calendar Invite",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "lead.dev@enterprise.org",
                    Options = null
                },
            }
        });

        // Form 54: Call for Proposals (CFP) Speaker Intake
        _allForms.Add(new FormSchema
        {
            Id = "speaker_cfp",
            Title = "Call for Proposals (CFP) Speaker Intake",
            Category = "Events, Parties & Hospitality",
            Description = "Submit talk abstracts, bio, technical difficulty level, and slide links.",
            Icon = "🎙️",
            Columns = 2,
            SubmitButtonText = "Submit Call",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "talk_title",
                    Label = "Proposed Session Title",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Inside WebAssembly: Zero-DOM Overhead Rendering",
                    Options = null
                },
                new()
                {
                    Name = "abstract",
                    Label = "Detailed Abstract (max 300 words)",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "How we scaled 125 components without third-party frameworks.",
                    Options = null
                },
                new()
                {
                    Name = "difficulty_level",
                    Label = "Audience Technical Level",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Advanced / Deep Dive",
                    Options = new List<string> { "Introductory", "Intermediate", "Advanced / Deep Dive" }
                },
                new()
                {
                    Name = "speaker_bio",
                    Label = "Speaker Professional Bio",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Principal Architect specializing in high-throughput compilers.",
                    Options = null
                },
            }
        });

        // Form 55: Event Venue Booking & Lease Inquiry
        _allForms.Add(new FormSchema
        {
            Id = "venue_booking",
            Title = "Event Venue Booking & Lease Inquiry",
            Category = "Events, Parties & Hospitality",
            Description = "Private event space rental request with headcount and acoustic requirements.",
            Icon = "🏛️",
            Columns = 2,
            SubmitButtonText = "Submit Event",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "venue_requested",
                    Label = "Venue Space / Hall",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Metropolitan Glasshouse Hall",
                    Options = new List<string> { "Metropolitan Glasshouse Hall", "Rooftop Observatory", "Executive Boardroom Amphitheater" }
                },
                new()
                {
                    Name = "expected_headcount",
                    Label = "Expected Guest Headcount",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 250,
                    Options = null
                },
                new()
                {
                    Name = "requires_av",
                    Label = "Full Audiovisual Stage & Lighting Package Required",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 56: Catering & Banquet Menu Customizer
        _allForms.Add(new FormSchema
        {
            Id = "catering_menu",
            Title = "Catering & Banquet Menu Customizer",
            Category = "Events, Parties & Hospitality",
            Description = "Design 3-course dinner, buffet stations, cocktail bar, and midnight snacks.",
            Icon = "🍽️",
            Columns = 2,
            SubmitButtonText = "Submit Catering",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "dining_style",
                    Label = "Service Format",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Plated 3-Course Gourmet Dinner",
                    Options = new List<string> { "Plated 3-Course Gourmet Dinner", "Interactive Artisan Food Stations", "Tapas & Cocktail Reception" }
                },
                new()
                {
                    Name = "open_bar_tier",
                    Label = "Beverage Package",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Premium Craft Spirits & Sommelier Wine",
                    Options = new List<string> { "Soft Drinks & Mocktails", "Beer & Local Wine", "Premium Craft Spirits & Sommelier Wine" }
                },
                new()
                {
                    Name = "special_courses",
                    Label = "Special Chef Tasting Requests",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Feature local Pacific Northwest organic produce.",
                    Options = null
                },
            }
        });

        // Form 57: Post-Event Attendee Experience Survey
        _allForms.Add(new FormSchema
        {
            Id = "post_event_feedback",
            Title = "Post-Event Attendee Experience Survey",
            Category = "Events, Parties & Hospitality",
            Description = "Evaluate keynote quality, food & beverage satisfaction, and networking value.",
            Icon = "📋",
            Columns = 2,
            SubmitButtonText = "Submit Post-Event",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "overall_experience",
                    Label = "Overall Event Rating (1-5 Stars)",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 5,
                    Options = null
                },
                new()
                {
                    Name = "favorite_session",
                    Label = "Most Valuable Session or Moment",
                    Type = FormFieldType.Text,
                    Required = false,
                    DefaultValue = "Live Candlestick Charting demo",
                    Options = null
                },
                new()
                {
                    Name = "would_recommend",
                    Label = "Likelihood to Attend Next Year (%)",
                    Type = FormFieldType.Slider,
                    Required = true,
                    DefaultValue = 95,
                    Options = null
                },
            }
        });

        // Form 58: VIP Executive Guest Fast Check-In
        _allForms.Add(new FormSchema
        {
            Id = "vip_checkin",
            Title = "VIP Executive Guest Fast Check-In",
            Category = "Events, Parties & Hospitality",
            Description = "Door management form for verifying credentials and issuing NFC passes.",
            Icon = "✨",
            Columns = 2,
            SubmitButtonText = "Submit VIP",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "guest_id",
                    Label = "VIP Guest Registration ID",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "VIP-2026-778",
                    Options = null
                },
                new()
                {
                    Name = "security_badge_issued",
                    Label = "Issued Holographic Security Lanyard",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "escort_assigned",
                    Label = "Designated Executive Host Escort",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Helena Vance",
                    Options = null
                },
            }
        });

        // Form 59: Corporate Event Sponsorship Application
        _allForms.Add(new FormSchema
        {
            Id = "event_sponsorship",
            Title = "Corporate Event Sponsorship Application",
            Category = "Events, Parties & Hospitality",
            Description = "Sponsor tiers (Title, Platinum, Gold) with booth dimensions and logo rights.",
            Icon = "🤝",
            Columns = 2,
            SubmitButtonText = "Submit Corporate",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "sponsor_company",
                    Label = "Sponsoring Enterprise",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "HyperScale Cloud Systems",
                    Options = null
                },
                new()
                {
                    Name = "sponsor_tier",
                    Label = "Sponsorship Package",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Platinum Headline Partner ($50,000)",
                    Options = new List<string> { "Platinum Headline Partner ($50,000)", "Gold Stage Sponsor ($25,000)", "Silver Booth Partner ($10,000)" }
                },
                new()
                {
                    Name = "booth_needs",
                    Label = "Booth Dimensions & Electrical Needs",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "20x20 ft Island with 3-phase power",
                    Options = null
                },
            }
        });

        // Form 60: Speaker Hotel & Airport Shuttle Dispatch
        _allForms.Add(new FormSchema
        {
            Id = "transportation_hotel",
            Title = "Speaker Hotel & Airport Shuttle Dispatch",
            Category = "Events, Parties & Hospitality",
            Description = "Coordinate flight arrivals, chauffeur pickup, and luxury suite reservations.",
            Icon = "✈️",
            Columns = 2,
            SubmitButtonText = "Submit Speaker",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "flight_number",
                    Label = "Arrival Flight Number",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "UA 894 (SFO -> SEA)",
                    Options = null
                },
                new()
                {
                    Name = "hotel_nights",
                    Label = "Hotel Reservation Nights",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 3,
                    Options = null
                },
                new()
                {
                    Name = "shuttle_requested",
                    Label = "Private Chauffeur Pickup Needed",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 61: Customer Support Ticket Intake
        _allForms.Add(new FormSchema
        {
            Id = "support_ticket",
            Title = "Customer Support Ticket Intake",
            Category = "Customer Support, IT & Helpdesk",
            Description = "Submit technical inquiries with severity level, environment details, and logs.",
            Icon = "🎫",
            Columns = 2,
            SubmitButtonText = "Submit Customer",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "ticket_subject",
                    Label = "Issue Summary",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "WebSocket Disconnection on Safari Mobile",
                    Options = null
                },
                new()
                {
                    Name = "severity",
                    Label = "Impact Severity",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "High (Production Impact)",
                    Options = new List<string> { "Critical (System Down)", "High (Production Impact)", "Medium (Degraded)", "Low (Cosmetic)" }
                },
                new()
                {
                    Name = "description",
                    Label = "Detailed Steps to Reproduce",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Client experiences handshake timeout when switching networks.",
                    Options = null
                },
                new()
                {
                    Name = "component_affected",
                    Label = "Component Affected",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "TwChatWidget",
                    Options = new List<string> { "TwChatWidget", "TwDataGrid", "TwFinancialChart", "TwMediaPlayer", "Other" }
                },
            }
        });

        // Form 62: Engineering Bug Report & System Specs
        _allForms.Add(new FormSchema
        {
            Id = "bug_report",
            Title = "Engineering Bug Report & System Specs",
            Category = "Customer Support, IT & Helpdesk",
            Description = "Developer-grade defect filing with OS, browser version, and stack traces.",
            Icon = "🐛",
            Columns = 2,
            SubmitButtonText = "Submit Engineering",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "bug_title",
                    Label = "Defect Title",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "SVG text clipping on high-DPI scaling",
                    Options = null
                },
                new()
                {
                    Name = "browser_os",
                    Label = "Browser & OS Environment",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Chrome 130 / macOS Sonoma 14.5",
                    Options = null
                },
                new()
                {
                    Name = "repro_steps",
                    Label = "Reproduction Sequence",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "1. Zoom canvas to 150%\n2. Observe label bounding box overflow.",
                    Options = null
                },
                new()
                {
                    Name = "stack_trace",
                    Label = "Console Error Output / Stack Trace",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Error: SVG Matrix transform bounds out of bounds.",
                    Options = null
                },
            }
        });

        // Form 63: Product Feature Suggestion & Vote
        _allForms.Add(new FormSchema
        {
            Id = "feature_request",
            Title = "Product Feature Suggestion & Vote",
            Category = "Customer Support, IT & Helpdesk",
            Description = "Submit user ideas for roadmapping with business justification.",
            Icon = "💡",
            Columns = 2,
            SubmitButtonText = "Submit Product",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "feature_title",
                    Label = "Feature Concept",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Gantt Chart & Resource Allocation View",
                    Options = null
                },
                new()
                {
                    Name = "target_persona",
                    Label = "Target User Persona",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Project Managers & Engineering Directors",
                    Options = null
                },
                new()
                {
                    Name = "value_proposition",
                    Label = "Business Value & Use Case",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Enables native sprint scheduling without third-party plugins.",
                    Options = null
                },
            }
        });

        // Form 64: Major Incident Escalation Declaration
        _allForms.Add(new FormSchema
        {
            Id = "service_outage_incident",
            Title = "Major Incident Escalation Declaration",
            Category = "Customer Support, IT & Helpdesk",
            Description = "P1 incident alert dispatching on-call engineering commanders.",
            Icon = "🚨",
            Columns = 2,
            SubmitButtonText = "Submit Major",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "incident_code",
                    Label = "War Room Incident Code",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "INC-9912",
                    Options = null
                },
                new()
                {
                    Name = "services_impacted",
                    Label = "Impacted Services",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "US-East Payment Processing API",
                    Options = null
                },
                new()
                {
                    Name = "estimated_users",
                    Label = "Estimated Affected Users",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 12000,
                    Options = null
                },
                new()
                {
                    Name = "incident_commander",
                    Label = "Designated Incident Commander",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Elena Rostova",
                    Options = null
                },
            }
        });

        // Form 65: Internal IT Hardware & Peripheral Request
        _allForms.Add(new FormSchema
        {
            Id = "hardware_request",
            Title = "Internal IT Hardware & Peripheral Request",
            Category = "Customer Support, IT & Helpdesk",
            Description = "Request test devices, ergonomic monitor arms, or USB-C docking hubs.",
            Icon = "🖥️",
            Columns = 2,
            SubmitButtonText = "Submit Internal",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "item_needed",
                    Label = "Requested Hardware Item",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Dual 4K Studio Displays",
                    Options = new List<string> { "Dual 4K Studio Displays", "Mechanical Ergo Keyboard", "Noise-Cancelling Headset", "Server Lab Testing Unit" }
                },
                new()
                {
                    Name = "manager_approval",
                    Label = "Direct Manager Has Approved",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 66: SaaS Software License & Seat Allocation
        _allForms.Add(new FormSchema
        {
            Id = "software_license_request",
            Title = "SaaS Software License & Seat Allocation",
            Category = "Customer Support, IT & Helpdesk",
            Description = "Request seat licenses for Figma, GitHub Enterprise, or JetBrains IDE.",
            Icon = "🔑",
            Columns = 2,
            SubmitButtonText = "Submit SaaS",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "software_name",
                    Label = "Software Tool Name",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "JetBrains All Products Pack",
                    Options = new List<string> { "JetBrains All Products Pack", "Figma Enterprise", "Datadog Cloud Monitoring", "AWS Sandbox Account" }
                },
                new()
                {
                    Name = "justification",
                    Label = "Business Need Justification",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Required for .NET 10+ WebAssembly native profiling.",
                    Options = null
                },
            }
        });

        // Form 67: Helpdesk Identity Verification & Password Reset
        _allForms.Add(new FormSchema
        {
            Id = "helpdesk_password_reset",
            Title = "Helpdesk Identity Verification & Password Reset",
            Category = "Customer Support, IT & Helpdesk",
            Description = "IT desk form to verify caller via employee ID before resetting MFA.",
            Icon = "📞",
            Columns = 2,
            SubmitButtonText = "Submit Helpdesk",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "employee_id",
                    Label = "Corporate Employee ID Number",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "EMP-8419",
                    Options = null
                },
                new()
                {
                    Name = "verification_question",
                    Label = "Manager Verified via Video Call",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "temp_password_issued",
                    Label = "Issue Temporary 24-Hour Passcode",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 68: Enterprise Customer Onboarding Kickoff
        _allForms.Add(new FormSchema
        {
            Id = "customer_onboarding_kickoff",
            Title = "Enterprise Customer Onboarding Kickoff",
            Category = "Customer Support, IT & Helpdesk",
            Description = "Account setup questionnaire capturing expected throughput and SSO lead.",
            Icon = "🚢",
            Columns = 2,
            SubmitButtonText = "Submit Enterprise",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "customer_account",
                    Label = "Customer Enterprise Account Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Global Logistics Consortium",
                    Options = null
                },
                new()
                {
                    Name = "projected_monthly_users",
                    Label = "Projected Monthly Active Users",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 75000,
                    Options = null
                },
                new()
                {
                    Name = "sso_lead_email",
                    Label = "Customer Identity Architect Email",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "architect@globallogistics.com",
                    Options = null
                },
            }
        });

        // Form 69: Responsible Security Vulnerability Disclosure
        _allForms.Add(new FormSchema
        {
            Id = "vulnerability_disclosure",
            Title = "Responsible Security Vulnerability Disclosure",
            Category = "Customer Support, IT & Helpdesk",
            Description = "White-hat hacker bug bounty submission with CVE details and PoC.",
            Icon = "🛡️",
            Columns = 2,
            SubmitButtonText = "Submit Responsible",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "vulnerability_type",
                    Label = "Vulnerability Classification",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Cross-Origin Token Leakage",
                    Options = new List<string> { "SQL / LINQ Injection", "Cross-Origin Token Leakage", "Authentication Bypass", "Remote Code Execution" }
                },
                new()
                {
                    Name = "cvss_score",
                    Label = "Estimated CVSS Severity Score (1-10)",
                    Type = FormFieldType.Slider,
                    Required = true,
                    DefaultValue = 7.5,
                    Options = null
                },
                new()
                {
                    Name = "poc_details",
                    Label = "Proof of Concept Steps",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "CORS origin validation bypass via null origin header.",
                    Options = null
                },
            }
        });

        // Form 70: Executive Account Escalation Form
        _allForms.Add(new FormSchema
        {
            Id = "escalation_request",
            Title = "Executive Account Escalation Form",
            Category = "Customer Support, IT & Helpdesk",
            Description = "Trigger senior leadership intervention for high-value client retention.",
            Icon = "📈",
            Columns = 2,
            SubmitButtonText = "Submit Executive",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "client_tier",
                    Label = "Client Tier",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Tier 1 Strategic ($500k+ ARR)",
                    Options = new List<string> { "Tier 1 Strategic ($500k+ ARR)", "Tier 2 Mid-Market", "Tier 3 Growth" }
                },
                new()
                {
                    Name = "executive_requested",
                    Label = "Executive Requested for Meeting",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Chief Customer Officer",
                    Options = null
                },
                new()
                {
                    Name = "urgent_summary",
                    Label = "Urgent Account Blocker",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Client requesting commitment for on-premise air-gapped deployment by Q4.",
                    Options = null
                },
            }
        });

        // Form 71: Contact Sales & General Inquiry
        _allForms.Add(new FormSchema
        {
            Id = "contact_us",
            Title = "Contact Sales & General Inquiry",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Standard multi-channel lead intake with phone, company size, and budget.",
            Icon = "📬",
            Columns = 2,
            SubmitButtonText = "Submit Contact",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "full_name",
                    Label = "Your Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Marcus Vance",
                    Options = null
                },
                new()
                {
                    Name = "business_email",
                    Label = "Work Email",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "m.vance@enterprise.com",
                    Options = null
                },
                new()
                {
                    Name = "company_size",
                    Label = "Company Employee Size",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "250 - 1,000 employees",
                    Options = new List<string> { "1 - 50 employees", "51 - 250 employees", "250 - 1,000 employees", "1,000+ employees" }
                },
                new()
                {
                    Name = "message",
                    Label = "How can our team help you?",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Evaluating Blazor Tailwind components for our upcoming enterprise dashboard.",
                    Options = null
                },
            }
        });

        // Form 72: Schedule a Live Product Architecture Demo
        _allForms.Add(new FormSchema
        {
            Id = "demo_request",
            Title = "Schedule a Live Product Architecture Demo",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Book a 30-minute tailored walkthrough with a solution architect.",
            Icon = "📅",
            Columns = 2,
            SubmitButtonText = "Submit Schedule",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "preferred_date",
                    Label = "Preferred Demo Date",
                    Type = FormFieldType.Date,
                    Required = true,
                    DefaultValue = "2026-11-05",
                    Options = null
                },
                new()
                {
                    Name = "primary_interest",
                    Label = "Primary Technology Focus",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Enterprise WebAssembly Architecture",
                    Options = new List<string> { "Enterprise WebAssembly Architecture", "Financial Charts & Treemaps", "Turnkey Task-Purpose Forms", "Multi-Dock IDE Manager" }
                },
                new()
                {
                    Name = "team_size",
                    Label = "Number of Engineers Attending",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 6,
                    Options = null
                },
            }
        });

        // Form 73: Engineering Architecture Newsletter
        _allForms.Add(new FormSchema
        {
            Id = "newsletter_subscribe",
            Title = "Engineering Architecture Newsletter",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Weekly technical dispatches on WebAssembly, C# 13, and Tailwind CSS.",
            Icon = "📰",
            Columns = 2,
            SubmitButtonText = "Submit Engineering",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "subscriber_email",
                    Label = "Email Address",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "engineer@techcompany.com",
                    Options = null
                },
                new()
                {
                    Name = "weekly_digest_only",
                    Label = "Send Weekly Digest Only (No Marketing)",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 74: Technology & Solution Partner Application
        _allForms.Add(new FormSchema
        {
            Id = "partner_application",
            Title = "Technology & Solution Partner Application",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Join enterprise certified reseller, system integrator, or ISV network.",
            Icon = "🤝",
            Columns = 2,
            SubmitButtonText = "Submit Technology",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "partner_company",
                    Label = "Partner Organization Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Nordic Cloud Consulting AS",
                    Options = null
                },
                new()
                {
                    Name = "partner_type",
                    Label = "Partnership Track",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "System Integrator / Consultancy",
                    Options = new List<string> { "System Integrator / Consultancy", "Independent Software Vendor (ISV)", "Global Reseller" }
                },
                new()
                {
                    Name = "annual_client_base",
                    Label = "Annual Active Enterprise Clients",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 45,
                    Options = null
                },
            }
        });

        // Form 75: 30-Day Enterprise Sandbox Activation
        _allForms.Add(new FormSchema
        {
            Id = "free_trial_activation",
            Title = "30-Day Enterprise Sandbox Activation",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Spin up instantaneous private cloud sandbox tenant with full features.",
            Icon = "⚡",
            Columns = 2,
            SubmitButtonText = "Submit 30-Day",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "tenant_slug",
                    Label = "Requested Tenant Subdomain (slug)",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "acme-test",
                    Options = null
                },
                new()
                {
                    Name = "cloud_region",
                    Label = "Deployment Cloud Region",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "US-East (Virginia)",
                    Options = new List<string> { "US-East (Virginia)", "US-West (Oregon)", "EU-Central (Frankfurt)", "AP-Southeast (Singapore)" }
                },
                new()
                {
                    Name = "seed_sample_data",
                    Label = "Pre-load 50,000 Record Financial Sample Data",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 76: Customer Case Study & Logo Usage Consent
        _allForms.Add(new FormSchema
        {
            Id = "case_study_consent",
            Title = "Customer Case Study & Logo Usage Consent",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Legal marketing release for quoting executive testimonials and logo displays.",
            Icon = "📄",
            Columns = 2,
            SubmitButtonText = "Submit Customer",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "authorized_signatory",
                    Label = "Authorized Corporate Representative",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Sarah Connor, CMO",
                    Options = null
                },
                new()
                {
                    Name = "approved_quote",
                    Label = "Approved Public Testimonial Quote",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "BlazorTwUI accelerated our internal portal delivery by over 6 months.",
                    Options = null
                },
                new()
                {
                    Name = "grant_logo_rights",
                    Label = "Grant Permission to Display Brand Logo on Site",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 77: Customer Referral & Bounty Submission
        _allForms.Add(new FormSchema
        {
            Id = "referral_submission",
            Title = "Customer Referral & Bounty Submission",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Refer peer enterprises and earn account credit or charity donation.",
            Icon = "🎁",
            Columns = 2,
            SubmitButtonText = "Submit Customer",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "referred_name",
                    Label = "Referred Colleague / Decision Maker",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "David Kim, VP Engineering",
                    Options = null
                },
                new()
                {
                    Name = "referred_company",
                    Label = "Referred Organization",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Pacific FinTech Labs",
                    Options = null
                },
                new()
                {
                    Name = "reward_choice",
                    Label = "Reward Preference",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "$1,000 Software Credit",
                    Options = new List<string> { "$1,000 Software Credit", "$1,000 Charity Donation", "VIP Conference Pass" }
                },
            }
        });

        // Form 78: Request for Proposal (RFP) Ingestion
        _allForms.Add(new FormSchema
        {
            Id = "rfp_submission",
            Title = "Request for Proposal (RFP) Ingestion",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Upload tender documents, deadline requirements, and security criteria.",
            Icon = "📑",
            Columns = 2,
            SubmitButtonText = "Submit Request",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "rfp_title",
                    Label = "RFP Project Title",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "State Transit Authority Fleet Analytics Portal",
                    Options = null
                },
                new()
                {
                    Name = "submission_deadline",
                    Label = "Formal Proposal Deadline",
                    Type = FormFieldType.Date,
                    Required = true,
                    DefaultValue = "2026-12-01",
                    Options = null
                },
                new()
                {
                    Name = "budget_ceiling",
                    Label = "Maximum Budget Ceiling ($ USD)",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 250000,
                    Options = null
                },
            }
        });

        // Form 79: Tradeshow Booth Badge Lead Capture
        _allForms.Add(new FormSchema
        {
            Id = "event_lead_scanner",
            Title = "Tradeshow Booth Badge Lead Capture",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Fast-intake badge scanner form for conference sales reps.",
            Icon = "🎯",
            Columns = 2,
            SubmitButtonText = "Submit Tradeshow",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "attendee_badge_id",
                    Label = "Scanned Badge QR Code",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "CONF-2026-9812",
                    Options = null
                },
                new()
                {
                    Name = "buying_intent",
                    Label = "Procurement Timeline / Intent",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Within 3 Months",
                    Options = new List<string> { "Immediate (Under 30 Days)", "Within 3 Months", "6-12 Months", "Information Gathering" }
                },
                new()
                {
                    Name = "notes",
                    Label = "Booth Conversation Summary",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Requested immediate pricing for 100 developer licenses.",
                    Options = null
                },
            }
        });

        // Form 80: Joint Webinar & Co-Marketing Proposal
        _allForms.Add(new FormSchema
        {
            Id = "co_marketing_request",
            Title = "Joint Webinar & Co-Marketing Proposal",
            Category = "Marketing, Sales & CRM Intake",
            Description = "Propose co-branded whitepapers, live podcasts, or developer benchmarks.",
            Icon = "🎙️",
            Columns = 2,
            SubmitButtonText = "Submit Joint",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "co_marketer_org",
                    Label = "Partner Organization",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Tailwind Labs Community",
                    Options = null
                },
                new()
                {
                    Name = "collaboration_type",
                    Label = "Campaign Format",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Joint High-Performance Benchmark Paper",
                    Options = new List<string> { "Joint High-Performance Benchmark Paper", "Virtual Hackathon", "Live Twitch Code Stream" }
                },
                new()
                {
                    Name = "target_audience_size",
                    Label = "Expected Combined Reach",
                    Type = FormFieldType.Number,
                    Required = true,
                    DefaultValue = 50000,
                    Options = null
                },
            }
        });

        // Form 81: Net Promoter Score (NPS) Survey
        _allForms.Add(new FormSchema
        {
            Id = "nps_survey",
            Title = "Net Promoter Score (NPS) Survey",
            Category = "Feedback, NPS & Surveys",
            Description = "Industry standard 0-10 customer loyalty and advocacy measurement.",
            Icon = "📊",
            Columns = 2,
            SubmitButtonText = "Submit Net",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "nps_score",
                    Label = "How likely are you to recommend us to a colleague? (0-10)",
                    Type = FormFieldType.Slider,
                    Required = true,
                    DefaultValue = 10,
                    Options = null
                },
                new()
                {
                    Name = "primary_reason",
                    Label = "What is the primary reason for your score?",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Fastest client-side rendering we have ever benchmarked.",
                    Options = null
                },
                new()
                {
                    Name = "can_contact",
                    Label = "May our product team contact you for follow-up?",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 82: Customer Satisfaction (CSAT) Rating
        _allForms.Add(new FormSchema
        {
            Id = "csat_satisfaction",
            Title = "Customer Satisfaction (CSAT) Rating",
            Category = "Feedback, NPS & Surveys",
            Description = "Transaction-level satisfaction measurement following support or checkout.",
            Icon = "😊",
            Columns = 2,
            SubmitButtonText = "Submit Customer",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "satisfaction_rating",
                    Label = "How satisfied were you with your support experience?",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 5,
                    Options = null
                },
                new()
                {
                    Name = "agent_helpfulness",
                    Label = "The support specialist resolved my issue efficiently",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Strongly Agree",
                    Options = new List<string> { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" }
                },
            }
        });

        // Form 83: Feature Usability & Ergonomics Feedback
        _allForms.Add(new FormSchema
        {
            Id = "product_feature_feedback",
            Title = "Feature Usability & Ergonomics Feedback",
            Category = "Feedback, NPS & Surveys",
            Description = "Targeted questionnaire on newly released components or workflows.",
            Icon = "💡",
            Columns = 2,
            SubmitButtonText = "Submit Feature",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "component_tested",
                    Label = "Component Evaluated",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "TwMarkdownEditor",
                    Options = new List<string> { "TwMarkdownEditor", "TwFinancialChart", "TwTreemap", "TwPivotGrid", "TwDockManager", "TwSpreadsheet" }
                },
                new()
                {
                    Name = "ease_of_integration",
                    Label = "Ease of Integration Rating (1-5)",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 5,
                    Options = null
                },
                new()
                {
                    Name = "missing_capabilities",
                    Label = "What features would make this a 10/10 for you?",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Export directly to styled PDF.",
                    Options = null
                },
            }
        });

        // Form 84: Website Navigation & Usability Survey
        _allForms.Add(new FormSchema
        {
            Id = "website_usability_survey",
            Title = "Website Navigation & Usability Survey",
            Category = "Feedback, NPS & Surveys",
            Description = "Identify friction points, broken links, or documentation gaps.",
            Icon = "🌐",
            Columns = 2,
            SubmitButtonText = "Submit Website",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "found_needed_info",
                    Label = "Were you able to find the code samples you needed?",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "navigation_clarity",
                    Label = "How clear was the documentation search?",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Very Intuitive",
                    Options = new List<string> { "Very Intuitive", "Acceptable", "Confusing", "Could Not Find" }
                },
            }
        });

        // Form 85: Industry Trends & Tech Stack Questionnaire
        _allForms.Add(new FormSchema
        {
            Id = "market_research",
            Title = "Industry Trends & Tech Stack Questionnaire",
            Category = "Feedback, NPS & Surveys",
            Description = "Annual developer survey tracking frontend paradigms and backend languages.",
            Icon = "🔬",
            Columns = 2,
            SubmitButtonText = "Submit Industry",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "primary_framework",
                    Label = "Primary Web UI Paradigm",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Blazor WebAssembly (.NET 10+)",
                    Options = new List<string> { "Blazor WebAssembly (.NET 10+)", "React / Next.js", "Vue / Nuxt", "Angular", "Svelte" }
                },
                new()
                {
                    Name = "css_framework",
                    Label = "Styling Approach",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Tailwind CSS v3.4+",
                    Options = new List<string> { "Tailwind CSS v3.4+", "CSS Modules", "Vanilla CSS", "Styled Components" }
                },
                new()
                {
                    Name = "use_ai_tools",
                    Label = "Use AI-Assisted Pair Programming Daily",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 86: Subscription Cancellation Exit Survey
        _allForms.Add(new FormSchema
        {
            Id = "churn_cancellation",
            Title = "Subscription Cancellation Exit Survey",
            Category = "Feedback, NPS & Surveys",
            Description = "Diagnose why subscribers leave and identify retention counter-offers.",
            Icon = "💔",
            Columns = 2,
            SubmitButtonText = "Submit Subscription",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "cancel_reason",
                    Label = "Main Reason for Cancellation",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Project Completed / Ended",
                    Options = new List<string> { "Project Completed / Ended", "Cost / Budget Constraints", "Missing Required Features", "Switching to In-House" }
                },
                new()
                {
                    Name = "save_offer_interest",
                    Label = "Would a 50% discount for 3 months change your mind?",
                    Type = FormFieldType.Select,
                    Required = false,
                    DefaultValue = "No, please cancel",
                    Options = new List<string> { "Yes, accept 50% discount", "No, please cancel" }
                },
            }
        });

        // Form 87: Weekly Employee Pulse & Morale Check-In
        _allForms.Add(new FormSchema
        {
            Id = "employee_pulse",
            Title = "Weekly Employee Pulse & Morale Check-In",
            Category = "Feedback, NPS & Surveys",
            Description = "Anonymous micro-survey monitoring team burnout and workload balance.",
            Icon = "💓",
            Columns = 2,
            SubmitButtonText = "Submit Weekly",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "morale_score",
                    Label = "How energizing was your work week? (1-5)",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 4,
                    Options = null
                },
                new()
                {
                    Name = "workload_balance",
                    Label = "Workload is Realistic & Sustainable",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Agree",
                    Options = new List<string> { "Strongly Agree", "Agree", "Neutral", "Disagree", "Overwhelmed" }
                },
                new()
                {
                    Name = "shoutout",
                    Label = "Anonymous Shoutout to an Inspiring Teammate",
                    Type = FormFieldType.Text,
                    Required = false,
                    DefaultValue = "Kudos to the infrastructure team for zero-lag deployments!",
                    Options = null
                },
            }
        });

        // Form 88: Technical Workshop & Training Evaluation
        _allForms.Add(new FormSchema
        {
            Id = "training_evaluation",
            Title = "Technical Workshop & Training Evaluation",
            Category = "Feedback, NPS & Surveys",
            Description = "Evaluate curriculum pacing, hands-on lab quality, and instructor clarity.",
            Icon = "🎓",
            Columns = 2,
            SubmitButtonText = "Submit Technical",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "instructor_rating",
                    Label = "Instructor Clarity & Domain Expertise",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 5,
                    Options = null
                },
                new()
                {
                    Name = "lab_quality",
                    Label = "Hands-on Code Labs were Practical & Realistic",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Excellent",
                    Options = new List<string> { "Excellent", "Good", "Needs Improvement" }
                },
                new()
                {
                    Name = "key_takeaway",
                    Label = "Biggest Technical Skill Acquired",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Authoring zero-JS inline SVG speedometer gauges.",
                    Options = null
                },
            }
        });

        // Form 89: Corporate Wellness & Gym Membership Payout
        _allForms.Add(new FormSchema
        {
            Id = "health_wellness_intake",
            Title = "Corporate Wellness & Gym Membership Payout",
            Category = "Feedback, NPS & Surveys",
            Description = "Annual health reimbursement form for gym dues or meditation apps.",
            Icon = "🏃",
            Columns = 2,
            SubmitButtonText = "Submit Corporate",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "program_name",
                    Label = "Wellness Facility / App Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Equinox Fitness & Headspace",
                    Options = null
                },
                new()
                {
                    Name = "annual_cost",
                    Label = "Total Annual Cost ($ USD)",
                    Type = FormFieldType.Currency,
                    Required = true,
                    DefaultValue = 1200.0,
                    Options = null
                },
                new()
                {
                    Name = "reimbursement_receipt",
                    Label = "Proof of Membership Payment Attached",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 90: Course & Academic Program Feedback
        _allForms.Add(new FormSchema
        {
            Id = "student_course_feedback",
            Title = "Course & Academic Program Feedback",
            Category = "Feedback, NPS & Surveys",
            Description = "University or bootcamp feedback on curriculum and project rigor.",
            Icon = "📚",
            Columns = 2,
            SubmitButtonText = "Submit Course",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "course_code",
                    Label = "Course Number & Title",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "CS-492: Modern Distributed Systems",
                    Options = null
                },
                new()
                {
                    Name = "rigor_rating",
                    Label = "Curriculum Challenge Rating (1-5)",
                    Type = FormFieldType.Rating,
                    Required = true,
                    DefaultValue = 5,
                    Options = null
                },
                new()
                {
                    Name = "recommended_to_peers",
                    Label = "Would recommend this course to peers",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 91: GDPR / CCPA Cookie Consent Manager
        _allForms.Add(new FormSchema
        {
            Id = "cookie_consent",
            Title = "GDPR / CCPA Cookie Consent Manager",
            Category = "Legal, Compliance & Privacy",
            Description = "Configure granular telemetry, functional, and marketing cookie switches.",
            Icon = "🍪",
            Columns = 2,
            SubmitButtonText = "Submit GDPR",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "essential_locked",
                    Label = "Strictly Necessary Cookies (Always Active)",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "functional_cookies",
                    Label = "Functional Personalization Cookies",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "telemetry_cookies",
                    Label = "Anonymized Analytics & Telemetry",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = false,
                    Options = null
                },
                new()
                {
                    Name = "marketing_cookies",
                    Label = "Targeted Marketing & Remarketing",
                    Type = FormFieldType.Toggle,
                    Required = false,
                    DefaultValue = false,
                    Options = null
                },
            }
        });

        // Form 92: Data Subject Access & Erasure Request (DSR)
        _allForms.Add(new FormSchema
        {
            Id = "dsr_privacy_portal",
            Title = "Data Subject Access & Erasure Request (DSR)",
            Category = "Legal, Compliance & Privacy",
            Description = "Self-service GDPR Art. 17 right to be forgotten and data export.",
            Icon = "🛡️",
            Columns = 2,
            SubmitButtonText = "Submit Data",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "requester_email",
                    Label = "Subject Account Email",
                    Type = FormFieldType.Email,
                    Required = true,
                    DefaultValue = "privacy.user@domain.eu",
                    Options = null
                },
                new()
                {
                    Name = "request_type",
                    Label = "DSR Request Category",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Export All My Data (JSON Archive)",
                    Options = new List<string> { "Export All My Data (JSON Archive)", "Rectify Personal Records", "Permanently Erase All Account Data" }
                },
                new()
                {
                    Name = "affirms_identity",
                    Label = "I swear under penalty of perjury that I own this account",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 93: Executive Terms of Service Acceptance
        _allForms.Add(new FormSchema
        {
            Id = "terms_of_service",
            Title = "Executive Terms of Service Acceptance",
            Category = "Legal, Compliance & Privacy",
            Description = "Formal contract execution with company binding and SHA-256 stamp.",
            Icon = "📜",
            Columns = 2,
            SubmitButtonText = "Submit Executive",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "signatory_name",
                    Label = "Authorized Signatory Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Marcus Vance",
                    Options = null
                },
                new()
                {
                    Name = "organization",
                    Label = "Company Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Global Frontier Systems",
                    Options = null
                },
                new()
                {
                    Name = "read_all_clauses",
                    Label = "I have reviewed all 12 contractual sections",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 94: Code of Conduct & Ethics Affidavit
        _allForms.Add(new FormSchema
        {
            Id = "ethics_pledge",
            Title = "Code of Conduct & Ethics Affidavit",
            Category = "Legal, Compliance & Privacy",
            Description = "Sworn declaration upholding compliance, integrity, and anti-corruption.",
            Icon = "✍️",
            Columns = 2,
            SubmitButtonText = "Submit Code",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "signatory_title",
                    Label = "Corporate Officer Title",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Chief Information Security Officer",
                    Options = null
                },
                new()
                {
                    Name = "uphold_code",
                    Label = "I swear adherence to the 3 ethical principles",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 95: HIPAA Business Associate Agreement (BAA)
        _allForms.Add(new FormSchema
        {
            Id = "hipaa_acknowledgment",
            Title = "HIPAA Business Associate Agreement (BAA)",
            Category = "Legal, Compliance & Privacy",
            Description = "Healthcare data protection protocols for processing Protected Health Information (PHI).",
            Icon = "🏥",
            Columns = 2,
            SubmitButtonText = "Submit HIPAA",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "covered_entity",
                    Label = "Covered Entity Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Providence Health Systems",
                    Options = null
                },
                new()
                {
                    Name = "phi_handling_certified",
                    Label = "All staff trained in HIPAA Security Rule compliance",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
                new()
                {
                    Name = "encryption_verified",
                    Label = "EHR databases encrypted with dedicated HSM keys",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 96: EAR / ITAR Export Control Compliance
        _allForms.Add(new FormSchema
        {
            Id = "export_control_disclosure",
            Title = "EAR / ITAR Export Control Compliance",
            Category = "Legal, Compliance & Privacy",
            Description = "Verify non-export of dual-use encryption algorithms to sanctioned regimes.",
            Icon = "🌐",
            Columns = 2,
            SubmitButtonText = "Submit EAR",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "destination_jurisdiction",
                    Label = "Destination Country of End Deployment",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "United Kingdom",
                    Options = null
                },
                new()
                {
                    Name = "is_dual_use",
                    Label = "Contains Military / Dual-Use Cryptographic Modules",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = false,
                    Options = null
                },
                new()
                {
                    Name = "certifies_non_diversion",
                    Label = "Certifies assets will not be diverted to sanctioned entities",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 97: Proprietary Information & Inventions Agreement (PIIA)
        _allForms.Add(new FormSchema
        {
            Id = "ip_assignment",
            Title = "Proprietary Information & Inventions Agreement (PIIA)",
            Category = "Legal, Compliance & Privacy",
            Description = "Assign patents, algorithms, and code created during tenure to enterprise.",
            Icon = "💡",
            Columns = 2,
            SubmitButtonText = "Submit Proprietary",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "inventor_name",
                    Label = "Inventor / Developer Name",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "Sophia Chen",
                    Options = null
                },
                new()
                {
                    Name = "assigns_all_ip",
                    Label = "Irrevocably assigns all inventions, patents, and source code",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 98: Annual Conflict of Interest Disclosure
        _allForms.Add(new FormSchema
        {
            Id = "conflict_of_interest",
            Title = "Annual Conflict of Interest Disclosure",
            Category = "Legal, Compliance & Privacy",
            Description = "Disclose secondary employment, board directorships, or family supplier ties.",
            Icon = "🔍",
            Columns = 2,
            SubmitButtonText = "Submit Annual",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "has_conflict",
                    Label = "Do you hold any directorship or outside financial interest?",
                    Type = FormFieldType.Toggle,
                    Required = true,
                    DefaultValue = false,
                    Options = null
                },
                new()
                {
                    Name = "disclosure_statement",
                    Label = "If yes, explain relationship and mitigating safeguards",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "None to report. Dedicated 100% full-time.",
                    Options = null
                },
            }
        });

        // Form 99: Foreign Corrupt Practices Act (FCPA) Certificate
        _allForms.Add(new FormSchema
        {
            Id = "anti_bribery_compliance",
            Title = "Foreign Corrupt Practices Act (FCPA) Certificate",
            Category = "Legal, Compliance & Privacy",
            Description = "Affirm strict prohibition against bribes, kickbacks, or facilitation payments.",
            Icon = "🤝",
            Columns = 2,
            SubmitButtonText = "Submit Foreign",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "officer_name",
                    Label = "Affirming Executive",
                    Type = FormFieldType.Text,
                    Required = true,
                    DefaultValue = "David Kim, VP Engineering",
                    Options = null
                },
                new()
                {
                    Name = "zero_kickbacks_affirmed",
                    Label = "Affirms strict zero-tolerance for bribes or foreign gifts",
                    Type = FormFieldType.Checkbox,
                    Required = true,
                    DefaultValue = true,
                    Options = null
                },
            }
        });

        // Form 100: Anonymous Whistleblower Hotline Intake
        _allForms.Add(new FormSchema
        {
            Id = "whistleblower_report",
            Title = "Anonymous Whistleblower Hotline Intake",
            Category = "Legal, Compliance & Privacy",
            Description = "Secure, encrypted channel for confidential reporting of fraud or waste.",
            Icon = "🔒",
            Columns = 2,
            SubmitButtonText = "Submit Anonymous",
            Fields = new List<FormFieldDefinition>
            {
                new()
                {
                    Name = "allegation_category",
                    Label = "Allegation Nature",
                    Type = FormFieldType.Select,
                    Required = true,
                    DefaultValue = "Accounting / Financial Fraud",
                    Options = new List<string> { "Accounting / Financial Fraud", "Securities Law Violation", "Environmental / Safety Coverup", "Bribery" }
                },
                new()
                {
                    Name = "detailed_allegation",
                    Label = "Facts, Dates, Locations, and Involved Parties",
                    Type = FormFieldType.Textarea,
                    Required = true,
                    DefaultValue = "Detailed report regarding undisclosed offshore holding entities.",
                    Options = null
                },
                new()
                {
                    Name = "evidence_description",
                    Label = "Description of Physical Documents / Records Held",
                    Type = FormFieldType.Textarea,
                    Required = false,
                    DefaultValue = "Financial ledger emails from March 2026.",
                    Options = null
                },
            }
        });

    }

    public IReadOnlyList<FormSchema> GetAllForms() => _allForms;

    public FormSchema? GetFormById(string id) =>
        _allForms.FirstOrDefault(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<FormSchema> GetFormsByCategory(string category) =>
        _allForms.Where(f => f.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

    public IReadOnlyList<string> GetCategories() =>
        _allForms.Select(f => f.Category).Distinct().ToList();

    public int TotalFormCount => _allForms.Count;
}
