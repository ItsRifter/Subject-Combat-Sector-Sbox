
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class SCSGame : Sandbox.Game
{

	SCSHud oldHUD;
	Sound soundPlaying;
	Sound musicPlaying;

	public SCSGame()
	{
		if(IsServer)
		{

		}

		if(IsClient)
		{
			oldHUD = new SCSHud();
		}
	}

	[Event.Hotload]
	public void UpdateHUD()
	{
		oldHUD?.Delete();

		if ( IsClient )
			oldHUD = new SCSHud();
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var player = new SCSPlayer();
		client.Pawn = player;

		if(Client.All.Count > 1 && soundPlaying.Finished )
			soundPlaying = Sound.FromScreen( "playerjoin" );

		player.Respawn();
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		if( soundPlaying.Finished )
			Sound.FromScreen("playerleave");
	}
}
