#!/bin/bash

# =============================================================================
# Azure Resources Provisioning Script for Icon Generator
# Subscription: Azure Sponsorship PROD
# =============================================================================

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
SUBSCRIPTION_ID="5fddbeee-d040-44ad-a7a0-a526d45d98a2"
SUBSCRIPTION_NAME="Azure Sponsorship PROD"
RESOURCE_GROUP="rg-icon-generator"
LOCATION="eastus"           # Primary location for OpenAI and Storage
COSMOS_LOCATION="westus2"   # Different region for Cosmos DB (East US had capacity issues)

# Static resource names (no random suffixes)
OPENAI_NAME="openai-icon-generator"
COSMOS_NAME="cosmos-icon-generator"
STORAGE_NAME="sticongen"

# Output file for credentials
OUTPUT_FILE="azure-credentials.env"

echo -e "${BLUE}=============================================${NC}"
echo -e "${BLUE}  Icon Generator - Azure Provisioning${NC}"
echo -e "${BLUE}=============================================${NC}"
echo ""
echo -e "${CYAN}Subscription:${NC} ${SUBSCRIPTION_NAME}"
echo -e "${CYAN}Subscription ID:${NC} ${SUBSCRIPTION_ID}"
echo -e "${CYAN}Location:${NC} ${LOCATION}"
echo ""
echo -e "${GREEN}Resource Names (Static):${NC}"
echo "  • OpenAI: ${OPENAI_NAME} (${LOCATION})"
echo "  • Cosmos DB: ${COSMOS_NAME} (${COSMOS_LOCATION})"
echo "  • Storage: ${STORAGE_NAME} (${LOCATION})"
echo ""
echo -e "${YELLOW}This script will create or verify:${NC}"
echo "  • Resource Group"
echo "  • Azure OpenAI (with DALL-E 3 and GPT-4o-mini)"
echo "  • Cosmos DB (FREE TIER - \$0/month)"
echo "  • Storage Account"
echo ""
echo -e "${CYAN}Note: This script is idempotent - safe to run multiple times.${NC}"
echo -e "${CYAN}It will skip resources that already exist.${NC}"
echo ""
echo -e "${YELLOW}Estimated setup time: 10-15 minutes (first run)${NC}"
echo -e "${YELLOW}Estimated monthly cost: ~$10 (mostly DALL-E usage)${NC}"
echo ""
read -p "Press Enter to continue or Ctrl+C to cancel..."
echo ""

# =============================================================================
# Step 1: Login and Set Subscription
# =============================================================================

echo -e "${BLUE}[1/10] Logging in to Azure...${NC}"

# Check if already logged in
if ! az account show &>/dev/null; then
    echo "Not logged in. Opening browser for authentication..."
    az login
else
    echo -e "${GREEN}✓ Already logged in${NC}"
fi

echo ""
echo -e "${BLUE}[2/10] Setting active subscription...${NC}"
az account set --subscription "${SUBSCRIPTION_ID}"

# Verify subscription
CURRENT_SUB=$(az account show --query "name" -o tsv)
echo -e "${GREEN}✓ Active subscription: ${CURRENT_SUB}${NC}"

if [ "$CURRENT_SUB" != "$SUBSCRIPTION_NAME" ]; then
    echo -e "${RED}Warning: Subscription name mismatch${NC}"
    echo "Expected: $SUBSCRIPTION_NAME"
    echo "Got: $CURRENT_SUB"
    read -p "Continue anyway? (y/N): " confirm
    if [[ ! $confirm =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

echo ""

# =============================================================================
# Step 2: Register Required Providers
# =============================================================================

echo -e "${BLUE}[3/10] Registering Azure resource providers...${NC}"

echo "Registering Microsoft.CognitiveServices..."
az provider register --namespace Microsoft.CognitiveServices --wait
echo -e "${GREEN}✓ Microsoft.CognitiveServices registered${NC}"

echo "Registering Microsoft.DocumentDB..."
az provider register --namespace Microsoft.DocumentDB --wait
echo -e "${GREEN}✓ Microsoft.DocumentDB registered${NC}"

echo "Registering Microsoft.Storage..."
az provider register --namespace Microsoft.Storage --wait
echo -e "${GREEN}✓ Microsoft.Storage registered${NC}"

echo ""

# =============================================================================
# Step 3: Create Resource Group
# =============================================================================

echo -e "${BLUE}[4/10] Creating resource group...${NC}"

if az group show --name "${RESOURCE_GROUP}" &>/dev/null; then
    echo -e "${YELLOW}Resource group already exists${NC}"
else
    az group create \
        --name "${RESOURCE_GROUP}" \
        --location "${LOCATION}" \
        --output table
    echo -e "${GREEN}✓ Resource group created: ${RESOURCE_GROUP}${NC}"
fi

echo ""

# =============================================================================
# Step 4: Create Azure OpenAI Service
# =============================================================================

echo -e "${BLUE}[5/10] Creating Azure OpenAI service...${NC}"
echo "Name: ${OPENAI_NAME}"

if az cognitiveservices account show --name "${OPENAI_NAME}" --resource-group "${RESOURCE_GROUP}" &>/dev/null; then
    echo -e "${YELLOW}Azure OpenAI service already exists, skipping creation${NC}"
else
    echo "This may take 2-3 minutes..."
    az cognitiveservices account create \
        --name "${OPENAI_NAME}" \
        --resource-group "${RESOURCE_GROUP}" \
        --kind OpenAI \
        --sku S0 \
        --location "${LOCATION}" \
        --yes \
        --output table
    echo -e "${GREEN}✓ Azure OpenAI service created${NC}"
fi

# Get endpoint and key
echo "Getting endpoint and API key..."
OPENAI_ENDPOINT=$(az cognitiveservices account show \
    --name "${OPENAI_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --query "properties.endpoint" \
    --output tsv)

OPENAI_KEY=$(az cognitiveservices account keys list \
    --name "${OPENAI_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --query "key1" \
    --output tsv)

echo -e "${GREEN}✓ Endpoint: ${OPENAI_ENDPOINT}${NC}"
echo -e "${GREEN}✓ API Key: ${OPENAI_KEY:0:20}...${NC}"

echo ""

# =============================================================================
# Step 5: Deploy DALL-E 3 Model
# =============================================================================

echo -e "${BLUE}[6/10] Deploying DALL-E 3 model...${NC}"

# Check if DALL-E 3 deployment already exists
if az cognitiveservices account deployment show \
    --name "${OPENAI_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --deployment-name "dall-e-3" &>/dev/null; then
    echo -e "${YELLOW}DALL-E 3 deployment already exists, skipping${NC}"
else
    echo "This may take 1-2 minutes..."
    az cognitiveservices account deployment create \
        --name "${OPENAI_NAME}" \
        --resource-group "${RESOURCE_GROUP}" \
        --deployment-name "dall-e-3" \
        --model-name "dall-e-3" \
        --model-version "3.0" \
        --model-format OpenAI \
        --sku-capacity 1 \
        --sku-name "Standard" \
        --output table || {
            echo -e "${YELLOW}⚠ CLI deployment failed. You'll need to deploy via Azure AI Foundry portal:${NC}"
            echo "  1. Go to https://ai.azure.com"
            echo "  2. Create/select project"
            echo "  3. Deploy DALL-E 3 model"
            echo "  4. Deployment name: dall-e-3"
            echo ""
            read -p "Press Enter after manual deployment or Ctrl+C to cancel..."
        }
    echo -e "${GREEN}✓ DALL-E 3 deployment created${NC}"
fi

echo ""

# =============================================================================
# Step 6: Deploy GPT-4o-mini Model (for Prompt Enhancement)
# =============================================================================

echo -e "${BLUE}[7/10] Deploying GPT-4o-mini model...${NC}"
echo -e "${CYAN}Using GPT-4o-mini (July 2024 release)${NC}"

# Check if GPT-4o-mini deployment already exists
if az cognitiveservices account deployment show \
    --name "${OPENAI_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --deployment-name "gpt-4o-mini" &>/dev/null; then
    echo -e "${YELLOW}GPT-4o-mini deployment already exists, skipping${NC}"
else
    echo "This may take 1-2 minutes..."
    az cognitiveservices account deployment create \
        --name "${OPENAI_NAME}" \
        --resource-group "${RESOURCE_GROUP}" \
        --deployment-name "gpt-4o-mini" \
        --model-name "gpt-4o-mini" \
        --model-version "2024-07-18" \
        --model-format OpenAI \
        --sku-capacity 10 \
        --sku-name "Standard" \
        --output table || {
            echo -e "${YELLOW}⚠ CLI deployment failed. You'll need to deploy via Azure AI Foundry portal:${NC}"
            echo "  1. Go to https://ai.azure.com"
            echo "  2. Click 'Deployments' → 'Deploy model'"
            echo "  3. Search for 'gpt-4o-mini'"
            echo "  4. Configure:"
            echo "     - Deployment name: gpt-4o-mini"
            echo "     - Model version: 2024-07-18"
            echo "     - Tokens per minute: 10K"
            echo "  5. Click 'Deploy'"
            echo ""
            read -p "Press Enter after manual deployment is complete..."
        }
    echo -e "${GREEN}✓ GPT-4o-mini deployment created${NC}"
fi

echo ""

# =============================================================================
# Step 7: Create Storage Account
# =============================================================================

echo -e "${BLUE}[8/10] Creating Storage Account...${NC}"
echo "Name: ${STORAGE_NAME}"

if az storage account show --name "${STORAGE_NAME}" --resource-group "${RESOURCE_GROUP}" &>/dev/null; then
    echo -e "${YELLOW}Storage account already exists, skipping creation${NC}"
else
    az storage account create \
        --name "${STORAGE_NAME}" \
        --resource-group "${RESOURCE_GROUP}" \
        --location "${LOCATION}" \
        --sku Standard_LRS \
        --kind StorageV2 \
        --output table
    echo -e "${GREEN}✓ Storage account created${NC}"
fi

# Get connection string
STORAGE_CONNECTION=$(az storage account show-connection-string \
    --name "${STORAGE_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --output tsv)

# Create blob container if it doesn't exist
echo "Checking blob container: generated-icons"
if az storage container exists \
    --name "generated-icons" \
    --connection-string "${STORAGE_CONNECTION}" \
    --output tsv | grep -q "True"; then
    echo -e "${YELLOW}Blob container already exists, skipping${NC}"
else
    az storage container create \
        --name "generated-icons" \
        --connection-string "${STORAGE_CONNECTION}" \
        --public-access blob \
        --output table
    echo -e "${GREEN}✓ Blob container created with public access${NC}"
fi

echo ""

# =============================================================================
# Step 8: Create Cosmos DB (FREE TIER)
# =============================================================================

echo -e "${BLUE}[9/10] Creating Cosmos DB (Free Tier)...${NC}"
echo "Name: ${COSMOS_NAME}"
echo "Location: ${COSMOS_LOCATION} (avoiding East US capacity issues)"

if az cosmosdb show --name "${COSMOS_NAME}" --resource-group "${RESOURCE_GROUP}" &>/dev/null; then
    echo -e "${YELLOW}Cosmos DB already exists, skipping creation${NC}"
else
    echo "This may take 3-5 minutes..."
    az cosmosdb create \
        --name "${COSMOS_NAME}" \
        --resource-group "${RESOURCE_GROUP}" \
        --locations regionName="${COSMOS_LOCATION}" failoverPriority=0 \
        --default-consistency-level Session \
        --enable-free-tier true \
        --output table
    echo -e "${GREEN}✓ Cosmos DB created with FREE TIER${NC}"
fi

# Get connection info
COSMOS_ENDPOINT=$(az cosmosdb show \
    --name "${COSMOS_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --query "documentEndpoint" \
    --output tsv)

COSMOS_KEY=$(az cosmosdb keys list \
    --name "${COSMOS_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --query "primaryMasterKey" \
    --output tsv)

echo -e "${GREEN}✓ Endpoint: ${COSMOS_ENDPOINT}${NC}"
echo -e "${GREEN}✓ Key obtained${NC}"

# Create database
echo "Checking database: IconGeneratorDB"
if az cosmosdb sql database show \
    --account-name "${COSMOS_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --name "IconGeneratorDB" &>/dev/null; then
    echo -e "${YELLOW}Database already exists, skipping${NC}"
else
    az cosmosdb sql database create \
        --account-name "${COSMOS_NAME}" \
        --resource-group "${RESOURCE_GROUP}" \
        --name "IconGeneratorDB" \
        --output table
    echo -e "${GREEN}✓ Database created${NC}"
fi

# Create containers
echo "Creating containers..."

for container in "Users:id" "Icons:userId" "Assets:userId" "Transactions:userId"; do
    CONTAINER_NAME=$(echo $container | cut -d: -f1)
    PARTITION_KEY=$(echo $container | cut -d: -f2)

    if az cosmosdb sql container show \
        --account-name "${COSMOS_NAME}" \
        --resource-group "${RESOURCE_GROUP}" \
        --database-name "IconGeneratorDB" \
        --name "${CONTAINER_NAME}" &>/dev/null; then
        echo -e "${YELLOW}Container ${CONTAINER_NAME} already exists, skipping${NC}"
    else
        az cosmosdb sql container create \
            --account-name "${COSMOS_NAME}" \
            --resource-group "${RESOURCE_GROUP}" \
            --database-name "IconGeneratorDB" \
            --name "${CONTAINER_NAME}" \
            --partition-key-path "/${PARTITION_KEY}" \
            --throughput 400 \
            --output table
        echo -e "${GREEN}✓ Container ${CONTAINER_NAME} created${NC}"
    fi
done

echo -e "${GREEN}✓ All containers ready${NC}"

echo ""

# =============================================================================
# Step 9: Save Credentials
# =============================================================================

echo -e "${BLUE}[10/10] Saving credentials...${NC}"

cat > "${OUTPUT_FILE}" << EOF
# =============================================================================
# Azure Icon Generator - Credentials
# Generated: $(date)
# Subscription: ${SUBSCRIPTION_NAME}
# =============================================================================

# Azure OpenAI Configuration
AZURE_OPENAI_ENDPOINT="${OPENAI_ENDPOINT}"
AZURE_OPENAI_API_KEY="${OPENAI_KEY}"
DALLE3_DEPLOYMENT_NAME="dall-e-3"
GPT4O_MINI_DEPLOYMENT_NAME="gpt-4o-mini"

# Cosmos DB Configuration (FREE TIER - \$0/month)
COSMOS_ENDPOINT="${COSMOS_ENDPOINT}"
COSMOS_KEY="${COSMOS_KEY}"
COSMOS_DATABASE="IconGeneratorDB"

# Storage Configuration
STORAGE_CONNECTION_STRING="${STORAGE_CONNECTION}"
STORAGE_CONTAINER_NAME="generated-icons"

# Resource Information
RESOURCE_GROUP="${RESOURCE_GROUP}"
LOCATION="${LOCATION}"
COSMOS_LOCATION="${COSMOS_LOCATION}"
OPENAI_RESOURCE_NAME="${OPENAI_NAME}"
COSMOS_ACCOUNT_NAME="${COSMOS_NAME}"
STORAGE_ACCOUNT_NAME="${STORAGE_NAME}"
EOF

echo -e "${GREEN}✓ Credentials saved to: ${OUTPUT_FILE}${NC}"

# Also create local.settings.json
echo "Creating local.settings.json..."

cat > "../api/local.settings.json" << EOF
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "AzureOpenAI__Endpoint": "${OPENAI_ENDPOINT}",
    "AzureOpenAI__ApiKey": "${OPENAI_KEY}",
    "AzureOpenAI__DallE3Deployment": "dall-e-3",
    "AzureOpenAI__Gpt4oMiniDeployment": "gpt-4o-mini",

    "Database__Type": "cosmosdb",
    "Database__CosmosEndpoint": "${COSMOS_ENDPOINT}",
    "Database__CosmosKey": "${COSMOS_KEY}",
    "Database__CosmosDatabase": "IconGeneratorDB",

    "Storage__ConnectionString": "${STORAGE_CONNECTION}",
    "Storage__ContainerName": "generated-icons",

    "Stripe__SecretKey": "",
    "Stripe__WebhookSecret": "",
    "Stripe__FrontendUrl": "http://localhost:5173",

    "AllowedOrigins": "http://localhost:5173,http://localhost:3000"
  }
}
EOF

echo -e "${GREEN}✓ local.settings.json created${NC}"

# Create .env for tests
cat > "../api/Tests/.env" << EOF
# Azure AI Foundry Configuration for Tests
# Generated: $(date)

AZURE_OPENAI_ENDPOINT=${OPENAI_ENDPOINT}
AZURE_OPENAI_API_KEY=${OPENAI_KEY}
DALLE3_DEPLOYMENT_NAME=dall-e-3
GPT4O_MINI_DEPLOYMENT_NAME=gpt-4o-mini
EOF

echo -e "${GREEN}✓ Tests/.env created${NC}"

echo ""

# =============================================================================
# Summary
# =============================================================================

echo -e "${GREEN}=============================================${NC}"
echo -e "${GREEN}  ✓ Provisioning Complete!${NC}"
echo -e "${GREEN}=============================================${NC}"
echo ""
echo -e "${CYAN}Resources Created:${NC}"
echo "  ✓ Resource Group: ${RESOURCE_GROUP}"
echo "  ✓ Azure OpenAI: ${OPENAI_NAME} (${LOCATION})"
echo "  ✓ Cosmos DB: ${COSMOS_NAME} (${COSMOS_LOCATION})"
echo "  ✓ Storage Account: ${STORAGE_NAME} (${LOCATION})"
echo ""
echo -e "${CYAN}Models Deployed:${NC}"
echo "  ✓ DALL-E 3 (dall-e-3)"
echo "  ✓ GPT-4o-mini (gpt-4o-mini) - July 2024 Release"
echo ""
echo -e "${CYAN}Cosmos DB Containers:${NC}"
echo "  ✓ Users"
echo "  ✓ Icons"
echo "  ✓ Assets"
echo "  ✓ Transactions"
echo ""
echo -e "${CYAN}Credentials Files:${NC}"
echo "  ✓ ${OUTPUT_FILE}"
echo "  ✓ ../api/local.settings.json"
echo "  ✓ ../api/Tests/.env"
echo ""
echo -e "${YELLOW}⚠ IMPORTANT: Keep credentials secure!${NC}"
echo -e "${YELLOW}   Do NOT commit ${OUTPUT_FILE} or local.settings.json to git${NC}"
echo ""
echo -e "${CYAN}Next Steps:${NC}"
echo ""
echo "1. Verify deployments in Azure AI Foundry:"
echo "   ${BLUE}https://ai.azure.com${NC}"
echo ""
echo "2. Test the setup:"
echo "   ${GREEN}cd ../api/Tests${NC}"
echo "   ${GREEN}source .env${NC}"
echo "   ${GREEN}dotnet build${NC}"
echo "   ${GREEN}dotnet run${NC}"
echo ""
echo "3. View your credentials:"
echo "   ${GREEN}cat ${OUTPUT_FILE}${NC}"
echo ""
echo -e "${CYAN}Monthly Cost Estimate:${NC}"
echo "  • Cosmos DB FREE TIER: \$0 (1000 RU/s + 25GB free)"
echo "  • Storage: ~\$1-2/month"
echo "  • Azure OpenAI: Pay per use"
echo "    - GPT-4o-mini: ~\$0.15 per 1M input tokens"
echo "    - DALL-E 3: \$0.04 per image (standard)"
echo "  • Total: ~\$5-10/month for development"
echo ""
echo -e "${GREEN}Happy icon generating! 🎨✨${NC}"
