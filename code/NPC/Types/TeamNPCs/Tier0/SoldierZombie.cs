using Sandbox;
using System;
using System.Linq;

public partial class SoldierZombie : NPCBase
{
	public override int BaseHealth => 110;
	public override float BaseSpeed => 20;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 1.0f;
	public override float AlertRange => 100;
	public override float AttackRange => 50;
	public override int AttackDamage => 25;
	public override int AttackCooldown => 3;
	public override void Spawn()
	{
		base.Spawn();

		var beard = new ModelEntity();
		beard.SetModel( "models/citizen_clothes/beards/beard_trucker_brown.vmdl_c" );
		beard.SetParent( this, true );

		var vest = new ModelEntity();
		vest.SetModel( "models/citizen_clothes/vest/Tactical_Vest/Models/tactical_vest.vmdl_c" );
		vest.SetParent( this, true );
	}
}
