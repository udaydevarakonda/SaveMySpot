using System.Collections.Concurrent;

namespace SaveMySpot.Services;

public enum ToastLevel
{
    Info,
    Success,
    Warning,
    Error
}

public record ToastMessage(Guid Id, string Message, ToastLevel Level, string? Title, DateTime CreatedAt);

public class ToastService
{
    private readonly ConcurrentDictionary<Guid, ToastMessage> _messages = new();

    public event Action? OnToastsUpdated;

    public IReadOnlyCollection<ToastMessage> Toasts => _messages.Values.ToList();

    public void ShowSuccess(string message, string? title = null, int durationMs = 4500) =>
        AddToast(message, ToastLevel.Success, title ?? "Success", durationMs);

    public void ShowInfo(string message, string? title = null, int durationMs = 4500) =>
        AddToast(message, ToastLevel.Info, title ?? "Info", durationMs);

    public void ShowWarning(string message, string? title = null, int durationMs = 4500) =>
        AddToast(message, ToastLevel.Warning, title ?? "Warning", durationMs);

    public void ShowError(string message, string? title = null, int durationMs = 6000) =>
        AddToast(message, ToastLevel.Error, title ?? "Error", durationMs);

    public void Dismiss(Guid id)
    {
        if (_messages.TryRemove(id, out _))
        {
            NotifyStateChanged();
        }
    }

    private void AddToast(string message, ToastLevel level, string title, int durationMs)
    {
        var toast = new ToastMessage(Guid.NewGuid(), message, level, title, DateTime.UtcNow);
        _messages[toast.Id] = toast;
        NotifyStateChanged();

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(durationMs);
                Dismiss(toast.Id);
            }
            catch
            {
                // Ignore cancellation/exception from background delay.
            }
        });
    }

    private void NotifyStateChanged() => OnToastsUpdated?.Invoke();
}
