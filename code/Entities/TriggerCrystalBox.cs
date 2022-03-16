using System;
using Sandbox;

[Library( "scs_trigger_crystalbox", Description = "A trigger multiple that does specific actions" )]
[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
[Hammer.Model]
[Hammer.SupportsSolid]
public partial class TriggerCrystalBox : BaseTrigger
{
	public enum TriggerType
	{
		Unknown,
		Storage,
		Randomizer,
		Upgrader
	}

	[Property( "Type" ), Description("What type of special trigger is this")]
	public TriggerType TypeOfTrigger { get; set; } = TriggerType.Unknown;

	protected Output OnFail { get; set; }
	protected Output OnSuccess { get; set; }

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

	private int CurTierIndex = 0;
	private int previousTiersLeveled = 0;
	public override void Spawn()
	{
		base.Spawn();

		if ( TeamTriggerAssigned == TeamTriggerBox.Unknown )
			return;		
	}

	public override void OnTouchStart( Entity toucher )
	{
		if(TeamPointTracker == null)
		{
			var ents = All;

			foreach ( var entity in ents )
			{
				if ( entity is TeamPoints teamPoint && teamPoint.TeamPointAssigned.ToString().Contains( TeamTriggerAssigned.ToString()) )
				{
					TeamPointTracker = teamPoint;
				}
			}
		}

		if( toucher is TeamCrystalBox crystalBox )
		{
			if( TypeOfTrigger == TriggerType.Randomizer)
			{
				if ( !TeamPointTracker.AttemptRandomize() )
				{
					OnFail.Fire( this );
					return;
				}

				crystalBox.RandomizeStats();
				TeamPointTracker.SubtractPoints( 1 );
				OnSuccess.Fire( this );
			}

			if(TypeOfTrigger == TriggerType.Upgrader)
			{
				if ( !TeamPointTracker.AttemptUpgrader( CurTierIndex ) )
				{
					OnFail.Fire( this );
					return;
				}

				TeamPointTracker.SubtractPoints( TeamPointTracker.GetUpgradeTierCost( CurTierIndex ) );
				crystalBox.Upgrade();
				CurTierIndex++;

				OnSuccess.Fire( this );
			}

			base.OnTouchStart( crystalBox );
		}
	}
}
