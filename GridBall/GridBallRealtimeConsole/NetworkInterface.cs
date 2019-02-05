using GridballCore.TurnCommands;
using RakNet;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace GridBallRealtimeConsole
{
    class NetworkInterface
    {
        public const short AF_INET = 2;
        RakPeerInterface client, server;
        RakPeerInterface rakPeer;
        const int BIG_PACKET_SIZE = 103296250;
        string ip = string.Empty;
        internal bool isServer {
            get
            {
                return server != null;
            }
        }

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


        public TurnCommand HandlePacketUpdates(byte frame, TurnCommand myTurnCommand)
        {
            Packet packet = new Packet();
            byte[] text;
            text = new byte[BIG_PACKET_SIZE];
            var binaryFormatter = new BinaryFormatter();

            MemoryStream ms = new MemoryStream();
            ms.WriteByte(255);
            ms.WriteByte(frame);
            ms.WriteByte(typeToInt(myTurnCommand));
            binaryFormatter.Serialize(ms, myTurnCommand);
            rakPeer.Send(ms.ToArray(), (int)ms.Position, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE_ORDERED, (char)1, RakNet.RakNet.UNASSIGNED_SYSTEM_ADDRESS, true);
            RakPeerInterface reciever = rakPeer;
            if(client!=null && server !=null)
            {
                reciever = client;
            }
            while (true) {
                packet = reciever.Receive();
                if (packet != null  && packet.data[0] == 255 && packet.data[1] == frame)
                {
                    MemoryStream readStream = new MemoryStream(packet.data);
                    readStream.Position = 3;
                    var turn = (TurnCommand)binaryFormatter.Deserialize(readStream);
                    reciever.DeallocatePacket(packet);
                    return turn;
                    
                }
                if(packet != null)
                {
                    reciever.DeallocatePacket(packet);
                    packet = null;
                }

                Thread.Sleep(200);
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
