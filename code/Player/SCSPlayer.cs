using Sandbox;
using System;
using System.Linq;

partial class SCSPlayer : Player
{
	public enum TeamEnum
	{
		Unspecified,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Net] public TeamEnum CurTeam { get; set; } = TeamEnum.Unspecified;

	[AdminCmd( "scs_doshit" )]
	public static void Doshit()
	{
		foreach ( var client in Client.All )
		{
			if ( client is SCSPlayer player )
			{
				switch ( player.CurTeam )
				{
					case SCSPlayer.TeamEnum.Red:
						player.Transform = Entity.FindByName( "dest_redroom" ).Transform;
						break;
					case SCSPlayer.TeamEnum.Blue:
						player.Position = new Vector3( 2217, 373, 0 );
						break;
				}

			}
		}
	}

	public override void Respawn()
	{
		base.Respawn();

		SetModel( "models/player/hevsuit_white.vmdl" );

		CameraMode = new FirstPersonCamera();
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		holdBody = new PhysicsBody( Map.Physics )
		{
			BodyType = PhysicsBodyType.Keyframed
		};
	}

	[AdminCmd( "scs_health" )]
	public static void SetHealthCMD( int setHP = 100 )
	{
		Event.Run( "scs_evnt_health", setHP );
	}

	[Event( "scs_evnt_health" )]
	public void SetHP( int newHP )
	{
		Health = newHP;
	}


	[AdminCmd("scs_setteam")]
	public static void SetTeamCMD(int team = 0)
	{
		if ( team == 1 )
			Event.Run( "scs_evnt_setteam", TeamEnum.Red );
		else if ( team == 2 )
			Event.Run( "scs_evnt_setteam", TeamEnum.Blue );
		else if ( team == 3 )
			Event.Run( "scs_evnt_setteam", TeamEnum.Green );
		else if ( team == 4 )
			Event.Run( "scs_evnt_setteam", TeamEnum.Yellow );
		else
			Event.Run( "scs_evnt_setteam", TeamEnum.Unspecified );
	}

	[Event("scs_evnt_setteam")]
	public void SetTeam(TeamEnum newTeam)
	{
		CurTeam = newTeam;

		if(CurTeam == TeamEnum.Unspecified)
			SetModel( "models/player/hevsuit_white.vmdl" );

		else if (CurTeam == TeamEnum.Red)
			SetModel( "models/player/hevsuit_red.vmdl" );

		else if (CurTeam == TeamEnum.Blue)
			SetModel( "models/player/hevsuit_blue.vmdl" );

		else if (CurTeam == TeamEnum.Green)
			RenderColor = Color.Green;

		else if (CurTeam == TeamEnum.Yellow)
			RenderColor = Color.Yellow;
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( info.Attacker is not SCSPlayer )
			return;

		base.TakeDamage( info );
	}
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		SimulateActiveChild( cl, ActiveChild );

		TickPlayerUse();
		SimulateGrabbing();
	}
}
