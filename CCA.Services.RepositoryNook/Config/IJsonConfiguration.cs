namespace CCA.Services.RepositoryNook.Config
{
    public interface IJsonConfiguration
    {
        string ConnectionString { get; }
        double TaskManagerIntervalSeconds { get; }
        string JwtSecretKey { get; }
        string JwtIssuer { get; }
        bool EnforceTokenLife { get; }
    }
}