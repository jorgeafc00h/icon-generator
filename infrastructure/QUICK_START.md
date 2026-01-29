# Quick Start Guide

Get your Icon Generator infrastructure up and running in minutes.

## 1. Prerequisites Setup (5 minutes)

```bash
# Install Azure CLI (if not already installed)
# macOS
brew install azure-cli

# Windows
winget install Microsoft.AzureCLI

# Linux
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login to Azure
az login

# Set your subscription
az account set --subscription "YOUR_SUBSCRIPTION_ID"

# Verify
az account show
```

## 2. Local Deployment (10 minutes)

```bash
# Navigate to infrastructure directory
cd infrastructure

# Deploy to dev environment
./deploy.sh dev eastus

# Or deploy manually
az group create --name rg-icon-generator --location eastus

az deployment group create \
  --resource-group rg-icon-generator \
  --template-file main.bicep \
  --parameters parameters.dev.json \
  --name infra-dev-$(date +%s)
```

## 3. Azure DevOps Setup (15 minutes)

### Create Service Connection

1. Go to **Project Settings** → **Service connections**
2. Create **Azure Resource Manager** connection
3. Use **Service principal (automatic)**
4. Scope: Subscription
5. Name: `icon-generator-service-connection`

### Create Variable Group

1. Go to **Pipelines** → **Library**
2. Create variable group: `icon-generator-variables`
3. Add variables:
   ```
   subscriptionId: YOUR_SUBSCRIPTION_ID
   AZURE_OPENAI_API_KEY: (mark as secret)
   STRIPE_SECRET_KEY: (mark as secret)
   VITE_STRIPE_PUBLISHABLE_KEY: (mark as secret)
   VITE_API_BASE_URL: https://func-icongen-dev-xxx.azurewebsites.net/api
   ```

### Configure Pipeline

1. Go to **Pipelines** → **New pipeline**
2. Select **Azure Repos Git** (or your repo location)
3. Choose **Existing Azure Pipelines YAML file**
4. Select `/azure-pipelines.yml`
5. Click **Run**

## 4. Deploy AI Models (5 minutes)

**Important**: This step must be done manually through Azure AI Foundry portal.

1. Navigate to [https://ai.azure.com](https://ai.azure.com)
2. Select your subscription and resource group
3. Find your OpenAI resource (e.g., `openai-icongen-dev-xxx`)
4. Go to **Deployments** → **Create new deployment**

**Deploy DALL-E 3**:
- Model: `dall-e-3`
- Deployment name: `dalle3-icon-generator`
- Version: Latest available
- Deployment type: Standard

**Deploy GPT-4o-mini**:
- Model: `gpt-4o-mini`
- Deployment name: `gpt-4o-mini-prompts`
- Version: Latest available
- Tokens per minute: 10K

## 5. Initialize Cosmos DB (2 minutes)

The pipeline handles this automatically, but you can verify:

```bash
# Check if database exists
az cosmosdb sql database show \
  --account-name cosmos-icongen-dev-xxx \
  --resource-group rg-icon-generator \
  --name IconGeneratorDB

# Check containers
az cosmosdb sql container list \
  --account-name cosmos-icongen-dev-xxx \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB
```

## 6. Verify Deployment (2 minutes)

```bash
# Get deployment outputs
az deployment group show \
  --resource-group rg-icon-generator \
  --name YOUR_DEPLOYMENT_NAME \
  --query properties.outputs

# Test Function App
FUNCTION_URL=$(az functionapp show \
  --name func-icongen-dev-xxx \
  --resource-group rg-icon-generator \
  --query defaultHostName -o tsv)

curl https://${FUNCTION_URL}/api/health

# Test Static Web App
STATIC_URL=$(az staticwebapp show \
  --name swa-icongen-dev-xxx \
  --resource-group rg-icon-generator \
  --query defaultHostname -o tsv)

curl https://${STATIC_URL}
```

## 7. Development Workflow

### Make Changes
```bash
# Work on your feature
git checkout -b feature/my-feature

# Make changes to code
code .
```

### Test Locally
```bash
# Frontend
cd frontend
npm run dev

# Backend
cd api
npm run start
```

### Deploy via Pipeline
```bash
# Commit and push
git add .
git commit -m "feat: add my feature"
git push origin feature/my-feature

# Create PR to main/develop
# Pipeline runs automatically
```

## Common Commands

### View all resources
```bash
az resource list \
  --resource-group rg-icon-generator \
  --output table
```

### Check Function App logs
```bash
az webapp log tail \
  --name func-icongen-dev-xxx \
  --resource-group rg-icon-generator
```

### Update Function App settings
```bash
az functionapp config appsettings set \
  --name func-icongen-dev-xxx \
  --resource-group rg-icon-generator \
  --settings "MY_SETTING=value"
```

### Restart Function App
```bash
az functionapp restart \
  --name func-icongen-dev-xxx \
  --resource-group rg-icon-generator
```

### View Cosmos DB data
```bash
# Using Data Explorer in Azure Portal
# Or via CLI
az cosmosdb sql container query \
  --account-name cosmos-icongen-dev-xxx \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --name Users \
  --query-text "SELECT * FROM c"
```

### Download Storage blob
```bash
az storage blob download \
  --account-name sticongendevxxx \
  --container-name generated-icons \
  --name user123/icon456.png \
  --file ./local-icon.png
```

## Troubleshooting

### "Deployment failed" error
```bash
# View detailed error
az deployment group show \
  --resource-group rg-icon-generator \
  --name YOUR_DEPLOYMENT_NAME \
  --query properties.error

# Delete and retry
az group delete --name rg-icon-generator --yes
./deploy.sh dev eastus
```

### Function App not starting
```bash
# Check app settings
az functionapp config appsettings list \
  --name func-icongen-dev-xxx \
  --resource-group rg-icon-generator

# Check logs
az functionapp log tail \
  --name func-icongen-dev-xxx \
  --resource-group rg-icon-generator
```

### OpenAI API errors
- Verify models are deployed in AI Foundry portal
- Check deployment names match exactly:
  - `dalle3-icon-generator`
  - `gpt-4o-mini-prompts`
- Verify API key is correct in Function App settings

## Next Steps

1. **Configure Stripe** - Add webhook endpoint to Stripe dashboard
2. **Setup Custom Domain** - Configure custom domain for Static Web App
3. **Enable CDN** - Add CDN profile for faster image delivery
4. **Configure Monitoring** - Set up alerts in Application Insights
5. **Production Deployment** - Deploy to production environment

## Estimated Costs

**Development Environment**: ~$35-50/month
- Includes all Azure services
- OpenAI usage is pay-per-call (variable)

**Production Environment**: ~$250-300/month + API usage
- Includes premium tier services
- Auto-scaling enabled

## Support

- Documentation: See [infrastructure/README.md](./README.md)
- Azure Support: [https://azure.microsoft.com/support/](https://azure.microsoft.com/support/)
- Bicep Reference: [https://learn.microsoft.com/azure/azure-resource-manager/bicep/](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
