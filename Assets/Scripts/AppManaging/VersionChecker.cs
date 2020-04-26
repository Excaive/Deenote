using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Octokit;
using Debug = UnityEngine.Debug;

namespace Deenote
{
    public sealed class VersionChecker : MonoBehaviour
    {
        public static VersionChecker Instance { get; private set; }

        [SerializeField] private string currentVersion;

        private int[] VersionStringToArray(string version) =>
            version.Split('.').Select(int.Parse).ToArray();

        private bool UpToDate(int[] current, int[] latest)
        {
            int currentLength = current.Length;
            int latestLength = latest.Length;
            int min = Mathf.Min(currentLength, latestLength);
            for (int i = 0; i < min; i++)
                if (current[i] < latest[i])
                    return false;
                else if (current[i] > latest[i])
                    return true;
            return currentLength >= latestLength;
        }

        public void CheckUpdate(bool noticeWhenUpToDate)
        {
            Task _ = Instance.CheckUpdateAsync(noticeWhenUpToDate);
        }

        private async Task CheckUpdateAsync(bool noticeWhenUpToDate)
        {
            if (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable)
            {
                MessageBox.Instance.Activate("UpdateCheckTitle", "NoInternet",
                    new MessageBox.ButtonInfo {key = "Retry", callback = () => CheckUpdate(noticeWhenUpToDate)},
                    new MessageBox.ButtonInfo {key = "Ignore"});
                return;
            }

            GitHubClient client = new GitHubClient(new ProductHeaderValue("Deenote"));
            var releases = await client.Repository.Release.GetAll("Chlorie", "Deenote");
            string latestVersion = releases[0].TagName.Remove(0, 1);
            int[] latest = VersionStringToArray(latestVersion);
            int[] current = VersionStringToArray(currentVersion);
            if (UpToDate(current, latest))
            {
                if (noticeWhenUpToDate)
                    MessageBox.Instance.Activate("UpdateCheckTitle", "UpToDate",
                        new MessageBox.ButtonInfo {key = "OK"});
                return;
            }

            string suffix = IntPtr.Size == 8 ? ".zip" : "-32bit.zip";
            string link = $"https://github.com/Chlorie/Deenote/releases/download/v{latestVersion}/" +
                          $"Deenote-{latestVersion}{suffix}";
            MessageBox.Instance.Activate("UpdateCheckTitle", "UpdateDetected",
                new MessageBox.ButtonInfo
                {
                    key = "ReleasePage",
                    callback = () => Process.Start("https://github.com/Chlorie/Deenote/releases/latest")
                }, new MessageBox.ButtonInfo
                {
                    key = "DownloadPage",
                    callback = () => Process.Start(link)
                }, new MessageBox.ButtonInfo {key = "Ignore"});
        }

        private void Awake()
        {
#if DEBUG
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(this);
                Debug.LogError("Error: Unexpected multiple instances of VersionChecker");
            }
#else
            Instance = this;
#endif
        }
    }
}
