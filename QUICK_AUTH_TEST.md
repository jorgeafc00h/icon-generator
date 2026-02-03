# Quick OAuth Authentication Test

## 🧪 What I Just Fixed

1. ✅ Added OAuth callback detection in Profile component
2. ✅ Added debug logging to see what's happening
3. ✅ Added visual feedback (loading toast) during authentication
4. ✅ Better error messages if auth fails

---

## 🚀 How to Test Right Now

### Step 1: Clear Everything
```javascript
// Open browser console (F12)
localStorage.clear()
location.reload()
```

### Step 2: Open Console
- Press F12
- Go to "Console" tab
- Keep it open to see debug messages

### Step 3: Click "Continue with Google"
- Should redirect to Google
- Select your account
- Redirects back to `/profile#id_token=...`

### Step 4: Watch the Console
You should see these messages in order:
```
1. "Checking for OAuth callback, hash: present"
2. "ID token found: yes"
3. "Processing ID token..."
4. "OAuth callback detected in Profile, waiting for GoogleSignIn to process..."
5. "Handling OAuth callback with ID token"
6. "Auth successful: {userId: ..., email: ...}"
```

### Step 5: What You Should See
- **Loading toast**: "Signing in with Google..."
- **Success toast**: "Welcome back, Jorge Flores Calles!"
- Page reloads automatically
- **Header shows**: Your profile picture and name
- **Profile page**: Your account details

---

## 🐛 If It Still Doesn't Work

### Check Console for Errors

**Look for**:
- Red error messages
- Failed network requests
- "Auth failed" messages

### Common Issues:

**1. "Failed to fetch" or CORS error**
```
Solution: Make sure backend is running
cd api
func start
```

**2. "Authentication failed: ..."**
```
Solution: Check backend logs for the actual error
The backend should show what went wrong
```

**3. "ID token found: no"**
```
Solution: The hash isn't being parsed correctly
Check the URL - should have #id_token=...
```

**4. Nothing happens after redirect**
```
Solution: GoogleSignIn component might not be mounting
Check if you see "Checking for OAuth callback" in console
```

---

## 🔍 Debug Checklist

Run through this:

- [ ] Backend running on port 7071?
  ```bash
  curl http://localhost:7071/api
  # Should not give "connection refused"
  ```

- [ ] Frontend running on port 5173?
  ```bash
  # Visit http://localhost:5173
  # Should see the app
  ```

- [ ] .env file has correct values?
  ```bash
  cat web/.env
  # Check VITE_API_ENDPOINT=http://localhost:7071/api
  # Check VITE_GOOGLE_CLIENT_ID=421206201443-...
  ```

- [ ] Google Console redirect URI configured?
  ```
  Should have: http://localhost:5173/profile
  ```

- [ ] localStorage cleared before testing?
  ```javascript
  localStorage.clear()
  ```

---

## 📋 Expected Flow

### What Should Happen:

1. **Click "Continue with Google"**
   - URL changes to `https://accounts.google.com/...`

2. **Select Google Account**
   - Google shows your accounts
   - Click your account

3. **Redirect Back**
   - URL becomes: `http://localhost:5173/profile#id_token=LONG_TOKEN...`
   - You should see the login page briefly

4. **Processing**
   - Toast appears: "Signing in with Google..."
   - Console shows: "Processing ID token..."

5. **Backend Call**
   - POST to `/api/auth/google`
   - Backend validates token
   - Returns user data

6. **Success**
   - Toast: "Welcome back, Jorge!"
   - Data saved to localStorage
   - Page reloads

7. **Logged In**
   - Header shows your profile picture
   - Name displayed: "Jorge Flores Calles"
   - Credits shown
   - Profile page shows your details

---

## 🎯 If You See Specific Errors

### Error: "redirect_uri_mismatch"
**Cause**: Google Console settings
**Fix**:
1. Go to Google Cloud Console
2. Check redirect URI is exactly: `http://localhost:5173/profile`
3. No trailing slash, exact match

### Error: "Invalid ID token"
**Cause**: Backend can't validate token
**Fix**:
1. Check backend logs
2. Verify `VITE_GOOGLE_CLIENT_ID` matches backend expectations
3. Token might be expired (try again)

### Error: "CORS policy"
**Cause**: Backend not allowing requests
**Fix**:
1. Backend should allow `http://localhost:5173`
2. Check Azure Functions CORS settings
3. Restart backend

### Error: Page just reloads infinitely
**Cause**: Token stored but user fetch fails
**Fix**:
```javascript
// Clear storage and try again
localStorage.clear()
location.reload()
```

---

## 📞 What to Share If Still Broken

If it's still not working, share:

1. **Console output** (copy everything)
2. **Network tab** (look for failed requests)
3. **Any error messages**
4. **Backend logs** (from the terminal running `func start`)

---

## ✅ Success Criteria

You'll know it's working when:

- ✅ No errors in console
- ✅ See "Auth successful" message
- ✅ Profile picture appears in header
- ✅ Name shows: "Jorge Flores Calles"
- ✅ Credits show (2 for new user)
- ✅ Can navigate to other pages without losing login
- ✅ Page refresh keeps you logged in

---

**Try it now and check the console for debug messages!** 🚀

If you see any errors, share the console output and I'll help fix it!
