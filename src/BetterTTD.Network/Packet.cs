using System.Net.Sockets;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Network
{
    public partial class Packet
    {
        private const int SendMtu = 1460;
        private const int PosPacketType = 2;

        private int _pos;
        private readonly byte[] _buf = new byte[SendMtu];
        private PacketType _type;

        public Socket Socket { get; }
        
        public Packet(Socket socket, PacketType type)
        {
            Socket = socket;
            _buf = new byte[SendMtu];
            SetType(type);
            _pos = PosPacketType + 1;
        }

        public Packet(Socket socket)
        {
            Socket = socket;

            if (socket.Connected == false)
                return;

            var length = Socket.Receive(_buf);

            if (length == 0)
                throw new SocketException();

            _pos = PosPacketType + 1;
        }

        public int Length()
        {
            var b1 = _buf[0] & 0xFF;
            var b2 = _buf[1] & 0xFF;

            var r = b1 + (b2 << 8);
            return r;
        }

        public PacketType GetPacketType()
        {
            if (_type != 0) 
                return _type;
            var type = (PacketType)(_buf[PosPacketType] & 0xFF);
            return _type = type;
        }

        public void Send()
        {
            _buf[0] = (byte)_pos;
            _buf[1] = (byte)(_pos >> 8);

            Socket.Send(_buf, _pos, SocketFlags.None);
        }
        
        private void SetType(PacketType type)
        {
            _buf[PosPacketType] = (byte)type;
        }
    }

}
