
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class SCSGame : Sandbox.Game
{
	public static new SCSGame Current => Sandbox.Game.Current as SCSGame;

	SCSHud oldHUD;
	Sound soundPlaying;
	Sound musicPlaying;

	[Net] public int MaxRounds { get; private set; } = -1;
	[Net] public int CurRound { get; private set; } = 1;

	[Net] public int RedRoundPoints { get; private set; } = 0;
	[Net] public int BlueRoundPoints { get; private set; } = 0;
	[Net] public int GreenRoundPoints { get; private set; } = 0;
	[Net] public int YellowRoundPoints { get; private set; } = 0;

	public int TotalTeams { get; private set; } = 2;
	public enum GameEnum
	{
		Idle,
		Active,
		Post
	}

	public GameEnum GameStatus;

	public enum RoundEnum
	{
		Waiting,
		Starting,
		Active,
		Active_Bonus,
		Post,
	}

	[Net, Change( nameof( UpdateRoundStatus ))]
	public RoundEnum RoundStatus { get; set; }

	public SCSGame()
	{
		if(IsServer)
		{
			GameStatus = GameEnum.Idle;
			RoundStatus = RoundEnum.Waiting;
		}

		if(IsClient)
		{
			oldHUD = new SCSHud();
		}
	}

	public void UpdateRoundStatus( RoundEnum oldStatus, RoundEnum newStatus )
	{
		if ( RoundStatus == RoundEnum.Starting )
		{
			Sound.FromScreen( "prepare" );
		}
		else if ( RoundStatus == RoundEnum.Active )
		{
			musicPlaying = Sound.FromScreen( "battle_music" );
			musicPlaying.SetVolume( 0.50f );
		}
		else if ( RoundStatus == RoundEnum.Active_Bonus )
		{
			musicPlaying = Sound.FromScreen( "bonus_round" );
			musicPlaying.SetVolume( 0.50f );
		}
		else
		{
			musicPlaying.Stop();
		}
	}

	public void DeclareRoundWinner()
	{
		NPCSpawner winningTeam = null;

		foreach ( var ents in All )
		{
			if(ents is NPCSpawner spawner && spawner.aliveNPCs.Count > 0)
			{
				winningTeam = spawner;
			}
		}

		if ( winningTeam != null )
		{
			switch ( winningTeam.TeamSide )
			{
				case NPCSpawner.TeamSideEnum.Red:
					Sound.FromScreen( "red_win" );
					RedRoundPoints++;
					break;

				case NPCSpawner.TeamSideEnum.Blue:
					Sound.FromScreen( "blue_win" );
					BlueRoundPoints++;
					break;
			}
		}

		CurRound++;

		if( CurRound > MaxRounds )
		{
			EndGame();
		}
	}

	public void EndGame()
	{
		Log.Info( "Game over, lets see who won" );

		if ( RedRoundPoints > BlueRoundPoints && RedRoundPoints > GreenRoundPoints && RedRoundPoints > YellowRoundPoints )
		{
			Log.Info( "Red team has won!" );
		}
		else if ( BlueRoundPoints > RedRoundPoints && BlueRoundPoints > GreenRoundPoints && BlueRoundPoints > YellowRoundPoints )
		{
			Log.Info( "Blue team has won!" );
		}
		else if ( GreenRoundPoints > RedRoundPoints && GreenRoundPoints > BlueRoundPoints && GreenRoundPoints > YellowRoundPoints )
		{
			Log.Info( "Green team has won!" );
		}
		else if ( YellowRoundPoints > RedRoundPoints && YellowRoundPoints > BlueRoundPoints && YellowRoundPoints > GreenRoundPoints )
		{
			Log.Info( "Green team has won!" );
		}
	}

	[Event.Hotload]
	public void UpdateHUD()
	{
		oldHUD?.Delete();

		if ( IsClient )
			oldHUD = new SCSHud();
	}

	[ServerCmd("scs_cmd_startgame")]
	public static void StartGame(int settingRound, int startPoints, bool redOn, bool blueOn, bool greenOn, bool yellowOn )
	{
		bool[] teamsEnabled = new bool[4] { redOn, blueOn, greenOn, yellowOn };

		Event.Run( "scs_evnt_startgame", settingRound );
		Event.Run( "scs_evnt_setupteams" , startPoints, teamsEnabled );
	}

	[Event("scs_evnt_startgame")]
	public void StartGameplay( int settingRound )
	{
		if ( GameStatus == GameEnum.Active )
			return;

		GameStatus = GameEnum.Active;

		MaxRounds = settingRound;
	}

	[Event( "scs_evnt_setupteams" )]
	public void SetUpTeams( int startPoints, bool[] teams )
	{
		foreach ( var entity in Entity.All )
		{
			if ( entity is TeamPoints pointTracker )
			{
				if( (pointTracker.TeamPointAssigned == TeamPoints.TeamPointsTypeEnum.Red && teams[0]) ||
					(pointTracker.TeamPointAssigned == TeamPoints.TeamPointsTypeEnum.Blue && teams[1]) ||
					(pointTracker.TeamPointAssigned == TeamPoints.TeamPointsTypeEnum.Green && teams[2]) ||
					(pointTracker.TeamPointAssigned == TeamPoints.TeamPointsTypeEnum.Yellow && teams[3])
				)
					pointTracker.SetPoints( startPoints );
			}

			if ( entity is AssignTeamTrigger teamAssigner )
			{
				if(teamAssigner.TeamAssign == AssignTeamTrigger.TeamType.Red && teams[0])
					teamAssigner.Enable();
				else if (teamAssigner.TeamAssign == AssignTeamTrigger.TeamType.Blue && teams[1] )
					teamAssigner.Enable();
				else if ( teamAssigner.TeamAssign == AssignTeamTrigger.TeamType.Green && teams[2] )
					teamAssigner.Enable();
				else if ( teamAssigner.TeamAssign == AssignTeamTrigger.TeamType.Yellow && teams[3] )
					teamAssigner.Enable();
			}

			int totalTeams = 0;
			
			for ( int i = 0; i < 4; i++ )
			{
				if ( teams[i] )
					totalTeams++;
			}

			if( entity is MathCounter counter )
			{
				if ( counter.Name == "counter_teams_ready" )
					counter.SetMax( totalTeams );
				else if ( counter.Name == "counter_teams_elim" )
					counter.SetMax( totalTeams - 1 );
			}
		}
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
