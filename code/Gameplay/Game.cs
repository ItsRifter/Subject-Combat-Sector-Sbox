using Sandbox;
using System;
using System.Linq;
using SCS.Player;

namespace SCS;

public partial class SCSGame : GameManager
{
	public static SCSGame Instance => Current as SCSGame;

	public enum GameStateEnum
	{
		Idle,
		Active,
		Post
	}

	public GameStateEnum GameState { get; set; }
	public static GameStateEnum StaticGameState => Instance?.GameState ?? GameStateEnum.Idle;

	public SCSGame()
	{
		if ( Game.IsServer )
		{
			GameState = GameStateEnum.Idle;
		}

		if ( Game.IsClient )
		{
			_ = new SCSHud();
		}
	}

	[Event.Hotload]
	public void GameHotload()
	{
		if ( Game.IsServer )
		{

		}

		if ( Game.IsClient )
		{
			_ = new SCSHud();
		}
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if(Game.IsServer)
		{
			
		}
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var pawn = new SCSPawn();
		client.Pawn = pawn;
		pawn.Spawn();
	}

	public override void ClientDisconnect( IClient cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );
	}
}
