namespace Domain.Interfaces.Repositories
{
    public interface IApplicationConfigRepository : ICrudRepository<ApplicationConfig>
    {
        ApplicationConfig Get();
    }
}
