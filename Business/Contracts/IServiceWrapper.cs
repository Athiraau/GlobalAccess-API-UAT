namespace Business.Contracts
{
    public interface IServiceWrapper
    {
        IUserService User { get; }
        IHelperService Helper { get; }
        IJwtUtils JwtUtils { get; }
    }
}
