using Sandbox;
using System;
using System.Linq;

namespace SCS.Entities.NPC;

public partial class SecurityZombie : NPCBase
{
	public override int BaseHealth => 80;
	public override float BaseSpeed => 20;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 1.0f;
	public override float AlertRange => 100;
	public override float AttackRange => 50;
	public override int AttackDamage => 15;
	public override float AttackCooldown => 3;
	public override void Spawn()
	{
		base.Spawn();

		var vest = new ModelEntity();
		vest.SetModel( "models/citizen_clothes/vest/Tactical_Vest/Models/tactical_vest.vmdl_c" );
		vest.SetParent( this, true );
	}
}
