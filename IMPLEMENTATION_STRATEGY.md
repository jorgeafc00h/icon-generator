# AI-Powered Icon Generator - Implementation Guide

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Azure AI Foundry Setup](#azure-ai-foundry-setup)
- [Development Environment Setup](#development-environment-setup)
- [Backend Implementation](#backend-implementation)
- [Frontend Implementation](#frontend-implementation)
- [Deployment](#deployment)
- [Testing](#testing)
- [Optimization](#optimization)
- [Cost Management](#cost-management)
- [Appendix](#appendix)

---

## Overview

This implementation guide provides step-by-step instructions for building an AI-powered icon generator platform similar to CandyIcons, using Azure AI Foundry (DALL-E 3), React, and Azure Static Web Apps.

### UI/UX Reference Sources

This platform's design is inspired by the following successful icon generator platforms:

**Primary Reference**: [CandyIcons](https://www.candyicons.com/)
- Playful, approachable design with candy-themed branding
- Clean three-step workflow: Describe → Choose Colors → Select Style
- 18+ style options displayed in an organized grid
- Social proof through user ratings and testimonials
- Dual discovery paths: generator form + browsable gallery
- Progressive disclosure to reduce cognitive load

**Secondary Reference**: [App Alchemy](https://appalchemy.ai/)
- Modern, professional color scheme (blues and neutrals)
- Minimalist header with clear CTAs
- Four-step numbered workflow with visual connectors
- Emphasis on speed and simplicity for entrepreneurs
- Video demonstrations to reduce friction
- Gradient overlays for depth and visual interest

### Key Features

- **AI-Powered Generation**: DALL-E 3 for creating unique app icons
- **18+ Style Options**: Retro, 3D, Clay, Abstract, Minimal, Gradient, Pixel, etc.
- **Color Customization**: User-defined color schemes
- **Multiple Formats**: PNG export with iOS/macOS asset generation
- **Credit System**: Pay-per-use model with secure payment processing
- **Full Ownership**: Users retain complete copyright

### Business Model

- **Starter Package**: $12 for 10 credits
- **Pro Package**: $29 for 50 credits
- **Business Package**: $49 for 150 credits
- **Margin**: 66-76% gross profit per package

---

## UI/UX Design Guidelines

> **Note**: This project uses the `.claude/interface-design` skill for all interface design work. The guidelines below integrate reference analysis with our craft foundations.

Based on analysis of CandyIcons and App Alchemy, combined with our interface design principles:

### Layout & Navigation

**Header Design**:
- Minimal top navigation with logo (left), primary links (center), and CTA buttons (right)
- "Buy Credits" button in vibrant color (primary action)
- "Log in" link in subtle text (secondary action)
- Optional hamburger menu for mobile responsiveness

**Footer Structure**:
- Three-column layout: Product/Features, Company/Legal, Social/Newsletter
- Keep it concise with essential links only
- Include trust badges or certifications if applicable

### Design Direction

**Before implementing, we must:**
1. Define the product domain (concepts, metaphors, vocabulary)
2. Explore the color world (what colors exist naturally in this icon generator's domain?)
3. Identify a signature element (what could only exist for THIS product?)
4. Name and reject defaults (avoid generic dashboard templates)

**Color Scheme Directions** (to be refined based on domain exploration):

**Option 1 - Playful Creative (CandyIcons-inspired)**:
- **Domain**: Art studio, creative workshop, design table
- **Color world**: Paint swatches, artist palettes, creative chaos
- **Feel**: Warm, approachable, inspiring
- **Primaries**: Vibrant but not harsh (terracotta #E86252, warm teal #2DD4BF)
- **Surfaces**: Warm neutrals with subtle warmth bias
- **Best for**: Consumer-focused, creative applications

**Option 2 - Professional Precision (App Alchemy-inspired)**:
- **Domain**: Design system, blueprint, precision tool
- **Color world**: Blueprint blues, technical grays, digital screens
- **Feel**: Clean, efficient, trustworthy
- **Primaries**: Cool blues (#3B82F6, #2563EB)
- **Surfaces**: Cool gray scale with blue undertones
- **Best for**: Business/enterprise targeting

### Typography

- **Headlines**: Bold, 32-48px for hero sections
- **Subheadings**: Semi-bold, 20-24px
- **Body**: Regular, 15-16px in gray-700 for readability
- **CTAs**: Bold, 16-18px in button text
- Font stack: Inter, SF Pro, or similar modern sans-serif

### Icon Generator Workflow

**Three-Step Process** (CandyIcons model):
1. **Describe**: Text input with placeholder examples
   - Include character counter (e.g., "0/100 characters")
   - Optional: "Suggest" button for AI-assisted ideation
2. **Choose Colors**: Visual palette selector
   - Show 2-3 color wells with color pickers
   - Provide preset color schemes for quick selection
3. **Select Style**: Grid of style cards
   - 3-4 columns on desktop, 2 on tablet, 1 on mobile
   - Visual thumbnails showing style examples
   - Clear selected state with border/background change

**Alternative: Four-Step Process** (App Alchemy model):
1. Describe → 2. Choose Style → 3. Select Colors → 4. Review/Generate

### Component Design

**Style Selector Cards**:
```
┌─────────────────────┐
│   [Preview Image]   │
│                     │
│   Style Name        │
│   (e.g., "3D")      │
└─────────────────────┘
```
- Hover effect: subtle elevation or border color change
- Selected state: blue border + background tint
- Grid gap: 12-16px

**Color Picker**:
- Large circular color wells (48-64px diameter)
- Click to open native color picker
- Display hex code below each well
- Preset palettes in expandable section

**Primary CTA Button**:
- Full-width on mobile, auto-width on desktop
- Height: 48-56px for easy tapping
- Clear text: "Generate Icon (1 credit)" or "Create Icon"
- Disabled state when form incomplete
- Loading state with spinner during generation

### Preview & Results

**Generated Icon Display**:
- Large preview (400-600px square on desktop)
- White/gray background with subtle shadow
- Download button directly below preview
- "Export Assets" button for app resources
- Show remaining credits prominently

**Social Proof Elements**:
- User count: "Used by 10,000+ creators"
- Star rating display
- 2-3 testimonial cards with user photos/names
- Product Hunt badge if applicable

### Additional UX Patterns

**Progressive Disclosure**:
- Show basic options first, hide advanced in collapsible section
- Reduce initial form fields to minimize friction
- Use tooltips for technical terms

**Feedback & Loading States**:
- Inline validation for text input
- Progress indicator during generation (10-30 seconds)
- Success state with animation when complete
- Error states with clear messaging and recovery options

**Gallery/Inspiration Section** (optional):
- Grid of pre-generated icons below main generator
- Categories for browsing
- Click to use as starting point
- Lazy loading for performance

### Responsive Design Breakpoints

- Mobile: < 640px (1 column, stacked layout)
- Tablet: 640px - 1024px (2 columns)
- Desktop: > 1024px (3-4 columns, side-by-side layout)

### Craft Foundations (From `.claude/interface-design`)

**Subtle Layering** (Critical):
- Surfaces must be barely different but distinguishable (Vercel/Supabase style)
- Borders should be light but not invisible (rgba with low opacity)
- Squint test: hierarchy visible, but nothing harsh
- Dark mode: higher elevation = slightly lighter surfaces
- No dramatic jumps between surface levels

**Surface Token Architecture**:
```
surface-0: Base canvas
surface-100: Cards, panels (same plane)
surface-200: Dropdowns, overlays (floating)
surface-300: Nested overlays
```

**Text Hierarchy**:
```
foreground: Primary text (highest contrast)
foreground-secondary: Supporting text
foreground-tertiary: Metadata, timestamps
foreground-muted: Disabled, placeholders
```

**Border Progression**:
```
border-default: Standard separation
border-subtle: Softer boundaries
border-strong: Emphasis, hover states
border-overlay: Floating components (dropdowns)
```

**Depth Strategy** (Choose ONE):
- **Borders-only**: Clean, technical (Linear, Raycast)
- **Subtle shadows**: Soft lift for approachability
- **Layered shadows**: Premium feel (Stripe)

**Spacing System**:
- Base unit: 4px or 8px
- All spacing must be multiples of base
- Symmetrical padding (TLBR must match)

### Accessibility

- WCAG 2.1 AA compliance minimum
- Keyboard navigation for all interactive elements
- ARIA labels for icon buttons
- Sufficient color contrast (4.5:1 for text)
- Focus indicators on all interactive elements

### Design Implementation Workflow

1. **Product Domain Exploration** (Required before any code):
   - 5+ domain concepts specific to icon generation
   - 5+ colors from the product's natural world
   - One signature element unique to this product
   - 3 defaults to explicitly reject

2. **Proposal & Confirmation**:
   - Present direction referencing domain exploration
   - Get user buy-in before building

3. **Build with Intent**:
   - Every choice must be defensible
   - Run mandate checks before presenting
   - Offer to save patterns to `.interface-design/system.md`

---

## Architecture

### System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         User Browser                             │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │            React Frontend (Static Web App)                 │  │
│  │  - Icon Generator UI                                       │  │
│  │  - Style & Color Selector                                  │  │
│  │  - Credit Dashboard                                        │  │
│  │  - Asset Export Tools                                      │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTPS
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Azure Functions (API)                       │
│  ┌────────────────┐  ┌────────────────┐  ┌─────────────────┐   │
│  │ Generate Icon  │  │ Purchase Credits│  │  Get User Data  │   │
│  │   Endpoint     │  │   Endpoint      │  │    Endpoint     │   │
│  └────────────────┘  └────────────────┘  └─────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┼─────────────┐
                │             │             │
                ▼             ▼             ▼
    ┌────────────────┐  ┌──────────┐  ┌──────────────┐
    │  Azure AI      │  │ Cosmos   │  │ Blob Storage │
    │  Foundry       │  │   DB     │  │              │
    │  (DALL-E 3)    │  │          │  │              │
    └────────────────┘  └──────────┘  └──────────────┘
         GPT-4o-mini      User Data     Generated
         (Prompt Opt)     Credits       Images
```

### Data Flow

1. **User Input**: User provides keywords, selects style & colors
2. **Prompt Enhancement**: GPT-4o-mini optimizes the prompt
3. **Image Generation**: DALL-E 3 generates the icon
4. **Post-Processing**: Resize, optimize, create transparent background
5. **Storage**: Save to Blob Storage with CDN caching
6. **Credit Management**: Update user's credit balance in Cosmos DB
7. **Delivery**: Return download URLs to frontend

---

## Technology Stack

### Frontend
- **React** 18.2+ with TypeScript
- **Vite** for build tooling
- **Tailwind CSS** for styling
- **shadcn/ui** for component library
- **Zustand** for state management
- **React Query** for API data fetching
- **Canvas API** for image preview/manipulation

### Backend
- **Azure Functions** (Node.js 18+)
- **TypeScript** for type safety
- **@azure/openai** SDK for AI Foundry
- **@azure/storage-blob** for image storage
- **@azure/cosmos** for database operations
- **Stripe** SDK for payments

### Infrastructure
- **Azure Static Web Apps** (hosting)
- **Azure Functions** (serverless compute)
- **Azure AI Foundry** (AI models)
- **Azure Cosmos DB** (NoSQL database)
- **Azure Blob Storage** (file storage)
- **Azure CDN** (content delivery)
- **Azure AD B2C** (authentication)
- **GitHub Actions** (CI/CD)

---

## Azure AI Foundry Setup

### Prerequisites

1. Active Azure subscription
2. Azure CLI installed
3. Node.js 18+ and npm

### Step 1: Create Azure AI Foundry Resource

```bash
# Login to Azure
az login

# Set subscription
az account set --subscription "YOUR_SUBSCRIPTION_ID"

# Create resource group
az group create \
  --name rg-icon-generator \
  --location eastus

# Create Azure OpenAI resource
az cognitiveservices account create \
  --name openai-icon-generator \
  --resource-group rg-icon-generator \
  --kind OpenAI \
  --sku S0 \
  --location eastus \
  --yes
```

### Step 2: Deploy DALL-E 3 Model

Navigate to Azure AI Foundry portal: https://ai.azure.com

1. Create a new project
2. Go to **Models + endpoints**
3. Click **Deploy model**
4. Select **dall-e-3** from the list
5. Configure deployment:
   - **Deployment name**: `dalle3-icon-generator`
   - **Model version**: Latest
   - **Deployment type**: Standard
6. Complete deployment

### Step 3: Deploy GPT-4o-mini for Prompt Enhancement

1. In **Models + endpoints**, click **Deploy model**
2. Select **gpt-4o-mini**
3. Configure deployment:
   - **Deployment name**: `gpt-4o-mini-prompts`
   - **Deployment type**: Standard
   - **Tokens per minute**: 10K (adjust based on needs)

### Step 4: Get API Keys and Endpoint

```bash
# Get the endpoint
az cognitiveservices account show \
  --name openai-icon-generator \
  --resource-group rg-icon-generator \
  --query "properties.endpoint" \
  --output tsv

# Get the API key
az cognitiveservices account keys list \
  --name openai-icon-generator \
  --resource-group rg-icon-generator \
  --query "key1" \
  --output tsv
```

Save these values for later use:
- `AZURE_OPENAI_ENDPOINT`: Your endpoint URL
- `AZURE_OPENAI_API_KEY`: Your API key
- `DALLE3_DEPLOYMENT_NAME`: `dalle3-icon-generator`
- `GPT4O_MINI_DEPLOYMENT_NAME`: `gpt-4o-mini-prompts`

---

## Development Environment Setup

### Step 1: Create Project Structure

```bash
# Create main project directory
mkdir icon-generator-platform
cd icon-generator-platform

# Create frontend (React)
npm create vite@latest frontend -- --template react-ts
cd frontend
npm install

# Install dependencies
npm install @tanstack/react-query zustand tailwindcss postcss autoprefixer
npm install @stripe/stripe-js axios date-fns clsx tailwind-merge
npm install -D @types/node

# Initialize Tailwind
npx tailwindcss init -p

# Go back to root
cd ..

# Create backend (Azure Functions)
mkdir api
cd api
npm init -y
npm install --save-dev @azure/functions typescript @types/node
npm install @azure/openai @azure/storage-blob @azure/cosmos stripe dotenv
npm install sharp # for image processing

# Initialize TypeScript
npx tsc --init
```

### Step 2: Configure Tailwind CSS

Update `frontend/tailwind.config.js`:

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
        }
      }
    },
  },
  plugins: [],
}
```

### Step 3: Environment Variables

Create `api/.env`:

```env
# Azure OpenAI
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_API_KEY=your-api-key-here
DALLE3_DEPLOYMENT_NAME=dalle3-icon-generator
GPT4O_MINI_DEPLOYMENT_NAME=gpt-4o-mini-prompts

# Azure Storage
AZURE_STORAGE_CONNECTION_STRING=your-connection-string
STORAGE_CONTAINER_NAME=generated-icons

# Cosmos DB
COSMOS_ENDPOINT=https://your-cosmos.documents.azure.com:443/
COSMOS_KEY=your-cosmos-key
COSMOS_DATABASE=IconGeneratorDB
COSMOS_CONTAINER=Users

# Stripe
STRIPE_SECRET_KEY=sk_test_your_stripe_key
STRIPE_WEBHOOK_SECRET=whsec_your_webhook_secret

# App Settings
ALLOWED_ORIGINS=http://localhost:5173,https://your-domain.com
```

Create `frontend/.env`:

```env
VITE_API_BASE_URL=http://localhost:7071/api
VITE_STRIPE_PUBLISHABLE_KEY=pk_test_your_stripe_key
```

---

## Backend Implementation

### Azure Functions Project Structure

```
api/
├── src/
│   ├── functions/
│   │   ├── generateIcon.ts
│   │   ├── getUserData.ts
│   │   ├── purchaseCredits.ts
│   │   └── stripeWebhook.ts
│   ├── services/
│   │   ├── aiService.ts
│   │   ├── storageService.ts
│   │   ├── databaseService.ts
│   │   └── imageService.ts
│   ├── utils/
│   │   ├── promptBuilder.ts
│   │   └── validators.ts
│   └── types/
│       └── index.ts
├── host.json
├── local.settings.json
└── package.json
```

### 1. AI Service Implementation

Create `api/src/services/aiService.ts`:

```typescript
import { AzureOpenAI } from '@azure/openai';

export interface IconGenerationParams {
  keywords: string;
  style: string;
  colors: string[];
  quality?: 'standard' | 'hd';
}

export class AIService {
  private client: AzureOpenAI;
  private dalle3Deployment: string;
  private gptMiniDeployment: string;

  constructor() {
    this.client = new AzureOpenAI({
      endpoint: process.env.AZURE_OPENAI_ENDPOINT!,
      apiKey: process.env.AZURE_OPENAI_API_KEY!,
      apiVersion: '2024-02-01',
    });
    this.dalle3Deployment = process.env.DALLE3_DEPLOYMENT_NAME!;
    this.gptMiniDeployment = process.env.GPT4O_MINI_DEPLOYMENT_NAME!;
  }

  /**
   * Enhance user prompt using GPT-4o-mini
   */
  async enhancePrompt(params: IconGenerationParams): Promise<string> {
    const systemPrompt = `You are an expert at creating DALL-E prompts for app icons. 
Generate a detailed, optimized prompt based on user inputs. Focus on:
- Clear, centered composition
- Professional quality suitable for iOS/Android
- Specified style and color scheme
- Simple background
- Icon-appropriate design elements

Return ONLY the enhanced prompt, no explanations.`;

    const userPrompt = `Create a ${params.style} app icon prompt for: ${params.keywords}
Colors: ${params.colors.join(', ')}`;

    const response = await this.client.getChatCompletions(
      this.gptMiniDeployment,
      [
        { role: 'system', content: systemPrompt },
        { role: 'user', content: userPrompt },
      ],
      {
        temperature: 0.7,
        maxTokens: 200,
      }
    );

    return response.choices[0].message.content || params.keywords;
  }

  /**
   * Generate icon using DALL-E 3
   */
  async generateIcon(enhancedPrompt: string, quality: 'standard' | 'hd' = 'standard'): Promise<string> {
    const response = await this.client.getImages(
      this.dalle3Deployment,
      enhancedPrompt,
      {
        n: 1,
        size: '1024x1024',
        quality: quality,
        style: 'vivid', // or 'natural' for more realistic
      }
    );

    if (!response.data || response.data.length === 0) {
      throw new Error('No image generated');
    }

    return response.data[0].url!;
  }
}
```

### 2. Storage Service Implementation

Create `api/src/services/storageService.ts`:

```typescript
import { BlobServiceClient, StorageSharedKeyCredential } from '@azure/storage-blob';
import axios from 'axios';

export class StorageService {
  private blobServiceClient: BlobServiceClient;
  private containerName: string;

  constructor() {
    this.blobServiceClient = BlobServiceClient.fromConnectionString(
      process.env.AZURE_STORAGE_CONNECTION_STRING!
    );
    this.containerName = process.env.STORAGE_CONTAINER_NAME!;
  }

  /**
   * Upload image to Azure Blob Storage
   */
  async uploadImage(imageUrl: string, userId: string, iconId: string): Promise<string> {
    // Download image from DALL-E URL
    const imageResponse = await axios.get(imageUrl, { responseType: 'arraybuffer' });
    const imageBuffer = Buffer.from(imageResponse.data);

    // Create container client
    const containerClient = this.blobServiceClient.getContainerClient(this.containerName);
    
    // Ensure container exists
    await containerClient.createIfNotExists({
      access: 'blob', // Public read access for images
    });

    // Generate blob name
    const blobName = `${userId}/${iconId}.png`;
    const blockBlobClient = containerClient.getBlockBlobClient(blobName);

    // Upload
    await blockBlobClient.upload(imageBuffer, imageBuffer.length, {
      blobHTTPHeaders: {
        blobContentType: 'image/png',
        blobCacheControl: 'public, max-age=31536000', // Cache for 1 year
      },
    });

    return blockBlobClient.url;
  }

  /**
   * Generate SAS URL for temporary access
   */
  async generateDownloadUrl(blobName: string, expiryMinutes: number = 60): Promise<string> {
    const containerClient = this.blobServiceClient.getContainerClient(this.containerName);
    const blockBlobClient = containerClient.getBlockBlobClient(blobName);
    
    // For production, implement SAS token generation
    // For now, return the blob URL (only works with public access)
    return blockBlobClient.url;
  }
}
```

### 3. Database Service Implementation

Create `api/src/services/databaseService.ts`:

```typescript
import { CosmosClient, Database, Container } from '@azure/cosmos';

export interface User {
  id: string;
  email: string;
  credits: number;
  createdAt: string;
  updatedAt: string;
}

export interface IconGeneration {
  id: string;
  userId: string;
  prompt: string;
  enhancedPrompt: string;
  style: string;
  colors: string[];
  imageUrl: string;
  createdAt: string;
}

export class DatabaseService {
  private client: CosmosClient;
  private database: Database;
  private usersContainer: Container;
  private iconsContainer: Container;

  constructor() {
    this.client = new CosmosClient({
      endpoint: process.env.COSMOS_ENDPOINT!,
      key: process.env.COSMOS_KEY!,
    });
    
    this.database = this.client.database(process.env.COSMOS_DATABASE!);
    this.usersContainer = this.database.container('Users');
    this.iconsContainer = this.database.container('Icons');
  }

  /**
   * Initialize database and containers
   */
  async initialize(): Promise<void> {
    // Create database if not exists
    await this.client.databases.createIfNotExists({ id: process.env.COSMOS_DATABASE! });

    // Create containers
    await this.database.containers.createIfNotExists({
      id: 'Users',
      partitionKey: { paths: ['/id'] },
    });

    await this.database.containers.createIfNotExists({
      id: 'Icons',
      partitionKey: { paths: ['/userId'] },
    });
  }

  /**
   * Get user by ID
   */
  async getUser(userId: string): Promise<User | null> {
    try {
      const { resource } = await this.usersContainer.item(userId, userId).read<User>();
      return resource || null;
    } catch (error: any) {
      if (error.code === 404) return null;
      throw error;
    }
  }

  /**
   * Create or update user
   */
  async upsertUser(user: User): Promise<User> {
    const { resource } = await this.usersContainer.items.upsert(user);
    return resource!;
  }

  /**
   * Deduct credits from user
   */
  async deductCredits(userId: string, amount: number): Promise<boolean> {
    const user = await this.getUser(userId);
    if (!user || user.credits < amount) {
      return false;
    }

    user.credits -= amount;
    user.updatedAt = new Date().toISOString();
    await this.upsertUser(user);
    return true;
  }

  /**
   * Add credits to user
   */
  async addCredits(userId: string, amount: number): Promise<User> {
    const user = await this.getUser(userId);
    if (!user) {
      throw new Error('User not found');
    }

    user.credits += amount;
    user.updatedAt = new Date().toISOString();
    return await this.upsertUser(user);
  }

  /**
   * Save icon generation record
   */
  async saveIconGeneration(icon: IconGeneration): Promise<IconGeneration> {
    const { resource } = await this.iconsContainer.items.create(icon);
    return resource!;
  }

  /**
   * Get user's icon history
   */
  async getUserIcons(userId: string, limit: number = 50): Promise<IconGeneration[]> {
    const querySpec = {
      query: 'SELECT * FROM c WHERE c.userId = @userId ORDER BY c.createdAt DESC OFFSET 0 LIMIT @limit',
      parameters: [
        { name: '@userId', value: userId },
        { name: '@limit', value: limit },
      ],
    };

    const { resources } = await this.iconsContainer.items.query<IconGeneration>(querySpec).fetchAll();
    return resources;
  }
}
```

### 4. Image Processing Service

Create `api/src/services/imageService.ts`:

```typescript
import sharp from 'sharp';
import axios from 'axios';

export class ImageService {
  /**
   * Process image: resize, optimize, transparent background
   */
  async processIcon(imageUrl: string): Promise<Buffer> {
    // Download image
    const response = await axios.get(imageUrl, { responseType: 'arraybuffer' });
    const imageBuffer = Buffer.from(response.data);

    // Process with sharp
    const processed = await sharp(imageBuffer)
      .resize(1024, 1024, {
        fit: 'contain',
        background: { r: 0, g: 0, b: 0, alpha: 0 }, // Transparent background
      })
      .png({ quality: 90, compressionLevel: 9 })
      .toBuffer();

    return processed;
  }

  /**
   * Generate multiple sizes for iOS/Android
   */
  async generateAssetSizes(imageBuffer: Buffer): Promise<Map<string, Buffer>> {
    const sizes = new Map<string, Buffer>();

    // iOS sizes
    const iosSizes = [20, 29, 40, 58, 60, 76, 80, 87, 120, 152, 167, 180, 1024];
    
    for (const size of iosSizes) {
      const resized = await sharp(imageBuffer)
        .resize(size, size)
        .png()
        .toBuffer();
      sizes.set(`icon-${size}.png`, resized);
    }

    return sizes;
  }
}
```

### 5. Generate Icon Function

Create `api/src/functions/generateIcon.ts`:

```typescript
import { app, HttpRequest, HttpResponseInit, InvocationContext } from '@azure/functions';
import { AIService } from '../services/aiService';
import { StorageService } from '../services/storageService';
import { DatabaseService } from '../services/databaseService';
import { ImageService } from '../services/imageService';
import { v4 as uuidv4 } from 'uuid';

interface GenerateIconRequest {
  keywords: string;
  style: string;
  colors: string[];
  quality?: 'standard' | 'hd';
}

export async function generateIcon(
  request: HttpRequest,
  context: InvocationContext
): Promise<HttpResponseInit> {
  context.log('Generate icon function triggered');

  try {
    // Parse request body
    const body: GenerateIconRequest = await request.json() as GenerateIconRequest;
    const userId = request.headers.get('x-user-id') || 'anonymous'; // From auth middleware

    // Validate input
    if (!body.keywords || !body.style || !body.colors) {
      return {
        status: 400,
        jsonBody: { error: 'Missing required fields' },
      };
    }

    // Initialize services
    const dbService = new DatabaseService();
    const aiService = new AIService();
    const storageService = new StorageService();
    const imageService = new ImageService();

    // Check user credits
    const hasCredits = await dbService.deductCredits(userId, 1);
    if (!hasCredits) {
      return {
        status: 402,
        jsonBody: { error: 'Insufficient credits' },
      };
    }

    // Step 1: Enhance prompt
    context.log('Enhancing prompt...');
    const enhancedPrompt = await aiService.enhancePrompt({
      keywords: body.keywords,
      style: body.style,
      colors: body.colors,
      quality: body.quality,
    });

    // Step 2: Generate icon
    context.log('Generating icon with DALL-E 3...');
    const imageUrl = await aiService.generateIcon(enhancedPrompt, body.quality);

    // Step 3: Process image (optional - for transparent background)
    // const processedBuffer = await imageService.processIcon(imageUrl);

    // Step 4: Upload to blob storage
    const iconId = uuidv4();
    context.log('Uploading to blob storage...');
    const storedUrl = await storageService.uploadImage(imageUrl, userId, iconId);

    // Step 5: Save to database
    await dbService.saveIconGeneration({
      id: iconId,
      userId,
      prompt: body.keywords,
      enhancedPrompt,
      style: body.style,
      colors: body.colors,
      imageUrl: storedUrl,
      createdAt: new Date().toISOString(),
    });

    // Return success
    return {
      status: 200,
      jsonBody: {
        iconId,
        imageUrl: storedUrl,
        enhancedPrompt,
        creditsRemaining: (await dbService.getUser(userId))?.credits || 0,
      },
    };
  } catch (error: any) {
    context.error('Error generating icon:', error);
    return {
      status: 500,
      jsonBody: { error: error.message || 'Internal server error' },
    };
  }
}

app.http('generateIcon', {
  methods: ['POST'],
  authLevel: 'anonymous',
  handler: generateIcon,
});
```

### 6. Additional Functions

Create `api/src/functions/getUserData.ts`:

```typescript
import { app, HttpRequest, HttpResponseInit, InvocationContext } from '@azure/functions';
import { DatabaseService } from '../services/databaseService';

export async function getUserData(
  request: HttpRequest,
  context: InvocationContext
): Promise<HttpResponseInit> {
  const userId = request.headers.get('x-user-id');

  if (!userId) {
    return { status: 401, jsonBody: { error: 'Unauthorized' } };
  }

  try {
    const dbService = new DatabaseService();
    const user = await dbService.getUser(userId);
    const icons = await dbService.getUserIcons(userId);

    return {
      status: 200,
      jsonBody: {
        user,
        icons,
      },
    };
  } catch (error: any) {
    context.error('Error fetching user data:', error);
    return {
      status: 500,
      jsonBody: { error: error.message },
    };
  }
}

app.http('getUserData', {
  methods: ['GET'],
  authLevel: 'anonymous',
  route: 'user/data',
  handler: getUserData,
});
```

---

## Frontend Implementation

### Project Structure

```
frontend/
├── src/
│   ├── components/
│   │   ├── IconGenerator/
│   │   │   ├── PromptInput.tsx
│   │   │   ├── StyleSelector.tsx
│   │   │   ├── ColorPicker.tsx
│   │   │   └── GeneratedIcon.tsx
│   │   ├── Dashboard/
│   │   │   ├── CreditBalance.tsx
│   │   │   └── IconHistory.tsx
│   │   └── Common/
│   │       ├── Button.tsx
│   │       └── Loading.tsx
│   ├── hooks/
│   │   ├── useIconGeneration.ts
│   │   └── useUserData.ts
│   ├── services/
│   │   ├── api.ts
│   │   └── stripe.ts
│   ├── store/
│   │   └── userStore.ts
│   ├── types/
│   │   └── index.ts
│   ├── App.tsx
│   └── main.tsx
```

### 1. API Service

Create `frontend/src/services/api.ts`:

```typescript
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export interface GenerateIconParams {
  keywords: string;
  style: string;
  colors: string[];
  quality?: 'standard' | 'hd';
}

export interface GenerateIconResponse {
  iconId: string;
  imageUrl: string;
  enhancedPrompt: string;
  creditsRemaining: number;
}

export interface UserData {
  user: {
    id: string;
    email: string;
    credits: number;
  };
  icons: Array<{
    id: string;
    imageUrl: string;
    prompt: string;
    style: string;
    createdAt: string;
  }>;
}

class ApiService {
  private client = axios.create({
    baseURL: API_BASE_URL,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  setAuthToken(token: string) {
    this.client.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  }

  setUserId(userId: string) {
    this.client.defaults.headers.common['x-user-id'] = userId;
  }

  async generateIcon(params: GenerateIconParams): Promise<GenerateIconResponse> {
    const { data } = await this.client.post('/generateIcon', params);
    return data;
  }

  async getUserData(): Promise<UserData> {
    const { data } = await this.client.get('/user/data');
    return data;
  }

  async createCheckoutSession(priceId: string) {
    const { data } = await this.client.post('/create-checkout-session', { priceId });
    return data;
  }
}

export const apiService = new ApiService();
```

### 2. User Store (Zustand)

Create `frontend/src/store/userStore.ts`:

```typescript
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface User {
  id: string;
  email: string;
  credits: number;
}

interface UserStore {
  user: User | null;
  isAuthenticated: boolean;
  setUser: (user: User) => void;
  updateCredits: (credits: number) => void;
  logout: () => void;
}

export const useUserStore = create<UserStore>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,
      setUser: (user) => set({ user, isAuthenticated: true }),
      updateCredits: (credits) =>
        set((state) => ({
          user: state.user ? { ...state.user, credits } : null,
        })),
      logout: () => set({ user: null, isAuthenticated: false }),
    }),
    {
      name: 'user-storage',
    }
  )
);
```

### 3. Icon Generation Hook

Create `frontend/src/hooks/useIconGeneration.ts`:

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { apiService, GenerateIconParams } from '../services/api';
import { useUserStore } from '../store/userStore';

export function useIconGeneration() {
  const queryClient = useQueryClient();
  const updateCredits = useUserStore((state) => state.updateCredits);

  return useMutation({
    mutationFn: (params: GenerateIconParams) => apiService.generateIcon(params),
    onSuccess: (data) => {
      // Update credits in store
      updateCredits(data.creditsRemaining);
      
      // Invalidate user data to refetch
      queryClient.invalidateQueries({ queryKey: ['userData'] });
    },
  });
}
```

### 4. Main Icon Generator Component

Create `frontend/src/components/IconGenerator/IconGenerator.tsx`:

```typescript
import React, { useState } from 'react';
import { useIconGeneration } from '../../hooks/useIconGeneration';
import { StyleSelector } from './StyleSelector';
import { ColorPicker } from './ColorPicker';
import { GeneratedIcon } from './GeneratedIcon';

const ICON_STYLES = [
  'retro', 'cartoon', 'geometric', 'neon', 'clay', 'abstract',
  'lineal', '3D', 'pixel', 'origami', 'minimal', 'gradient',
  'steel', 'fibonacci', 'b&w', 'crayon', 'sticker', 'watercolor'
];

export function IconGenerator() {
  const [keywords, setKeywords] = useState('');
  const [selectedStyle, setSelectedStyle] = useState('3D');
  const [selectedColors, setSelectedColors] = useState<string[]>(['#3B82F6', '#8B5CF6']);
  const [generatedIcon, setGeneratedIcon] = useState<string | null>(null);

  const { mutate: generate, isPending, error } = useIconGeneration();

  const handleGenerate = () => {
    if (!keywords.trim()) return;

    generate(
      {
        keywords: keywords.trim(),
        style: selectedStyle,
        colors: selectedColors,
        quality: 'standard',
      },
      {
        onSuccess: (data) => {
          setGeneratedIcon(data.imageUrl);
        },
      }
    );
  };

  return (
    <div className="max-w-6xl mx-auto p-6">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Input Section */}
        <div className="space-y-6">
          <div>
            <h2 className="text-2xl font-bold mb-4">Generate Your Icon</h2>
            
            {/* Keywords Input */}
            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">
                Describe your icon
              </label>
              <input
                type="text"
                value={keywords}
                onChange={(e) => setKeywords(e.target.value)}
                placeholder="e.g., coffee cup, mountain peak, rocket"
                className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                maxLength={100}
              />
              <p className="text-xs text-gray-500 mt-1">
                {keywords.length}/100 characters
              </p>
            </div>

            {/* Style Selector */}
            <StyleSelector
              styles={ICON_STYLES}
              selected={selectedStyle}
              onSelect={setSelectedStyle}
            />

            {/* Color Picker */}
            <ColorPicker
              colors={selectedColors}
              onChange={setSelectedColors}
            />

            {/* Generate Button */}
            <button
              onClick={handleGenerate}
              disabled={isPending || !keywords.trim()}
              className="w-full bg-blue-600 text-white py-3 rounded-lg font-semibold
                       hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed
                       transition-colors"
            >
              {isPending ? 'Generating...' : 'Generate Icon (1 credit)'}
            </button>

            {error && (
              <div className="mt-4 p-3 bg-red-50 text-red-700 rounded-lg">
                {error.message}
              </div>
            )}
          </div>
        </div>

        {/* Preview Section */}
        <div>
          <h3 className="text-xl font-semibold mb-4">Preview</h3>
          <GeneratedIcon
            imageUrl={generatedIcon}
            isLoading={isPending}
          />
        </div>
      </div>
    </div>
  );
}
```

### 5. Style Selector Component

Create `frontend/src/components/IconGenerator/StyleSelector.tsx`:

```typescript
import React from 'react';

interface StyleSelectorProps {
  styles: string[];
  selected: string;
  onSelect: (style: string) => void;
}

export function StyleSelector({ styles, selected, onSelect }: StyleSelectorProps) {
  return (
    <div className="mb-6">
      <label className="block text-sm font-medium mb-3">Choose a style</label>
      <div className="grid grid-cols-3 sm:grid-cols-4 gap-3">
        {styles.map((style) => (
          <button
            key={style}
            onClick={() => onSelect(style)}
            className={`
              px-4 py-2 rounded-lg border-2 font-medium transition-all
              ${selected === style
                ? 'border-blue-600 bg-blue-50 text-blue-700'
                : 'border-gray-200 hover:border-gray-300'
              }
            `}
          >
            {style}
          </button>
        ))}
      </div>
    </div>
  );
}
```

### 6. Generated Icon Display

Create `frontend/src/components/IconGenerator/GeneratedIcon.tsx`:

```typescript
import React from 'react';

interface GeneratedIconProps {
  imageUrl: string | null;
  isLoading: boolean;
}

export function GeneratedIcon({ imageUrl, isLoading }: GeneratedIconProps) {
  if (isLoading) {
    return (
      <div className="aspect-square bg-gray-100 rounded-lg flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Generating your icon...</p>
        </div>
      </div>
    );
  }

  if (!imageUrl) {
    return (
      <div className="aspect-square bg-gray-50 rounded-lg flex items-center justify-center border-2 border-dashed border-gray-300">
        <p className="text-gray-400">Your icon will appear here</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="aspect-square bg-white rounded-lg shadow-lg p-4">
        <img
          src={imageUrl}
          alt="Generated icon"
          className="w-full h-full object-contain"
        />
      </div>
      
      <div className="flex gap-3">
        <a
          href={imageUrl}
          download
          className="flex-1 bg-blue-600 text-white py-2 px-4 rounded-lg text-center
                   hover:bg-blue-700 transition-colors"
        >
          Download PNG
        </a>
        <button
          onClick={() => {/* Open asset generator modal */}}
          className="flex-1 bg-gray-100 text-gray-700 py-2 px-4 rounded-lg
                   hover:bg-gray-200 transition-colors"
        >
          Export Assets
        </button>
      </div>
    </div>
  );
}
```

---

## Deployment

### Azure Static Web Apps Setup

#### Step 1: Create Static Web App

```bash
# Create static web app
az staticwebapp create \
  --name icon-generator-app \
  --resource-group rg-icon-generator \
  --source https://github.com/YOUR_USERNAME/icon-generator \
  --location eastus2 \
  --branch main \
  --app-location "/frontend" \
  --api-location "/api" \
  --output-location "dist" \
  --login-with-github
```

#### Step 2: Configure GitHub Actions

The Azure portal will automatically create a GitHub Actions workflow. Update `.github/workflows/azure-static-web-apps-*.yml`:

```yaml
name: Azure Static Web Apps CI/CD

on:
  push:
    branches:
      - main
  pull_request:
    types: [opened, synchronize, reopened, closed]
    branches:
      - main

jobs:
  build_and_deploy_job:
    if: github.event_name == 'push' || (github.event_name == 'pull_request' && github.event.action != 'closed')
    runs-on: ubuntu-latest
    name: Build and Deploy Job
    steps:
      - uses: actions/checkout@v3
        with:
          submodules: true
      
      - name: Build And Deploy
        id: builddeploy
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: "upload"
          app_location: "/frontend"
          api_location: "/api"
          output_location: "dist"
        env:
          # Environment variables for build
          VITE_API_BASE_URL: ${{ secrets.VITE_API_BASE_URL }}
          VITE_STRIPE_PUBLISHABLE_KEY: ${{ secrets.VITE_STRIPE_PUBLISHABLE_KEY }}
```

#### Step 3: Configure Environment Variables

In Azure Portal > Static Web App > Configuration:

```
AZURE_OPENAI_ENDPOINT=your-endpoint
AZURE_OPENAI_API_KEY=your-key
DALLE3_DEPLOYMENT_NAME=dalle3-icon-generator
GPT4O_MINI_DEPLOYMENT_NAME=gpt-4o-mini-prompts
AZURE_STORAGE_CONNECTION_STRING=your-connection
STORAGE_CONTAINER_NAME=generated-icons
COSMOS_ENDPOINT=your-cosmos-endpoint
COSMOS_KEY=your-cosmos-key
COSMOS_DATABASE=IconGeneratorDB
COSMOS_CONTAINER=Users
STRIPE_SECRET_KEY=your-stripe-key
```

#### Step 4: Deploy

```bash
# Commit and push to trigger deployment
git add .
git commit -m "Initial deployment"
git push origin main
```

### Local Testing

```bash
# Terminal 1 - Frontend
cd frontend
npm run dev

# Terminal 2 - API (Azure Functions Core Tools required)
cd api
npm run start  # or: func start
```

---

## Testing

### Unit Tests

Create `api/src/__tests__/aiService.test.ts`:

```typescript
import { AIService } from '../services/aiService';

describe('AIService', () => {
  let aiService: AIService;

  beforeEach(() => {
    aiService = new AIService();
  });

  test('enhancePrompt returns valid string', async () => {
    const params = {
      keywords: 'coffee cup',
      style: '3D',
      colors: ['#8B4513', '#FFFFFF'],
    };

    const enhanced = await aiService.enhancePrompt(params);
    expect(enhanced).toBeTruthy();
    expect(typeof enhanced).toBe('string');
  });
});
```

### Integration Testing

Test the complete flow:

```bash
# Use curl or Postman
curl -X POST http://localhost:7071/api/generateIcon \
  -H "Content-Type: application/json" \
  -H "x-user-id: test-user-123" \
  -d '{
    "keywords": "mountain peak",
    "style": "minimal",
    "colors": ["#4A90E2", "#FFFFFF"]
  }'
```

---

## Optimization

### 1. Prompt Caching

Implement caching for similar prompts:

```typescript
import NodeCache from 'node-cache';

const promptCache = new NodeCache({ stdTTL: 3600 }); // 1 hour

async enhancePrompt(params: IconGenerationParams): Promise<string> {
  const cacheKey = `${params.keywords}-${params.style}-${params.colors.join(',')}`;
  
  const cached = promptCache.get<string>(cacheKey);
  if (cached) return cached;
  
  const enhanced = await this.callGPT(params);
  promptCache.set(cacheKey, enhanced);
  
  return enhanced;
}
```

### 2. Image Optimization

```typescript
// In ImageService
async optimizeForWeb(buffer: Buffer): Promise<Buffer> {
  return sharp(buffer)
    .resize(1024, 1024)
    .png({
      quality: 85,
      compressionLevel: 9,
      adaptiveFiltering: true,
    })
    .toBuffer();
}
```

### 3. CDN Configuration

Enable Azure CDN for faster delivery:

```bash
az cdn profile create \
  --name icon-generator-cdn \
  --resource-group rg-icon-generator \
  --sku Standard_Microsoft

az cdn endpoint create \
  --name icon-images \
  --profile-name icon-generator-cdn \
  --resource-group rg-icon-generator \
  --origin your-storage-account.blob.core.windows.net
```

---

## Cost Management

### Monitoring

Set up cost alerts in Azure Portal:

```bash
# Create budget alert
az consumption budget create \
  --budget-name icon-generator-budget \
  --amount 100 \
  --resource-group rg-icon-generator \
  --time-grain Monthly \
  --start-date 2025-02-01 \
  --end-date 2026-01-31
```

### Cost Optimization Tips

1. **Use Standard Quality by Default**: DALL-E 3 HD costs 2x more
2. **Enable Cosmos DB Autoscale**: Only pay for what you use
3. **Use Blob Storage Cool Tier**: For older icons
4. **Implement Request Throttling**: Prevent abuse
5. **Cache Aggressively**: Reduce duplicate generations

### Pricing Calculator

Estimated costs per 1,000 icons:
- DALL-E 3: $40 (1024x1024 standard)
- GPT-4o-mini: ~$5 (prompt enhancement)
- Blob Storage: ~$1
- Cosmos DB: ~$5
- Azure Functions: ~$2
- **Total**: ~$53/1,000 icons

Revenue at $1.00/icon = $1,000
Profit margin = ~95%

---

## Appendix

### A. Style Prompt Templates

```typescript
export const STYLE_TEMPLATES = {
  '3D': 'Create a 3D rendered icon with depth, shadows, and realistic lighting.',
  'minimal': 'Design a minimal, clean icon with simple shapes and limited colors.',
  'gradient': 'Generate a modern gradient icon with smooth color transitions.',
  'pixel': 'Create a pixel art style icon with a retro 8-bit aesthetic.',
  'clay': 'Design a clay/plasticine style icon with soft, rounded forms.',
  // ... add more
};
```

### B. Color Scheme Presets

```typescript
export const COLOR_PRESETS = {
  ocean: ['#006994', '#0096D6', '#00BCD4', '#80DEEA'],
  sunset: ['#FF6B6B', '#FFA07A', '#FFD93D', '#F7DC6F'],
  forest: ['#27AE60', '#52BE80', '#7DCEA0', '#A9DFBF'],
  royal: ['#4A148C', '#7B1FA2', '#9C27B0', '#BA68C8'],
  // ... add more
};
```

### C. Useful Resources

- [Azure AI Foundry Docs](https://learn.microsoft.com/en-us/azure/ai-foundry/)
- [DALL-E 3 Best Practices](https://platform.openai.com/docs/guides/images)
- [Azure Static Web Apps Guide](https://learn.microsoft.com/en-us/azure/static-web-apps/)
- [Stripe Integration Guide](https://stripe.com/docs/payments)
- [React Query Documentation](https://tanstack.com/query/latest)

---

## Next Steps

1. **Setup Azure Resources** (Day 1-2)
2. **Implement Backend Functions** (Week 1)
3. **Build Frontend UI** (Week 2)
4. **Integrate Payments** (Week 3)
5. **Testing & Optimization** (Week 4)
6. **Beta Launch** (Week 5)

Good luck with your implementation! 🚀