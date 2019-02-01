using GridballCore.TurnCommands;
using RakNet;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GridBallRealtimeConsole
{
    class NetworkInterface
    {
        public const short AF_INET = 2;
        RakPeerInterface client, server;
        const int BIG_PACKET_SIZE = 103296250;
        string ip = string.Empty;

        byte typeToInt(TurnCommand command)
        {
            if(command is MoveTurnCommand)
            {
                return 1;
            }
            
            if(command is ThrowTurnCommand)
            {
                return 2;
            }
            if(command is NullTurnCommand)
            {
                return 3;
            }
            throw new NotImplementedException();
        }


        public void HandlePacketUpdates(int frame, TurnCommand myTurnCommand)
        {
            Packet packet = new Packet();
            byte[] text;
            text = new byte[BIG_PACKET_SIZE];

            if (server != null)
            {
                for (packet = server.Receive(); packet != null; server.DeallocatePacket(packet), packet = server.Receive())
                {
                    if ((DefaultMessageIDTypes)packet.data[0] == DefaultMessageIDTypes.ID_NEW_INCOMING_CONNECTION || packet.data[0] == (int)253)
                    {
                        Console.WriteLine("Starting send");
                        MemoryStream ms = new MemoryStream();
                        ms.WriteByte(255);
                        ms.WriteByte(typeToInt(myTurnCommand));
                        var binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(ms, myTurnCommand);
                 
                        
                        DefaultMessageIDTypes idtype = (DefaultMessageIDTypes)packet.data[0];
                        if (idtype == DefaultMessageIDTypes.ID_CONNECTION_LOST)
                            Console.WriteLine("ID_CONNECTION_LOST from {0}", packet.systemAddress.ToString());
                        else if (idtype == DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION)
                            Console.WriteLine("ID_DISCONNECTION_NOTIFICATION from {0}", packet.systemAddress.ToString());
                        else if (idtype == DefaultMessageIDTypes.ID_NEW_INCOMING_CONNECTION)
                            Console.WriteLine("ID_NEW_INCOMING_CONNECTION from {0}", packet.systemAddress.ToString());
                        else if (idtype == DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED)
                            Console.WriteLine("ID_CONNECTION_REQUEST_ACCEPTED from {0}", packet.systemAddress.ToString());

                        server.Send(text, BIG_PACKET_SIZE, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED_WITH_ACK_RECEIPT, (char)0, packet.systemAddress, false);
                    }
                }
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            Console.WriteLine("Sending medium priority message");
                            byte[] t = new byte[1];
                            t[0] = 254;
                            server.Send(t, 1, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE_ORDERED, (char)1, RakNet.RakNet.UNASSIGNED_SYSTEM_ADDRESS, true);
                            break;

                        default:
                            break;
                    }
                }
            }
            if (client != null)
            {
                
                packet = client.Receive();
                while (packet != null)
                {
                    DefaultMessageIDTypes idtype = (DefaultMessageIDTypes)packet.data[0];

                    if (idtype == DefaultMessageIDTypes.ID_DOWNLOAD_PROGRESS)
                    {
                        BitStream progressBS = new BitStream(packet.data, packet.length, false);
                        progressBS.IgnoreBits(8);
                        byte[] progress = new byte[4], total = new byte[4], partlength = new byte[4];

                        progressBS.ReadBits(progress, sizeof(uint) << 3, true);
                        progressBS.ReadBits(total, sizeof(uint) << 3, true);
                        progressBS.ReadBits(partlength, sizeof(uint) << 3, true);

                        Console.WriteLine("Progress: msgID= {0}, Progress: {1} / {2}, Partsize: {3}", packet.data[0].ToString(),
                            BitConverter.ToUInt32(progress, 0).ToString(),
                            BitConverter.ToUInt32(total, 0).ToString(),
                            BitConverter.ToUInt32(partlength, 0).ToString());

                    }
                    else if (packet.data[0] == 255)
                    {
                        

                    }
                    else if ((int)packet.data[0] == 254)
                    {
                        Console.WriteLine("Got high priority message.");
                    }
                    else if ((DefaultMessageIDTypes)packet.data[0] == DefaultMessageIDTypes.ID_CONNECTION_LOST)
                        Console.WriteLine("ID_CONNECTION_LOST from {0}", packet.systemAddress.ToString());
                    else if ((DefaultMessageIDTypes)packet.data[0] == DefaultMessageIDTypes.ID_NEW_INCOMING_CONNECTION)
                        Console.WriteLine("ID_NEW_INCOMING_CONNECTION from {0}", packet.systemAddress.ToString());
                    else if ((DefaultMessageIDTypes)packet.data[0] == DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED)
                    {
                        Console.WriteLine("ID_CONNECTION_REQUEST_ACCEPTED from {0}", packet.systemAddress.ToString());
                    }
                    else if ((DefaultMessageIDTypes)packet.data[0] == DefaultMessageIDTypes.ID_CONNECTION_ATTEMPT_FAILED)
                        Console.WriteLine("ID_CONNECTION_ATTEMPT_FAILED from {0}", packet.systemAddress.ToString());

                    client.DeallocatePacket(packet);
                    packet = client.Receive();
                }
            }
            
        }

        public void SetupNetworkInterface()
        {
            bool quit;
            bool sentPacket = false;
            byte[] text;
            string message;
            client = server = null;

            text = new byte[BIG_PACKET_SIZE];
            quit = false;
            char ch;

            Console.WriteLine("Enter 's' to run as server, 'c' to run as client, space to run local.");
            ch = ' ';
            message = Console.ReadLine();

            ch = message.ToCharArray()[0];

            if (ch == 'c')
            {
                client = RakPeerInterface.GetInstance();
                Console.WriteLine("Working as client");
                Console.WriteLine("Enter remote IP: ");
                ip = Console.ReadLine();
                if (ip.Length == 0)
                    ip = "127.0.0.1";
            }
            else if (ch == 's')
            {
                server = RakPeerInterface.GetInstance();
                server.SetMaximumIncomingConnections(1); //1v1 yo!
                Console.WriteLine("Working as server");
            }
            else
            {
                client = RakPeerInterface.GetInstance();
                server = RakPeerInterface.GetInstance();
                ip = "127.0.0.1";
            }

            short socketFamily;
            socketFamily = AF_INET;
            if (server != null)
            {
                server.SetTimeoutTime(5000, RakNet.RakNet.UNASSIGNED_SYSTEM_ADDRESS);
                SocketDescriptor socketDescriptor = new SocketDescriptor(3000, "0");
                socketDescriptor.socketFamily = socketFamily;
                server.SetMaximumIncomingConnections(4);
                StartupResult sr = new StartupResult();
                sr = server.Startup(4, socketDescriptor, 1);
                if (sr != StartupResult.RAKNET_STARTED)
                {
                    Console.WriteLine("Error: Server failed to start: {0} ", sr.ToString());
                    return;
                }

                // server.SetPerConnectionOutgoingBandwidthLimit(50000);
                Console.WriteLine("Server started on {0}", server.GetMyBoundAddress().ToString());
            }

            if (client != null)
            {
                client.SetTimeoutTime(5000, RakNet.RakNet.UNASSIGNED_SYSTEM_ADDRESS);
                SocketDescriptor socketDescriptor = new SocketDescriptor(0, "0");
                socketDescriptor.socketFamily = socketFamily;
                client.SetMaximumIncomingConnections(4);
                StartupResult sr = new StartupResult();
                sr = client.Startup(4, socketDescriptor, 1);
                if (sr != StartupResult.RAKNET_STARTED)
                {
                    Console.WriteLine("Error: Client failed to start: " + sr.ToString());
                    return;
                }
                client.SetSplitMessageProgressInterval(10000); // Get ID_DOWNLOAD_PROGRESS notifications

                client.SetPerConnectionOutgoingBandwidthLimit(10000);
                Console.WriteLine("Client started on {0}", client.GetMyBoundAddress().ToString());
                client.Connect(ip, 3000, null, 0);
            }

            System.Threading.Thread.Sleep(500);

            Console.WriteLine("My IP addresses: ");
            RakPeerInterface rakPeer;
            if (server != null)
                rakPeer = server;
            else
                rakPeer = client;

            for (uint i = 0; i < rakPeer.GetNumberOfAddresses(); i++)
            {
                Console.WriteLine("{0}. {1}", (i + 1).ToString(), rakPeer.GetLocalIP(i).ToString());
            }

            if(server != null)
            {
                Console.WriteLine("Waiting for client...");
                while (server.NumberOfConnections() == 0)
                {
                    System.Threading.Thread.Sleep(500);

                }
                Console.WriteLine("Client connected!");
            }


        }
    }
}
