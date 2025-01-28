namespace AdmIn.UI.Services.UtilityServices
{
    public class PageTitleService
    {
        private Action _notifyStateChanged;

        public string Titulo { get; private set; }
        public string Subtitulo { get; private set; }

        public void SetTitle(string titulo, string subtitulo)
        {
            Titulo = titulo;
            Subtitulo = subtitulo;
            NotifyStateChanged();
        }

        public void ResetTitle()
        {
            Titulo = string.Empty;
            Subtitulo = string.Empty;
            NotifyStateChanged();
        }

        public void Register(Action notifyStateChanged)
        {
            _notifyStateChanged = notifyStateChanged;
        }

        private void NotifyStateChanged() => _notifyStateChanged?.Invoke();
    }
}
