using System;
using Sandbox;

[Library( "scs_trigger_crystalbox_enhance", Description = "A trigger multiple that does specific actions" )]
[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
[Hammer.Model]
[Hammer.SupportsSolid]
public partial class TriggerCrystalBox : TriggerMultiple
{
	[Property( "Upgrader" ), Description("Should this be an upgrade trigger")]
	public bool IsUpgrader { get; set; } = false;
	public enum TeamTriggerBox
	{
		Unknown,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Property( "TeamPointEnum" ), Description( "Which side does this trigger belong to" )]
	public TeamTriggerBox TeamTriggerAssigned { get; set; } = TeamTriggerBox.Unknown;

	public TeamPoints TeamPointTracker;

	public override void Spawn()
	{
		base.Spawn();

		if ( TeamTriggerAssigned == TeamTriggerBox.Unknown )
			return;

		var ents = FindAllByName( "TeamPoints" );

		if ( TeamTriggerAssigned == TeamTriggerBox.Red )
		{
			foreach ( var entity in ents )
			{
				if( entity is TeamPoints teamPoint && teamPoint.TeamPointAssigned.ToString() == TeamTriggerAssigned.ToString())
				{
					TeamPointTracker = teamPoint;
				}
			}
		}
	}

	public override void OnTouchStart( Entity toucher )
	{
		base.OnTouchStart( toucher );

		if( toucher is TeamCrystalBox crystalBox )
		{
			if ( !TeamPointTracker.AttemptRandomize() )
			{
				PlaySound( "insufficent_points" );
				return;
			}

			crystalBox.RandomizeStats();
			TeamPointTracker.SubtractPoints( 4 );
		}
	}

}
