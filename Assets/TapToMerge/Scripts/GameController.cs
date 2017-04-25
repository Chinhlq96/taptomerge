using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManager;
using DG.Tweening;

public class GameController : SingletonMonoBehaviour<GameController>
{
	private int gridSize = 5;
	private int x;
	private int y;
	private int[,] grid;
	//

	private int maxValue = 0;
	private int[] nullCounter;

	//-------
	private bool isGameOver;
	private bool checkGameOver;
	private BlockController[,] board;
	private Vector3 someVector = new Vector3(0.0f,0.77f,0.0f);
	//-------
	private int[,] listDestroy; 

	public Transform startPos;
	public Transform fillPos;


	[SerializeField]
	private BlockController[] blocks; 
	[SerializeField]
	private float offsetX;
	[SerializeField]
	private float offsetY;



	void OnEnable()
	{
		this.RegisterListener (EventID.BlockTap, (sender, param) => CheckTap ((BlockController)param));

	}

	void OnDisable()
	{
		this.RemoveListener (EventID.BlockTap, (sender, param) => CheckTap ((BlockController)param));
	}
	void Start () 
	{
		
		GenerateGrid ();
		InstanceBlocks ();

	}

	void Update()
	{	//CheckMaxValue ();

		//CountNull ();
		//Fall ();
		//Fill ();
	//	Move();

		if (isGameOver)
			Debug.Log ("Game Over!");	
	}
	void GenerateGrid() {
		var test = 0;
		grid = new int[gridSize, gridSize];
		listDestroy = new int[gridSize, gridSize];
		board = new BlockController[gridSize,gridSize];
		//Gen blocks value
		for (x = 0; x < gridSize; x++)
			for (y = 0; y < gridSize; y++) 
			{
				if (test < 10)
					test++;
				else
					test = 1;
				grid [x, y] = (int)Random.Range (1f, 5f);
			}
	}

	void InstanceBlocks() {

		for (x = 0; x < gridSize; x++)
		{
			for (y = 0; y < gridSize; y++) 
				for (int i = 0; i<blocks.Length; i++) 
					if (blocks[i].value == grid [x, y]) 
					{
						var pos = ConvertBoardToPosition (x, y+2*gridSize);
						var block = Instantiate (blocks [i], pos, blocks [i].transform.rotation);
						block.x = x;
						block.y = y;
						board [x, y] = block.GetComponent<BlockController> ();
						block.Drop (x,y,true);
					}

			for (int i = 0; i<blocks.Length; i++) 
				blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder--;
		}
		//Set lai order
		for (int i = 0; i<blocks.Length; i++) 
			blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder = 0;
	}

	public Vector3 ConvertBoardToPosition(int x,int y)
	{
		Vector3 offset = new Vector3 (offsetY*x,offsetX*y);
		return startPos.position + offset;
	}

	public void CheckTap (BlockController blockTap) 
	{
		//Transform savePos = blockTap.transform;
		CheckGameOver();
		//Debug.Log (savePos);
		if (blocksActivated.Find(x=>x==blockTap) == null) {
			foreach (BlockController block in blocksActivated) {
				block.isActivated = false;
			}
			blocksActivated = BFSCheckBlock (blockTap);
			if (blocksActivated.Count == 0)
				blockTap.isActivated = false;
			else {
				blockTap.isActivated = true;
				blocksActivated.Add (blockTap);
			}
		} else {
			//int saveValue = blockTap.value;
			int i;
			for (i = 0; i < blocks.Length; i++)
				if (blocks [i].value == (blockTap.value + 1)) {
					break;
				}
			var newBlock = Instantiate (blocks [i], blockTap.transform.position - new Vector3 (0f, blockTap.offset, 0f), blockTap.transform.rotation);
			newBlock.x = blockTap.x;
			newBlock.y = blockTap.y;
			newBlock.gameObject.GetComponent<Renderer> ().sortingOrder = blockTap.gameObject.GetComponent<Renderer> ().sortingOrder;

			foreach (BlockController block in blocksActivated) {
				//Move ve theo path
				if (block != blockTap) {
					var listOfPoint = new List<Vector3>();
					listOfPoint.Add (blockTap.transform.position);
					//listOfPoint.Add (new Vector3 (1f, 1f, 1f));
					Vector3[] path = listOfPoint.ToArray();


					//for (int j = 0; j < path.Length; j++) 

					//block.gameObject.transform.DOMove (blockTap.transform.position,0.2f);
					//}

					block.Move (path);
					//Destroy (block.gameObject);
				}
			}

			Destroy (blockTap.gameObject);
			board [newBlock.x, newBlock.y] = newBlock.GetComponent<BlockController> ();
			if (newBlock.value == 10) 
			{
				isGameOver = true;
			}
			/*for (int i = 0; i < blocks.Length; i++)
				if (blocks [i].value == (saveValue + 1)) {
					Instantiate (blocks [i], savePos.position - new Vector3(0,0.07f,0), savePos.transform.rotation);
				}*/
			
		}


	}
	List<BlockController> blocksActivated = new List<BlockController> ();

	List<BlockController> BFSCheckBlock(BlockController block) 
	{
		List<BlockController> result = new List<BlockController> ();
		Queue<BlockController> queue = new Queue<BlockController> ();
		BlockController currentBlock;

		queue.Enqueue (block);
		while (queue.Count > 0) 
		{
			currentBlock = queue.Dequeue ();
			//Kiem tra tim dc 4 huong.
			if ((currentBlock.x + 1 < gridSize) 
				&& (board [currentBlock.x + 1, currentBlock.y].value == currentBlock.value) && (!board [currentBlock.x + 1, currentBlock.y].isActivated)) 
			{
				board [currentBlock.x + 1, currentBlock.y].isActivated = true;
				queue.Enqueue (board [currentBlock.x + 1, currentBlock.y]);
				result.Add (board [currentBlock.x + 1, currentBlock.y]);
			}
			if ((currentBlock.y + 1 < gridSize) 
				&& (board [currentBlock.x, currentBlock.y + 1].value == currentBlock.value) && (!board [currentBlock.x, currentBlock.y + 1].isActivated))
			{
				board [currentBlock.x, currentBlock.y + 1].isActivated = true;
				queue.Enqueue (board [currentBlock.x, currentBlock.y + 1]);
				result.Add (board [currentBlock.x, currentBlock.y + 1]);

			}
			if ((currentBlock.x - 1 >= 0) 
				&& (board [currentBlock.x - 1, currentBlock.y].value == currentBlock.value) && (!board [currentBlock.x - 1, currentBlock.y].isActivated)) 
			{
				board [currentBlock.x - 1, currentBlock.y].isActivated = true;
				queue.Enqueue (board [currentBlock.x - 1, currentBlock.y]);
				result.Add (board [currentBlock.x - 1, currentBlock.y]);

			}
			if ((currentBlock.y - 1 >= 0) 
				&& (board [currentBlock.x, currentBlock.y - 1].value == currentBlock.value) && (!board [currentBlock.x, currentBlock.y - 1].isActivated)) 
			{
				board [currentBlock.x, currentBlock.y - 1].isActivated = true;
				queue.Enqueue (board [currentBlock.x, currentBlock.y - 1]);
				result.Add (board [currentBlock.x, currentBlock.y - 1]);

			}
		}
		return result;
	}

	List<Vector3> BFSFindPath(BlockController block) 
	{
		List<Vector3> result = new List<Vector3> ();
		Queue<BlockController> queue = new Queue<BlockController> ();
		BlockController currentBlock;

		queue.Enqueue (block);
		while (queue.Count > 0) 
		{
			currentBlock = queue.Dequeue ();
			//Kiem tra tim dc 4 huong.
			if ((currentBlock.x + 1 < gridSize) 
				&& (board [currentBlock.x + 1, currentBlock.y].value == currentBlock.value) && (!board [currentBlock.x + 1, currentBlock.y].isActivated)) 
			{
				board [currentBlock.x + 1, currentBlock.y].isActivated = true;
				queue.Enqueue (board [currentBlock.x + 1, currentBlock.y]);
				//result.Add (board [currentBlock.x + 1, currentBlock.y]);
			}
			if ((currentBlock.y + 1 < gridSize) 
				&& (board [currentBlock.x, currentBlock.y + 1].value == currentBlock.value) && (!board [currentBlock.x, currentBlock.y + 1].isActivated))
			{
				board [currentBlock.x, currentBlock.y + 1].isActivated = true;
				queue.Enqueue (board [currentBlock.x, currentBlock.y + 1]);
				//result.Add (board [currentBlock.x, currentBlock.y + 1]);

			}
			if ((currentBlock.x - 1 >= 0) 
				&& (board [currentBlock.x - 1, currentBlock.y].value == currentBlock.value) && (!board [currentBlock.x - 1, currentBlock.y].isActivated)) 
			{
				board [currentBlock.x - 1, currentBlock.y].isActivated = true;
				queue.Enqueue (board [currentBlock.x - 1, currentBlock.y]);
				//result.Add (board [currentBlock.x - 1, currentBlock.y]);

			}
			if ((currentBlock.y - 1 >= 0) 
				&& (board [currentBlock.x, currentBlock.y - 1].value == currentBlock.value) && (!board [currentBlock.x, currentBlock.y - 1].isActivated)) 
			{
				board [currentBlock.x, currentBlock.y - 1].isActivated = true;
				queue.Enqueue (board [currentBlock.x, currentBlock.y - 1]);
				//result.Add (board [currentBlock.x, currentBlock.y - 1]);

			}
		}
		return result;
	}

	void CheckGameOver() {
		foreach (BlockController block in board) 
		{
			checkGameOver = true;
			for (x = 0; x < gridSize; x += 2)
				for (y = 0; y < gridSize; y += 2) 
				{
					var currentBlock = board [x, y];
					//Kiem tra 4 huong.
					if ((currentBlock.x + 1 < gridSize)
						&& (board [currentBlock.x + 1, currentBlock.y].value == currentBlock.value)) 
						checkGameOver = false;
					if ((currentBlock.y + 1 < gridSize) 
						&& (board [currentBlock.x, currentBlock.y + 1].value == currentBlock.value)) 
						checkGameOver = false;
					if ((currentBlock.x - 1 >= 0) 
						&& (board [currentBlock.x - 1, currentBlock.y].value == currentBlock.value)) 
						checkGameOver = false;
					if ((currentBlock.y - 1 >= 0) 
						&& (board [currentBlock.x, currentBlock.y - 1].value == currentBlock.value)) 
						checkGameOver = false;
				}
			if (checkGameOver)
				isGameOver = true;

		}

	}

	//Dem so block null tung cot 
	/*void CountNull()
	{ 
		nullCounter = new int[gridSize];
		for (int i = 0; i < gridSize; i++) 
		{
			nullCounter [i] = 0;
		}
		for (int j = 0; j < gridSize; j++) 
		{ 
			for (int i = 0; i < gridSize; i++) 
			{	
				if (board [i, j] == null)
					
					nullCounter[j]++;
			}
		}

	}*/

	//Cac Block con lai roi xuong
	void Fall()
	{
		

		for (int j = 0; j < gridSize; j++) 
		{ 
			for (int i = 1; i < gridSize; i++) 
			{	
				//Bat dau tu i=1, kiem tra block duoi no co la null ko, co thi dich xuong.
				if (board [i, j] != null) 
				{   int a = 0;
					int temp = i;
					while (board [i - 1, j] == null) 
					{ 
						

						//Gan lai gia tri x cua block
						board [i, j].x -= 1;
						board [i, j].GetComponent<SpriteRenderer> ().sortingOrder ++;
						//Dao vi tri tuong ung trong board
						board [i - 1, j] = board [i, j];
						board [i, j] = null;
						//tiep tuc dich xuong
						i--;
						//ngan ko cho xuong -1
						if (i == 0)
							break;
						
					}
					//Test DoMove2
					//board [i, j].gameObject.transform.DOMove (board [i, j].transform.position - someVector*a, 0.5f).WaitForCompletion();
					i = temp;
				}
			}

		}
	}

	//Tao block lap day khoang trong
	/*void Fill()
	{ 
		Vector3 offset;
		Vector3 offsetNgang;
		Vector3 offsetDoc;

		int randomValue = 0;

		for (int j = 0; j < gridSize; j++) 
		{
			//nullCounter[j] la so block null cua cot j trong board[i,j]
			for (int amountToFill = nullCounter [j]; amountToFill > 0; amountToFill--) 
			{	
				//Check block co value max
				foreach (BlockController blockCheck in board) 
				{	
					if(blockCheck!=null)
					if (blockCheck.value >= maxValue)
						maxValue = blockCheck.value;
				}

				//Cac truong hop tuong ung
				switch (maxValue) 
				{
				case 4:
					randomValue = (int)Random.Range (0f, 3f);
					break;
				case 5:
				case 6:
					randomValue = (int)Random.Range (0f, 4f);
					break;
				case 7:
					randomValue = (int)Random.Range (0f, 5f);
					break;
				case 8:
					randomValue = (int)Random.Range (0f, 5f);
					break;
				case 9:
					randomValue = (int)Random.Range (0f, 6f);
					break;
				default:
					randomValue = (int)Random.Range (0f, 7f);
					break;


				}

				// Vi du j = 0 , co 1 block null (amountToFill = nullCounter = 1)  , i = gridSize - amountToFill = 5-1 =4 => board[i,j] 0,4 la cho can fill
				int i = gridSize - amountToFill;
				//vi fillPos o vi tri [0,6] nen phai "+ 1"
				//offset = new Vector3 (offsetX*j, -(offsetY*(amountToFill+1)));
				offsetNgang = new Vector3 (offsetX*j,0);
				offsetDoc = new Vector3 (0, -(offsetY*(amountToFill+1))); 
				//Tao Block roi move di
				//var block = Instantiate (blocks [randomValue], fillPos.position + offset, blocks [randomValue].transform.rotation);
				//Test DoMoveFill
				var block = Instantiate (blocks [randomValue], fillPos.position+offsetNgang, blocks [randomValue].transform.rotation);
				block.gameObject.transform.DOMove (block.transform.position + offsetDoc, 0.5f);
		//block.gameObject.transform.Translate(offsetDoc,);
				//Vector2.MoveTowards((Vector2)block.gameObject.transform.position,(Vector2)(block.gameObject.transform.position + offsetDoc),2f);
		//block.gameObject.transform.position = Vector3.MoveTowards(block.transform.position,block.transform.position+offsetDoc,Time.deltaTime*1f);

				//Gan gia tri x,y cho block / Gan block vao board
				block.x = i;
				block.y = j;
				board [i, j] = block.GetComponent<BlockController> ();
				board [i, j].GetComponent<SpriteRenderer> ().sortingOrder = -(i);
			}

		}
	}*/



	IEnumerator MoveTo(Vector3 current,Vector3 target)
	{
		current = Vector3.MoveTowards (current, target, 1 * Time.deltaTime);
		yield return null;
	}

	void Move()
	{
		Vector3 offset;
		Vector3 offsetNgang;
		Vector3 offsetDoc;
		int randomValue = 0;





		nullCounter = new int[gridSize];
		for (int j = 0; j < gridSize; j++) 
		{
			nullCounter [j] = 0;
		}

		for (int j = 0; j < gridSize; j++) { 
			
			for (int i = 0; i < gridSize; i++)
			{	
				if (board [i, j] == null)
					nullCounter [j]++;
				if (board [i, j] != null) 
				{	
					//Vector3 current = board [i, j].gameObject.transform.position;
					//Vector3 target = board [i, j].gameObject.transform.position - (someVector * nullCounter [j]);
					//MoveTo (current, target);
					board [i, j].gameObject.transform.position -= (someVector * nullCounter [j]);

					Debug.Log (board [4, 1].transform.position);
					Debug.Log(board [4, 1].transform.position- (someVector * nullCounter [j]));
					Debug.Log (nullCounter [1]);

				}

					
			}
		}

		//Fall ();

		/*for (int j = 0; j < gridSize; j++) 
		{
			//nullCounter[j] la so block null cua cot j trong board[i,j]
			for (int amountToFill = nullCounter [j]; amountToFill > 0; amountToFill--) 
			{	
			for (int slotToFill = nullCounter [j]; slotToFill > 0; slotToFill--) 
			{	
				//Check block co value max
				foreach (BlockController blockCheck in board) 
				{	
					if(blockCheck!=null)
					if (blockCheck.value >= maxValue)
						maxValue = blockCheck.value;
				}

				//Cac truong hop tuong ung
				switch (maxValue) 
				{
				case 4:
					randomValue = (int)Random.Range (0f, 3f);
					break;
				case 5:
				case 6:
					randomValue = (int)Random.Range (0f, 4f);
					break;
				case 7:
					randomValue = (int)Random.Range (0f, 5f);
					break;
				case 8:
					randomValue = (int)Random.Range (0f, 5f);
					break;
				case 9:
					randomValue = (int)Random.Range (0f, 6f);
					break;
				default:
					randomValue = (int)Random.Range (0f, 7f);
					break;


				}

				// Vi du j = 0 , co 1 block null (amountToFill = nullCounter = 1)  , i = gridSize - amountToFill = 5-1 =4 => board[i,j] 0,4 la cho can fill
				int i = gridSize - slotToFill;
				//vi fillPos o vi tri [0,6] nen phai "+ 1"
				//offset = new Vector3 (offsetX*j, -(offsetY*(amountToFill+1)));
				offsetNgang = new Vector3 (offsetX*j,0);
				offsetDoc = new Vector3 (0, -(offsetY*(slotToFill+1))); 
				//Tao Block roi move di
				//var block = Instantiate (blocks [randomValue], fillPos.position + offset, blocks [randomValue].transform.rotation);
				//Test DoMoveFill
				var block = Instantiate (blocks [randomValue], fillPos.position+offsetNgang, blocks [randomValue].transform.rotation);
				//block.gameObject.transform.DOMove (block.transform.position + offsetDoc, 0.5f);
				Vector2.MoveTowards(block.transform.position,block.transform.position + offsetDoc,10.0f);
				//block.gameObject.transform.Translate(offsetDoc,);
				//Vector2.MoveTowards((Vector2)block.gameObject.transform.position,(Vector2)(block.gameObject.transform.position + offsetDoc),2f);
				//block.gameObject.transform.position = Vector3.MoveTowards(block.transform.position,block.transform.position+offsetDoc,Time.deltaTime*1f);

				//Gan gia tri x,y cho block / Gan block vao board
				block.x = i;
				block.y = j;
				board [i, j] = block.GetComponent<BlockController> ();
				board [i, j].GetComponent<SpriteRenderer> ().sortingOrder = -(i);
				block.x = i;
				block.y = j;
				board [i, j] = block.GetComponent<BlockController> ();
			}


		}
	}*/

	}
}
