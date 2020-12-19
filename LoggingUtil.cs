using InfinityScript;
using System.Collections.Generic;
using static InfinityScript.GSCFunctions;

namespace LoggingUtil
{
    public class LoggingUtil : BaseScript
    {
        private CustomLogger customLog = new CustomLogger();
        private bool sv_HWIDProtect;
        private HashSet<string> IPList = new HashSet<string>();
        private HashSet<string> HWIDList = new HashSet<string>();

        /*
         * Every time a player connects all his personal data is sent to the Logger
         */
        public LoggingUtil()
        {
            SetDvarIfUninitialized("sv_HWIDProtect", "1");
            sv_HWIDProtect = GetDvarInt("sv_HWIDProtect") == 1;

            PlayerConnecting += (Entity player) =>
            {
                customLog.Info("Player {0} connecting with IP: {1} with HWID: {2} with GUID: {3} with XUID: {4} with ClanTag: {5} with Title: {6}", player.Name, player.IP.Address, player.HWID, player.GUID, player.GetXUID(), player.GetClanTag(), player.GetPlayerTitle());

                if(sv_HWIDProtect)
                {
                    if (IPList.Contains(player.IP.Address.ToString()))
                    {
                        Utilities.ExecuteCommand($"dropclient {player.EntRef} \"Same IP, different HWID. Is the cat stepping on the keyboard?\"");
                        customLog.Info("Player {0} connecting with IP: {1} with HWID: {2} was kicked because he has the same IP of another player but different HWID", player.Name, player.IP.Address, player.HWID);
                    }
                    else
                        IPList.Add(player.IP.Address.ToString());

                    if(HWIDList.Contains(player.HWID))
                    {
                        Utilities.ExecuteCommand($"dropclient {player.EntRef} \"Same HWID of another online player. Is the cat stepping on the keyboard?\"");
                        customLog.Info("Player {0} connecting with IP: {1} with HWID: {2} was kicked because he has the same HWID of another online player", player.Name, player.IP.Address, player.HWID);
                    }
                    else
                        HWIDList.Add(player.HWID);
                }
            };
        }

        public override void OnPlayerDisconnect(Entity player)
        {
            IPList.Remove(player.IP.ToString());
            HWIDList.Remove(player.HWID);
        }
    }
}
