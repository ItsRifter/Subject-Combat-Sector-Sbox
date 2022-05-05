using Sandbox;
using System;
using System.Linq;

public partial class Rebel : NPCBase
{
	public override int BaseHealth => 225;
	public override float BaseSpeed => 40;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 1.0f;
	public override float AlertRange => 130;
	public override float AttackRange => 80;
	public override int AttackDamage => 25;
	public override float AttackCooldown => 2.25f;
	public override void Spawn()
	{
		base.Spawn();
	}
}
