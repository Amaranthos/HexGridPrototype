using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour{

	public List<Tile> GetPath(Tile start, Tile goal) {
		List<Tile> path = new List<Tile>();

		Logic.Inst.Grid.ClearHerustics();

		if(FindPath(start, goal))
			path = RetracePath(start, goal);

		return path;
	}

	private bool FindPath(Tile start, Tile goal) {
		bool successful = false;
		if (start.IsPassable && goal.IsPassable) {
			HashSet<Tile> closed = new HashSet<Tile>();
			BinaryHeap<Tile> open = new BinaryHeap<Tile>(Logic.Inst.Grid.TilesList.Count);

			open.Add(start);

			while(open.Count > 0) {
				Tile current = open.RemoveFirst();
				closed.Add(current);

				if (current == goal){
					successful = true;
					break;
				}

				foreach (Tile tile in Logic.Inst.Grid.Neighbours(current)){
					if (tile) {
						if (!tile.IsPassable || tile.OccupyingUnit || closed.Contains(tile))
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
		return successful;
	}

	private List<Tile> RetracePath(Tile start, Tile goal) {
		List<Tile> tiles = new List<Tile>();

		Tile current = goal;

		while (current != start) {
			tiles.Add(current);
			current = current.Parent;
		}

		// tiles.Add(start);

		return tiles;
	}

	public int PathCost(List<Tile> path){
		int cost = 0;

		foreach(Tile tile in path)
			cost += tile.MoveCost;

		return cost;
	}
}
