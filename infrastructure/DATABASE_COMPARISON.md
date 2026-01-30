# Database Options Comparison

## Azure Cosmos DB Free Tier (RECOMMENDED for Dev)

### Pricing
- **FREE TIER**: 1000 RU/s + 25GB storage forever
- **Limitation**: One free tier per Azure subscription
- **Beyond free tier**:
  - Serverless: $0.285 per million RUs
  - Provisioned: Starting at ~$24/month (400 RU/s)

### Pros
✅ **Completely free for dev** (within limits)
✅ **NoSQL - schema flexibility** (perfect for evolving data models)
✅ **Global distribution** (can add regions later)
✅ **Automatic scaling** with serverless
✅ **Low latency** (<10ms reads/writes)
✅ **No database setup needed** (document store ready to use)
✅ **Perfect for icon metadata** (JSON documents)

### Cons
❌ One free tier per subscription
❌ Learning curve if unfamiliar with NoSQL
❌ Query complexity for complex joins

### When to Use
- ✅ Development and testing
- ✅ Early stage / MVP
- ✅ Apps with flexible schema
- ✅ When you want free tier
- ✅ Global distribution needs

### Configuration
```bicep
databaseType: 'cosmosdb'
enableFreeTier: true  // 1000 RU/s + 25GB free
enableServerless: true // Pay per request beyond free tier
```

---

## Azure SQL Database Basic

### Pricing
- **Basic Tier**: ~$5/month
  - 2GB storage
  - 5 DTUs (Database Transaction Units)
  - Good for light workloads
- **S0 Tier**: ~$15/month
  - 250GB storage
  - 10 DTUs
  - Better performance

### Pros
✅ **Very cheap** ($5/month Basic tier)
✅ **Familiar SQL** (if you know SQL)
✅ **Strong consistency** (ACID transactions)
✅ **Complex queries** (JOINs, aggregations)
✅ **Relational integrity** (foreign keys, constraints)
✅ **Compatible with existing tools** (SSMS, Azure Data Studio)

### Cons
❌ **Not free** (minimum $5/month)
❌ **Rigid schema** (migrations needed for changes)
❌ **Slower for document operations** (vs Cosmos)
❌ **Manual scaling** (need to upgrade tiers)
❌ **Basic tier has performance limits**

### When to Use
- ✅ Already used free tier Cosmos elsewhere
- ✅ Need SQL/relational features
- ✅ Complex reporting requirements
- ✅ Team familiar with SQL
- ✅ Budget allows $5/month

### Configuration
```bicep
databaseType: 'sql'
sku: 'Basic'  // $5/month
sqlAdminPassword: 'YourSecurePassword123!'
```

---

## Recommendation by Scenario

### Scenario 1: Starting Fresh (New Azure Subscription)
**Use: Cosmos DB Free Tier**
- Cost: $0/month
- You haven't used the free tier yet
- Perfect for MVP/testing

### Scenario 2: Already Used Cosmos Free Tier
**Option A: Cosmos DB Serverless**
- Cost: ~$10-20/month (light usage)
- Better performance than SQL Basic
- More flexible

**Option B: Azure SQL Basic**
- Cost: $5/month fixed
- Predictable billing
- Good enough for early stage

### Scenario 3: Production Ready
**Use: Cosmos DB Provisioned** (400-1000 RU/s)
- Cost: $24-60/month
- Better performance
- Auto-scaling available

**Or: Azure SQL S1+**
- Cost: $30+/month
- Better performance than Basic
- More storage

---

## Cost Comparison Over Time

| Scenario | Cosmos Free | Cosmos Serverless | SQL Basic | SQL S0 |
|----------|-------------|-------------------|-----------|---------|
| **Month 1-12** (Low traffic: 10k requests/month) | $0 | $3 | $5 | $15 |
| **Growing** (100k requests/month) | $0 | $15 | $5 | $15 |
| **Medium** (1M requests/month) | Over limit | $80 | $5* | $15* |
| **High** (10M requests/month) | Over limit | $800 | $30** | $60** |

\* SQL Basic may be too slow
\** Need to upgrade SQL tier

---

## Data Model Considerations

### Icon Generator Data Model

```typescript
// Users collection/table
{
  id: string
  email: string
  credits: number
  createdAt: timestamp
  updatedAt: timestamp
}

// Icons collection/table
{
  id: string
  userId: string  // Foreign key in SQL
  prompt: string
  enhancedPrompt: string
  style: string
  colors: string[]  // JSON in SQL
  imageUrl: string
  createdAt: timestamp
}
```

### Cosmos DB Approach (Document Store)
```json
{
  "id": "user123",
  "email": "user@example.com",
  "credits": 50,
  "recentIcons": [
    {
      "id": "icon1",
      "imageUrl": "https://...",
      "style": "3D",
      "colors": ["#FF0000", "#00FF00"]
    }
  ]
}
```
**Pros**: Flexible, denormalized, fast reads
**Cons**: Data duplication

### SQL Approach (Relational)
```sql
CREATE TABLE Users (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  Email NVARCHAR(255),
  Credits INT,
  CreatedAt DATETIME2
);

CREATE TABLE Icons (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(Id),
  Prompt NVARCHAR(MAX),
  ImageUrl NVARCHAR(MAX),
  Style NVARCHAR(50),
  Colors NVARCHAR(MAX), -- JSON
  CreatedAt DATETIME2
);
```
**Pros**: Normalized, referential integrity
**Cons**: Requires JOINs for related data

---

## Current Configuration

By default, the infrastructure uses:

```
Environment: dev
Database: Cosmos DB Free Tier
Cost: $0/month
```

## Switching Between Options

### Switch to Azure SQL

```bash
# Edit parameters file
vi infrastructure/parameters.dev.json

# Change databaseType to "sql"
# Add SQL password (or reference Key Vault)

# Or use the SQL-specific parameters file
az deployment group create \
  --resource-group rg-icon-generator \
  --template-file infrastructure/main.bicep \
  --parameters infrastructure/parameters.dev.sql.json \
  --parameters sqlAdminPassword='YourSecurePassword123!'
```

### Switch to Cosmos DB Serverless

```bash
# Use default dev parameters (already configured)
az deployment group create \
  --resource-group rg-icon-generator \
  --template-file infrastructure/main.bicep \
  --parameters infrastructure/parameters.dev.json
```

---

## Backend Code Changes Needed

If you switch from Cosmos DB to SQL, you'll need to update the backend:

### Install SQL Package
```bash
cd api
npm install mssql
```

### Update Database Service
```typescript
// api/src/services/databaseService.ts

import sql from 'mssql';

const config = {
  server: process.env.SQL_SERVER,
  database: process.env.SQL_DATABASE,
  authentication: {
    type: 'default',
    options: {
      userName: 'sqladmin',
      password: process.env.SQL_PASSWORD
    }
  },
  options: {
    encrypt: true,
    trustServerCertificate: false
  }
};

// Or use connection string
const pool = await sql.connect(process.env.SQL_CONNECTION_STRING);
```

---

## Final Recommendation

**For your Icon Generator project, use Cosmos DB Free Tier:**

1. **Start with Cosmos DB Free Tier** (current default)
   - $0/month
   - Perfect for development
   - 1000 RU/s handles ~1M requests/month
   - 25GB storage is plenty

2. **When you exceed free tier limits:**
   - Upgrade to Cosmos Serverless (~$10-20/month)
   - Or switch to SQL Basic if you prefer ($5/month)

3. **For production:**
   - Cosmos DB Provisioned (400-1000 RU/s) = $24-60/month
   - Or SQL S1 = $30/month

The current infrastructure is already configured for Cosmos DB Free Tier, which is the best option to start with!
