using System;
using System.Net;

namespace VpnAddressPublisher
{
	public interface IVpnAddressHoster
	{
		void PublishVpnAddress(IPAddress ipaddress);
	}
}

