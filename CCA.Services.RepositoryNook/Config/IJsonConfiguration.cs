namespace CCA.Services.RepositoryNook.Config
{
    public interface IJsonConfiguration
    {
        string AtlasMongoConnection { get; }
        double TaskManagerIntervalSeconds { get; }
        bool EnforceTokenLife { get; }
    }
}