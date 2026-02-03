# Google OAuth Fix - Complete Guide

## ✅ What Was Fixed

### 1. **OAuth Flow Changed to ID Token (Implicit Flow)**
- **Before**: Using authorization code flow (requires backend secret)
- **After**: Using ID token implicit flow (works with client ID only)
- **Benefit**: Simpler, works without client secret

### 2. **User State Management at App Level**
- User data is now loaded once and shared across all components
- Profile picture and credits persist across navigation
- No need to re-authenticate when navigating between pages

### 3. **Profile Picture in Header**
- Beautiful circular profile picture in top-right corner
- Falls back to initial avatar if no picture
- Shows user name next to picture
- Clickable to go to profile page

### 4. **Persistent Authentication**
- Login state persists across page refreshes
- Tokens stored in localStorage
- Auto-loads user data on app mount

---

## 🔧 Google Cloud Console Configuration

### Required Redirect URI:

**Add this EXACT redirect URI to Google Cloud Console:**

```
http://localhost:5173/profile
```

### Steps to Configure:

1. **Go to Google Cloud Console**:
   - Visit: https://console.cloud.google.com/

2. **Select Your Project**:
   - Project: "icon-gen-486320"

3. **Navigate to Credentials**:
   - Click "APIs & Services" → "Credentials"

4. **Edit OAuth 2.0 Client**:
   - Find your client ID: `421206201443-rsvce5hacsmc00fol75gc865uhelau1e`
   - Click the edit (pencil) icon

5. **Update Authorized Redirect URIs**:
   - Add: `http://localhost:5173/profile`
   - Remove any old URIs that include `/auth/callback`

6. **Update Authorized JavaScript Origins** (if needed):
   - Should include: `http://localhost:5173`

7. **Save Changes**

### For Production:

Also add:
```
https://mango-bay-068c07f0f.6.azurestaticapps.net/profile
```

---

## 🔄 How the New OAuth Flow Works

### Detailed Flow:

1. **User Clicks "Continue with Google"**
   ```
   User on: http://localhost:5173/profile
   ```

2. **Redirect to Google**
   ```
   https://accounts.google.com/o/oauth2/v2/auth?
     client_id=YOUR_CLIENT_ID&
     redirect_uri=http://localhost:5173/profile&
     response_type=id_token&
     scope=openid email profile&
     nonce=RANDOM_NONCE
   ```

3. **User Authenticates**
   - Google login page
   - User selects account
   - Grants permissions

4. **Google Redirects Back**
   ```
   http://localhost:5173/profile#id_token=LONG_JWT_TOKEN_HERE&...
   ```
   Note: ID token is in the **URL hash** (after #), not query params

5. **Frontend Extracts ID Token**
   - GoogleSignIn component reads `window.location.hash`
   - Extracts `id_token` parameter
   - Sends to backend: `POST /api/auth/google`

6. **Backend Verifies & Creates User**
   - Decodes and validates ID token
   - Creates new user or updates existing
   - Returns access token and user data

7. **Frontend Stores Data**
   - Saves to localStorage:
     - accessToken
     - userId
     - userEmail
     - userName
     - userPicture
   - Cleans URL (removes hash)
   - Reloads page

8. **User is Logged In**
   - App.tsx loads user data
   - Header shows profile picture
   - Credits displayed
   - Ready to use the app

---

## 🎨 What You'll See After Login

### Header (Top Right):

```
┌─────────────────────────────────────┐
│  [Credits: 2]  [👤 Profile Pic] John │
└─────────────────────────────────────┘
```

**Features**:
- Credits count in a badge
- Circular profile picture with blue ring
- User's name next to picture
- Click picture to go to profile
- "Buy Credits" button (if not unlimited)

### Profile Page:

```
┌────────────────────────────────────┐
│  👤 John Doe             💰 2       │
│  john@example.com    [Buy Credits] │
│  Member since Feb 2026              │
├────────────────────────────────────┤
│  ✨ 0        📈 0       📅 0        │
│  Generated   Purchased  Spent      │
└────────────────────────────────────┘
```

---

## 🚀 How to Test

### 1. Clear Everything First:

```javascript
// Open browser console (F12)
localStorage.clear()
// Refresh page
location.reload()
```

### 2. Start Servers:

```bash
# Terminal 1 - Backend
cd api
func start

# Terminal 2 - Frontend
cd web
npm run dev
```

### 3. Test Login:

1. Open: `http://localhost:5173`
2. Click "Profile" in navigation
3. Click "Continue with Google"
4. Browser redirects to Google
5. Select your Google account
6. Google redirects back to profile
7. **Should see**: Profile picture in header, credits, user name

### 4. Test Persistence:

1. After login, navigate to "Generator"
2. Check header - should still show profile picture
3. Navigate to "Pricing"
4. Check header - still logged in
5. Refresh the page
6. **Should still be logged in** (no re-authentication needed)

### 5. Test Profile Picture:

1. Go to Profile page
2. Should see your Google profile picture in:
   - Header (top-right)
   - Profile card (large version)
   - All properly styled with rings and shadows

---

## 🐛 Troubleshooting

### Issue: "redirect_uri_mismatch" Error

**Cause**: Redirect URI in Google Console doesn't match exactly

**Solution**:
1. Check error message for the exact redirect URI used
2. Copy it exactly
3. Add to Google Console
4. Must include `http://` and exact path `/profile`

### Issue: Still Asking to Login After Redirect

**Cause**: ID token not being extracted from URL hash

**Check**:
1. Open browser console after redirect
2. Should see logs: "Handling OAuth callback..."
3. Check localStorage for `accessToken` and `userId`

**Solution**:
- Make sure URL has `#id_token=...` in the hash
- Clear browser cache and try again
- Check browser console for errors

### Issue: Profile Picture Not Showing

**Cause**: User data not loaded at app level

**Check**:
1. Open console
2. Check for "Error loading user data"
3. Verify backend is running
4. Check API endpoint in .env

**Solution**:
- Ensure backend is running on port 7071
- Check `VITE_API_ENDPOINT` in .env
- Look at network tab for failed requests

### Issue: Credits Not Showing

**Cause**: User data API call failing

**Check**:
1. Open Network tab in DevTools
2. Look for call to `/users/{userId}`
3. Check response status

**Solution**:
- Ensure you have valid accessToken
- Check backend logs for errors
- Try re-authenticating

---

## 📁 Files Modified

### Frontend (4 files):

1. **`web/src/App.tsx`**
   - Added user state management
   - Loads user data on mount
   - Passes user to Header and Profile
   - Handles user updates

2. **`web/src/components/Profile/GoogleSignIn.tsx`**
   - Changed to ID token implicit flow
   - Extracts token from URL hash
   - Stores user data in localStorage
   - Improved error handling

3. **`web/src/components/Profile/Profile.tsx`**
   - Added onUserUpdate callback
   - Notifies app when user logs in/out
   - Improved sign-in success handling

4. **`web/src/components/Layout/Header.tsx`**
   - Added profile picture display
   - Shows circular avatar
   - User name next to picture
   - "Sign In" button when not logged in

---

## 🎯 Key Improvements

### Before:
- ❌ Login state didn't persist
- ❌ Had to re-login on every page
- ❌ No profile picture
- ❌ Only showed "User" icon
- ❌ Confusing auth flow

### After:
- ✅ Login persists across pages
- ✅ No re-authentication needed
- ✅ Profile picture in header
- ✅ User name displayed
- ✅ Clear, simple auth flow
- ✅ Better UX overall

---

## 🔐 Security Notes

**ID Token Implicit Flow**:
- ✅ No client secret needed
- ✅ ID token is verified by backend
- ✅ Tokens stored in localStorage
- ✅ Access token generated by backend

**Best Practices**:
- ID token is short-lived (Google expires them)
- Backend validates all tokens
- Access token is custom (can add expiration)
- In production, consider:
  - Using authorization code flow with PKCE
  - Storing tokens in httpOnly cookies
  - Adding token refresh mechanism

---

## ✅ Testing Checklist

Run through this checklist:

- [ ] Start backend and frontend servers
- [ ] Clear localStorage
- [ ] Go to Profile page
- [ ] See beautiful login UI
- [ ] Click "Continue with Google"
- [ ] Redirects to Google
- [ ] Select Google account
- [ ] Redirects back to profile
- [ ] See profile picture in header
- [ ] See user name in header
- [ ] See credits count
- [ ] Navigate to Generator - still logged in
- [ ] Navigate to Pricing - still logged in
- [ ] Refresh page - still logged in
- [ ] Click profile picture - goes to profile
- [ ] Profile shows all user data
- [ ] Click "Sign Out" - logs out properly
- [ ] Header shows "Sign In" button

---

## 🎉 Summary

**What's Fixed**:
- ✅ OAuth flow now works with redirect (no popup)
- ✅ Login state persists across navigation
- ✅ Profile picture shows in header
- ✅ User data loaded at app level
- ✅ No more "asking to login again"
- ✅ Beautiful, user-friendly UI

**How to Test**:
1. Configure redirect URI in Google Console
2. Clear localStorage
3. Sign in with Google
4. Should see profile picture and stay logged in

**Files Changed**: 4 frontend files
**Backend Changes**: None needed (uses existing endpoint)

---

**Everything is ready to test! 🚀**

Make sure to add the redirect URI to Google Cloud Console first!
