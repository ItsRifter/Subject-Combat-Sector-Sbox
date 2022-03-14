using System;
using Sandbox;

[Library("scs_teampoints"), Description("The teams point tracker used for randomizing or upgrading")]
public partial class TeamPoints : Entity
{
	public enum TeamPointsTypeEnum
	{
		Unknown,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Property( "TeamPointEnum" ), Description( "Which side will the points belong to" )]
	public TeamPointsTypeEnum TeamPointAssigned { get; set; } = TeamPointsTypeEnum.Unknown;

	public int Points { get; set; } = 0;

	[Input]
	public void SetPoints(int setPoints)
	{
		Points = setPoints;
	}

	public int GetTotalPoints()
	{
		return Points;
	}
	
	public bool AttemptRandomize()
	{
		if ( Points < 4 )
			return false;

		return true;

	}

	[Input]
	public void AddPoints( int addPoints )
	{
		Points += addPoints;
	}

	[Input]
	public void SubtractPoints(int takePoints)
	{
		Points -= takePoints;
	}

}

