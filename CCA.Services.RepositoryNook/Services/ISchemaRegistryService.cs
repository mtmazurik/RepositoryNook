using CCA.Services.RepositoryNook.Models;

namespace CCA.Services.RepositoryNook.Services
{
    public interface ISchemaRegistryService
    {
        SchemaRegistry Create(SchemaRegistry sro);
    }
}