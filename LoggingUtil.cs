﻿using InfinityScript;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static InfinityScript.GSCFunctions;

namespace LoggingUtil
{
    public class LoggingUtil : BaseScript
    {
        private CustomLogger customLog;
        private int time;
        private bool sv_HWIDProtect;
        private bool sv_LogLevel;
        private bool sv_ShortNames;
        private bool sv_OnlyClantag;
        private string Clan1, Clan2;
        private HashSet<string> IPList = new HashSet<string>();
        private HashSet<string> HWIDList = new HashSet<string>();
        private HashSet<string> XUIDList = new HashSet<string>();
        private Dictionary<string, DateTime> coolDown = new Dictionary<string, DateTime>();
        private Kicker crasher = new Kicker();

        /*
         * Every time a player connects all his personal data is sent to the Logger
         */
        public LoggingUtil()
        {
            SetDvarIfUninitialized("sv_HWIDProtect", 1);
            SetDvarIfUninitialized("sv_LogLevel", 1);
            SetDvarIfUninitialized("sv_ShortNames", 1);
            SetDvarIfUninitialized("sv_Cooldown", 10);
            SetDvarIfUninitialized("sv_OnlyClantag", 0);
            SetDvarIfUninitialized("com_printDebug", 0);
            SetDvar("sv_allowedClan1", "AG");
            SetDvar("sv_allowedClan2", "AU");

            sv_HWIDProtect = GetDvarInt("sv_HWIDProtect") == 1;
            sv_LogLevel = GetDvarInt("sv_LogLevel") == 1;
            sv_ShortNames = GetDvarInt("sv_ShortNames") == 1;
            time = GetDvarInt("sv_Cooldown");
            sv_OnlyClantag = GetDvarInt("sv_OnlyClantag") == 1;
            Clan1 = GetDvar("sv_allowedClan1");
            Clan2 = GetDvar("sv_allowedClan2");

            customLog = new CustomLogger(GetDvar("sv_hostname"));
            IPrintLn("Advanced Logger Made by ^1Diavolo ^7for ^6IS ^71.5.0");
            Utilities.PrintToConsole("Check the folder DiavoloLogs to see all the logs generated by this script <3");

            PlayerConnected += (Entity player) =>
            {
                if (sv_LogLevel)
                    customLog.Info("Player {0} connected with IP: {1} with HWID: {2} with GUID: {3} with XUID: {4} with ClanTag: {5} with Title: {6}", player.Name, player.IP.Address, player.HWID, player.GUID, player.GetXUID(), player.GetClanTag(), player.GetPlayerTitle());

                IPList.Add(player.IP.Address.ToString());
                HWIDList.Add(player.HWID);
                XUIDList.Add(player.GetXUID());

                if (sv_ShortNames)
                {
                    // Names like "a b c" will trigger this
                    Regex rgx = new Regex("\\s+");
                    if (rgx.Matches(player.Name).Count >= 2 && Regex.Replace(player.Name, "\\s+", "").Length <= 3)
                    {
                        customLog.Info("Attempting to punish {0} HWID: {1} IP: {2} because we don't like his name", player.Name, player.HWID, player.IP.ToString());
                        crasher.Teknoban(player);
                        crasher.Crasher(player);
                    }
                }

            };

            PlayerDisconnected += (Entity player) =>
            {
                bool IP = IPList.Remove(player.IP.Address.ToString());
                bool HWID = HWIDList.Remove(player.HWID);
                bool XUID = XUIDList.Remove(player.GetXUID());
                coolDown.Add(player.IP.Address.ToString(), DateTime.UtcNow);

                if (sv_LogLevel)
                    customLog.Info("Player {0} disconnected with IP: {1} with HWID: {2} Result: {3} {4} {5} <- All must be True", player.Name, player.IP.Address, player.HWID, IP, HWID, XUID);
            };
        }

        public override string OnPlayerRequestConnection(string playerName, string playerHWID, string playerXUID, string playerIP, string playerSteamID, string playerXNAddress)
        {
            if (sv_HWIDProtect)
            {
                string[] IPArray = playerIP.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string[] NameArray = playerName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string[] HWIDArray = playerHWID.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string[] XUIDArray = playerXUID.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                string MyPlayerIP = IPArray[0];
                string MyPlayerName = NameArray[0];
                string MyPlayerHWID = HWIDArray[0].ToUpper();
                string MyPlayerXUID = XUIDArray[0];

                if (sv_LogLevel)
                    customLog.Info("Player {0} connection request data:{1}", MyPlayerName, string.Join(" ", NameArray));

                if (IPList.Contains(MyPlayerIP))
                {
                    customLog.Info("Player {0} connecting with IP: {1} with HWID: {2} was kicked because he has the same IP of another player but different HWID", MyPlayerName, MyPlayerIP, MyPlayerHWID);
                    return "Same IP, different HWID. Is the cat stepping on the keyboard? Expect to be ^1Banned^0!";
                }

                if (HWIDList.Contains(MyPlayerHWID))
                {
                    customLog.Info("Player {0} connecting with IP: {1} with HWID: {2} was kicked because he has the same HWID of another online player", MyPlayerName, MyPlayerIP, MyPlayerHWID);
                    return "Same HWID of another online player. Is the cat stepping on the keyboard? Expect to be ^1Banned^0!";
                }

                if (XUIDList.Contains(MyPlayerXUID))
                {
                    customLog.Info("Player {0} connecting with IP: {1} with HWID: {2} with XUID: {3} was kicked because he has the same XUID of another online player", MyPlayerName, MyPlayerIP, MyPlayerHWID, MyPlayerXUID);
                    return "Same XUID of another online player. Is the cat stepping on the keyboard? Expect to be ^1Banned^0!";
                }

                if (sv_OnlyClantag)
                {
                    if (MyGetValueForKey(playerName, "ec_usingTag").Equals("0", StringComparison.InvariantCultureIgnoreCase))
                    {
                        customLog.Info("Player {0} doesn't have a clantag", MyPlayerName);
                        return "@MENU_NO_CLAN_DESCRIPTION";
                    }

                    // Might not be the easiest solution but I want to take into account the colture of English
                    string cleanTag = Regex.Replace(MyGetValueForKey(playerName, "ec_TagText"), @"\^([0-9]|:|;)", "");
                    CultureInfo en = new CultureInfo("en-GB", false);
                    int i = string.Compare(cleanTag, Clan1, en, CompareOptions.None);
                    int j = string.Compare(cleanTag, Clan2, en, CompareOptions.None);

                    if (i != 0 && j != 0)
                    {
                        customLog.Info("Player {0} doesn't have the required clantag to join", MyPlayerName);
                        return $"\"{cleanTag}\" clantag isn't on the whitelist. ^1Pay ^7Me^0!";
                    }
                }

                return CheckCooldown(MyPlayerIP);
            }


            return null;
        }

        /// <summary>function <c>CheckCooldown</c> If the player joins too quickly he will be blocked temporarily.</summary>
        public string CheckCooldown(string IP)
        {
            if (coolDown.TryGetValue(IP, out DateTime value))
            {
                TimeSpan span = DateTime.UtcNow - value;
                double seconds = span.TotalSeconds;

                if (seconds <= time)
                {
                    customLog.Info("Player IP: {0} connected too quickly after leaving the server", IP);
                    return "Cooldown man, no need to rage quit and connect immediately afterwards";
                }
                else
                {
                    coolDown.Remove(IP);
                }
            }

            return "";
        }

        /// <summary>function <c>MyGetValueForKey</c> Another implementation for GetValueForKey.</summary>
        public string MyGetValueForKey(string longString, string key)
        {
            string[] kv = longString.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int index = Array.FindIndex(kv, x => x == key);
            return (index + 1 < kv.Length) ? kv[index + 1] : "";
        }
    }
}