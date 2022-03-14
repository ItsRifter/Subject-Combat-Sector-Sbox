using System;
using Sandbox;

[Library( "scs_crystalbox" ), Description("The teams crystal box, DO NOT CHANGE THE MODEL")]
[Hammer.EditorModel( "models/game/crystal_box.vmdl" )]
public partial class TeamCrystalBox : ModelEntity
{
	public enum TeamCrystalBoxType
	{
		Unknown,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Property( "AssignCrystalBoxTeam" ), Description( "Assigns the box to the selected team" )]
	public TeamCrystalBoxType TeamBoxAssignment { get; set; } = TeamCrystalBoxType.Unknown;

	public int CrystalTierLevel = 0;

	public int CrystalStrength = 1;

	public NPCBase[] Tier0NPCs = new NPCBase[1] { Library.Create<NPCBase>( "Zombie" ) };

	public NPCBase NPCToSpawn;

	public Output OnUpgrade;

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/game/crystal_box.vmdl" );
		NPCToSpawn = Library.Create<NPCBase>( "Zombie" );
		NPCToSpawn.Delete();

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public void Upgrade()
	{
		CrystalTierLevel++;
		CrystalStrength = 1;

		OnUpgrade.Fire(this);
	}

	[Input]
	public void RandomizeStats()
	{
		CrystalStrength = Rand.Int( 1, 3 );

		if(CrystalTierLevel == 0)
			NPCToSpawn = Tier0NPCs[Rand.Int( 0, Tier0NPCs.Length )];
	}


}
