# Icon Generator Web API

ASP.NET Core Web API (.NET 10) for the Icon Generator application.

## Local Development Setup

### Prerequisites
- .NET 10 SDK
- Azure Cosmos DB account (or connection string)
- Azure OpenAI account (or API key)
- Azure Storage account (or connection string)
- Stripe test keys
- Google OAuth Client ID

### Configuration

1. **Update `appsettings.Development.json`** with your Azure credentials:
   - Get Cosmos DB connection from Azure Portal
   - Get OpenAI API key from Azure Portal
   - Get Storage connection string from Azure Portal
   - Get Stripe keys from Stripe Dashboard
   - Google Client ID is already configured

2. **JWT Secret Key** is already set for local development (you can change it if needed)

### Running the API

**Option 1: Command Line**
```bash
cd IconGenerator.WebApi
dotnet run
```
The API will start at `http://localhost:5199`

**Option 2: Visual Studio / Rider**
- Open `IconGenerator.WebApi.sln`
- Press F5 or click Run
- The API will start with debugger attached

**Option 3: VS Code**
- Open the `api-webapi` folder in VS Code
- Press F5 (launch configuration is in `.vscode/launch.json`)

### Testing Endpoints

**Health Check:**
```bash
curl http://localhost:5199/health
```

**Get Credit Packages (Anonymous):**
```bash
curl http://localhost:5199/api/payments/packages
```

**Swagger UI (Development only):**
Open browser to: `http://localhost:5199/swagger`

### Frontend Integration

The React app (in `/web`) is already configured to connect to `http://localhost:5199/api` for local development.

To run the full stack locally:
1. Start the API (this project): `dotnet run`
2. Start the React app: `cd ../web && npm run dev`
3. Open `http://localhost:5173` in your browser

### Debugging Tips

1. **Enable detailed logging**: Set log level to `Debug` in `appsettings.Development.json`
2. **Check Application Insights**: Telemetry is enabled even in development
3. **CORS issues**: Allowed origins are configured in `Program.cs`
4. **JWT tokens**: Use browser DevTools → Application → Local Storage to inspect tokens
5. **Database queries**: Check logs for Cosmos DB operations

### Common Issues

**Build warnings about ImageSharp vulnerability:**
- This is inherited from the original project
- Not blocking for local development
- Update the package version when ready

**JWT validation errors:**
- Make sure the JWT secret key is at least 32 characters
- Check token expiration (default: 7 days)
- Clear browser localStorage if switching between environments

**CORS errors:**
- Make sure your frontend URL is in the `allowedOrigins` list in `Program.cs`
- Currently configured: `http://localhost:5173`, `http://localhost:3000`

### Environment Variables

You can override `appsettings.Development.json` with environment variables:
```bash
export Jwt__SecretKey="your-custom-secret"
export Database__CosmosKey="your-cosmos-key"
export AzureOpenAI__ApiKey="your-openai-key"
dotnet run
```

### Project Structure

```
IconGenerator.WebApi/
├── Controllers/        # API endpoints
│   ├── AuthController.cs
│   ├── IconsController.cs
│   ├── UsersController.cs
│   ├── PaymentsController.cs
│   └── WebhooksController.cs
├── Services/           # Business logic
├── Models/             # Data models
├── Options/            # Configuration classes
├── Middleware/         # Custom middleware
├── Program.cs          # Application setup
└── appsettings.json    # Configuration
```

## Deployment

See `/infrastructure/main.bicep` for Azure deployment configuration.

The application deploys to Azure App Service (B1/S1 tier) via Azure DevOps pipeline.
