namespace ASyncFramework.Domain.Interface
{
    public interface ICurrentUserService
    {
        string SystemCode { get; }
        int ServiceCode { get; }
    }
}
