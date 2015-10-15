using UnityEngine;
using System.Collections;
using System;

public class BinaryHeap<T> where T : IBinaryHeapItem<T> {
	T[] items;
	int count;

	public BinaryHeap(int maxSize){
		items = new T[maxSize];
	}

	public void Add(T item){
		item.HeapIndex = count;
		items[count] = item;
		SortUp(item);
		count++;
	}

	public T RemoveFirst() {
		T item = items[0];
		count--;
		items[0] = items[count];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return item;
	}

	public void UpdateItem(T item) {
		SortUp(item);
		SortDown(item);
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	private void SortUp(T item) {
		int parentI = (item.HeapIndex-1)/2;

		while(true) {
			T parent = items[parentI];

			if(item.CompareTo(parent) > 0)
				Swap(item, parent);
			else
				break;
		}

		parentI = (item.HeapIndex-1)/2;
	}

	private void SortDown(T item) {
		while(true) {
			int iL = item.HeapIndex * 2 + 1;
			int iR = item.HeapIndex * 2 + 2;

			int iS = 0;

			if(iL < count){
				iS = iL;

				if(iR < count)
					if(items[iL].CompareTo(items[iR])<0)
						iS = iR;

				if(item.CompareTo(items[iS]) < 0)
					Swap(item, items[iS]);
				else
					return;
			}
			else
				return;
		}
	}

	private void Swap(T a, T b){
		items[a.HeapIndex] = b;
		items[b.HeapIndex] = a;

		int i = a.HeapIndex;

		a.HeapIndex = b.HeapIndex;
		b.HeapIndex = i;
	}

	public int Count {
		get {return count;}
	}
}

public interface IBinaryHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}