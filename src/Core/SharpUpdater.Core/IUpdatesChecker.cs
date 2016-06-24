namespace CnSharp.Updater
{
    public interface IUpdatesChecker
    {
        string GetUpdateLogBetweenVersion(string localVersion, string remoteVersion);
    }
}
