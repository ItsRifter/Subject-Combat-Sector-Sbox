using Sandbox;
using System.Collections.Generic;

[Library( "info_scs_npc_spawner" )]
[Hammer.EditorModel( "models/citizen/citizen.vmdl" )]
[Hammer.EntityTool( "NPC Spawnpoint", "Subject Combat Sector", "Defines a point where NPCs can spawn" )]
public class NPCSpawner : Entity
{
	private TimeSince timeLastSpawn;

	public enum TeamSideEnum
	{
		Unknown,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Property("TeamEnum"), Description("Which side is this NPC spawner on")]
	public TeamSideEnum TeamSide { get; set; } = TeamSideEnum.Unknown;

	public double spawnCooldown;

	public int spawnCount;

	public List<NPCBase> aliveNPCs;

	public override void Spawn()
	{
		base.Spawn();
	}


	[Event("scs_npckilled")]
	public void OnNPCKilled(NPCBase killedNPC)
	{
		aliveNPCs.Remove( killedNPC );
	}
}
