# Icon Generator - Deployment Guide

Complete deployment guide for the Icon Generator platform using Azure DevOps and Bicep.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [Infrastructure Setup](#infrastructure-setup)
4. [Azure DevOps Configuration](#azure-devops-configuration)
5. [Deployment Process](#deployment-process)
6. [Post-Deployment](#post-deployment)
7. [Monitoring & Maintenance](#monitoring--maintenance)

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         Azure Resources                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌──────────────┐     ┌──────────────┐     ┌──────────────┐   │
│  │   Static     │     │   Function   │     │   Azure      │   │
│  │   Web App    │────▶│     App      │────▶│   OpenAI     │   │
│  │  (Frontend)  │     │  (Backend)   │     │  (DALL-E 3)  │   │
│  └──────────────┘     └──────────────┘     └──────────────┘   │
│                              │                                  │
│                              ├──────────────┬──────────────┐   │
│                              ▼              ▼              ▼   │
│                       ┌──────────┐   ┌──────────┐  ┌──────┐   │
│                       │ Cosmos   │   │  Blob    │  │ Key  │   │
│                       │    DB    │   │ Storage  │  │Vault │   │
│                       └──────────┘   └──────────┘  └──────┘   │
│                                                                  │
│  Monitoring: Application Insights + Log Analytics               │
└─────────────────────────────────────────────────────────────────┘
```

## Prerequisites

### Required Tools

```bash
# Azure CLI (v2.50+)
az --version

# Node.js (v18+)
node --version

# Git
git --version

# Optional: Bicep CLI
az bicep version
```

### Azure Subscription

- Active Azure subscription
- Contributor or Owner role
- Azure OpenAI service access (may require application)

### Azure DevOps

- Azure DevOps organization
- Project created
- Git repository connected

## Infrastructure Setup

### 1. Clone Repository

```bash
git clone https://github.com/YOUR_ORG/icon-generator.git
cd icon-generator
```

### 2. Review Infrastructure Code

```bash
# View main template
cat infrastructure/main.bicep

# View parameters for dev environment
cat infrastructure/parameters.dev.json
```

### 3. Deploy Infrastructure Manually (Optional)

For testing or initial setup:

```bash
cd infrastructure

# Make deploy script executable
chmod +x deploy.sh

# Deploy to dev
./deploy.sh dev eastus

# Deploy to production
./deploy.sh main eastus
```

## Azure DevOps Configuration

### Step 1: Create Service Connection

1. Navigate to **Project Settings** → **Service connections**
2. Click **New service connection**
3. Select **Azure Resource Manager**
4. Choose **Service principal (automatic)**
5. Configure:
   - Scope level: **Subscription**
   - Subscription: Select your Azure subscription
   - Resource group: Leave empty (or select `rg-icon-generator`)
   - Service connection name: `icon-generator-service-connection`
6. Grant access permission to all pipelines
7. Click **Save**

### Step 2: Create Variable Group

1. Go to **Pipelines** → **Library**
2. Click **+ Variable group**
3. Name: `icon-generator-variables`
4. Add the following variables:

| Variable Name | Value | Secret |
|--------------|-------|--------|
| `subscriptionId` | Your Azure subscription ID | No |
| `AZURE_OPENAI_API_KEY` | OpenAI API key (added after deployment) | Yes |
| `STRIPE_SECRET_KEY` | Stripe secret key | Yes |
| `STRIPE_WEBHOOK_SECRET` | Stripe webhook secret | Yes |
| `VITE_STRIPE_PUBLISHABLE_KEY` | Stripe publishable key | Yes |
| `VITE_API_BASE_URL` | Function App URL (added after deployment) | No |

5. Click **Save**

### Step 3: Create Pipeline

1. Go to **Pipelines** → **Pipelines**
2. Click **New pipeline**
3. Select your repository source (Azure Repos, GitHub, etc.)
4. Choose **Existing Azure Pipelines YAML file**
5. Path: `/azure-pipelines.yml`
6. Click **Continue**
7. Review the pipeline YAML
8. Click **Run**

### Step 4: Configure Pipeline Environments

1. Go to **Pipelines** → **Environments**
2. Create environments:
   - `main` (for production)
   - `develop` (for development)
3. Add approvals and checks if needed for production

## Deployment Process

### Automatic Deployment (Recommended)

Deployments trigger automatically on push to `main` or `develop` branches:

```bash
# Create feature branch
git checkout -b feature/my-feature

# Make changes
# ... code changes ...

# Commit and push
git add .
git commit -m "feat: add new feature"
git push origin feature/my-feature

# Create Pull Request to develop/main
# Pipeline runs automatically on PR merge
```

### Manual Deployment

Trigger pipeline manually from Azure DevOps:

1. Go to **Pipelines** → **Pipelines**
2. Select **Icon Generator CI/CD**
3. Click **Run pipeline**
4. Select branch: `main` or `develop`
5. Click **Run**

### Pipeline Stages

The pipeline executes in the following order:

1. **Infrastructure** (5-10 min)
   - Validates Bicep templates
   - Deploys/updates Azure resources
   - Outputs resource names and URLs

2. **BuildBackend** (2-3 min)
   - Installs dependencies
   - Compiles TypeScript
   - Runs tests
   - Creates deployment package

3. **BuildFrontend** (2-3 min)
   - Installs dependencies
   - Builds React app
   - Optimizes assets

4. **DeployBackend** (3-5 min)
   - Deploys to Azure Functions
   - Configures app settings
   - Restarts function app

5. **DeployFrontend** (2-3 min)
   - Deploys to Static Web App
   - Updates configuration

6. **PostDeployment** (2-3 min)
   - Initializes Cosmos DB
   - Creates storage containers
   - Configures CORS

7. **SmokeTests** (1 min)
   - Health check endpoints
   - Verify deployments

**Total Time**: ~15-25 minutes

## Post-Deployment

### 1. Deploy AI Models

**Critical**: Must be done manually via Azure AI Foundry portal.

1. Navigate to [https://ai.azure.com](https://ai.azure.com)
2. Sign in with your Azure account
3. Select your subscription and resource group
4. Find your OpenAI resource (e.g., `openai-icongen-dev-xxx`)
5. Go to **Deployments** section

**Deploy DALL-E 3**:
- Click **Create new deployment**
- Model: `dall-e-3`
- Deployment name: `dalle3-icon-generator` (must match exactly)
- Model version: Latest available
- Deployment type: Standard
- Click **Create**

**Deploy GPT-4o-mini**:
- Click **Create new deployment**
- Model: `gpt-4o-mini`
- Deployment name: `gpt-4o-mini-prompts` (must match exactly)
- Model version: Latest available
- Tokens per minute rate limit: 10,000
- Click **Create**

### 2. Retrieve Resource Information

```bash
# Get all outputs from infrastructure deployment
az deployment group show \
  --resource-group rg-icon-generator \
  --name infrastructure-main-TIMESTAMP \
  --query properties.outputs \
  --output table

# Get OpenAI endpoint and key
az cognitiveservices account show \
  --name openai-icongen-main-xxx \
  --resource-group rg-icon-generator

az cognitiveservices account keys list \
  --name openai-icongen-main-xxx \
  --resource-group rg-icon-generator
```

### 3. Update Variable Group

Add the following values to the `icon-generator-variables` variable group:

```bash
# Function App URL (from deployment outputs)
VITE_API_BASE_URL=https://func-icongen-main-xxx.azurewebsites.net/api

# OpenAI API Key (from step 2)
AZURE_OPENAI_API_KEY=xxx...xxx
```

### 4. Configure Stripe Webhooks

1. Go to [Stripe Dashboard](https://dashboard.stripe.com)
2. Navigate to **Developers** → **Webhooks**
3. Click **Add endpoint**
4. Endpoint URL: `https://func-icongen-main-xxx.azurewebsites.net/api/stripeWebhook`
5. Events to send:
   - `checkout.session.completed`
   - `payment_intent.succeeded`
   - `payment_intent.payment_failed`
6. Click **Add endpoint**
7. Copy the **Signing secret**
8. Add to variable group as `STRIPE_WEBHOOK_SECRET`

### 5. Verify Deployment

```bash
# Test Function App health endpoint
curl https://func-icongen-main-xxx.azurewebsites.net/api/health

# Test Static Web App
curl https://swa-icongen-main-xxx.azurestaticapps.net

# Check Cosmos DB
az cosmosdb sql database show \
  --account-name cosmos-icongen-main-xxx \
  --resource-group rg-icon-generator \
  --name IconGeneratorDB
```

## Monitoring & Maintenance

### Application Insights

View metrics and logs:

```bash
# Open in Azure Portal
az monitor app-insights component show \
  --app ai-icongen-main-xxx \
  --resource-group rg-icon-generator \
  --query appId -o tsv

# Query logs
az monitor app-insights query \
  --app ai-icongen-main-xxx \
  --resource-group rg-icon-generator \
  --analytics-query "requests | where timestamp > ago(1h) | summarize count() by resultCode"
```

### Cost Management

Monitor costs:

```bash
# View cost analysis
az consumption usage list \
  --start-date 2024-01-01 \
  --end-date 2024-01-31 \
  --query "[?contains(instanceName, 'icongen')]" \
  --output table
```

### Scaling

Scale Function App:

```bash
# Scale out (add instances)
az functionapp plan update \
  --name asp-icongen-main-xxx \
  --resource-group rg-icon-generator \
  --max-burst 20

# Scale up (upgrade tier)
az functionapp plan update \
  --name asp-icongen-main-xxx \
  --resource-group rg-icon-generator \
  --sku P2v3
```

### Backup & Disaster Recovery

Enable backups for Cosmos DB:

```bash
# Continuous backup is enabled by default
# Verify backup policy
az cosmosdb show \
  --name cosmos-icongen-main-xxx \
  --resource-group rg-icon-generator \
  --query backupPolicy
```

### Updates & Maintenance

```bash
# Update infrastructure
cd infrastructure
git pull
./deploy.sh main eastus

# Update application code
git checkout main
git pull
git push  # Triggers pipeline automatically
```

## Troubleshooting

### Common Issues

**Issue**: Pipeline fails at Infrastructure stage
```bash
# Check deployment errors
az deployment group show \
  --resource-group rg-icon-generator \
  --name DEPLOYMENT_NAME \
  --query properties.error
```

**Issue**: Function App returns 500 errors
```bash
# Check application logs
az webapp log tail \
  --name func-icongen-main-xxx \
  --resource-group rg-icon-generator

# Verify app settings
az functionapp config appsettings list \
  --name func-icongen-main-xxx \
  --resource-group rg-icon-generator
```

**Issue**: OpenAI API errors
- Verify models are deployed with exact names
- Check API key is correct
- Verify endpoint URL format

### Support Resources

- [Azure Support](https://azure.microsoft.com/support/)
- [Azure DevOps Documentation](https://learn.microsoft.com/azure/devops/)
- [Bicep Documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)

## Security Best Practices

1. **Enable Azure AD Authentication** for Function App
2. **Use Managed Identities** instead of connection strings
3. **Store secrets in Key Vault** and reference in app settings
4. **Enable DDoS Protection** for production
5. **Configure IP restrictions** for management access
6. **Regular security updates** via pipeline automation
7. **Monitor with Security Center** and enable recommendations

---

**Last Updated**: January 2026
**Version**: 1.0.0
