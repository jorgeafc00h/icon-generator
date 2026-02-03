# Unlimited Users Configuration

This document explains how to configure unlimited icon generation for specific users.

## Overview

The icon generator supports granting unlimited icon generation to specific users based on their email address. This is useful for:
- Admin accounts
- Testing accounts
- VIP users
- Internal team members

## How It Works

1. **Email-based whitelist**: Users are identified by their Gmail email address
2. **No credit deduction**: Unlimited users can generate icons without consuming credits
3. **No transaction logs**: Usage is not recorded in transactions for unlimited users
4. **Credits displayed as "Unlimited"**: Frontend shows unlimited status

## Configuration

### Adding Unlimited Users

Edit the `appsettings.json` file in the `api` folder:

```json
{
  "AppSettings": {
    "UnlimitedUsers": [
      "jorgeafc00h@gmail.com",
      "another-admin@gmail.com",
      "team-member@gmail.com"
    ]
  }
}
```

### For Production/Deployment

If you're using Azure Functions or another cloud provider, set the configuration in your application settings:

**Azure Functions**:
1. Go to your Function App in Azure Portal
2. Navigate to Configuration → Application Settings
3. Add a new setting:
   - **Name**: `AppSettings__UnlimitedUsers__0`
   - **Value**: `jorgeafc00h@gmail.com`
4. For additional users, increment the number:
   - `AppSettings__UnlimitedUsers__1` = `second-email@gmail.com`
   - `AppSettings__UnlimitedUsers__2` = `third-email@gmail.com`

**Environment Variables**:
```bash
export AppSettings__UnlimitedUsers__0="jorgeafc00h@gmail.com"
export AppSettings__UnlimitedUsers__1="another-admin@gmail.com"
```

## Currently Configured Unlimited Users

As of now, the following user has unlimited access:
- **jorgeafc00h@gmail.com** (Primary admin account)

## Security Notes

- Unlimited users are matched **case-insensitive** by email
- Users must authenticate with Google using the exact email in the list
- The unlimited user list is only accessible from the backend (not exposed to frontend)
- Changes to the unlimited users list require an application restart to take effect

## Frontend Display

For unlimited users:
- Credit count displays as "∞ Unlimited" or similar
- "Buy Credits" button is hidden or disabled
- No credit warnings are shown

## Logs

When an unlimited user generates an icon, you'll see this in the logs:
```
Unlimited user jorgeafc00h@gmail.com generating icon without credit deduction
```
