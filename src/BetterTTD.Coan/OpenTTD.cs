using System.Numerics;
using BetterTTD.Coan.Domain;
using BetterTTD.Coan.Enums;
using BetterTTD.Coan.Networks;

namespace BetterTTD.Coan
{
    public class OpenTTD
    {
        public Game Game { get; } = new();
        public Pool Pool { get; } = new();
        public Network Network { get; }

        public string BotName { get; set; }
        public string BotVersion { get; set; }

        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 3977;
        public string Password { get; set; }

        public OpenTTD()
        {
            Network = new Network(this);
        }

        public bool Connect(string host, int port, string password)
        {
            HostName = host;
            Port = port;
            Password = password;

            return Connect();
        }

        public bool Connect()
        {
            var connect = Network.Connect(HostName, Port);
            Network.Receive();
            return connect;
        }

        protected void RegisterUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            Network.SendAdminUpdateFrequency(type, freq);
        }

        public void PollAll()
        {
            PollCmdNames();
            PollDate();
            PollClientInfos();
            PollCompanyInfos();
            PollCompanyStats();
            PollCompanyEconomy();
        }

        public void PollDate()
        {
            Network.PollDate();
        }

        public void PollClientInfos()
        {
            Network.PollClientInfos();
        }

        public void PollClientInfo(long clientId)
        {
            Network.PollClientInfo(clientId);
        }

        public void PollCompanyInfos()
        {
            Network.PollCompanyInfos();
        }

        public void PollCompanyInfo(int companyId)
        {
            Network.PollCompanyInfo(companyId);
        }

        public void PollCompanyStats()
        {
            Network.PollCompanyStats();
        }

        public void PollCompanyEconomy()
        {
            Network.PollCompanyEconomy();
        }

        public void PollCmdNames()
        {
            Network.PollCmdNames();
        }

        public void ServerMessagePublic(string msg)
        {
            Network.ServerMessagePublic(msg);
        }

        public void ServerMessagePrivate(long client, string msg)
        {
            Network.ServerMessagePrivate(client, msg);
        }

        public void SendAdminUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
            Network.SendAdminUpdateFrequency(type, freq);
        }

        public void SendAdminRcon(string command)
        {
            Network.SendAdminRcon(command);
        }

        public void SendAdminQuit()
        {
            Network.SendAdminQuit();
        }

        public void SendAdminChat(NetworkAction action, DestType type, long dest, string message, long data)
        {
            Network.SendAdminChat(action, type, dest, message, data);
        }

        public void SendAdminGameScript(string json)
        {
            Network.SendAdminGameScript(json);
        }

        public void SendAdminPing(long d1)
        {
            Network.SendAdminPing(d1);
        }

        public void ChatPublic(string msg)
        {
            Network.ChatPublic(msg);
        }

        public void ChatPrivate(long client, string msg)
        {
            Network.ChatPrivate(client, msg);
        }

        public void OnServerFull()
        {
        }

        public void OnServerBanned()
        {
        }

        public void OnServerError(NetworkErrorCode error)
        {
        }

        public void OnServerWelcome(Game game)
        {
        }

        public void OnServerDate(GameDate date)
        {
        }

        public void OnClientJoin(Client client)
        {
        }

        public void OnClientInfo(Client client)
        {
        }

        public void OnClientUpdate(Client client)
        {
        }

        public void OnClientQuit(Client client)
        {
        }

        public void OnClientError(Client client, NetworkErrorCode error)
        {
        }

        public void OnCompanyNew(Company company)
        {
        }

        public void OnCompanyInfo(Company company)
        {
        }

        public void OnCompanyUpdate(Company company)
        {
        }

        public void OnCompanyStats(Company company)
        {
        }

        public void OnCompanyEconomy(Company company)
        {
        }

        public void OnCompanyRemove(Company company, AdminCompanyRemoveReason crr)
        {
        }

        public void OnRcon(RconBuffer rconBuffer)
        {
        }

        public void OnChat(NetworkAction action, DestType destType, Client client, string message, BigInteger data)
        {
        }

        public void OnNewGame()
        {
        }

        public void OnShutdown()
        {
        }

        public void OnProtocol(Protocol protocol)
        {
        }

        public void OnConsole(string origin, string message)
        {
        }

        public void OnCmdLogging(Client client, Company company, DoCommandName command, long p1, long p2, long tile,
            string text, long frame)
        {
        }

        public void OnGameScript(string json)
        {
        }

        public void OnPong(long payload)
        {
        }

        public void OnPause(PauseMode pm, bool paused)
        {
        }
    }
}