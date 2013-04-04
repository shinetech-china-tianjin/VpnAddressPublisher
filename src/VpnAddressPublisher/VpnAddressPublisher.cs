using System;
using System.Net;

namespace VpnAddressPublisher
{
	public class VpnAddressPublisher
	{
		private IVpnAddressHosterFactory _hosterFactory;

		public VpnAddressPublisher (IVpnAddressHosterFactory vpnAddressHosterFactory = null)
		{
			_hosterFactory = vpnAddressHosterFactory ?? new DefaultVpnAddressHosterFactory ();
		}

		public void PublishVpnAddress (IPAddress ipaddress)
		{
			var hoster = _hosterFactory.GetVpnAddressHoster ();
			hoster.PublishVpnAddress (ipaddress);
		}
	}
}

