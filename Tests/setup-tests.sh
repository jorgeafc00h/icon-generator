#!/bin/bash

# Icon Generator - Integration Tests Setup Script
# This script helps you configure environment variables for testing

set -e

echo "================================================"
echo "  Icon Generator - Integration Tests Setup"
echo "================================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if running from Tests directory
if [ ! -f "setup-tests.sh" ]; then
    echo -e "${RED}Error: Please run this script from the api/Tests directory${NC}"
    echo "cd /Users/jorgeflores/github/icon-generator/api/Tests"
    exit 1
fi

echo -e "${BLUE}This script will help you configure Azure AI Foundry credentials${NC}"
echo ""
echo "You'll need:"
echo "  1. Azure OpenAI Endpoint (from Azure AI Foundry)"
echo "  2. Azure OpenAI API Key"
echo "  3. DALL-E 3 deployment name"
echo "  4. GPT-4o-mini deployment name"
echo ""
echo -e "${YELLOW}Tip: Have the Azure AI Foundry setup guide open:${NC}"
echo "Docs/AZURE_AI_FOUNDRY_SETUP.md"
echo ""

read -p "Press Enter to continue or Ctrl+C to cancel..."
echo ""

# Function to prompt for input with default
prompt_with_default() {
    local var_name=$1
    local prompt_text=$2
    local default_value=$3
    local current_value=$(eval echo \$$var_name)

    if [ -n "$current_value" ]; then
        echo -e "${GREEN}✓ $var_name already set: $current_value${NC}"
    else
        read -p "$prompt_text [$default_value]: " input_value
        input_value=${input_value:-$default_value}
        export $var_name="$input_value"
        echo -e "${GREEN}✓ Set $var_name${NC}"
    fi
}

# Check if .env file exists
if [ -f ".env" ]; then
    echo -e "${YELLOW}Found existing .env file. Loading...${NC}"
    source .env
    echo ""
fi

# Prompt for each variable
echo -e "${BLUE}=== Azure OpenAI Configuration ===${NC}"
echo ""

prompt_with_default "AZURE_OPENAI_ENDPOINT" \
    "Azure OpenAI Endpoint (e.g., https://your-resource.openai.azure.com/)" \
    "https://your-resource.openai.azure.com/"

prompt_with_default "AZURE_OPENAI_API_KEY" \
    "Azure OpenAI API Key" \
    "your-api-key-here"

prompt_with_default "DALLE3_DEPLOYMENT_NAME" \
    "DALL-E 3 Deployment Name" \
    "dall-e-3"

prompt_with_default "GPT4O_MINI_DEPLOYMENT_NAME" \
    "GPT-4o-mini Deployment Name" \
    "gpt-4o-mini"

echo ""
echo -e "${BLUE}=== Saving Configuration ===${NC}"
echo ""

# Create .env file
cat > .env << EOF
# Azure AI Foundry Configuration
# Generated: $(date)

AZURE_OPENAI_ENDPOINT=$AZURE_OPENAI_ENDPOINT
AZURE_OPENAI_API_KEY=$AZURE_OPENAI_API_KEY
DALLE3_DEPLOYMENT_NAME=$DALLE3_DEPLOYMENT_NAME
GPT4O_MINI_DEPLOYMENT_NAME=$GPT4O_MINI_DEPLOYMENT_NAME
EOF

echo -e "${GREEN}✓ Configuration saved to .env${NC}"
echo ""

# Also export for current session
export AZURE_OPENAI_ENDPOINT
export AZURE_OPENAI_API_KEY
export DALLE3_DEPLOYMENT_NAME
export GPT4O_MINI_DEPLOYMENT_NAME

echo -e "${BLUE}=== Testing Configuration ===${NC}"
echo ""

# Validate endpoint format
if [[ ! $AZURE_OPENAI_ENDPOINT =~ ^https://.*\.openai\.azure\.com/?$ ]]; then
    echo -e "${YELLOW}⚠ Warning: Endpoint format looks unusual${NC}"
    echo "  Expected: https://your-resource.openai.azure.com/"
    echo "  Got: $AZURE_OPENAI_ENDPOINT"
fi

# Check if API key looks valid (should be 64 chars)
if [ ${#AZURE_OPENAI_API_KEY} -lt 32 ]; then
    echo -e "${YELLOW}⚠ Warning: API key seems too short${NC}"
    echo "  Azure OpenAI keys are typically 64 characters"
fi

echo -e "${GREEN}✓ Basic validation passed${NC}"
echo ""

echo -e "${BLUE}=== Quick Test (Optional) ===${NC}"
echo ""
echo "Would you like to run a quick connectivity test?"
echo "This will attempt to call GPT-4o-mini (costs ~$0.0001)"
echo ""
read -p "Run test? (y/N): " run_test

if [[ $run_test =~ ^[Yy]$ ]]; then
    echo ""
    echo "Testing Azure OpenAI connection..."

    # Simple curl test
    RESPONSE=$(curl -s -w "\n%{http_code}" \
        "${AZURE_OPENAI_ENDPOINT}openai/deployments/${GPT4O_MINI_DEPLOYMENT_NAME}/chat/completions?api-version=2024-02-01" \
        -H "Content-Type: application/json" \
        -H "api-key: ${AZURE_OPENAI_API_KEY}" \
        -d '{
            "messages": [{"role": "user", "content": "Say hello"}],
            "max_tokens": 10
        }')

    HTTP_CODE=$(echo "$RESPONSE" | tail -n 1)
    BODY=$(echo "$RESPONSE" | head -n -1)

    if [ "$HTTP_CODE" = "200" ]; then
        echo -e "${GREEN}✓ Connection successful!${NC}"
        echo "Response: $BODY" | jq -r '.choices[0].message.content' 2>/dev/null || echo "$BODY"
    else
        echo -e "${RED}✗ Connection failed (HTTP $HTTP_CODE)${NC}"
        echo "Response: $BODY"
        echo ""
        echo "Common issues:"
        echo "  - Wrong endpoint or API key"
        echo "  - Deployment name mismatch"
        echo "  - Resource not provisioned yet"
        echo ""
        echo "See Docs/AZURE_AI_FOUNDRY_SETUP.md for help"
    fi
else
    echo "Skipping connectivity test"
fi

echo ""
echo -e "${BLUE}=== Next Steps ===${NC}"
echo ""
echo "1. Load environment variables (run in your terminal):"
echo -e "   ${GREEN}source .env${NC}"
echo ""
echo "2. Build the test project:"
echo -e "   ${GREEN}dotnet build${NC}"
echo ""
echo "3. Run integration tests:"
echo -e "   ${GREEN}dotnet run${NC}"
echo ""
echo "4. Select a test from the menu (recommend starting with #1)"
echo ""
echo -e "${YELLOW}Note: Tests will ONLY enhance prompts (cheap: ~$0.0001 each)${NC}"
echo -e "${YELLOW}      Image generation is commented out to save costs${NC}"
echo ""
echo "See api/Tests/README.md for more information"
echo ""
echo -e "${GREEN}Setup complete! 🎉${NC}"
