namespace Infrastructure.Authorization
{
    public interface IJwtTokenManger
    {
        string Authenticate(User login);
    }
}
