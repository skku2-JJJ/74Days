using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SortByY : MonoBehaviour {

	int rangePerUnit=100;

    public int targetOffset = 0;
	
	SpriteRenderer sr;
	void Update(){
		sr=GetComponent<SpriteRenderer>();
		sr.sortingOrder=-(int)(rangePerUnit*transform.position.y)+targetOffset;
	}
}
