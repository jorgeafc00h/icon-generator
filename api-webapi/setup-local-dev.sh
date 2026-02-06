#!/bin/bash

# Icon Generator Web API - Local Development Setup Script
# This script helps you configure local development settings

echo "🚀 Icon Generator Web API - Local Development Setup"
echo "=================================================="
echo ""

# Check if user is logged into Azure
if ! az account show &> /dev/null; then
    echo "❌ Not logged into Azure CLI"
    echo "Please run: az login"
    exit 1
fi

echo "✅ Azure CLI authenticated"
echo ""

# Get Azure resources
RESOURCE_GROUP="rg-icon-generator"
COSMOS_ACCOUNT="cosmos-icon-generator"
STORAGE_ACCOUNT="sticongen"
OPENAI_ACCOUNT="openai-icon-generator"

echo "📦 Fetching Azure resource credentials..."
echo ""

# Get Cosmos DB key
echo "🔑 Cosmos DB:"
COSMOS_KEY=$(az cosmosdb keys list \
    --resource-group $RESOURCE_GROUP \
    --name $COSMOS_ACCOUNT \
    --query primaryMasterKey \
    --output tsv 2>/dev/null)

if [ -n "$COSMOS_KEY" ]; then
    echo "  ✅ Primary key retrieved"
    echo "  Endpoint: https://$COSMOS_ACCOUNT.documents.azure.com:443/"
else
    echo "  ⚠️  Could not retrieve key (check permissions)"
fi

echo ""

# Get Storage connection string
echo "🗄️  Storage Account:"
STORAGE_CONN=$(az storage account show-connection-string \
    --resource-group $RESOURCE_GROUP \
    --name $STORAGE_ACCOUNT \
    --query connectionString \
    --output tsv 2>/dev/null)

if [ -n "$STORAGE_CONN" ]; then
    echo "  ✅ Connection string retrieved"
else
    echo "  ⚠️  Could not retrieve connection string"
fi

echo ""

# Get OpenAI key
echo "🤖 Azure OpenAI:"
OPENAI_KEY=$(az cognitiveservices account keys list \
    --resource-group $RESOURCE_GROUP \
    --name $OPENAI_ACCOUNT \
    --query key1 \
    --output tsv 2>/dev/null)

if [ -n "$OPENAI_KEY" ]; then
    echo "  ✅ API key retrieved"
    echo "  Endpoint: https://$OPENAI_ACCOUNT.openai.azure.com/"
else
    echo "  ⚠️  Could not retrieve API key"
fi

echo ""
echo "=================================================="
echo ""

# Update appsettings.Development.json
if [ -n "$COSMOS_KEY" ] && [ -n "$STORAGE_CONN" ] && [ -n "$OPENAI_KEY" ]; then
    echo "📝 Updating appsettings.Development.json..."

    # Use jq to update the JSON file
    if command -v jq &> /dev/null; then
        CONFIG_FILE="IconGenerator.WebApi/appsettings.Development.json"

        jq --arg cosmosKey "$COSMOS_KEY" \
           --arg storageConn "$STORAGE_CONN" \
           --arg openaiKey "$OPENAI_KEY" \
           '.Database.CosmosKey = $cosmosKey | .Storage.ConnectionString = $storageConn | .AzureOpenAI.ApiKey = $openaiKey' \
           $CONFIG_FILE > $CONFIG_FILE.tmp && mv $CONFIG_FILE.tmp $CONFIG_FILE

        echo "  ✅ Configuration updated!"
    else
        echo "  ⚠️  jq not found. Please update appsettings.Development.json manually"
        echo ""
        echo "Database.CosmosKey: $COSMOS_KEY"
        echo "Storage.ConnectionString: $STORAGE_CONN"
        echo "AzureOpenAI.ApiKey: $OPENAI_KEY"
    fi
else
    echo "⚠️  Some credentials missing. Please update appsettings.Development.json manually:"
    echo ""
    [ -n "$COSMOS_KEY" ] && echo "Database.CosmosKey: $COSMOS_KEY"
    [ -n "$STORAGE_CONN" ] && echo "Storage.ConnectionString: $STORAGE_CONN"
    [ -n "$OPENAI_KEY" ] && echo "AzureOpenAI.ApiKey: $OPENAI_KEY"
fi

echo ""
echo "=================================================="
echo ""
echo "🎉 Setup complete! You can now:"
echo ""
echo "  1. Run the API:"
echo "     cd IconGenerator.WebApi && dotnet run"
echo ""
echo "  2. Or open in VS Code:"
echo "     code . && press F5"
echo ""
echo "  3. Or open in Visual Studio/Rider:"
echo "     open IconGenerator.WebApi.sln"
echo ""
echo "  4. Test the API:"
echo "     curl http://localhost:5199/health"
echo ""
echo "  5. View Swagger:"
echo "     open http://localhost:5199/swagger"
echo ""
echo "📝 Note: For Stripe keys, update appsettings.Development.json manually"
echo "   Get them from: https://dashboard.stripe.com/test/apikeys"
echo ""
