using Sandbox;
using System;
using System.Linq;

public partial class Zombie : NPCBase
{
	public override int BaseHealth => 60;
	public override float BaseSpeed => 25;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 1.0f;
	public override float AlertRange => 100;
	public override float AttackRange => 50;
	public override int AttackDamage => 15;
	public override int AttackCooldown => 3;
	public override void Spawn()
	{
		base.Spawn();		
	}
}
