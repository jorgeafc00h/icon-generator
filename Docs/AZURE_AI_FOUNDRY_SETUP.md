# Azure AI Foundry Setup Guide

Complete step-by-step guide for configuring Azure AI Foundry resources for the Icon Generator application.

## 📋 Prerequisites

- ✅ Azure subscription: **Azure Sponsorship PROD**
- ✅ Subscription ID: `5fddbeee-d040-44ad-a7a0-a526d45d98a2`
- ✅ Azure CLI installed (`brew install azure-cli` on macOS)
- ✅ Access to Azure AI Foundry (https://ai.azure.com)

## 🚀 Quick Start (Automated)

**Recommended:** Use the automated provisioning script:

```bash
cd /Users/jorgeflores/github/icon-generator/infrastructure
./provision-azure-resources.sh
```

This will automatically create all resources with your subscription.

**OR** follow the manual steps below.

## 🎯 What You'll Create

```
Azure Resources:
├── Resource Group: rg-icon-generator
├── Azure OpenAI Service
│   ├── Model: DALL-E 3 (for image generation)
│   └── Model: GPT-4o-mini (for prompt enhancement)
├── Storage Account (for generated images)
├── Cosmos DB (for user data)
└── Key Vault (for secrets)
```

---

## Step 1: Login and Set Subscription

### 1.1 Login to Azure

```bash
# Login to Azure
az login

# This will open a browser window for authentication
# Select your account and close the browser when complete
```

### 1.2 List Your Subscriptions

```bash
# List all subscriptions
az account list --output table
```

**Output:**
```
Name                      SubscriptionId                        TenantId
------------------------  ------------------------------------  ------------------------------------
Your Subscription Name    xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx  yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
```

### 1.3 Set Active Subscription

```bash
# Set to your Azure Sponsorship PROD subscription
az account set --subscription "5fddbeee-d040-44ad-a7a0-a526d45d98a2"

# Verify it's set
az account show --output table
```

**Expected Output:**
```
Name                      SubscriptionId
------------------------  ------------------------------------
Azure Sponsorship PROD    5fddbeee-d040-44ad-a7a0-a526d45d98a2
```

---

## Step 2: Create Resource Group

```bash
# Create resource group in East US (best for AI services)
az group create \
  --name rg-icon-generator \
  --location eastus

# Output should show:
# "provisioningState": "Succeeded"
```

**Alternative Regions** (if East US unavailable):
- `eastus2`
- `westeurope`
- `swedencentral`

---

## Step 3: Create Azure OpenAI Resource

### 3.1 Create the Service

```bash
az cognitiveservices account create \
  --name openai-icon-generator-$(date +%s) \
  --resource-group rg-icon-generator \
  --kind OpenAI \
  --sku S0 \
  --location eastus \
  --yes
```

**Note:** We append timestamp to ensure unique name globally.

**Expected Output:**
```json
{
  "provisioningState": "Succeeded",
  "endpoint": "https://openai-icon-generator-1234567890.openai.azure.com/",
  ...
}
```

⚠️ **Save the endpoint URL** - you'll need it later!

### 3.2 Get Your API Key

```bash
# Get the resource name from above (e.g., openai-icon-generator-1234567890)
OPENAI_RESOURCE_NAME="openai-icon-generator-1234567890"

# Get API key
az cognitiveservices account keys list \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --query "key1" \
  --output tsv
```

**Output:** `abc123def456...` (64-character key)

⚠️ **Save this key securely** - treat it like a password!

### 3.3 Get the Endpoint

```bash
az cognitiveservices account show \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --query "properties.endpoint" \
  --output tsv
```

**Output:** `https://openai-icon-generator-1234567890.openai.azure.com/`

---

## Step 4: Deploy Models in Azure AI Foundry Portal

### 4.1 Open Azure AI Foundry

1. Go to https://ai.azure.com
2. Sign in with your Azure account
3. Click **"Build"** in the top menu
4. Click **"+ Create project"** or select existing project

### 4.2 Create a Project (if new)

1. **Project Name:** `icon-generator-project`
2. **Hub:** Create new hub or select existing
   - **Hub Name:** `icon-generator-hub`
   - **Subscription:** Select your subscription
   - **Resource Group:** `rg-icon-generator`
   - **Location:** `East US`
3. Click **"Create"**

Wait 2-3 minutes for provisioning.

### 4.3 Deploy DALL-E 3 Model

1. In your project, click **"Deployments"** (left sidebar)
2. Click **"+ Deploy model"** → **"Deploy base model"**
3. Search for **"dall-e-3"**
4. Click **"Confirm"** on DALL-E 3
5. Configure deployment:
   ```
   Deployment name: dall-e-3
   Model version:   Latest (3)
   Deployment type: Standard
   Content filter:  Default
   Rate limit:      Auto (or set to 1 TPM for testing)
   ```
6. Click **"Deploy"**
7. Wait ~1 minute for deployment

✅ **Verify:** You should see "dall-e-3" in your deployments list with status "Succeeded"

### 4.4 Deploy GPT-4o-mini Model (For Prompt Enhancement)

**Important:** We use GPT-4o-mini instead of GPT-4 to save costs on prompt enhancement.

1. Click **"+ Deploy model"** → **"Deploy base model"**
2. Search for **"gpt-4o-mini"**
3. Click **"Confirm"**
4. Configure deployment:
   ```
   Deployment name:         gpt-4o-mini
   Model version:           2024-07-18 (or latest)
   Deployment type:         Standard
   Tokens per Minute Rate:  10K (start here, increase if needed)
   Content filter:          Default
   ```
5. Click **"Deploy"**

✅ **Verify:** Both models show "Succeeded" status

**Why GPT-4o-mini?**
- 75% cheaper than GPT-4 (~$0.15 vs $0.60 per 1M tokens)
- Sufficient quality for prompt enhancement
- Faster response times
- Lower latency

---

## Step 5: Test Your Deployments

### 5.1 Test in Azure AI Foundry Portal

1. Click on **"dall-e-3"** deployment
2. Click **"Open in playground"**
3. Try a simple prompt: `"A red apple on a white background"`
4. Click **"Generate"**
5. You should see an image appear (~10-30 seconds)

✅ If image generates successfully, DALL-E 3 is working!

**For GPT-4o-mini:**
1. Click **"Chat"** in playground
2. Select **"gpt-4o-mini"** deployment
3. Type: `"Say hello"`
4. Should respond quickly

✅ If you get a response, GPT-4o-mini is working!

### 5.2 Get Your Deployment Credentials

In Azure AI Foundry:
1. Click **"Project settings"** (gear icon)
2. Under **"API Keys and Endpoint"**, note:
   - **Endpoint:** `https://your-resource.openai.azure.com/`
   - **Key:** Click **"Show"** to reveal

Or use Azure CLI:

```bash
# Get endpoint
az cognitiveservices account show \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --query "properties.endpoint" \
  --output tsv

# Get key
az cognitiveservices account keys list \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --query "key1" \
  --output tsv
```

---

## Step 6: Create Storage Account

```bash
# Create storage account (name must be globally unique, lowercase, no hyphens)
STORAGE_NAME="sticongenerator$(date +%s)"

az storage account create \
  --name $STORAGE_NAME \
  --resource-group rg-icon-generator \
  --location eastus \
  --sku Standard_LRS \
  --kind StorageV2

# Get connection string
az storage account show-connection-string \
  --name $STORAGE_NAME \
  --resource-group rg-icon-generator \
  --output tsv
```

**Output:** `DefaultEndpointsProtocol=https;AccountName=...`

⚠️ **Save this connection string** - needed for local.settings.json

### 6.1 Create Blob Container

```bash
# Get connection string into variable
CONNECTION_STRING=$(az storage account show-connection-string \
  --name $STORAGE_NAME \
  --resource-group rg-icon-generator \
  --output tsv)

# Create container for generated icons
az storage container create \
  --name generated-icons \
  --connection-string "$CONNECTION_STRING" \
  --public-access blob
```

✅ Container created with public read access for generated images

---

## Step 7: Create Cosmos DB (FREE TIER)

**Important:** Using Cosmos DB Free Tier (1000 RU/s included - $0 cost!)

```bash
# Create Cosmos DB account with FREE TIER (this takes 3-5 minutes)
az cosmosdb create \
  --name cosmos-icon-generator-$(date +%s) \
  --resource-group rg-icon-generator \
  --locations regionName=eastus failoverPriority=0 \
  --default-consistency-level Session \
  --enable-free-tier true

# Note the account name from output
COSMOS_ACCOUNT="cosmos-icon-generator-1234567890"

# Get connection info
az cosmosdb show \
  --name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --query "documentEndpoint" \
  --output tsv

# Get primary key
az cosmosdb keys list \
  --name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --query "primaryMasterKey" \
  --output tsv
```

⚠️ **Save endpoint and key** for configuration

### 7.1 Create Database and Containers

```bash
# Create database
az cosmosdb sql database create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --name IconGeneratorDB

# Create Users container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --name Users \
  --partition-key-path "/id" \
  --throughput 400

# Create Icons container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --name Icons \
  --partition-key-path "/userId" \
  --throughput 400

# Create Assets container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --name Assets \
  --partition-key-path "/userId" \
  --throughput 400

# Create Transactions container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --name Transactions \
  --partition-key-path "/userId" \
  --throughput 400
```

✅ All containers created with 400 RU/s (minimum for serverless scenarios)

---

## Step 8: Configure Local Development

### 8.1 Create `local.settings.json`

```bash
cd /Users/jorgeflores/github/icon-generator/api
```

Create or update `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "AzureOpenAI__Endpoint": "https://your-resource.openai.azure.com/",
    "AzureOpenAI__ApiKey": "your-api-key-here",
    "AzureOpenAI__DallE3Deployment": "dall-e-3",
    "AzureOpenAI__Gpt4oMiniDeployment": "gpt-4o-mini",

    "Database__Type": "cosmosdb",
    "Database__CosmosEndpoint": "https://your-cosmos.documents.azure.com:443/",
    "Database__CosmosKey": "your-cosmos-key-here",
    "Database__CosmosDatabase": "IconGeneratorDB",

    "Storage__ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "Storage__ContainerName": "generated-icons",

    "Stripe__SecretKey": "sk_test_your_stripe_key",
    "Stripe__WebhookSecret": "whsec_your_webhook_secret",
    "Stripe__FrontendUrl": "http://localhost:5173",

    "AllowedOrigins": "http://localhost:5173,http://localhost:3000"
  }
}
```

### 8.2 Set Environment Variables for Tests

```bash
# Create .env file for tests
cat > /Users/jorgeflores/github/icon-generator/api/Tests/.env << 'EOF'
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_API_KEY=your-api-key-here
DALLE3_DEPLOYMENT_NAME=dall-e-3
GPT4O_MINI_DEPLOYMENT_NAME=gpt-4o-mini
EOF
```

**Or export as environment variables:**

```bash
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_API_KEY="your-api-key-here"
export DALLE3_DEPLOYMENT_NAME="dall-e-3"
export GPT4O_MINI_DEPLOYMENT_NAME="gpt-4o-mini"
```

---

## Step 9: Verify Configuration

### 9.1 Test Azure CLI Access

```bash
# List your OpenAI deployments
az cognitiveservices account deployment list \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --output table
```

**Expected Output:**
```
Name           Model          Version  ProvisioningState
-------------  -------------  -------  ------------------
dall-e-3       dall-e-3       3        Succeeded
gpt-4o-mini    gpt-4o-mini    latest   Succeeded
```

### 9.2 Test Cosmos DB Connection

```bash
# List databases
az cosmosdb sql database list \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --output table

# List containers
az cosmosdb sql container list \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --output table
```

### 9.3 Test Storage Account

```bash
# List containers
az storage container list \
  --connection-string "$CONNECTION_STRING" \
  --output table
```

---

## Step 10: Quick Reference - Copy Your Values

Fill this out with your actual values for easy reference:

```bash
# ===== AZURE OPENAI =====
AZURE_OPENAI_ENDPOINT="https://_____.openai.azure.com/"
AZURE_OPENAI_API_KEY="_____"
DALLE3_DEPLOYMENT_NAME="dall-e-3"
GPT4O_MINI_DEPLOYMENT_NAME="gpt-4o-mini"

# ===== COSMOS DB =====
COSMOS_ENDPOINT="https://_____.documents.azure.com:443/"
COSMOS_KEY="_____"
COSMOS_DATABASE="IconGeneratorDB"

# ===== STORAGE =====
STORAGE_CONNECTION_STRING="DefaultEndpointsProtocol=https;AccountName=_____..."
STORAGE_CONTAINER_NAME="generated-icons"

# ===== RESOURCE NAMES =====
RESOURCE_GROUP="rg-icon-generator"
OPENAI_RESOURCE_NAME="_____"
COSMOS_ACCOUNT="_____"
STORAGE_ACCOUNT="_____"
```

---

## 💰 Cost Estimates

### Development/Testing (Light Usage)
- **Azure OpenAI:** Pay per use
  - GPT-4o-mini: ~$0.15 per 1M input tokens, ~$0.60 per 1M output tokens
  - DALL-E 3: $0.040 per image (standard), $0.080 per image (HD)
- **Cosmos DB:** $0.008/hour (400 RU/s) = ~$5.76/month
- **Storage:** ~$0.02/GB/month + negligible transactions
- **Total for testing:** ~$10-20/month

### Production (1000 icons/month)
- DALL-E 3: $40-80
- GPT-4o-mini: $5-10
- Cosmos DB: $25-50 (if scaling up)
- Storage: $5-10
- **Total:** ~$75-150/month

💡 **Free Tier Options:**
- Cosmos DB: 1000 RU/s free forever (enough for dev)
- Storage: First 5GB free
- Azure OpenAI: No free tier, pay per use

---

## 🔒 Security Best Practices

### 1. Use Key Vault for Production

```bash
# Create Key Vault
az keyvault create \
  --name kv-icon-gen-$(date +%s) \
  --resource-group rg-icon-generator \
  --location eastus

# Store secrets
az keyvault secret set \
  --vault-name kv-icon-gen-1234567890 \
  --name "AzureOpenAIKey" \
  --value "your-api-key"
```

### 2. Enable Managed Identity

For Azure Functions:
```bash
# Enable system-assigned identity
az functionapp identity assign \
  --name func-icon-generator \
  --resource-group rg-icon-generator
```

### 3. Rotate Keys Regularly

```bash
# Regenerate Azure OpenAI key
az cognitiveservices account keys regenerate \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --key-name key2
```

---

## 🐛 Troubleshooting

### Error: "Resource provider not registered"

```bash
# Register providers
az provider register --namespace Microsoft.CognitiveServices
az provider register --namespace Microsoft.DocumentDB
az provider register --namespace Microsoft.Storage

# Wait 1-2 minutes, then verify
az provider show -n Microsoft.CognitiveServices --query "registrationState"
```

### Error: "Location not available for OpenAI"

Try alternative regions:
```bash
# Check available locations
az cognitiveservices account list-skus \
  --kind OpenAI \
  --output table

# Common alternatives: eastus2, westeurope, swedencentral
```

### Error: "Quota exceeded" for DALL-E 3

1. Go to https://ai.azure.com
2. Click your project → Quotas
3. Request quota increase (usually approved quickly)

### Can't access generated images

```bash
# Ensure container has public access
az storage container set-permission \
  --name generated-icons \
  --connection-string "$CONNECTION_STRING" \
  --public-access blob
```

---

## ✅ Verification Checklist

Before running integration tests, verify:

- [ ] Azure subscription active
- [ ] Resource group created
- [ ] Azure OpenAI resource created
- [ ] DALL-E 3 model deployed and shows "Succeeded"
- [ ] GPT-4o-mini model deployed and shows "Succeeded"
- [ ] Storage account created with "generated-icons" container
- [ ] Cosmos DB created with 4 containers (Users, Icons, Assets, Transactions)
- [ ] `local.settings.json` configured with all values
- [ ] Environment variables set for tests
- [ ] Can successfully generate test image in Azure AI Foundry playground

---

## 🚀 Next Steps

Now you're ready to:
1. Run integration tests to evaluate prompt quality
2. Start local development with `func start`
3. Deploy to Azure when ready

See **`api/Tests/README.md`** for running integration tests!

---

## 📚 Useful Azure CLI Commands

```bash
# List all resources in resource group
az resource list \
  --resource-group rg-icon-generator \
  --output table

# Check costs
az consumption usage list \
  --start-date 2024-01-01 \
  --end-date 2024-01-31 \
  --output table

# Delete everything (cleanup)
az group delete \
  --name rg-icon-generator \
  --yes --no-wait
```

---

**Need Help?**
- Azure AI Foundry Docs: https://learn.microsoft.com/azure/ai-studio/
- Azure OpenAI Docs: https://learn.microsoft.com/azure/ai-services/openai/
- Azure CLI Reference: https://learn.microsoft.com/cli/azure/
