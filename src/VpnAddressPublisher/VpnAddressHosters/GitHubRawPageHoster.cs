using System;
using System.Linq;
using System.Net;
using LibGit2Sharp;
using System.IO;
using System.Configuration;

namespace VpnAddressPublisher
{
	public class GitHubRawPageHoster : IVpnAddressHoster
	{
		private const string HOSTER_REPOSITORY = @"https://github.com/shinetech-china-tianjin/VpnAddressHoster.git";
		private const string LOCAL_REPOSITORY = @"VpnAddressHoster";
		private const string ADDRESS_BRANCH = @"LOCAL_REPOSITORY";
		private const string VPN_ADDRESS_FILE = @"vpn.address";

		public void PublishVpnAddress (IPAddress ipaddress) {
			EnsureGitRepositoryExist();
			
			using (var repository = new Repository(LOCAL_REPOSITORY))
			{
				var vpnAddressBranch = repository.Branches.First(branch => string.Equals(branch.Name, ADDRESS_BRANCH, StringComparison.CurrentCultureIgnoreCase));

				CleanUpAndCheckoutAddressBranch(repository, vpnAddressBranch);
				ModifyVpnAddressFileAndCommit(repository, ipaddress);
				PushAddressToRemote(repository, vpnAddressBranch);
			}
		}

		private string GetLocalRepositoryDirectory(){
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOCAL_REPOSITORY);
		}

		private void EnsureGitRepositoryExist(){
			if(Directory.Exists(GetLocalRepositoryDirectory())){
				return;
			}

			Repository.Clone(HOSTER_REPOSITORY, LOCAL_REPOSITORY);
		}

		private void CleanUpAndCheckoutAddressBranch (Repository repository, Branch vpnAddressBranch)
		{
			repository.Reset();
			repository.Checkout(vpnAddressBranch.CanonicalName);
			vpnAddressBranch.Remote.Fetch();
		}

		private void ModifyVpnAddressFileAndCommit (Repository repository, IPAddress ipaddress)
		{	
			ModifyVpnAddress(ipaddress);

			var signature = new Signature(
				ConfigurationManager.AppSettings["CommitterName"],
				ConfigurationManager.AppSettings["CommitterEmail"],
				DateTimeOffset.Now
			);

			foreach (var changed in repository.Diff.Compare())
			{
				repository.Index.Stage(changed.Path);
			}

			repository.Commit(GetCommitMessage(), signature);
		}

		private string GetCommitMessage ()
		{
			var format = "[0]Commit new vpn address on {1} ({2})";
			return string.Format(format, 
			                     Environment.MachineName, 
			                     DateTime.Now, 
			                     TimeZone.CurrentTimeZone.StandardName);
		}

		private void ModifyVpnAddress (IPAddress ipaddress)
		{
			var filePath = Path.Combine(GetLocalRepositoryDirectory(), VPN_ADDRESS_FILE);
			using(var writer = new StreamWriter(filePath)){
				writer.WriteLine(ipaddress.ToString());
			}
		}

		private void PushAddressToRemote (Repository repository, Branch vpnAddressBranch)
		{
			var credentials = new Credentials() { 
				Username = ConfigurationManager.AppSettings["GitHubUserName"], 
				Password = ConfigurationManager.AppSettings["GitHubPassword"]
			};
			repository.Network.Push(vpnAddressBranch.Remote, repository.Head.CanonicalName, credentials);
		}
	}
}

