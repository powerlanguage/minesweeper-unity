using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public Tile tilePrefab;
	public int numberOfTiles = 10;
	public float distanceBetweenTiles = 1f;
	public int tilesPerRow = 4;
	public int numberOfMines = 5;

	public static Tile[] tilesAll;
	public static List<Tile> tilesMined;
	public static List<Tile> tilesUnmined;

	// Use this for initialization
	void Start () {
		CreateTiles ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreateTiles(){

		tilesAll = new Tile[numberOfTiles];

		float xOffset = 0f;
		float zOffset = 0f;

		for (int tilesCreated = 0; tilesCreated < numberOfTiles; tilesCreated++) {
			xOffset += distanceBetweenTiles;

			if(tilesCreated % tilesPerRow == 0){
				zOffset += distanceBetweenTiles;
				xOffset = 0;
			}

			Tile newTile = (Tile)Instantiate(tilePrefab, new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset), transform.rotation);
			newTile.ID = tilesCreated;
			newTile.tilesPerRow = tilesPerRow;
			tilesAll[tilesCreated] = newTile;
		}

		AssignMines ();
	}

	void AssignMines(){
		tilesUnmined = new List<Tile>(tilesAll);
		tilesMined = new List<Tile>();

		for (int minesAssigned = 0; minesAssigned < numberOfMines; minesAssigned++) {
		
			Tile currentTile = (Tile)tilesUnmined[Random.Range(0, tilesUnmined.Count)];

			currentTile.GetComponent<Tile>().isMined = true;

			//Add to Tiles mined
			tilesMined.Add(currentTile);
			//Remove from unmined
			tilesUnmined.Remove(currentTile);
		}

	}
}
