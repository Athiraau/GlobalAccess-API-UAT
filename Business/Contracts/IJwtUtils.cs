namespace Business.Contracts
{
    public interface IJwtUtils
    {
        public int? ValidateToken(string token);
    }
}
