using System;
using Sandbox;

[Library("scs_gameplay_manager")]
public partial class GameplayManager : Entity
{
	protected Output OnActiveStart { get; set; }
	protected Output OnPost { get; set; }

	[Input]
	public void SetStatusIdle()
	{
		SCSGame.Current.RoundStatus = SCSGame.RoundEnum.Waiting;
	}

	[Input]
	public void SetStatusStarting()
	{
		SCSGame.Current.RoundStatus = SCSGame.RoundEnum.Starting;
	}

	[Input]
	public void SetStatusActive()
	{
		SCSGame.Current.RoundStatus = SCSGame.RoundEnum.Active;
		OnActiveStart.Fire( this );
	}

	[Input]
	public void SetStatusActiveBonus()
	{
		SCSGame.Current.RoundStatus = SCSGame.RoundEnum.Active_Bonus;
	}

	[Input]
	public void SetStatusPost()
	{
		SCSGame.Current.RoundStatus = SCSGame.RoundEnum.Post;
		SCSGame.Current.DeclareRoundWinner();

		Event.Run( "scs_clearnpcs" );
		OnPost.Fire( this );
	}
}
