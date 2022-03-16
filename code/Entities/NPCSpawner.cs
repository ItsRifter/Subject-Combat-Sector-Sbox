using Sandbox;
using System.Collections.Generic;

[Library( "info_scs_npc_spawner" )]
[Hammer.EditorModel( "models/citizen/citizen.vmdl" )]
[Hammer.EntityTool( "NPC Spawnpoint", "Subject Combat Sector", "Defines a point where NPCs can spawn" )]
public partial class NPCSpawner : Entity
{
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

	public List<NPCBase> aliveNPCs { get; private set; }

	protected Output NPCSpawned { get; set; }
	protected Output NPCKilled { get; set; }
	protected Output AllNPCsDead { get; set; }

	private TimeSince timeUntilSpawn;
	private bool shouldSpawn = true;

	public override void Spawn()
	{
		base.Spawn();
		aliveNPCs = new List<NPCBase>();
	}

	[Input]
	public async void SpawnNPC()
	{
		var entities = All;
		List<TeamCrystalBox> boxes = new List<TeamCrystalBox>();
		
		foreach ( var ent in entities )
		{
			if ( ent is TeamCrystalBox box )
				boxes.Add( box );
		}

		for ( int i = 0; i < 4 * SCSGame.Current.TotalTeams; i++ )
		{
			if ( boxes[i].TeamBoxAssignment.ToString().Contains( TeamSide.ToString() ) )
			{
				for ( int s = 1; s <= boxes[i].CrystalStrength; s++ )
				{
					while ( shouldSpawn == false )
					{
						if ( timeUntilSpawn > 1.25f )
							shouldSpawn = true;

						await Task.Delay( 60 );
					}

					var npc = Library.Create<NPCBase>( boxes[i].NPCToSpawn );

					npc.SetStatsWithRarity( boxes[i].NPCRarity);

					npc.Position = Position;
					npc.Rotation = Rotation;

					aliveNPCs.Add( npc );

					switch ( TeamSide )
					{
						case TeamSideEnum.Red:
							npc.TeamNPC = NPCBase.TeamAssignEnum.Red;
							npc.RenderColor = Color.Red;
							break;
						case TeamSideEnum.Blue:
							npc.TeamNPC = NPCBase.TeamAssignEnum.Blue;
							npc.RenderColor = Color.Blue;
							break;
						case TeamSideEnum.Green:
							npc.TeamNPC = NPCBase.TeamAssignEnum.Green;
							npc.RenderColor = Color.Green;
							break;
						case TeamSideEnum.Yellow:
							npc.TeamNPC = NPCBase.TeamAssignEnum.Yellow;
							npc.RenderColor = Color.Yellow;
							break;
					}

					shouldSpawn = false;
					timeUntilSpawn = 0;

					_ = NPCSpawned.Fire( this );
				}
			}
		}
	}

	[Event("scs_npckilled")]
	public void OnNPCKilled(NPCBase killedNPC)
	{
		if ( SCSGame.Current.RoundStatus != SCSGame.RoundEnum.Active )
			return;

		aliveNPCs.Remove( killedNPC );

		if(aliveNPCs.Count <= 0)
		{
			Sound.FromScreen( TeamSide.ToString() + "_defeated");
			AllNPCsDead.Fire( this );
		}

		NPCKilled.Fire( this );
	}

	[Event( "scs_clearnpcs" )]
	public void ClearNPCs()
	{
		foreach ( var alive in aliveNPCs )
			alive.Delete();

		aliveNPCs.Clear();
	}
}
