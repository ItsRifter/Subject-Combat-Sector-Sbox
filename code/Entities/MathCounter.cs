using System;
using Sandbox;
[Library( "scs_math_counter" )]
[Hammer.EntityTool( "Math Counter", "Subject Combat Sector", "A Math counter tracking values and outputting results" )]
public partial class MathCounter : AnimEntity
{
	[Property( "StartValue" ), Description( "The initial value setting" )]
	public int StartingValue { get; set; } = 0;

	[Property( "MinValue" ), Description( "The minimum value" )]
	public int MinValue { get; set; } = 0;

	[Property( "MaxValue" ), Description( "The maximum value" )]
	public int MaxValue { get; set; } = 1;

	public int Value;

	protected Output OnValueChange { get; set; }
	protected Output OnHitMin { get; set; }
	protected Output OnHitMax { get; set; }
	protected Output OnLostMin { get; set; }
	protected Output OnLostMax { get; set; }

	public override void Spawn()
	{
		base.Spawn();
		Value = StartingValue;
	}

	[Input]
	public void Add(int addedValue)
	{
		if ( (Value + addedValue) > MinValue )
			OnLostMin.Fire( this );

		if ( Value >= MaxValue )
			return;

		Value += addedValue;
		OnValueChange.Fire( this );


		if ( Value == MaxValue )
		{
			Value = MaxValue;
			OnHitMax.Fire( this );
		}
	}

	[Input]
	public void SetMin( int newMin )
	{
		MinValue = newMin; 

		if ( Value == MinValue )
		{
			Value = MinValue;
			OnHitMin.Fire( this );
		}
	}

	[Input]
	public void SetMax(int newMax)
	{
		MaxValue = newMax;

		if ( Value == MaxValue )
		{
			Value = MaxValue;
			OnHitMax.Fire( this );
		}
	}

	[Input]
	public void SetValue( int newValue )
	{
		Value = newValue;

		if ( Value >= MaxValue )
		{
			OnHitMax.Fire( this );
		}

		if ( Value == MinValue )
		{
			OnHitMin.Fire( this );
		}
	}

	[Input]
	public void Subtract( int takenValue )
	{
		if ( (Value - takenValue) < MaxValue )
			OnLostMax.Fire( this );

		if ( Value == MinValue )
			return;

		Value -= takenValue;
		OnValueChange.Fire( this );

		if(Value == MinValue)
		{
			Value = MinValue;
			OnHitMin.Fire( this );
		}
	}

	
}
