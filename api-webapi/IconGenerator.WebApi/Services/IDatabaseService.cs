namespace IconGenerator.Functions.Services;

using IconGenerator.Functions.Models;

public interface IDatabaseService
{
    // User operations
    Task<User?> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default) => GetUserAsync(userId, cancellationToken);
    Task<User?> GetUserByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeductCreditsAsync(string userId, int amount, CancellationToken cancellationToken = default);
    Task<User> AddCreditsAsync(string userId, int amount, CancellationToken cancellationToken = default);

    // Icon operations
    Task<IconGeneration> SaveIconGenerationAsync(IconGeneration icon, CancellationToken cancellationToken = default);
    Task<IconGeneration?> GetIconGenerationAsync(string iconId, CancellationToken cancellationToken = default);
    Task<List<IconGeneration>> GetUserIconsAsync(string userId, int limit = 50, CancellationToken cancellationToken = default);

    // Asset operations
    Task<AssetGeneration> SaveAssetGenerationAsync(AssetGeneration asset, CancellationToken cancellationToken = default);
    Task<AssetGeneration?> GetAssetGenerationAsync(string assetId, CancellationToken cancellationToken = default);

    // Transaction operations
    Task<Transaction> SaveTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<List<Transaction>> GetUserTransactionsAsync(string userId, int limit = 50, CancellationToken cancellationToken = default);

    // Chat session operations
    Task<ChatSession> SaveChatSessionAsync(ChatSession chatSession, CancellationToken cancellationToken = default);
    Task<ChatSession?> GetChatSessionAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<ChatSession> UpdateChatSessionAsync(ChatSession chatSession, CancellationToken cancellationToken = default);
    Task<List<ChatSession>> GetUserChatSessionsAsync(string userId, int limit = 50, CancellationToken cancellationToken = default);
}
