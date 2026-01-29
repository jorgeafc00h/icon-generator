# Infrastructure as Code - Icon Generator

This directory contains Bicep templates for deploying the Icon Generator platform infrastructure to Azure.

## Architecture Overview

The infrastructure includes:

- **Azure Static Web Apps** - Hosts the React frontend
- **Azure Functions** - Serverless backend API (Node.js 18)
- **Azure OpenAI** - DALL-E 3 and GPT-4o-mini deployments
- **Cosmos DB** - NoSQL database for user data and icon history
- **Blob Storage** - Store generated icons and app resources
- **Application Insights** - Monitoring and telemetry
- **Log Analytics** - Centralized logging
- **Key Vault** - Secrets management

## Directory Structure

```
infrastructure/
├── main.bicep                      # Main orchestration template
├── parameters.dev.json             # Dev environment parameters
├── parameters.staging.json         # Staging environment parameters
├── parameters.main.json            # Production environment parameters
└── modules/
    ├── storage-account.bicep       # Blob storage configuration
    ├── cosmos-db.bicep             # Cosmos DB account
    ├── cognitive-services.bicep    # Azure OpenAI service
    ├── app-service-plan.bicep      # Function App hosting plan
    ├── function-app.bicep          # Azure Functions configuration
    ├── static-web-app.bicep        # Static Web App for React
    ├── log-analytics.bicep         # Log Analytics workspace
    ├── app-insights.bicep          # Application Insights
    └── key-vault.bicep             # Key Vault for secrets
```

## Prerequisites

1. **Azure CLI** installed and authenticated
   ```bash
   az login
   az account set --subscription "YOUR_SUBSCRIPTION_ID"
   ```

2. **Azure DevOps** service connection configured
   - Create a service principal with Contributor role
   - Add service connection in Azure DevOps Project Settings

3. **Variable Group** in Azure DevOps Library named `icon-generator-variables`:
   ```
   AZURE_OPENAI_API_KEY
   STRIPE_SECRET_KEY
   VITE_STRIPE_PUBLISHABLE_KEY
   VITE_API_BASE_URL
   subscriptionId
   ```

## Local Deployment

### Validate Templates

```bash
# Validate for dev environment
az deployment group validate \
  --resource-group rg-icon-generator \
  --template-file ./main.bicep \
  --parameters ./parameters.dev.json
```

### Deploy Infrastructure

```bash
# Create resource group
az group create \
  --name rg-icon-generator \
  --location eastus

# Deploy to dev
az deployment group create \
  --resource-group rg-icon-generator \
  --template-file ./main.bicep \
  --parameters ./parameters.dev.json \
  --name infrastructure-dev-$(date +%Y%m%d-%H%M%S)

# Deploy to production
az deployment group create \
  --resource-group rg-icon-generator \
  --template-file ./main.bicep \
  --parameters ./parameters.main.json \
  --name infrastructure-prod-$(date +%Y%m%d-%H%M%S)
```

### View Deployment Outputs

```bash
# Get outputs from deployment
az deployment group show \
  --resource-group rg-icon-generator \
  --name infrastructure-dev-<timestamp> \
  --query properties.outputs
```

## Azure DevOps Pipeline Deployment

The pipeline is triggered automatically on commits to `main` or `develop` branches.

### Pipeline Stages

1. **Infrastructure** - Deploy Bicep templates
2. **BuildBackend** - Build and test Azure Functions
3. **BuildFrontend** - Build React application
4. **DeployBackend** - Deploy Functions to Azure
5. **DeployFrontend** - Deploy to Static Web App
6. **PostDeployment** - Configure Cosmos DB and Storage
7. **SmokeTests** - Verify deployments

### Manual Pipeline Trigger

```bash
# Trigger pipeline via Azure CLI
az pipelines run \
  --name "Icon Generator CI/CD" \
  --organization https://dev.azure.com/YOUR_ORG \
  --project YOUR_PROJECT
```

## Post-Deployment Configuration

### 1. Deploy AI Models (Manual)

Azure OpenAI model deployments must be done through the Azure AI Foundry portal:

1. Navigate to [https://ai.azure.com](https://ai.azure.com)
2. Select your subscription and OpenAI resource
3. Go to **Models + endpoints** → **Deploy model**
4. Deploy **DALL-E 3**:
   - Deployment name: `dalle3-icon-generator`
   - Model version: Latest
   - Deployment type: Standard
5. Deploy **GPT-4o-mini**:
   - Deployment name: `gpt-4o-mini-prompts`
   - Tokens per minute: 10K (adjust as needed)

### 2. Initialize Cosmos DB

The pipeline automatically creates the database and containers, but you can also do it manually:

```bash
# Create database
az cosmosdb sql database create \
  --account-name cosmos-icongen-dev-<uniqueid> \
  --resource-group rg-icon-generator \
  --name IconGeneratorDB \
  --throughput 400

# Create Users container
az cosmosdb sql container create \
  --account-name cosmos-icongen-dev-<uniqueid> \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --name Users \
  --partition-key-path "/id"

# Create Icons container
az cosmosdb sql container create \
  --account-name cosmos-icongen-dev-<uniqueid> \
  --resource-group rg-icon-generator \
  --database-name IconGeneratorDB \
  --name Icons \
  --partition-key-path "/userId"
```

### 3. Configure CORS for Storage

```bash
az storage cors add \
  --services b \
  --methods GET POST PUT \
  --origins '*' \
  --allowed-headers '*' \
  --exposed-headers '*' \
  --max-age 3600 \
  --account-name <storage-account-name>
```

## Environment Differences

### Development
- Cosmos DB: Serverless + Free Tier
- App Service Plan: Consumption (Y1)
- Static Web App: Free tier
- Storage: Locally redundant (LRS)
- Log retention: 30 days

### Production
- Cosmos DB: Provisioned throughput
- App Service Plan: Premium V3 (P1v3)
- Static Web App: Standard tier
- Storage: Geo-redundant (GRS)
- Log retention: 90 days
- Auto-scaling enabled

## Cost Estimation

### Development Environment (Monthly)
- Azure Functions (Consumption): ~$5
- Cosmos DB (Serverless): ~$25
- Storage Account: ~$5
- OpenAI API calls: Variable (pay-per-use)
- **Estimated Total**: ~$35-50/month

### Production Environment (Monthly)
- Azure Functions (Premium): ~$150
- Cosmos DB (Provisioned): ~$50
- Storage Account (GRS): ~$20
- Static Web App (Standard): ~$9
- Application Insights: ~$10
- OpenAI API calls: Variable based on usage
- **Estimated Total**: ~$250-300/month + API usage

## Troubleshooting

### Deployment Fails with "Resource Already Exists"

```bash
# Delete and redeploy
az group delete --name rg-icon-generator --yes
az group create --name rg-icon-generator --location eastus
# Re-run deployment
```

### Function App Not Starting

Check application settings:
```bash
az functionapp config appsettings list \
  --name func-icongen-dev-<uniqueid> \
  --resource-group rg-icon-generator
```

### Cosmos DB Connection Issues

Verify connection string:
```bash
az cosmosdb keys list \
  --name cosmos-icongen-dev-<uniqueid> \
  --resource-group rg-icon-generator \
  --type connection-strings
```

## Security Best Practices

1. **Use Managed Identities** - Function App uses system-assigned managed identity
2. **Key Vault Integration** - Store secrets in Key Vault, reference in app settings
3. **HTTPS Only** - All services enforce HTTPS
4. **Minimum TLS 1.2** - All services require TLS 1.2 or higher
5. **RBAC** - Use role-based access control instead of access keys where possible

## Clean Up Resources

To delete all resources:

```bash
az group delete \
  --name rg-icon-generator \
  --yes \
  --no-wait
```

## Additional Resources

- [Azure Bicep Documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
- [Azure Functions Documentation](https://learn.microsoft.com/azure/azure-functions/)
- [Azure OpenAI Documentation](https://learn.microsoft.com/azure/ai-services/openai/)
- [Cosmos DB Documentation](https://learn.microsoft.com/azure/cosmos-db/)
