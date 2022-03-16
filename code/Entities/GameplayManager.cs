using System;
using Sandbox;

[Library("scs_gameplay_manager")]
public partial class GameplayManager : Entity
{
	protected Output OnActiveStart { get; set; }
	protected Output OnPost { get; set; }

	int elimTeamOrder = 0;

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

		elimTeamOrder = 0;

		Event.Run( "scs_clearnpcs" );
		OnPost.Fire( this );
	}

	[Input]
	public void GivePointsBonus( string team, int bonusPoints )
	{
		var teampoints = FindByName( team + "_point_counter" ) as TeamPoints;

		if ( teampoints == null )
			return;

		teampoints.AddPoints( bonusPoints );
	}

	[Input]
	public void GivePoints(string team)
	{
		//elimTeamOrder
		//0 = eliminated first
		//1 = elimintaed second or won the round
		//2 = eliminated third or won the round
		//3 = Won the round

		var teampoints = FindByName( team + "_point_counter" ) as TeamPoints;

		if ( teampoints == null )
			return;

		if ( elimTeamOrder == 0 )
			teampoints.AddPoints( 2 );
		else if ( elimTeamOrder == 1 )
			teampoints.AddPoints( 4 );
		else if ( elimTeamOrder == 2 )
			teampoints.AddPoints( 6 );
		else if ( elimTeamOrder == 3 )
			teampoints.AddPoints( 8 );

		elimTeamOrder++;
	}
}
