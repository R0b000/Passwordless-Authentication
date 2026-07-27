namespace Shared.Data.Wrapper
{
    public interface IResponse
    {
        string? Messages { get; set; }
        bool Succeeded { get; set; }
        string? Datas { get; set; }
    }

    public interface IResponse<out T> : IResponse
    {
        T? Data { get; }
    }
}

