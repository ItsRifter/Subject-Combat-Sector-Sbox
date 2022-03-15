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
		if ( StartDisabled )
			return;

		if ( other is SCSPlayer player )
		{
			switch ( TeamAssign )
			{
				case TeamType.Unspecified:
					player.CurTeam = SCSPlayer.TeamEnum.Unspecified;
					SetModel( "models/player/hevsuit_white.vmdl" );
					break;
				case TeamType.Red:
					player.CurTeam = SCSPlayer.TeamEnum.Red;
					SetModel( "models/player/hevsuit_red.vmdl" );
					break;
				case TeamType.Blue:
					player.CurTeam = SCSPlayer.TeamEnum.Blue;
					SetModel( "models/player/hevsuit_blue.vmdl" );
					break;
				case TeamType.Green:
					player.CurTeam = SCSPlayer.TeamEnum.Green;
					break;
				case TeamType.Yellow:
					player.CurTeam = SCSPlayer.TeamEnum.Yellow;
					break;
			}
		}

		base.StartTouch( other );
	}
}

