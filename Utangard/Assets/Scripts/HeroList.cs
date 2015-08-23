using UnityEngine;
using System.Collections.Generic;

public class HeroList : MonoBehaviour {
	public List<Hero> heroes;

	public GameObject GetHero(HeroType hero) {
		return (heroes.Find(item => item.type == hero)).gameObject;
	}
}
