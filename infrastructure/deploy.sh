#!/bin/bash

# ==============================================
# Icon Generator - Infrastructure Deployment Script
# ==============================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Functions
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
    print_info "Checking prerequisites..."

    # Check Azure CLI
    if ! command -v az &> /dev/null; then
        print_error "Azure CLI is not installed. Please install it first."
        exit 1
    fi

    # Check if logged in
    if ! az account show &> /dev/null; then
        print_error "Not logged in to Azure. Run 'az login' first."
        exit 1
    fi

    print_info "Prerequisites check passed ✓"
}

# Parse arguments
ENVIRONMENT=${1:-dev}
LOCATION=${2:-eastus}
RESOURCE_GROUP="rg-icon-generator"
DEPLOYMENT_NAME="infrastructure-${ENVIRONMENT}-$(date +%Y%m%d-%H%M%S)"

# Validate environment
if [[ ! "$ENVIRONMENT" =~ ^(dev|staging|main)$ ]]; then
    print_error "Invalid environment. Use: dev, staging, or main"
    exit 1
fi

# Banner
echo "=========================================="
echo "  Icon Generator - Infrastructure Deploy"
echo "=========================================="
echo ""
echo "Environment:      ${ENVIRONMENT}"
echo "Location:         ${LOCATION}"
echo "Resource Group:   ${RESOURCE_GROUP}"
echo "Deployment:       ${DEPLOYMENT_NAME}"
echo ""

# Confirm deployment
read -p "Continue with deployment? (y/N) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    print_warn "Deployment cancelled."
    exit 0
fi

# Start deployment
print_info "Starting deployment..."

# Create resource group
print_info "Creating resource group..."
az group create \
    --name "${RESOURCE_GROUP}" \
    --location "${LOCATION}" \
    --tags Environment="${ENVIRONMENT}" Project=IconGenerator ManagedBy=Bicep

# Validate templates
print_info "Validating Bicep templates..."
VALIDATION_OUTPUT=$(az deployment group validate \
    --resource-group "${RESOURCE_GROUP}" \
    --template-file ./main.bicep \
    --parameters "./parameters.${ENVIRONMENT}.json" \
    2>&1)

if [ $? -ne 0 ]; then
    print_error "Template validation failed:"
    echo "${VALIDATION_OUTPUT}"
    exit 1
fi
print_info "Template validation passed ✓"

# Deploy infrastructure
print_info "Deploying infrastructure..."
print_warn "This may take 5-10 minutes..."

az deployment group create \
    --resource-group "${RESOURCE_GROUP}" \
    --template-file ./main.bicep \
    --parameters "./parameters.${ENVIRONMENT}.json" \
    --name "${DEPLOYMENT_NAME}" \
    --verbose

if [ $? -ne 0 ]; then
    print_error "Deployment failed!"
    exit 1
fi

# Get outputs
print_info "Retrieving deployment outputs..."
OUTPUTS=$(az deployment group show \
    --resource-group "${RESOURCE_GROUP}" \
    --name "${DEPLOYMENT_NAME}" \
    --query properties.outputs \
    --output json)

# Save outputs to file
OUTPUT_FILE="outputs.${ENVIRONMENT}.json"
echo "${OUTPUTS}" > "${OUTPUT_FILE}"
print_info "Outputs saved to ${OUTPUT_FILE}"

# Display key outputs
echo ""
echo "=========================================="
echo "  Deployment Complete!"
echo "=========================================="
echo ""

# Parse and display important outputs
FUNCTION_APP_NAME=$(echo "${OUTPUTS}" | jq -r '.functionAppName.value')
FUNCTION_APP_URL=$(echo "${OUTPUTS}" | jq -r '.functionAppUrl.value')
STATIC_WEB_APP_URL=$(echo "${OUTPUTS}" | jq -r '.staticWebAppUrl.value')
COSMOS_ENDPOINT=$(echo "${OUTPUTS}" | jq -r '.cosmosEndpoint.value')
OPENAI_ENDPOINT=$(echo "${OUTPUTS}" | jq -r '.openAIEndpoint.value')

echo "Function App:"
echo "  Name: ${FUNCTION_APP_NAME}"
echo "  URL:  ${FUNCTION_APP_URL}"
echo ""
echo "Static Web App:"
echo "  URL:  ${STATIC_WEB_APP_URL}"
echo ""
echo "Cosmos DB:"
echo "  Endpoint: ${COSMOS_ENDPOINT}"
echo ""
echo "Azure OpenAI:"
echo "  Endpoint: ${OPENAI_ENDPOINT}"
echo ""

# Post-deployment tasks
print_warn "Post-deployment tasks required:"
echo "  1. Deploy AI models via Azure AI Foundry portal (https://ai.azure.com)"
echo "     - DALL-E 3: dalle3-icon-generator"
echo "     - GPT-4o-mini: gpt-4o-mini-prompts"
echo ""
echo "  2. Initialize Cosmos DB (run: ./scripts/init-cosmos.sh ${ENVIRONMENT})"
echo ""
echo "  3. Configure application secrets in Azure DevOps Library"
echo ""

print_info "Deployment completed successfully ✓"
