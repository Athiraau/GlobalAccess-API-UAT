namespace DataAccess.Contracts
{
    public interface IRepositoryWrapper 
    { 
        IUserRepository User { get; }
        IHelperRepository Helper { get; } 
        void Save(); 
    }
}
