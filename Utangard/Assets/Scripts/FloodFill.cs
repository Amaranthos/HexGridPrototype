using UnityEngine;
using System.Collections.Generic;

public class FloodFill {

	public List<Tile> Flood(Tile start, int range){
		Logic.Inst.Grid.ClearHerustics();
		List<Tile> ret = new List<Tile>();
		Queue<Tile> fringe = new Queue<Tile>();

		start.PathCost = 0;
		fringe.Enqueue(start);

		while(fringe.Count > 0){
			Tile current = fringe.Dequeue();

			List<Tile> neighbours = Logic.Inst.Grid.Neighbours(current);
			for (int i = 0; i < neighbours.Count; i++){

				Tile neighbour = neighbours[i];

				int cost = neighbour.MoveCost + current.PathCost;
				if(neighbour.PathCost == int.MaxValue){
					if(neighbour.IsPassable){
						if(cost <= range){
							neighbour.PathCost = cost;
							ret.Add(neighbour);
							fringe.Enqueue(neighbour);
						}
					}
				}
			}
		}
		return ret;
	}	
}
