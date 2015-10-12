// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Protocol & Photon Client Lib - Copyright (C) 2010 Exit Games GmbH
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
#if !UNITY_PS3
namespace ExitGames.Client.Photon
{
    using System;
    using System.Security;
    using System.Threading;
    using System.Runtime.InteropServices;

    internal class SocketUdpNativeDllImport : IPhotonSocket
    {
        [DllImport("PhotonSocketPlugin")]
        private static extern IntPtr egconnect([MarshalAs(UnmanagedType.LPStr)] string address);

        [DllImport("PhotonSocketPlugin")]
        private static extern byte eggetState(IntPtr pConnectionHandler);

        [DllImport("PhotonSocketPlugin")]
        private static extern void egdisconnect(IntPtr pConnectionHandler);

        [DllImport("PhotonSocketPlugin")]
        private static extern int egservice(IntPtr pConnectionHandler);

        [DllImport("PhotonSocketPlugin")]
        private static extern void egsend(IntPtr pConnectionHandler, byte[] arr, int size);

        [DllImport("PhotonSocketPlugin")]
        private static extern int egread(IntPtr pConnectionHandler, byte[] arr, int size);

        private IntPtr pConnectionHandler = IntPtr.Zero;
        
        private readonly object syncer = new object();

        // Native socket states - according to EnetConnect.h state definitions
        enum NativeSocketState : byte
        {
            Disconnected = 0,
            Connecting = 1,
            Connected = 2,
            ConnectionError = 3,
            SendError = 4,
            ReceiveError = 5,
            Disconnecting = 6
        }

        public SocketUdpNativeDllImport(PeerBase npeer) : base(npeer)
        {
            if (this.ReportDebugOfLevel(DebugLevel.ALL))
            {
                this.Listener.DebugReturn(DebugLevel.ALL, "SocketWrapper: UDP, Unity Android Native.");
            }

            this.Protocol = ConnectionProtocol.Udp;
            this.PollReceive = false;
        }

        public override bool Connect()
        {
            bool baseOk = base.Connect();
            if (!baseOk)
            {
                return false;
            }

            this.State = PhotonSocketState.Connecting;

            Thread dns = new Thread(this.DnsAndConnect);
            dns.IsBackground = true;
            dns.Start();

            return true;
        }

        public override bool Disconnect()
        {
            if (this.ReportDebugOfLevel(DebugLevel.INFO))
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "SocketWrapper.Disconnect()");
            }

            this.State = PhotonSocketState.Disconnecting;

            lock (this.syncer)
            {
                if (this.pConnectionHandler != IntPtr.Zero)
                {
                    try
                    {
                        egdisconnect(pConnectionHandler);
                        pConnectionHandler = IntPtr.Zero;
                    }
                    catch (Exception ex)
                    {
                        this.Listener.DebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
                    }
                }
            }

            this.State = PhotonSocketState.Disconnected;
            return true;
        }

        /// <summary>used by PhotonPeer*</summary>
        public override PhotonSocketError Send(byte[] data, int length)
        {
            lock (this.syncer)
            {
                if (pConnectionHandler != IntPtr.Zero)
                {
                    egsend(pConnectionHandler, data, length);
                    return PhotonSocketError.Success; // TODO: check success by accessing the state
                }
            }

            return PhotonSocketError.Success;
        }

        public override PhotonSocketError Receive(out byte[] data)
        {
            data = null;
            return PhotonSocketError.NoData;
        }

        internal void DnsAndConnect()
        {
            try
            {
                lock (this.syncer)
                {
                    //Debug.Log("DnsAndConnect: " + pConnectionHandler + " ServerAddress: " + this.ServerAddress);

                    pConnectionHandler = egconnect(this.ServerAddress + ":" + this.ServerPort);
                    int state = egservice(pConnectionHandler);  // need to call egService once to make it connect and switch from connecting to connected state
                    
                    this.State = PhotonSocketState.Connected; // TODO: access real low-level state
                }
            }
            catch (SecurityException se)
            {
                if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    this.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: " + se.ToString());
                }

                this.HandleException(StatusCode.SecurityExceptionOnConnect);
                return;
            }
            catch (Exception se)
            {
                if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    this.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: " + se.ToString());
                }

                this.HandleException(StatusCode.ExceptionOnConnect);
                return;
            }

            Thread run = new Thread(new ThreadStart(ReceiveLoop));
            run.IsBackground = true;
            run.Start();
        }

        public void ReceiveLoop()
        {
            while (this.State == PhotonSocketState.Connecting || this.State == PhotonSocketState.Connected)
            {
                try
                {
                    if (pConnectionHandler != IntPtr.Zero)
                    {
                        int serviceResult = egservice(pConnectionHandler);
                        if (serviceResult != 0)
                        {
                            //Debug.Log("ReceiveLoop: " + pConnectionHandler + " res: " + serviceResult);
                            byte[] bytes = new byte[serviceResult];

                            int read = egread(pConnectionHandler, bytes, serviceResult); // returns byte-count of next datagram. or 0 if none available
                            this.HandleReceivedDatagram(bytes, serviceResult, false);

                            if (read > 0)
                            {
                                continue; // read next immediately if available
                            }
                            //Debug.Log("ReceiveLoop: " + pConnectionHandler + " res: " + serviceResult + " read: " + read + " bytes: " + SupportClass.ByteArrayToString(bytes));
                        }

                        // check if native socket is still ok and connected
                        byte state = eggetState(pConnectionHandler);
                        if (state != (byte)NativeSocketState.Connected && state != (byte)NativeSocketState.Connecting)
                        {
                            if (state == (byte) NativeSocketState.ConnectionError || state == (byte) NativeSocketState.ReceiveError || state == (byte) NativeSocketState.SendError)
                            {
                                throw new Exception("Native socket has receive error or is disconnected: " + state);
                            }
                            if (state == (byte)NativeSocketState.Disconnected || state == (byte)NativeSocketState.Disconnecting)
                            {
                                EnqueueDebugReturn(DebugLevel.ERROR, "Disconnecting cause native socket's state during read is: " + state);
                                this.State = PhotonSocketState.Disconnecting;
                            }
                        }
                    }

                    Thread.Sleep(0);
                }
                catch (ObjectDisposedException ode)
                {
                    if (this.State != PhotonSocketState.Disconnecting && this.State != PhotonSocketState.Disconnected)
                    {
                        if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                        {
                            this.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue (ObjectDisposedException). State: " + this.State + " Exception: " + ode);
                        }

                        this.HandleException(StatusCode.ExceptionOnReceive);
                        break;
                    }
                }
                catch (System.Exception e)
                {
                    if (this.State != PhotonSocketState.Disconnecting && this.State != PhotonSocketState.Disconnected)
                    {
                        if (this.ReportDebugOfLevel(DebugLevel.ERROR))
                        {
                            this.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + this.State + " Exception: " + e);
                        }

                        this.HandleException(StatusCode.ExceptionOnReceive);
                        break;
                    }
                }

            } //while !obsolete receive


            // when exiting the receive-loop, disconnect
            this.Disconnect();
        }
    }
}
#endif