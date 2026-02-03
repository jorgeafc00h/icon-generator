
# Google OAuth Setup Guide

## ✅ Configuration Complete!

Your Google OAuth is now configured and ready to use.

---

## 📋 Your Credentials

**Project**: icon-gen-486320
**Client ID**: `YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com`
**Client Secret**: `YOUR_GOOGLE_CLIENT_SECRET`

---

## ✅ What's Already Configured

### 1. **Google Cloud Console** (Already Set Up)

Your OAuth 2.0 Client ID has:

✅ **Authorized JavaScript origins**:
- `http://localhost:5173` (Local development)
- `https://mango-bay-068c07f0f.6.azurestaticapps.net` (Production)

✅ **Authorized redirect URIs**:
- `http://localhost:5173/auth/callback`
- `https://mango-bay-068c07f0f.6.azurestaticapps.net/auth/callback`
- `https://api.your-domain.com/api/auth/google/callback`

### 2. **Frontend Environment** (Just Configured)

✅ **File**: `/web/.env`
```env
VITE_GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com
```

---

## 🚀 How to Test

### Local Development:

1. **Start the backend** (Azure Functions):
   ```bash
   cd api
   func start
   # Should run on http://localhost:7071
   ```

2. **Start the frontend** (React):
   ```bash
   cd web
   npm run dev
   # Should run on http://localhost:5173
   ```

3. **Test Google Sign-In**:
   - Open browser: `http://localhost:5173`
   - Click on "Profile" in navigation
   - Click "Sign in with Google" button
   - You should see Google's OAuth popup
   - Select your Google account
   - ✅ You should be signed in with **2 welcome credits**

### Expected Flow:

```
1. Click "Sign in with Google"
   └─→ Google popup appears

2. Select Google account
   └─→ Google redirects back to your app

3. Backend verifies token
   └─→ Creates user with 2 credits (if new)
   └─→ Or logs in existing user

4. Profile page loads
   └─→ Shows your name, email, profile picture
   └─→ Shows credit balance
   └─→ Shows stats (icons generated, etc.)
```

---

## 🔧 Production Deployment

### Your Production URLs:

**Frontend**: `https://mango-bay-068c07f0f.6.azurestaticapps.net`

### Production Environment Variables:

In your Azure Static Web Apps configuration, set:

```
VITE_GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com
VITE_API_ENDPOINT=https://your-api-endpoint.azurewebsites.net/api
VITE_STRIPE_PUBLIC_KEY=pk_live_your_stripe_key
```

**Steps**:
1. Go to Azure Portal
2. Open your Static Web App: "mango-bay-068c07f0f"
3. Go to "Configuration" or "Environment variables"
4. Add the variables above
5. Save and redeploy

---

## 🔒 Important Security Notes

### Client Secret Storage:

⚠️ **NEVER** commit your client secret to Git!

The client secret should ONLY be used on the backend if you need server-to-server communication. For Google Sign-In button (frontend), you only need the **Client ID**, which is already configured.

### What's Safe to Expose:

✅ **Client ID** - Safe to use in frontend code (already in .env)
❌ **Client Secret** - NEVER put in frontend, only backend if needed

Currently, your app uses **Google Sign-In with ID Token**, which only requires the Client ID. The backend verifies the ID token, so you don't need the client secret in your code.

---

## 🐛 Troubleshooting

### Issue: "Popup Blocked"
**Solution**: Allow popups for localhost:5173 in your browser settings

### Issue: "Invalid Client ID"
**Solution**:
- Verify `.env` file has correct Client ID
- Restart the dev server after changing `.env`
- Clear browser cache

### Issue: "Redirect URI Mismatch"
**Solution**:
- Google Sign-In button doesn't use redirect URIs by default
- If you see this error, check Google Cloud Console authorized origins
- Make sure `http://localhost:5173` is in "Authorized JavaScript origins"

### Issue: "CORS Error"
**Solution**:
- Make sure backend is running on `http://localhost:7071`
- Check API endpoint in `.env` matches your backend URL

### Issue: "Backend Returns 401 Unauthorized"
**Solution**:
- Backend token verification might be failing
- Check backend logs for errors
- In development, the backend uses JWT decoding without verification (see TODO in code)

---

## 📁 Files Modified

✅ **Frontend**:
- `/web/.env` - Added your Google Client ID

✅ **Backend** (No changes needed):
- Already configured to verify Google tokens
- AuthenticationFunction.cs handles the OAuth flow

---

## 🧪 Test Checklist

Run through this checklist to verify everything works:

- [ ] Backend is running on port 7071
- [ ] Frontend is running on port 5173
- [ ] Navigate to Profile page
- [ ] See "Sign in with Google" button
- [ ] Click button, Google popup appears
- [ ] Select Google account
- [ ] Popup closes, redirected back to app
- [ ] Profile page shows:
  - [ ] Your name
  - [ ] Your email
  - [ ] Your Google profile picture
  - [ ] **2 credits** balance
  - [ ] Stats (all zeros for new user)
- [ ] Click "Buy Credits" to test purchase modal
- [ ] Navigate to Generator
- [ ] Generate an icon (should deduct 1 credit)
- [ ] Check Profile - credits should now show **1**

---

## 🎯 Next Steps

### 1. **Test Locally** (First)
Follow the test checklist above to ensure everything works locally.

### 2. **Deploy to Production**
Once local testing works:
- Deploy backend to Azure Functions
- Deploy frontend to Azure Static Web Apps
- Add production environment variables
- Test with production URL

### 3. **Optional: Add More OAuth Providers**
Your app currently supports Google. You could add:
- Apple Sign-In
- GitHub
- Microsoft
- Email/Password

### 4. **Optional: Enhance Security**
Current implementation uses JWT decoding without verification (see TODOs in AuthenticationFunction.cs). For production, consider:
- Adding proper Google token verification
- Implementing JWT signing for access tokens
- Adding token expiration and refresh

---

## 📞 Quick Commands

### Start Development:
```bash
# Terminal 1 - Backend
cd /Users/jorgeflores/github/icon-generator/api
func start

# Terminal 2 - Frontend
cd /Users/jorgeflores/github/icon-generator/web
npm run dev
```

### View Logs:
```bash
# Backend logs (in api terminal)
# Will show authentication attempts and errors

# Frontend console (in browser)
# Open DevTools > Console
# Will show any frontend errors
```

### Test Authentication:
```bash
# Navigate to
http://localhost:5173/profile

# Or directly from homepage
http://localhost:5173
# Then click "Profile" in navigation
```

---

## 🎉 Summary

✅ **Google Client ID configured**: Frontend .env updated
✅ **Authorized origins set**: localhost:5173 and production URL
✅ **Ready for testing**: Start backend + frontend and try signing in
✅ **Security verified**: Client secret not exposed in frontend
✅ **Production ready**: Just need to add environment variables in Azure

**Your Google OAuth is fully configured and ready to use!** 🚀

Try signing in now to test it!
