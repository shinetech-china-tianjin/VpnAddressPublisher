using System;
using System.Linq;
using System.Net;
using LibGit2Sharp;
using System.IO;
using System.Configuration;

namespace VpnAddressPublisher
{
	// make it to internal?
    public class GitHubRawPageHoster : IVpnAddressHoster
    {
        public void PublishVpnAddress(IPAddress ipaddress) {
            EnsureGitRepositoryExist();

            using (var repository = new Repository(GitHubHosterConfiguration.LocalRepositoryName)) {
                

                CleanUpCurrentBranch(repository);
                ModifyVpnAddressFileAndCommit(repository, ipaddress);
                PushAddressToRemote(repository);
            }
        }

        private string GetLocalRepositoryDirectory() {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GitHubHosterConfiguration.LocalRepositoryName);
        }

        private void EnsureGitRepositoryExist() {
            if (Directory.Exists(GetLocalRepositoryDirectory())) {
                return;
            }

            using (var repository = Repository.Clone(GitHubHosterConfiguration.HosterRepository, GitHubHosterConfiguration.LocalRepositoryName)) {
                var remoteVpnAddressBranch = repository.Branches.First(branch =>
                        string.Equals(
                            branch.Name,
                            "origin/" + GitHubHosterConfiguration.HosterBranch,
                            StringComparison.CurrentCultureIgnoreCase));
                var localBranch = repository.CreateBranch(GitHubHosterConfiguration.HosterBranch, remoteVpnAddressBranch.CanonicalName);
                repository.Branches.Update(localBranch, branch => branch.Upstream = remoteVpnAddressBranch.CanonicalName);
                repository.Checkout(localBranch);
            }
        }

        private void CleanUpCurrentBranch(Repository repository) {
            repository.Reset(ResetOptions.Hard);
            repository.RemoveUntrackedFiles();

            repository.Head.Remote.Fetch();
        }

        private void ModifyVpnAddressFileAndCommit(Repository repository, IPAddress ipaddress) {
            ModifyVpnAddress(ipaddress);

            var signature = new Signature(
                GitHubHosterConfiguration.CommitterName,
                GitHubHosterConfiguration.CommitterEmail,
                DateTimeOffset.Now
            );

            foreach (var changed in repository.Diff.Compare()) {
                repository.Index.Stage(changed.Path);
            }

            repository.Commit(GetCommitMessage(), signature);
        }

        private string GetCommitMessage() {
            var format = "[0]Commit new vpn address on {1} ({2})";
            return string.Format(format,
                                 Environment.MachineName,
                                 DateTime.Now,
                                 TimeZone.CurrentTimeZone.StandardName);
        }

        private void ModifyVpnAddress(IPAddress ipaddress) {
            var filePath = Path.Combine(
                GetLocalRepositoryDirectory(),
                GitHubHosterConfiguration.HosterVpnAddressFile);
            using (var writer = new StreamWriter(filePath)) {
                writer.WriteLine(ipaddress.ToString());
            }
        }

        private void PushAddressToRemote(Repository repository) {
            var credentials = new Credentials() {
                Username = GitHubHosterConfiguration.GitHubUserName,
                Password = GitHubHosterConfiguration.GitHubPassword
            };
            var head = repository.Head;
            repository.Network.Push(head.Remote, head.CanonicalName, credentials);
        }
    }
}

