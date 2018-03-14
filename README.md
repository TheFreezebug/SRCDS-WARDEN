# SRCDS-WARDEN
## What is WARDEN?
WARDEN is a server watchdog, that ensures operation and connectivity of source-based servers.  It uses the source query protocol to check whether or not a server is reachable. If it isn't reachable, after a specified timeout, it will end the server task and restart it with the supplied parameters.

## Known working games

* Team Fortress 2
* Garry's Mod
* Synergy
* Half-Life 2 Deathmatch
* Counter Strike: Source
* Most source-based games.

# CONFIG.WA 
## This section explains the config.wa file. 
Config.wa should be in the same directory as the WARDEN executable.

The format of config.wa must be EXACTLY as follows.

### key=value

There is no space between the = and the value, all configuration keys are case sensitive. 

### Configuration items
`EXECUTABLE=srcds.exe`  the path or name to the server's executable. 

`parameters=-console` the parameters of which to execute the server's exe with.

#### QuickRestart configuration
QuickRestart protects against startup loops that use constant resources. Basically, if the server crashes X amount of times in Y seconds, then it will wait a time period before restarting the server. 

`QuickRestartTime=20` This controls how fast is considered a "fast restart". Meaning the server didn't start successfully, and crashed too soon. 

`QuickRestartMax=5` The amount of times the server can restart before waiting for QuickRestartWait time

`QuickRestartWait=20` The amount of seconds to wait after restarting too fast too many times.

#### Misc settings

`HideWindow=false` Attempts to FreeConsole on the SRCDS window and redirect stdout / stderr to the WARDEN window. Only works if you're using SrcdsConRedirect. 

#### ProtoWatch settings
ProtoWatch periodically querys your server to see if it will respond to a challenge request. If it does, this means that it's likely running correctly, and accepting players.  If the server consecutively fails a specified number of challenge requests, ProtoWatch will force WARDEN to restart the server. 

`PW_Enable=true` Enables or disables ProtoWatch

`PW_IP=127.0.0.1` The IP of which ProtoWatch looks for, this should be the public IP of your server. 

`PW_PORT=27015` The port of which your game server is running on, usually 27015, it's different if you specify it.

`PW_PingDelay=2` The time in between challenges in seconds.

`PW_PingTimeout=2` How long the game server has to respond to the challenge in seconds. 

`PW_MaxFailedPings=6` How many times the server can fail the challenges (consecutively) before it is forcibly restarted.

`PW_StartupDelay=40` Time <in seconds> it takes for your server to start up to its map -- this controls delayed activation of ProtoWatch. 
  
 The maximum amount of downtime this will have your server can be calculated by.
 
 StartupDelay + ((PingTimeout + PingDelay) * MaxFailedPings) + 2

# Warranty
WARDEN is provided with ABSOLUTELY NO WARRANTY, express or implied. I am not responsible for any damages or sufferage that may be caused by the use or misuse of WARDEN. 

  
