using InfinityScript;
using System;

namespace LoggingUtil
{
    public class LoggingUtil : BaseScript
    {
      private CustomLogger customLog = new CustomLogger();

      /*
       * Every time a player connects all his personal data is sent to the Logger
       */
      public LoggingUtil()
        {
            base.PlayerConnected += delegate (Entity player)
            {
                customLog.info("Player {0} joined with IP: {1} with HWID: {2} with GUID: {3} with XUID: {4}", player.Name, player.IP.Address, player.HWID, player.GUID, player.GetXUID());
            };
        }
        
    }
}
