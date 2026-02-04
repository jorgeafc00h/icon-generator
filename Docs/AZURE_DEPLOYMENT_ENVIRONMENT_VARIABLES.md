# Azure Deployment Environment Variables

This document describes all environment variables required for deploying the Icon Generator Azure Functions.

## Fixed Issues

### Local Development Issue
**Problem**: Cosmos DB was throwing error: `The input content is invalid because the required properties - 'id; ' - are missing`

**Solution**: Added proper JSON serialization configuration to CosmosDbService:
```csharp
var cosmosClientOptions = new CosmosClientOptions
{
    SerializerOptions = new CosmosSerializationOptions
    {
        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
    }
};
```

This ensures the `Id` property in the User model is correctly serialized to lowercase `id` for Cosmos DB.

## Bicep Template Configuration

The bicep templates have been updated to include all required environment variables:

### Automatically Configured (from Azure Resources)

These are automatically set by the bicep deployment:

1. **AzureOpenAI__Endpoint** - Azure OpenAI endpoint URL
2. **AzureOpenAI__ApiKey** - Azure OpenAI API key
3. **AzureOpenAI__DallE3Deployment** - Set to "dall-e-3"
4. **AzureOpenAI__Gpt4oMiniDeployment** - Set to "gpt-4o-mini"
5. **Database__Type** - Set to "cosmosdb" or "sql"
6. **Database__CosmosEndpoint** - Cosmos DB endpoint
7. **Database__CosmosKey** - Cosmos DB primary key
8. **Database__CosmosDatabase** - Set to "IconGeneratorDB"
9. **Storage__ConnectionString** - Storage account connection string
10. **Storage__ContainerName** - Set to "generated-icons"
11. **AllowedOrigins** - Set to "*" (configure for production)

### Required Parameters (Must be Provided)

These must be provided as parameters to the bicep deployment:

#### parameters.dev.json
```json
{
  "parameters": {
    "stripeSecretKey": {
      "value": "sk_test_your_stripe_secret_key"
    },
    "stripeWebhookSecret": {
      "value": "whsec_your_webhook_secret"
    },
    "googleClientId": {
      "value": "your_google_client_id.apps.googleusercontent.com"
    },
    "frontendUrl": {
      "value": "https://your-frontend-url.azurestaticapps.net"
    }
  }
}
```

## Azure Pipeline Configuration

The Azure pipeline is configured to:

1. **Deploy Infrastructure** (main.bicep)
2. **Build .NET 10 Azure Functions**
3. **Deploy Functions to the created Function App**

### Required Pipeline Variables

None! All secrets should be passed as parameters to the bicep deployment.

### Recommended: Use Azure Key Vault

For production, store secrets in Azure Key Vault and reference them in bicep:

```bicep
resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' existing = {
  name: 'stripeSecretKey'
  scope: resourceGroup()
}

// Use in Function App settings
{
  name: 'Stripe__SecretKey'
  value: '@Microsoft.KeyVault(SecretUri=${keyVaultSecret.properties.secretUri})'
}
```

## Required Cosmos DB Setup

The Cosmos DB deployment creates the account but **does NOT create the database and containers**.

### Manual Setup Required

After the first bicep deployment, you must create:

1. **Database**: `IconGeneratorDB`
2. **Containers**:
   - `Users` (partition key: `/id`)
   - `Icons` (partition key: `/id`)
   - `Assets` (partition key: `/id`)
   - `Transactions` (partition key: `/id`)

You can do this via:
- Azure Portal
- Azure CLI
- Azure SDK
- Cosmos DB Data Explorer

### Create via Azure CLI

```bash
# Set variables
COSMOS_ACCOUNT="cosmos-icongen-dev-xyz"
DATABASE_NAME="IconGeneratorDB"

# Create database
az cosmosdb sql database create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --name $DATABASE_NAME

# Create Users container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name $DATABASE_NAME \
  --name Users \
  --partition-key-path "/id" \
  --throughput 400

# Create Icons container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name $DATABASE_NAME \
  --name Icons \
  --partition-key-path "/id" \
  --throughput 400

# Create Assets container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name $DATABASE_NAME \
  --name Assets \
  --partition-key-path "/id" \
  --throughput 400

# Create Transactions container
az cosmosdb sql container create \
  --account-name $COSMOS_ACCOUNT \
  --resource-group rg-icon-generator \
  --database-name $DATABASE_NAME \
  --name Transactions \
  --partition-key-path "/id" \
  --throughput 400
```

## Azure OpenAI Deployments

After creating the Azure OpenAI resource, you must deploy the models:

1. **GPT-4o-mini** - Deployment name: `gpt-4o-mini`
2. **DALL-E 3** - Deployment name: `dall-e-3`

### Create via Azure Portal

1. Go to Azure OpenAI Studio
2. Navigate to Deployments
3. Create new deployments with the exact names above

## Local Development

For local development, your `local.settings.json` should have:

```json
{
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    
    "AzureOpenAI__Endpoint": "https://your-openai.openai.azure.com/",
    "AzureOpenAI__ApiKey": "your-api-key",
    "AzureOpenAI__DallE3Deployment": "dall-e-3",
    "AzureOpenAI__Gpt4oMiniDeployment": "gpt-4o-mini",
    
    "Database__Type": "cosmosdb",
    "Database__CosmosEndpoint": "https://your-cosmos.documents.azure.com:443/",
    "Database__CosmosKey": "your-cosmos-key",
    "Database__CosmosDatabase": "IconGeneratorDB",
    
    "Storage__ConnectionString": "your-storage-connection-string",
    "Storage__ContainerName": "generated-icons",
    
    "Stripe__SecretKey": "sk_test_your_key",
    "Stripe__WebhookSecret": "whsec_your_secret",
    "Stripe__FrontendUrl": "http://localhost:5173",
    
    "AllowedOrigins": "http://localhost:5173,http://localhost:3000"
  }
}
```

## Testing the Deployment

1. **Restart the Function App** after bicep deployment
2. **Verify environment variables** in Azure Portal → Function App → Configuration
3. **Test endpoints**:
   - `/api/auth/google` (POST)
   - `/api/generate-icon` (POST)
   - `/api/user-data` (GET)

## Troubleshooting

### "Missing required properties - 'id'"
- Ensure CosmosDbService has proper serialization options configured
- Check that User model has `[JsonPropertyName("id")]` attribute

### "401 Unauthorized" from Azure OpenAI
- Verify API key is correct in environment variables
- Check that deployments exist with correct names

### "Container not found"
- Create Cosmos DB containers manually (see above)
- Ensure partition key is `/id`

### CORS Issues
- Update `AllowedOrigins` in Function App settings
- Include your Static Web App URL
