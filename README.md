This repository contains code for a Logger that logs all the data of a connecting player and saves it in a .txt file inside the game directory.
The directory is called DiavoloLogs and each file inside of the folder will contain the logs ordered by date.
You can set the dvar sv_HWIDProtect in the server cfg to 1 or 0 (default is 1) to enable HWID spoofing protection.
This feature protects online players if someone tries to kick them or if someone tries to fill the lobby with bots (Same IP but different HWID).
If sv_LogLevel is set to 1, everything will be logged. If set to 0 only punishments or connections denied will be logged.
If sv_ShortNames is set to 1, players that have a suspiciously high amount of white spaces in their names and if the whitespaces are removed the length of the name is less than or equal to 3 they will be severely punished.