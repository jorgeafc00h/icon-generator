# Azure Infrastructure Provisioning

Automated provisioning for Icon Generator Azure resources.

## 📋 Your Subscription Details

- **Subscription Name:** Azure Sponsorship PROD
- **Subscription ID:** `5fddbeee-d040-44ad-a7a0-a526d45d98a2`
- **OpenAI/Storage Region:** East US
- **Cosmos DB Region:** West US 2 (avoiding East US capacity constraints)
- **Cosmos DB:** FREE TIER ($0/month forever - 1000 RU/s + 25GB)
- **Models:** DALL-E 3 + GPT-4o-mini

## 🚀 Quick Provision (One Command)

```bash
cd /Users/jorgeflores/github/icon-generator/infrastructure
./provision-azure-resources.sh
```

**Time:** 10-15 minutes
**Cost:** ~$5-10/month (Cosmos DB is FREE!)

The script will:
1. ✅ Set your Azure subscription
2. ✅ Create resource group
3. ✅ Create Azure OpenAI service
4. ✅ Deploy DALL-E 3 model
5. ✅ Deploy GPT-4o-mini model (for prompt enhancement)
6. ✅ Create Storage Account with blob container
7. ✅ Create Cosmos DB (FREE TIER) with all containers
8. ✅ Generate credentials files automatically

## 📦 What Gets Created

### Resource Group
- **Name:** `rg-icon-generator`
- **Location:** `eastus`

### Azure OpenAI Service
- **Name:** `openai-icon-generator` (static name)
- **Location:** East US
- **SKU:** S0 (Standard)
- **Models:**
  - **DALL-E 3** - Image generation
    - Deployment: `dall-e-3`
    - Version: 3.0
    - Capacity: 1 unit
  - **GPT-4o-mini** - Prompt enhancement
    - Deployment: `gpt-4o-mini`
    - Version: 2024-07-18 (July 2024 release)
    - Capacity: 10K tokens/min
    - Cost: ~$0.15 per 1M input tokens (75% cheaper than GPT-4)

### Cosmos DB (FREE TIER)
- **Name:** `cosmos-icon-generator` (static name)
- **Location:** West US 2
- **Tier:** FREE TIER ($0/month forever)
- **Capacity:** 1000 RU/s + 25GB storage
- **Database:** `IconGeneratorDB`
- **Containers:**
  - `Users` (partition: `/id`, 400 RU/s)
  - `Icons` (partition: `/userId`, 400 RU/s)
  - `Assets` (partition: `/userId`, 400 RU/s)
  - `Transactions` (partition: `/userId`, 400 RU/s)

### Storage Account
- **Name:** `sticongen` (static name)
- **Location:** East US
- **SKU:** Standard_LRS
- **Container:** `generated-icons` (public blob access)

## 📄 Generated Files

After provisioning, you'll get:

```
infrastructure/
├── azure-credentials.env         ← All your credentials
├── provision-azure-resources.sh  ← Provisioning script

api/
├── local.settings.json           ← Function app configuration
└── Tests/
    └── .env                      ← Test environment variables
```

## 🔐 Credentials File Structure

`azure-credentials.env`:
```bash
# Azure OpenAI
AZURE_OPENAI_ENDPOINT=https://openai-icon-gen-xxxxx.openai.azure.com/
AZURE_OPENAI_API_KEY=abc123...
DALLE3_DEPLOYMENT_NAME=dall-e-3
GPT4O_MINI_DEPLOYMENT_NAME=gpt-4o-mini

# Cosmos DB (FREE TIER)
COSMOS_ENDPOINT=https://cosmos-icon-gen-xxxxx.documents.azure.com:443/
COSMOS_KEY=xyz789...
COSMOS_DATABASE=IconGeneratorDB

# Storage
STORAGE_CONNECTION_STRING=DefaultEndpointsProtocol=https;...
STORAGE_CONTAINER_NAME=generated-icons

# Resource Names
RESOURCE_GROUP=rg-icon-generator
LOCATION=eastus
```

## ✅ Verification Steps

### 1. Check Resources in Portal
```bash
# List all resources
az resource list \
  --resource-group rg-icon-generator \
  --output table
```

### 2. Verify OpenAI Deployments
```bash
# Find your OpenAI resource name from azure-credentials.env
source azure-credentials.env

# List deployments
az cognitiveservices account deployment list \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --output table
```

Expected output:
```
Name          Model         Version      ProvisioningState
------------  ------------  -----------  ------------------
dall-e-3      dall-e-3      3.0          Succeeded
gpt-4o-mini   gpt-4o-mini   2024-07-18   Succeeded
```

### 3. Verify Cosmos DB Containers
```bash
# List containers
az cosmosdb sql container list \
  --account-name $COSMOS_ACCOUNT_NAME \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --output table
```

Expected: Users, Icons, Assets, Transactions

### 4. Test in Azure AI Foundry Portal

1. Go to https://ai.azure.com
2. Sign in with your Azure account
3. Select your project (or create one)
4. Click "Deployments" - you should see:
   - ✅ dall-e-3 (Succeeded)
   - ✅ gpt-4o-mini (Succeeded)
5. Test in Playground:
   - **Images:** Select dall-e-3, generate test image
   - **Chat:** Select gpt-4o-mini, send test message

## 🧪 Test Your Setup

```bash
# 1. Load credentials
cd /Users/jorgeflores/github/icon-generator/api/Tests
source .env

# 2. Build tests
dotnet build

# 3. Run integration tests
dotnet run

# 4. Select test #1 (Style Variations)
# This will use GPT-4o-mini to enhance prompts
```

**Cost:** ~$0.001 for first test

## 💰 Cost Breakdown

### Monthly Costs (Development Usage)

| Service | Usage | Cost |
|---------|-------|------|
| **Cosmos DB** | FREE TIER | **$0** |
| **Storage** | 5GB, minimal transactions | ~$1-2 |
| **Azure OpenAI** | Pay per use | |
| - GPT-4o-mini | ~100K tokens/month | ~$0.015 |
| - DALL-E 3 | ~100 images/month | ~$4 |
| **Total** | | **~$5-7/month** |

### Production Costs (1000 icons/month)

| Service | Usage | Cost |
|---------|-------|------|
| **Cosmos DB** | Need to scale beyond free tier | ~$25 |
| **Storage** | 50GB + CDN | ~$5 |
| **Azure OpenAI** | | |
| - GPT-4o-mini | ~1M tokens | ~$0.15 |
| - DALL-E 3 | 1000 images | ~$40 |
| **Total** | | **~$70/month** |

💡 **Free Tier Benefits:**
- Cosmos DB: 1000 RU/s free forever (perfect for development!)
- First 5GB storage free
- No upfront costs

## 🔧 Manual Configuration (If Script Fails)

If the automated script fails, follow these manual steps:

### 1. Set Subscription
```bash
az login
az account set --subscription "5fddbeee-d040-44ad-a7a0-a526d45d98a2"
```

### 2. Create Resource Group
```bash
az group create \
  --name rg-icon-generator \
  --location eastus
```

### 3. Create Azure OpenAI
```bash
az cognitiveservices account create \
  --name openai-icon-gen-$(date +%s) \
  --resource-group rg-icon-generator \
  --kind OpenAI \
  --sku S0 \
  --location eastus \
  --yes
```

### 4. Deploy Models via Portal
- Go to https://ai.azure.com
- Create project → Deployments
- Deploy DALL-E 3 (name: `dall-e-3`)
- Deploy GPT-4o-mini (name: `gpt-4o-mini`, 10K TPM)

### 5. Create Cosmos DB
```bash
az cosmosdb create \
  --name cosmos-icon-gen-$(date +%s) \
  --resource-group rg-icon-generator \
  --locations regionName=eastus \
  --enable-free-tier true
```

See `Docs/AZURE_AI_FOUNDRY_SETUP.md` for detailed manual steps.

## 🗑️ Cleanup (Delete Everything)

**Warning:** This will delete ALL resources and data!

```bash
az group delete \
  --name rg-icon-generator \
  --yes \
  --no-wait
```

## 📚 Additional Resources

- **Azure AI Foundry:** https://ai.azure.com
- **Azure Portal:** https://portal.azure.com
- **Cosmos DB Free Tier:** https://learn.microsoft.com/azure/cosmos-db/free-tier
- **OpenAI Models:** https://learn.microsoft.com/azure/ai-services/openai/concepts/models

## 🆘 Troubleshooting

### Error: "Location not available for OpenAI"
Try alternative regions: `eastus2`, `westeurope`, `swedencentral`

### Error: "Deployment failed"
Model deployment via CLI sometimes fails. Use Azure AI Foundry portal:
1. https://ai.azure.com
2. Deployments → Deploy model
3. Select DALL-E 3 and GPT-4o-mini

### Error: "Free tier already used"
You can only have one Cosmos DB free tier per subscription. If you already have one:
- Remove `--enable-free-tier true` flag
- Cost will be ~$25/month for 1600 RU/s

### Cosmos DB takes too long
Cosmos DB creation can take 3-5 minutes. The script waits automatically.

### Can't find credentials file
```bash
cd /Users/jorgeflores/github/icon-generator/infrastructure
cat azure-credentials.env
```

## 🔒 Security Best Practices

### 1. Protect Credentials
```bash
# Add to .gitignore (already done)
echo "azure-credentials.env" >> .gitignore
echo "local.settings.json" >> .gitignore
```

### 2. Use Key Vault (Production)
For production, move secrets to Azure Key Vault:
```bash
# Create Key Vault
az keyvault create \
  --name kv-icon-gen-$(date +%s) \
  --resource-group rg-icon-generator
```

### 3. Enable Managed Identity
For Azure Functions deployment, use managed identity instead of keys.

### 4. Rotate Keys Regularly
```bash
# Regenerate OpenAI key
az cognitiveservices account keys regenerate \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --key-name key2
```

---

**Ready to provision?**

```bash
cd /Users/jorgeflores/github/icon-generator/infrastructure
./provision-azure-resources.sh
```

The script will guide you through everything! 🚀
