# Quick Start: Azure Deployment Checklist

## Prerequisites ✅

- [ ] Azure subscription with credits
- [ ] Azure CLI installed (`az --version`)
- [ ] Azure DevOps project with "Azure PROD" service connection
- [ ] Stripe account with API keys
- [ ] Google OAuth Client ID configured

## Step 1: Update Secrets

Edit `infrastructure/parameters.dev.json`:

```json
{
  "parameters": {
    "stripeSecretKey": {
      "value": "sk_test_your_actual_key_here"
    },
    "stripeWebhookSecret": {
      "value": "whsec_your_actual_secret_here"
    },
    "googleClientId": {
      "value": "your-client-id.apps.googleusercontent.com"
    },
    "frontendUrl": {
      "value": "https://your-static-web-app-url.azurestaticapps.net"
    }
  }
}
```

⚠️ **DO NOT commit this file with real secrets!** Use `.gitignore` or Azure Key Vault.

## Step 2: Deploy Infrastructure

### Option A: Via Azure Pipeline (Recommended)
```bash
git push origin main
```
The pipeline will automatically deploy everything.

### Option B: Manual Deployment
```bash
cd infrastructure

az login

az deployment group create \
  --resource-group rg-icon-generator \
  --template-file main.bicep \
  --parameters parameters.dev.json \
  --parameters location=eastus
```

## Step 3: Create Cosmos DB Containers

After first deployment, get the Cosmos DB account name from outputs:

```bash
# Get account name
COSMOS_ACCOUNT=$(az deployment group show \
  --resource-group rg-icon-generator \
  --name main \
  --query properties.outputs.cosmosAccountName.value \
  --output tsv)

echo "Cosmos Account: $COSMOS_ACCOUNT"

# Create database
az cosmosdb sql database create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --name IconGeneratorDB

# Create containers
for container in Users Icons Assets Transactions; do
  az cosmosdb sql container create \
    --account-name $COSMOS_ACCOUNT \
    --resource-group rg-icon-generator \
    --database-name IconGeneratorDB \
    --name $container \
    --partition-key-path "/id" \
    --throughput 400
done
```

## Step 4: Deploy Azure OpenAI Models

1. Go to [Azure OpenAI Studio](https://oai.azure.com/)
2. Select your resource
3. Navigate to **Deployments** → **Create new deployment**
4. Deploy:
   - **Model**: gpt-4o-mini, **Deployment name**: `gpt-4o-mini`
   - **Model**: dall-e-3, **Deployment name**: `dall-e-3`

## Step 5: Verify Function App

```bash
# Get Function App name
FUNC_APP=$(az deployment group show \
  --resource-group rg-icon-generator \
  --name main \
  --query properties.outputs.functionAppName.value \
  --output tsv)

# Get Function App URL
FUNC_URL=$(az functionapp show \
  --resource-group rg-icon-generator \
  --name $FUNC_APP \
  --query defaultHostName \
  --output tsv)

echo "Function App URL: https://$FUNC_URL"

# Test health endpoint
curl "https://$FUNC_URL/api/health"
```

## Step 6: Update Google OAuth Redirect URIs

Add to Google Cloud Console → APIs & Services → Credentials:

**Authorized JavaScript origins:**
- `https://$FUNC_URL`
- `https://your-static-web-app.azurestaticapps.net`

**Authorized redirect URIs:**
- `https://$FUNC_URL/api/auth/google/callback`
- `https://your-static-web-app.azurestaticapps.net/oauth-callback.html`

## Step 7: Configure Static Web App

Update environment variables in Azure Portal:

```
VITE_API_ENDPOINT=https://$FUNC_URL/api
VITE_GOOGLE_CLIENT_ID=your-client-id.apps.googleusercontent.com
VITE_STRIPE_PUBLIC_KEY=pk_test_your_public_key
```

## Step 8: Test End-to-End

1. **Test Authentication**:
   ```bash
   curl -X POST "https://$FUNC_URL/api/auth/google" \
     -H "Content-Type: application/json" \
     -d '{"idToken": "test_token"}'
   ```

2. **Visit your app**:
   - Go to Static Web App URL
   - Sign in with Google
   - Verify user created in Cosmos DB
   - Test icon generation

## Troubleshooting

### "Container not found"
→ Run Step 3 to create Cosmos DB containers

### "Deployment not found"
→ Run Step 4 to deploy Azure OpenAI models

### "Invalid token"
→ Update Google OAuth redirect URIs (Step 6)

### "CORS error"
→ Check Function App `AllowedOrigins` setting

## Production Checklist

- [ ] Move secrets to Azure Key Vault
- [ ] Configure custom domain for Static Web App
- [ ] Set up Application Insights alerts
- [ ] Enable diagnostic logging
- [ ] Configure backup for Cosmos DB
- [ ] Set up CI/CD approval gates
- [ ] Update CORS to specific origins only
- [ ] Use production Stripe keys
- [ ] Enable Function App authentication

## Cost Estimate (Dev Environment)

| Resource | Tier | Monthly Cost |
|----------|------|--------------|
| Azure Functions | Consumption | ~$5 |
| Cosmos DB | Free Tier | $0 (1000 RU/s) |
| Storage Account | Standard LRS | ~$1 |
| Azure OpenAI | Pay-per-use | Variable |
| Static Web App | Free | $0 |
| Application Insights | Basic | ~$2 |
| **Total** | | **~$8-10/month** |

*Excludes Azure OpenAI usage costs*
