using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour{

	public List<Tile> GetPath(Tile start, Tile goal) {
		List<Tile> path = new List<Tile>();

		FindPath(start, goal);
		path = RetracePath(start, goal);

		return path;
	}

	private void FindPath(Tile start, Tile goal) {
		if (start.IsPassable && goal.IsPassable) {
			HashSet<Tile> closed = new HashSet<Tile>();
			List<Tile> open = new List<Tile>();

			open.Add(start);

			while(open.Count > 0) {
				Tile current = open[0];
				open.RemoveAt(0);
				closed.Add(current);

				if (current == goal)
					break;

				foreach (Tile tile in Logic.Inst.Grid.Neighbours(current)){
					if (tile) {
						if (!tile.IsPassable || tile.OccupyngUnit || closed.Contains(tile))
							continue;

						int cost = current.GCost + Logic.Inst.Grid.Distance(current, tile) + tile.MoveCost;

						if (cost < tile.GCost || !open.Contains(tile)) {
							tile.GCost = cost;
							tile.HCost = Logic.Inst.Grid.Distance(current, tile);
							tile.Parent = current;

							if (!open.Contains(tile))
								open.Add(tile);							
						}
					}
				}
			}
		}
	}

	private List<Tile> RetracePath(Tile start, Tile goal) {
		List<Tile> tiles = new List<Tile>();

		Tile current = goal;

		while (current != start) {
			tiles.Add(current);
			current = current.Parent;
		}

		tiles.Add(start);

		return tiles;
	}
}
