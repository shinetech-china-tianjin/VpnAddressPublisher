using System;

namespace VpnAddressPublisher
{
	public class DefaultVpnAddressHosterFactory : IVpnAddressHosterFactory
	{
		public IVpnAddressHoster GetVpnAddressHoster ()
		{
			return new GitHubRawPageHoster();
		}
	}
}

