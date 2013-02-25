using System;

namespace VpnAddressPublisher
{
	public interface IVpnAddressHosterFactory
	{
		IVpnAddressHoster GetVpnAddressHoster();
	}
}

