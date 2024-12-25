using custdev.domain.db;


namespace custdev.domain.interfaces.db
{
    public interface IDbMessageService
    {
        Task<bool> AddDbMessage(DbMessage model);
    }
}
