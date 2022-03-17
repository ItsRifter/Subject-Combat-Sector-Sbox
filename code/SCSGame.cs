
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

	Sound soundPlaying;
	Sound musicPlaying;

	[Net] protected int MaxRounds { get; private set; } = -1;
	[Net] protected int RedRoundPoints { get; private set; } = 0;
	[Net] protected int BlueRoundPoints { get; private set; } = 0;
	[Net] protected int GreenRoundPoints { get; private set; } = 0;
	[Net] protected int YellowRoundPoints { get; private set; } = 0;

	[Net] protected SCSPlayer.TeamEnum WinningTeam { get; private set; }

	[ConVar.Replicated]
	protected static bool DebugMode { get; set; } = false;

	[AdminCmd("scs_debug")]
	public static void DebugCmd()
	{
		DebugMode = !DebugMode;
		Log.Info( "Debug: " + DebugMode );
	}

	protected int TotalTeams { get; private set; } = 2;
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
			new SCSHud();
		}
	}

	public SCSPlayer.TeamEnum GetWinningTeam()
	{
		return WinningTeam;
	}

	public int GetTotalTeams()
	{
		return TotalTeams;
	}

	public int GetTargetScore()
	{
		return MaxRounds;
	}

	public int GetTeamScore(string team)
	{
		if ( team == "red" )
			return RedRoundPoints;
		else if ( team == "blue" )
			return BlueRoundPoints;
		else if ( team == "green" )
			return GreenRoundPoints;
		else if ( team == "yellow" )
			return YellowRoundPoints;

		return 0;
	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( !DebugMode )
			return;

		if(player.Pawn is SCSPlayer pl)
		{
			if ( pl.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				pl.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				pl.DevController = new NoclipController();
			}
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
		TeamPoints winningPoints = null;

		foreach ( var ents in All )
		{
			if(ents is NPCSpawner spawner && spawner.aliveNPCs.Count > 0)
				winningTeam = spawner;

			if(ents is TeamPoints pointGiver && winningTeam.TeamSide.ToString().Contains( pointGiver.TeamPointAssigned.ToString() ) )
				winningPoints = pointGiver;
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

				case NPCSpawner.TeamSideEnum.Green:
					Sound.FromScreen( "green_win" );
					GreenRoundPoints++;
					break;

				case NPCSpawner.TeamSideEnum.Yellow:
					Sound.FromScreen( "yellow_win" );
					YellowRoundPoints++;
					break;
			}

			winningPoints.AddPoints( 2 * TotalTeams );
		}

		if ( RedRoundPoints >= MaxRounds )
			EndGame( SCSPlayer.TeamEnum.Red );
		else if ( BlueRoundPoints >= MaxRounds )
			EndGame( SCSPlayer.TeamEnum.Blue );
		else if ( GreenRoundPoints >= MaxRounds )
			EndGame( SCSPlayer.TeamEnum.Green );
		else if ( YellowRoundPoints >= MaxRounds )
			EndGame( SCSPlayer.TeamEnum.Yellow );
	}

	public void EndGame(SCSPlayer.TeamEnum winningTeam)
	{
		Log.Info( "Game over" );
		Log.Info( winningTeam + " has won!" );

		WinningTeam = winningTeam;

		var teleporter = Entity.FindByName( "tele_" + winningTeam.ToString().ToLower() + "_win" ) as TeamTeleporter;

		teleporter.Enable();

		GameStatus = GameEnum.Post;
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

			if(entity is TeamCrystalBox teamBox)
			{
				if ( (teamBox.TeamBoxAssignment == TeamCrystalBox.TeamCrystalBoxType.Red && !teams[0]) ||
				(teamBox.TeamBoxAssignment == TeamCrystalBox.TeamCrystalBoxType.Blue && !teams[1]) ||
				(teamBox.TeamBoxAssignment == TeamCrystalBox.TeamCrystalBoxType.Green && !teams[2]) ||
				(teamBox.TeamBoxAssignment == TeamCrystalBox.TeamCrystalBoxType.Yellow && !teams[3]) )
					teamBox.Delete();
				
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

			TotalTeams = totalTeams;
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
