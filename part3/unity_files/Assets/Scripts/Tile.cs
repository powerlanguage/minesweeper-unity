using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Tile : MonoBehaviour {

	public bool isMined = false;
	public Material materialIdle;
	public Material materialLightup;
	public Material materialUncovered;
	public Material materialDetonated;
	public int ID;
	public int tilesPerRow;

	public Tile tileUpper;
	public Tile tileLower;
	public Tile tileLeft;
	public Tile tileRight;

	public Tile tileUpperLeft;
	public Tile tileUpperRight;
	public Tile tileLowerLeft;
	public Tile tileLowerRight;
		
	public List<Tile> adjacentTiles = new List<Tile>();
	public int adjacentMines = 0;

	public string state = "idle";

	public TextMesh displayText;
	public GameObject displayFlag;

	// Use this for initialization
	void Start () {
		//name tiles in heirachy to help with debugging
		gameObject.name = "Tile " + ID.ToString();

		if(inBounds(Grid.tilesAll, ID + tilesPerRow)) 						{ tileUpper = Grid.tilesAll[ID + tilesPerRow]; }
		if(inBounds(Grid.tilesAll, ID - tilesPerRow)) 						{ tileLower = Grid.tilesAll[ID - tilesPerRow]; }
		if(inBounds(Grid.tilesAll, ID + 1) && (ID+1) % tilesPerRow != 0)	{ tileRight = Grid.tilesAll[ID + 1]; }
		if(inBounds(Grid.tilesAll, ID - 1) && ID % tilesPerRow != 0) 		{ tileLeft = Grid.tilesAll[ID - 1]; }

		if(inBounds(Grid.tilesAll, ID + tilesPerRow + 1) && (ID + tilesPerRow + 1) % tilesPerRow != 0) { tileUpperRight = Grid.tilesAll[ID + tilesPerRow + 1]; }
		if(inBounds(Grid.tilesAll, ID + tilesPerRow - 1) &&     ID % tilesPerRow != 0) { tileUpperLeft  = Grid.tilesAll[ID + tilesPerRow - 1]; }
		if(inBounds(Grid.tilesAll, ID - tilesPerRow + 1) && (ID+1) % tilesPerRow != 0) { tileLowerRight = Grid.tilesAll[ID - tilesPerRow + 1]; }
		if(inBounds(Grid.tilesAll, ID - tilesPerRow - 1) &&     ID % tilesPerRow != 0) { tileLowerLeft  = Grid.tilesAll[ID - tilesPerRow - 1]; }

		if(tileUpper)	{ adjacentTiles.Add(tileUpper); }
		if(tileLower)	{ adjacentTiles.Add(tileLower); }
		if(tileLeft)	{ adjacentTiles.Add(tileLeft); }
		if(tileRight)	{ adjacentTiles.Add(tileRight); }

		if(tileUpperLeft)	{ adjacentTiles.Add(tileUpperLeft); }
		if(tileUpperRight)	{ adjacentTiles.Add(tileUpperRight); }
		if(tileLowerLeft)	{ adjacentTiles.Add(tileLowerLeft); }
		if(tileLowerRight)	{ adjacentTiles.Add(tileLowerRight); }

		countMines ();

		displayText.renderer.enabled = false;
		displayFlag.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private bool inBounds(Tile[] inputArray, int targetID){
		if (targetID < 0 || targetID >= inputArray.Length) {
			return false;
		} else {
			return true;
		}
	}

	void countMines(){

		foreach (Tile currentTile in adjacentTiles) {
			if(currentTile.isMined) {
				adjacentMines += 1;
			}
		}

		displayText.text = adjacentMines.ToString();

		if (adjacentMines <= 0) {
			displayText.text = "";
		}
	}

	public void setFlag(){
		if (state == "idle") {
			state = "flagged";
			displayFlag.renderer.enabled = true;

			Grid.minesRemaining -= 1;
			if(isMined){ Grid.minesMarkedCorrectly += 1; }

		} else if (state == "flagged") {
			state = "idle";
			displayFlag.renderer.enabled = false;

			Grid.minesRemaining += 1;
			if(isMined){ Grid.minesMarkedCorrectly -= 1; }
		}
	}

	void uncoverTile(){
		if (!isMined) {
			state = "uncovered";
			displayText.renderer.enabled = true;
			renderer.material = materialUncovered;

			Grid.tilesUncovered += 1;

			if(adjacentMines == 0){
				uncoverAdjacentTiles();
			}

		} else {
			explode();
		}
	}

	public void uncoverTileExternal(){
		state = "uncovered";
		displayText.renderer.enabled = true;
		renderer.material = materialUncovered;
		Grid.tilesUncovered += 1;
	}

	private void uncoverAdjacentTiles(){

		foreach(Tile currentTile in adjacentTiles){
			//uncover all adjacent tiles with 0 mines
			if(!currentTile.isMined && currentTile.state == "idle" && currentTile.adjacentMines == 0){
				currentTile.uncoverTile();
			} 
			//uncover all adjacent tiles with more than 1 adjacent mine, then stop uncovering
			else if(!currentTile.isMined && currentTile.state == "idle" && currentTile.adjacentMines > 0) {
				currentTile.uncoverTileExternal();
			}
		}
	}

	void explode(){
		state = "detonated";
		renderer.material = materialDetonated;
		foreach (Tile currentTile in Grid.tilesMined) {
			currentTile.explodeExternal();
		}

		Grid.state = "gameOver";
	}

	void explodeExternal(){
		state = "detonated";
		renderer.material = materialDetonated;
	}

	void OnMouseOver(){
		if(Grid.state == "inGame"){
			if (state == "idle") {
				renderer.material = materialLightup;
				if (Input.GetMouseButtonDown (0)) {
					uncoverTile();;
				}

				if (Input.GetMouseButtonDown (1)) {
					setFlag();
				}
			}

			else if (state == "flagged") {
				renderer.material = materialLightup;
				if (Input.GetMouseButtonDown (1)) {
					setFlag();
				}
			}
		}
	}

	void OnMouseExit(){
		if(Grid.state == "inGame"){
			if (state == "idle" || state == "flagged") {
				renderer.material = materialIdle;
			}
		}
	}

	void OnMouseUp(){
		//Clunky Debugger

		StringBuilder sb = new StringBuilder ();

		if(tileUpperLeft) { sb.Append(tileUpperLeft.ID); } else { sb.Append("-"); }
		sb.Append (",");
		if(tileUpper) { sb.Append(tileUpper.ID); } else { sb.Append("-"); }
		sb.Append (",");
		if(tileUpperRight) { sb.Append(tileUpperRight.ID); } else { sb.Append("-"); }
		sb.Append ("\n");
		if(tileLeft) { sb.Append(tileLeft.ID); } else { sb.Append("-"); }
		sb.Append (",");
		sb.Append(ID);
		sb.Append (",");
		if(tileRight) { sb.Append(tileRight.ID); } else { sb.Append("-"); }
		sb.Append ("\n");
		if(tileLowerLeft) { sb.Append(tileLowerLeft.ID); } else { sb.Append("-"); }
		sb.Append (",");
		if(tileLower) { sb.Append(tileLower.ID); } else { sb.Append("-"); }
		sb.Append (",");
		if(tileLowerRight) { sb.Append(tileLowerRight.ID); } else { sb.Append("-"); }

		print (sb);

	}
}
