# Icon Generator - Quick Start Guide

## 🚀 Provision Azure Resources (Run This First!)

```bash
# Navigate to infrastructure folder
cd /Users/jorgeflores/github/icon-generator/infrastructure

# Run automated provisioning script
./provision-azure-resources.sh
```

**What it does:**
- ✅ Sets your Azure subscription (Azure Sponsorship PROD)
- ✅ Creates all Azure resources
- ✅ Deploys DALL-E 3 and GPT-4o-mini models
- ✅ Sets up Cosmos DB FREE TIER ($0/month forever!)
- ✅ Creates storage account
- ✅ Generates all credential files automatically

**Time:** 10-15 minutes
**Cost:** ~$5-10/month for development (Cosmos DB is FREE!)

---

## 📋 Your Configuration

- **Subscription:** Azure Sponsorship PROD
- **Subscription ID:** `5fddbeee-d040-44ad-a7a0-a526d45d98a2`
- **Region:** East US
- **Cosmos DB:** FREE TIER (saves ~$25/month!)
- **Models:**
  - **DALL-E 3** - Image generation
  - **GPT-4o-mini** - Prompt enhancement (July 2024 Release)
    - Latest small model from OpenAI
    - Cost-effective (~$0.15 per 1M input tokens)
    - Excellent quality for prompt enhancement

---

## ⚡ After Provisioning

### 1. Verify Resources
```bash
# Check what was created
az resource list \
  --resource-group rg-icon-generator \
  --output table
```

### 2. View Your Credentials
```bash
# All credentials saved here
cat infrastructure/azure-credentials.env
```

### 3. Run Integration Tests
```bash
# Navigate to tests
cd api/Tests

# Load environment variables
source .env

# Build and run
dotnet build && dotnet run

# Select: 1 (Style Variations)
```

**Cost:** ~$0.001 per test (negligible!)

---

## 📁 Generated Files

After provisioning, you'll have:

```
infrastructure/
└── azure-credentials.env          ← All your credentials

api/
├── local.settings.json            ← Function app config (auto-created)
└── Tests/
    └── .env                       ← Test config (auto-created)
```

⚠️ **These files are git-ignored for security**

---

## 💰 Cost Summary

| Service | Tier | Monthly Cost |
|---------|------|--------------|
| Cosmos DB | **FREE TIER** | **$0** |
| Storage | Standard LRS | ~$1-2 |
| Azure OpenAI | Pay per use | |
| - GPT-4o-mini | ~100K tokens | ~$0.015 |
| - DALL-E 3 | ~100 images | ~$4 |
| **Total** | | **~$5-10** |

**Cosmos DB Free Tier Benefits:**
- FREE FOREVER (not just a trial)
- 1000 RU/s throughput (handles ~1M requests/month)
- 25GB storage included
- Perfect for development and small production apps

---

## 🧪 Test Workflow

### Phase 1: Prompt Experimentation (Free!)
```bash
cd api/Tests
dotnet run

# Run tests 1-6
# Cost: ~$0.02 for all tests
# Time: 5 minutes
```

### Phase 2: Image Generation (When Ready)
```bash
# Uncomment image generation in test code
# Generate 5-10 test images
# Cost: ~$0.20-0.80
```

### Phase 3: Production
```bash
# Deploy to Azure
# Start generating real icons
```

---

## 🔧 Manual Verification (Optional)

### Check Model Deployments
```bash
# Load credentials
source infrastructure/azure-credentials.env

# List deployments
az cognitiveservices account deployment list \
  --name $OPENAI_RESOURCE_NAME \
  --resource-group rg-icon-generator \
  --output table
```

Expected:
```
Name          Model         ProvisioningState
------------  ------------  ------------------
dall-e-3      dall-e-3      Succeeded
gpt-4o-mini   gpt-4o-mini   Succeeded
```

### Check Cosmos DB Containers
```bash
az cosmosdb sql container list \
  --account-name $COSMOS_ACCOUNT_NAME \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --output table
```

Expected: Users, Icons, Assets, Transactions

### Test in Azure AI Foundry Portal
1. Go to https://ai.azure.com
2. Sign in
3. Click "Deployments"
4. Verify both models show "Succeeded"
5. Test in playground

---

## 🎯 Next Steps After Provisioning

### 1. Run Your First Test
```bash
cd /Users/jorgeflores/github/icon-generator/api/Tests
source .env
dotnet run
# Select: 1
```

### 2. Review Results
Check console output for:
- Enhanced prompts
- Quality scores (should be 90-100%)
- Design expertise applied

### 3. Save Results
```bash
# Run test #6 to save results
dotnet run
# Select: 6

# View results
cat test-results/prompt-results-*.md
```

### 4. Iterate
Based on results:
- Adjust design knowledge base
- Try different styles
- Experiment with colors
- Generate actual images

---

## 📚 Documentation

- **Provisioning Details:** `infrastructure/README.md`
- **Azure Setup Guide:** `Docs/AZURE_AI_FOUNDRY_SETUP.md`
- **Prompt Strategy:** `Docs/PROMPT_ENGINEERING_STRATEGY.md`
- **Test Guide:** `api/Tests/README.md`
- **Setup Checklist:** `SETUP_CHECKLIST.md`

---

## 🆘 Troubleshooting

### Script fails to deploy models
**Solution:** Models may need manual deployment via portal
1. Go to https://ai.azure.com
2. Create project
3. Deployments → Deploy DALL-E 3 and GPT-4o-mini
4. Use deployment names: `dall-e-3` and `gpt-4o-mini`

### "Free tier already used" error
**Solution:** You can only have one Cosmos DB free tier per subscription
- Check if you have another Cosmos DB with free tier
- Remove `--enable-free-tier true` (costs ~$25/month)

### Can't find credentials
```bash
cd /Users/jorgeflores/github/icon-generator/infrastructure
ls -la
cat azure-credentials.env
```

### Tests won't run
```bash
# Make sure environment is loaded
cd /Users/jorgeflores/github/icon-generator/api/Tests
source .env

# Verify variables are set
echo $AZURE_OPENAI_ENDPOINT
echo $AZURE_OPENAI_API_KEY
```

---

## 🗑️ Cleanup (if needed)

Delete everything:
```bash
az group delete \
  --name rg-icon-generator \
  --yes
```

**Warning:** This deletes ALL resources and data!

---

## ✨ You're Ready!

Run the provisioning script now:

```bash
cd /Users/jorgeflores/github/icon-generator/infrastructure
./provision-azure-resources.sh
```

It will guide you through everything automatically! 🚀
