using UnityEngine;

namespace Puzzle.Room
{
	public interface IWallNode
	{
		GameObject GameObject { get; }
		void AutomaticWalls();
	}
}
