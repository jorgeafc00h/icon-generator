namespace IconGenerator.Functions.Services;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IconGenerator.Functions.Options;
using IconGenerator.Functions.Models;
using CosmosUser = Microsoft.Azure.Cosmos.User;

public class CosmosDbService : IDatabaseService
{
    private readonly CosmosClient _client;
    private readonly Container _usersContainer;
    private readonly Container _iconsContainer;
    private readonly Container _assetsContainer;
    private readonly Container _transactionsContainer;
    private readonly ILogger<CosmosDbService> _logger;

    public CosmosDbService(IOptions<DatabaseOptions> options, ILogger<CosmosDbService> logger)
    {
        _logger = logger;
        var dbOptions = options.Value;

        _client = new CosmosClient(dbOptions.CosmosEndpoint, dbOptions.CosmosKey);
        var database = _client.GetDatabase(dbOptions.CosmosDatabase);

        _usersContainer = database.GetContainer("Users");
        _iconsContainer = database.GetContainer("Icons");
        _assetsContainer = database.GetContainer("Assets");
        _transactionsContainer = database.GetContainer("Transactions");
    }

    // User operations
    public async Task<Models.User?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _usersContainer.ReadItemAsync<Models.User>(
                userId,
                new PartitionKey(userId),
                cancellationToken: cancellationToken);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Models.User?> GetUserByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.Auth.GoogleId = @googleId")
                .WithParameter("@googleId", googleId);

            var iterator = _usersContainer.GetItemQueryIterator<Models.User>(query);

            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                return response.FirstOrDefault();
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user by Google ID: {googleId}");
            return null;
        }
    }

    public async Task<Models.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.Email = @email")
                .WithParameter("@email", email);

            var iterator = _usersContainer.GetItemQueryIterator<Models.User>(query);

            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                return response.FirstOrDefault();
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user by email: {email}");
            return null;
        }
    }

    public async Task<Models.User> CreateUserAsync(Models.User user, CancellationToken cancellationToken = default)
    {
        var response = await _usersContainer.CreateItemAsync(
            user,
            new PartitionKey(user.Id),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public async Task<Models.User> UpdateUserAsync(Models.User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;

        var response = await _usersContainer.UpsertItemAsync(
            user,
            new PartitionKey(user.Id),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public async Task<bool> DeductCreditsAsync(string userId, int amount, CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(userId, cancellationToken);

        if (user == null || user.Credits < amount)
        {
            return false;
        }

        user.Credits -= amount;
        await UpdateUserAsync(user, cancellationToken);

        return true;
    }

    public async Task<Models.User> AddCreditsAsync(string userId, int amount, CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(userId, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException($"User {userId} not found");
        }

        user.Credits += amount;
        return await UpdateUserAsync(user, cancellationToken);
    }

    // Icon operations
    public async Task<IconGeneration> SaveIconGenerationAsync(IconGeneration icon, CancellationToken cancellationToken = default)
    {
        var response = await _iconsContainer.CreateItemAsync(
            icon,
            new PartitionKey(icon.UserId),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public async Task<IconGeneration?> GetIconGenerationAsync(string iconId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @iconId")
            .WithParameter("@iconId", iconId);

        var iterator = _iconsContainer.GetItemQueryIterator<IconGeneration>(query);
        var response = await iterator.ReadNextAsync(cancellationToken);

        return response.FirstOrDefault();
    }

    public async Task<List<IconGeneration>> GetUserIconsAsync(string userId, int limit = 50, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.userId = @userId ORDER BY c.createdAt DESC OFFSET 0 LIMIT @limit")
            .WithParameter("@userId", userId)
            .WithParameter("@limit", limit);

        var results = new List<IconGeneration>();
        var iterator = _iconsContainer.GetItemQueryIterator<IconGeneration>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    // Asset operations
    public async Task<AssetGeneration> SaveAssetGenerationAsync(AssetGeneration asset, CancellationToken cancellationToken = default)
    {
        var response = await _assetsContainer.CreateItemAsync(
            asset,
            new PartitionKey(asset.UserId),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public async Task<AssetGeneration?> GetAssetGenerationAsync(string assetId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @assetId")
            .WithParameter("@assetId", assetId);

        var iterator = _assetsContainer.GetItemQueryIterator<AssetGeneration>(query);
        var response = await iterator.ReadNextAsync(cancellationToken);

        return response.FirstOrDefault();
    }

    // Transaction operations
    public async Task<Transaction> SaveTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var response = await _transactionsContainer.CreateItemAsync(
            transaction,
            new PartitionKey(transaction.UserId),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    public async Task<List<Transaction>> GetUserTransactionsAsync(string userId, int limit = 50, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.userId = @userId ORDER BY c.createdAt DESC OFFSET 0 LIMIT @limit")
            .WithParameter("@userId", userId)
            .WithParameter("@limit", limit);

        var results = new List<Transaction>();
        var iterator = _transactionsContainer.GetItemQueryIterator<Transaction>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }
}
