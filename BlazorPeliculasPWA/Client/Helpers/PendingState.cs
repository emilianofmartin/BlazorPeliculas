namespace BlazorPeliculas.Client.Helpers {
    public class PendingState {
        public event Func<Task> OnUpdatePendingSynchronizations;
        public async Task NotifyUpdatePendingSynchronizations() => await OnUpdatePendingSynchronizations?.Invoke();
    }
}
