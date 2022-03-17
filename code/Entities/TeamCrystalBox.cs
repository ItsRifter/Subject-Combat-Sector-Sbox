using System;
using System.Linq;
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

	[Net] protected int CrystalTierLevel { get; private set; } = 0;

	[Net] public int CrystalStrength { get; private set; }

	public string[] Tier0NPCs = new string[3] { "Zombie", "SecurityZombie", "SoldierZombie" };
	public string[] Tier0NPCDesc = new string[3] { "Basic infected", "An armored security zombie", "A soldier zombie\nhe served his country well" };
	
	public string[] Tier1NPCs = new string[4] { "Security", "Rebel", "Military", "Scout" };
	public string[] Tier1NPCDesc = new string[4] { "A security officer", "A rule breaking rebel", "Military Soldier", "A Scoutsman" };

	public string[] NPCRarityTypes = new string[6] { "Common", "Rare", "Legendary", "Godlike", "Awesome", "Epic" };

	private int pastCrystalTiers;
	private int currentTiers = 0;

	[Net] public string NPCToSpawn { get; private set; }
	[Net] public string NPCDescription { get; private set; }
	[Net] public string NPCRarity { get; private set; }

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/game/crystal_box.vmdl" );

		NPCToSpawn = Tier0NPCs[0];
		NPCDescription = Tier0NPCDesc[0];
		NPCRarity = NPCRarityTypes[0];

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		CrystalStrength = 1;

		pastCrystalTiers = 0;
	}

	public void Upgrade()
	{
		CrystalTierLevel++;
		CrystalStrength = 1;
		NPCRarity = NPCRarityTypes[0];

		pastCrystalTiers++;

		PlaySound( "tierup_" + CrystalTierLevel );

		if(CrystalTierLevel == 1)
		{
			NPCToSpawn = Tier1NPCs[0];
			NPCDescription = Tier1NPCDesc[0];
		}

		if ( pastCrystalTiers >= 4)
		{
			PlaySound( "newtech" );
			pastCrystalTiers = 0;
			currentTiers++;
		}
	}

	public bool CanUpgradeNextTier()
	{
		if ( currentTiers == CrystalTierLevel )
			return true;

		return false;
	}

	public int GetTierLevel()
	{
		return CrystalTierLevel;
	}

	public int GetStrengthLevel()
	{
		return CrystalStrength;
	}

	public void RandomizeStats()
	{
		CrystalStrength = Rand.Int( 1, 3 );

		int chanceRarity = Rand.Int( 1, 100 );

		if ( chanceRarity <= 50 )
			NPCRarity = NPCRarityTypes[0];
		else if ( chanceRarity > 50 && chanceRarity <= 65)
			NPCRarity = NPCRarityTypes[1];
		else if ( chanceRarity > 65 && chanceRarity <= 85 )
			NPCRarity = NPCRarityTypes[2];
		else if ( chanceRarity > 85 && chanceRarity <= 97 )
			NPCRarity = NPCRarityTypes[3];
		else if ( chanceRarity > 97 && chanceRarity <= 99 )
			NPCRarity = NPCRarityTypes[4];
		else if ( chanceRarity > 99 && chanceRarity <= 100 )
			NPCRarity = NPCRarityTypes[5];

		if ( CrystalTierLevel == 0 )
		{
			int random = Rand.Int( 1, Tier0NPCs.Length - 1 );

			NPCToSpawn = Tier0NPCs[random];
			NPCDescription = Tier0NPCDesc[random];
		} else if (CrystalTierLevel == 1)
		{
			int random = Rand.Int( 1, Tier1NPCs.Length - 1 );

			NPCToSpawn = Tier1NPCs[random];
			NPCDescription = Tier1NPCDesc[random];
		}
	}


}
