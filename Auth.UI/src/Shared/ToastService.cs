namespace Auth.UI.src.Shared
{
    public enum ToastType { Success, Warning, Danger, Info }

    public class Toast
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Message { get; init; } = string.Empty;
        public ToastType Type { get; init; } = ToastType.Warning;
        public int DurationMs { get; init; } = 4000;

        public Toast(ToastType type, string message)
        {
            Type = type;
            Message = message;
        }
    }

    public class ToastService
    {
        public void Notify(Toast toast)
        {
        }
    }
}
