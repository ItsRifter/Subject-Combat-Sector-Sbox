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

	[Net] public int CrystalStrength { get; private set; }

	public string[] Tier0NPCs = new string[3] { "Zombie", "SecurityZombie", "SoldierZombie" };
	public string[] Tier0NPCDesc = new string[3] { "Basic infected", "An armored security zombie", "A soldier zombie\nhe served his country well" };

	public string[] NPCRarityTypes = new string[5] { "Common", "Rare", "Legendary", "Godlike", "Awesome" };

	[Net] public string NPCToSpawn { get; private set; }
	[Net] public string NPCDescription { get; private set; }
	[Net] public string NPCRarity { get; private set; }

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/game/crystal_box.vmdl" );

		NPCToSpawn = Tier0NPCs[0];
		NPCDescription = Tier0NPCDesc[0];

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		CrystalStrength = 1;
		NPCRarity = NPCRarityTypes[0];
	}

	public void Upgrade()
	{
		CrystalTierLevel++;
		CrystalStrength = 1;
		NPCRarity = NPCRarityTypes[0];
	}

	public void RandomizeStats()
	{
		CrystalStrength = Rand.Int( 1, 3 );

		int chanceRarity = Rand.Int( 1, 100 );

		if ( chanceRarity <= 40 )
			NPCRarity = NPCRarityTypes[0];
		else if ( chanceRarity > 40 && chanceRarity <= 65)
			NPCRarity = NPCRarityTypes[1];
		else if ( chanceRarity > 65 && chanceRarity <= 80 )
			NPCRarity = NPCRarityTypes[2];
		else if ( chanceRarity > 80 && chanceRarity <= 95 )
			NPCRarity = NPCRarityTypes[3];
		else if ( chanceRarity > 95 && chanceRarity <= 99 )
			NPCRarity = NPCRarityTypes[4];
		else if ( chanceRarity > 99 && chanceRarity <= 100 )
			NPCRarity = NPCRarityTypes[5];

		if ( CrystalTierLevel == 0 )
		{
			int random = Rand.Int( 1, Tier0NPCs.Length - 1 );

			NPCToSpawn = Tier0NPCs[random];
			NPCDescription = Tier0NPCDesc[random];
		}
	}


}
