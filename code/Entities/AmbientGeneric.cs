using System;
using Sandbox;

[Library("scs_ambient_generic")]
public partial class AmbientGeneric : SoundEventEntity
{

	[Property( "PlayEverywhere" )]
	public bool PlayEverywhere { get; set; } = false;

	[Property( "Volume" )]
	public double Volume { get; set; } = 1.0;

	public override void Spawn()
	{
		base.Spawn();
		if ( PlayEverywhere )
			StartSoundEverywhere();
	}

	[Input]
	protected void StartSoundEverywhere()
	{
		PlayingSound = Sound.FromScreen( SoundName );
		PlayingSound.SetVolume( (float)Volume );
	}
}
