﻿using System;
using System.Net;

namespace Lidgren.Network
{
	public partial class NetPeer
	{
		/// <summary>
		/// Emit a discovery signal to all hosts on your subnet
		/// </summary>
		public void DiscoverLocalPeers(int serverPort)
		{
			NetOutgoingMessage om = CreateMessage(0);
			om.m_messageType = NetMessageType.Discovery;
			m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(new IPEndPoint(IPAddress.Broadcast, serverPort), om));
		}

		/// <summary>
		/// Emit a discovery signal to a single known host
		/// </summary>
		public bool DiscoverKnownPeer(string host, int serverPort)
		{
			IPAddress address = NetUtility.Resolve(host);
			if (address == null)
				return false;
			DiscoverKnownPeer(new IPEndPoint(address, serverPort));
			return true;
		}

		/// <summary>
		/// Emit a discovery signal to a single known host
		/// </summary>
		public void DiscoverKnownPeer(IPEndPoint endpoint)
		{
			NetOutgoingMessage om = CreateMessage(0);
			om.m_messageType = NetMessageType.Discovery;
			m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(endpoint, om));
		}

		/// <summary>
		/// Send a discovery response message
		/// </summary>
		public void SendDiscoveryResponse(NetOutgoingMessage msg, IPEndPoint recipient)
		{
			if (recipient == null)
				throw new ArgumentNullException("recipient");

			if (msg == null)
				msg = CreateMessage(0);
			else if (msg.m_isSent)
				throw new NetException("Message has already been sent!");

			msg.m_messageType = NetMessageType.DiscoveryResponse;
			m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(recipient, msg));
		}
	}
}