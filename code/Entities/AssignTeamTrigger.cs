using System;
using Sandbox;

[Library( "scs_assignteam", Description = "Behaves like a teleporter but assigns the player to a team" )]
[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
[Hammer.Model]
[Hammer.SupportsSolid]
public partial class AssignTeamTrigger : TriggerTeleport
{
	public enum TeamType
	{
		Unspecified,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Property("Assignment"), Description("Assign the player to which team")]
	public TeamType TeamAssign { get; set; } = TeamType.Unspecified;

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !Enabled )
			return;

		if ( other is SCSPlayer player )
		{
			switch ( TeamAssign )
			{
				case TeamType.Unspecified:
					player.CurTeam = SCSPlayer.TeamEnum.Unspecified;
					player.SetModel( "models/player/hevsuit_white.vmdl" );
					break;
				case TeamType.Red:
					player.CurTeam = SCSPlayer.TeamEnum.Red;
					player.SetModel( "models/player/hevsuit_red.vmdl" );
					break;
				case TeamType.Blue:
					player.CurTeam = SCSPlayer.TeamEnum.Blue;
					player.SetModel( "models/player/hevsuit_blue.vmdl" );
					break;
				case TeamType.Green:
					player.CurTeam = SCSPlayer.TeamEnum.Green;
					player.SetModel( "models/player/hevsuit_green.vmdl" );
					break;
				case TeamType.Yellow:
					player.CurTeam = SCSPlayer.TeamEnum.Yellow;
					player.SetModel( "models/player/hevsuit_yellow.vmdl" );
					break;
			}
		}
	}
}

