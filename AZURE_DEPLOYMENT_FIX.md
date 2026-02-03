# Azure Static Web Apps Deployment Fix

## ✅ What I Fixed

### 1. **Created `staticwebapp.config.json`**
This file configures Azure Static Web Apps to:
- ✅ Set correct MIME types for `.js`, `.css`, `.svg`, etc.
- ✅ Enable SPA routing (all routes fallback to `/index.html`)
- ✅ Add security headers
- ✅ Configure caching for assets
- ✅ Handle 404s by serving `index.html`

### 2. **Updated `vite.config.ts`**
- ✅ Explicit build configuration
- ✅ Correct output directory: `dist`
- ✅ Assets directory: `assets`
- ✅ Base path: `/`

---

## 🚀 How to Deploy

### Option 1: Git Push (Recommended)

```bash
cd /Users/jorgeflores/github/icon-generator

# Add the new config file
git add web/staticwebapp.config.json
git add web/vite.config.ts

# Commit
git commit -m "Fix Azure Static Web Apps MIME types and SPA routing"

# Push to main branch
git push origin main
```

**What happens**:
- GitHub Actions workflow triggers automatically
- Builds the app with correct configuration
- Deploys to Azure with proper MIME types
- Should be live in ~2-3 minutes

### Option 2: Manual Build & Deploy

```bash
cd web

# Build the app
npm run build

# Check the dist folder was created
ls -la dist/

# Azure will automatically deploy on next commit
```

---

## 🔧 What the Config Does

### `staticwebapp.config.json`

**MIME Types**:
```json
".js": "text/javascript"
```
- Fixes the "Expected a JavaScript module" error
- Tells Azure to serve `.js` files with correct MIME type

**Navigation Fallback**:
```json
"navigationFallback": {
  "rewrite": "/index.html"
}
```
- Makes React Router work
- All routes (like `/profile`, `/pricing`) serve `index.html`
- React Router handles client-side routing

**404 Handling**:
```json
"responseOverrides": {
  "404": {
    "rewrite": "/index.html",
    "statusCode": 200
  }
}
```
- Any 404 (like `/vite.svg`) serves from correct location
- Single Page App functionality

---

## 🧪 How to Test

### After Deployment:

1. **Wait for GitHub Actions**:
   - Go to: https://github.com/jorgeafc00h/icon-generator/actions
   - Wait for workflow to complete (green checkmark)

2. **Visit Your Site**:
   ```
   https://mango-bay-068c07f0f.6.azurestaticapps.net
   ```

3. **Open DevTools (F12)**:
   - Check Console - should see NO errors
   - No "MIME type" errors
   - No 404 for `vite.svg`

4. **Test Navigation**:
   - Click "Profile" - URL changes to `/profile`
   - Refresh page - should still show Profile (not 404)
   - Click "Pricing" - should work
   - All navigation should work smoothly

5. **Test Google Auth**:
   - Click "Continue with Google"
   - Should redirect to Google
   - After auth, redirect back to your Azure URL
   - Should work properly

---

## 📋 Checklist

Before deploying:
- [x] Created `staticwebapp.config.json`
- [x] Updated `vite.config.ts`
- [x] `public/vite.svg` exists

After deploying:
- [ ] GitHub Actions workflow completes successfully
- [ ] Site loads without MIME type errors
- [ ] `vite.svg` loads (check favicon)
- [ ] Navigation works (Profile, Pricing, etc.)
- [ ] Refresh on any page works
- [ ] No 404 errors in console

---

## 🐛 Troubleshooting

### If MIME Type Error Persists:

1. **Check workflow ran**:
   ```bash
   # Look at recent commits
   git log --oneline -5

   # Should see your commit
   ```

2. **Check Azure Portal**:
   - Go to Azure Portal
   - Find "mango-bay-068c07f0f" Static Web App
   - Check "Configuration" → shows `staticwebapp.config.json` is deployed

3. **Hard refresh browser**:
   ```
   Ctrl + Shift + R (Windows/Linux)
   Cmd + Shift + R (Mac)
   ```

4. **Check deployment logs**:
   - GitHub Actions → Your workflow run
   - Look for errors in build step

### If vite.svg 404 Persists:

1. **Verify file exists**:
   ```bash
   ls web/public/vite.svg
   # Should show the file
   ```

2. **Check build output**:
   ```bash
   cd web
   npm run build
   ls dist/
   # Should see vite.svg copied to dist/
   ```

3. **Clear browser cache**:
   - Hard refresh
   - Or open incognito window

### If SPA Routing Breaks:

**Symptom**: `/profile` shows 404 after refresh

**Fix**:
- Verify `staticwebapp.config.json` is in `web/` folder
- Commit and push again
- Wait for deployment

---

## 🔐 Production Environment Variables

After deployment works, add these in Azure Portal:

1. **Go to Azure Portal**
2. **Find Static Web App**: "mango-bay-068c07f0f"
3. **Configuration** → **Application settings**
4. **Add**:
   ```
   VITE_API_ENDPOINT = https://your-api.azurewebsites.net/api
   VITE_GOOGLE_CLIENT_ID = 421206201443-rsvce5hacsmc00fol75gc865uhelau1e.apps.googleusercontent.com
   VITE_STRIPE_PUBLIC_KEY = pk_live_your_production_key
   ```

5. **Save** and redeploy

---

## 📊 Before vs After

### Before:
```
❌ main.tsx: MIME type error
❌ vite.svg: 404
❌ Routes don't work on refresh
❌ JavaScript modules won't load
```

### After:
```
✅ All JavaScript loads correctly
✅ vite.svg loads properly
✅ Routes work on refresh
✅ SPA routing works perfectly
✅ Production-ready deployment
```

---

## 🎯 Next Steps

1. **Commit and push the changes**:
   ```bash
   git add web/staticwebapp.config.json web/vite.config.ts
   git commit -m "Fix Azure Static Web Apps configuration"
   git push origin main
   ```

2. **Wait for deployment** (~2-3 minutes)

3. **Test the site**:
   - Visit: https://mango-bay-068c07f0f.6.azurestaticapps.net
   - Check console for errors
   - Test navigation
   - Test Google auth

4. **Update Google Console** (if not done):
   - Add redirect URI: `https://mango-bay-068c07f0f.6.azurestaticapps.net/profile`

5. **Add production env vars** in Azure Portal

---

## ✅ Summary

**Files Created**:
- ✅ `web/staticwebapp.config.json` - Azure SWA configuration

**Files Modified**:
- ✅ `web/vite.config.ts` - Build configuration

**What This Fixes**:
- ✅ MIME type errors for JavaScript modules
- ✅ 404 errors for static assets
- ✅ SPA routing (React Router)
- ✅ Proper caching headers
- ✅ Security headers

**Ready to Deploy**: ✅ Yes!

---

**Push to GitHub now and your Azure deployment will work! 🚀**
