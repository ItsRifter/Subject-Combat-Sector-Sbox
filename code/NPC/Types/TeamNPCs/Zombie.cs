using Sandbox;
using System;
using System.Linq;

public partial class Zombie : NPCBase
{
	public override int BaseHealth => 60;
	public override float BaseSpeed => 10;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 1.0f;

	public override void Spawn()
	{
		base.Spawn();		

		RenderColor = Color.Green;
	}
}
