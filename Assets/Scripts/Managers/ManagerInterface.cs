public interface Manager
{
    bool IsLoaded { get; }
    void StartManager();
    void PauseManager();
    void ResumeManager();
}