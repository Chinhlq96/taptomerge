using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManager;
using DG.Tweening;

public class GameController : SingletonMonoBehaviour<GameController>
{
	private int gridSize = 5;
	//private int x;
	//private int y;
	private int[,] grid;
	private int score;
	//

	private int maxValue = 0;
	private int[] nullCounter;

	public bool isMerging;
	public float offset = 0.07f;

	//-------
	// Kiem tra game over = 10 hoac het block ket noi
	private bool isGameOver;
	// Kiem tra game over het block ket noi
	private bool checkGameOver;
	private BlockController[,] board;
	private Vector3 someVector = new Vector3(0.0f,0.777f,0.0f);
	//-------

	[SerializeField]
	private Transform startPos;
	[SerializeField]
	private Transform fillPos;
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
		isMerging = false;
		score = 0;
		GenerateGrid ();
		InstanceBlocks ();

	}

	void Update()
	{	//CheckMaxValue ();

		//CountNull ();
		//Fall ();
		//Fill ();


		if (isGameOver)
			Debug.Log ("Game Over!");	
	}
	void GenerateGrid() {
		var test = 0;
		grid = new int[gridSize, gridSize];
		board = new BlockController[gridSize,gridSize];
		//Gen blocks value
		for (int x = 0; x < gridSize; x++)
			for (int y = 0; y < gridSize; y++) 
			{
				if (test < 10)
					test++;
				else
					test = 1;
				grid [x, y] = (int)Random.Range (1f, 5f);
			}
	}

	void InstanceBlocks() {

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++) 
				for (int i = 0; i<blocks.Length; i++) 
					if (blocks[i].value == grid [x, y]) 
					{
						var pos = ConvertBoardToPosition (x, y+2*gridSize);
						var block = Instantiate (blocks [i], pos, blocks [i].transform.rotation);
						block.x = x;
						block.y = y;

						board [x, y] = block.GetComponent<BlockController> ();
						board[x,y].Drop (x,y,true);
						board [x, y].gameObject.name = x +""+ y;
						board [x, y].gameObject.GetComponent<Renderer> ().sortingOrder = -y;

					}

//			for (int i = 0; i<blocks.Length; i++) 
//				blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder++;		
		}
		//Set lai order
//		for (int i = 0; i<blocks.Length; i++) 
//			blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder = 0;
	}


	public Vector3 ConvertBoardToPosition(int x,int y)
	{
		Vector3 offset = new Vector3 (offsetY*x,offsetX*y);
		return startPos.position + offset;
	}


	public void CheckTap (BlockController blockTap) 
	{
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
			isMerging = true;
			int i, count = 0;
			for (i = 0; i < blocks.Length; i++)
				if (blocks [i].value == (blockTap.value + 1)) {
					break;
				}
			var newBlock = Instantiate (blocks [i], blockTap.transform.position, blockTap.transform.rotation);
			newBlock.x = blockTap.x;
			newBlock.y = blockTap.y;
			newBlock.gameObject.GetComponent<Renderer> ().sortingOrder = blockTap.gameObject.GetComponent<Renderer> ().sortingOrder;
			newBlock.gameObject.SetActive (false);
			foreach (BlockController block in blocksActivated) {
				//Move ve theo path
				count++;
				if (block != blockTap) {
					var listOfPoint = new List<Vector3>();

					Vector3[] path = BFSFindPath (block, blockTap).ToArray ();
					block.gameObject.GetComponent<Renderer> ().sortingOrder -= 10;
					block.Move (path);
				}
			}
			count--;

			int pointPerBlock = blockTap.value + 2;
			if (blockTap.value < 3)
				score += pointPerBlock * (blockTap.value * 2 + count - 2);
			else
				score += pointPerBlock * (blockTap.value * 2 + count - 2) - (blockTap.value - 2);
			
			StartCoroutine(WaitMerge(0.2f,blockTap,newBlock));

		}

	}
	IEnumerator WaitMerge(float duration,BlockController blockTap,BlockController newBlock)
	{
		yield return new WaitForSeconds(duration);   //Wait
		newBlock.gameObject.SetActive (true);
		newBlock.transform.position -= new Vector3 (0f, offset, 0f);
		Destroy (blockTap.gameObject);
		board [newBlock.x, newBlock.y] = newBlock.GetComponent<BlockController> ();
		//CheckGameOver();
		if (newBlock.value == 10) 
		{
			isGameOver = true;
		}
		isMerging = false;
//		Debug.Log (score);
		Move();
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
	//Tim duong di ngan nhat
	List<Vector3> BFSFindPath (BlockController block, BlockController targetBlock)
	{
		List<Vector3> result = new List<Vector3> ();
		Queue<BlockController> queue = new Queue<BlockController> ();
		BlockController currentBlock;
		BlockController[,] parentBlock = new BlockController[gridSize, gridSize];

		int[,] visit = new int[gridSize, gridSize];
		//Khoi tao mang visit dnah dau da tham va hoan thanh tham
		for (int i = 0; i < gridSize; i++) {
			for (int j = 0; j < gridSize; j++) {
				visit [i, j] = 0; // = 0 la chua tham
			}
		}

		queue.Enqueue (block);
		while (queue.Count > 0) {
			currentBlock = queue.Dequeue ();
			visit [currentBlock.x, currentBlock.y] = 1; // = 1 la da tham
			// Neu bang target thi Add vi tri va thoat
			if ((currentBlock.x == targetBlock.x) && (currentBlock.y == targetBlock.y)) {
				break;
			}

			//Kiem tra tim 4 huong.
			if ((currentBlock.x + 1 < gridSize) && (board [currentBlock.x + 1, currentBlock.y].isActivated)
			    && (visit [currentBlock.x + 1, currentBlock.y] == 0)) {
				queue.Enqueue (board [currentBlock.x + 1, currentBlock.y]);
				parentBlock [currentBlock.x + 1, currentBlock.y] = currentBlock;
			}
			if ((currentBlock.y + 1 < gridSize) && (board [currentBlock.x, currentBlock.y + 1].isActivated)
			    && (visit [currentBlock.x, currentBlock.y + 1] == 0)) {
				queue.Enqueue (board [currentBlock.x, currentBlock.y + 1]);
				parentBlock [currentBlock.x, currentBlock.y + 1] = currentBlock;
			}
			if ((currentBlock.x - 1 >= 0) && (board [currentBlock.x - 1, currentBlock.y].isActivated)
			    && (visit [currentBlock.x - 1, currentBlock.y] == 0)) {
				queue.Enqueue (board [currentBlock.x - 1, currentBlock.y]);
				parentBlock [currentBlock.x - 1, currentBlock.y] = currentBlock;
			}
			if ((currentBlock.y - 1 >= 0) && (board [currentBlock.x, currentBlock.y - 1].isActivated)
			    && (visit [currentBlock.x, currentBlock.y - 1] == 0)) {
				queue.Enqueue (board [currentBlock.x, currentBlock.y - 1]);
				parentBlock [currentBlock.x, currentBlock.y - 1] = currentBlock;
			}
		}

		currentBlock = parentBlock [targetBlock.x, targetBlock.y];
		result.Add (targetBlock.transform.position);
		while ((currentBlock.x != block.x) || (currentBlock.y != block.y)) {
			result.Add (currentBlock.transform.position);
			currentBlock = parentBlock [currentBlock.x, currentBlock.y];
		}
		result.Reverse ();
		return result;
	}

	void CheckGameOver() {
		foreach (BlockController block in board) 
		{
			checkGameOver = true;
			for (int x = 0; x < gridSize; x += 2)
				for (int y = 0; y < gridSize; y += 2) 
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
//	void Fall()
//	{
//		
//
//		for (int j = 0; j < gridSize; j++) 
//		{ 
//			for (int i = 1; i < gridSize; i++) 
//			{	
//				//Bat dau tu i=1, kiem tra block duoi no co la null ko, co thi dich xuong.
//				if (board [i, j] != null) 
//				{   int a = 0;
//					int temp = i;
//					while (board [i - 1, j] == null) 
//					{ 
//						
//
//						//Gan lai gia tri x cua block
//						board [i, j].x -= 1;
//						board [i, j].GetComponent<SpriteRenderer> ().sortingOrder ++;
//						//Dao vi tri tuong ung trong board
//						board [i - 1, j] = board [i, j];
//						board [i, j] = null;
//						//tiep tuc dich xuong
//						i--;
//						//ngan ko cho xuong -1
//						if (i == 0)
//							break;
//						
//					}
//					//Test DoMove2
//					//board [i, j].gameObject.transform.DOMove (board [i, j].transform.position - someVector*a, 0.5f).WaitForCompletion();
//					i = temp;
//				}
//			}
//
//		}
//	}

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



//	IEnumerator MoveTo(Vector3 current,Vector3 target)
//	{
//		current = Vector3.MoveTowards (current, target, 1 * Time.deltaTime);
//		yield return null;
//	}

	void Move()
	{	bool doneMove = false;
		int randomValue = 0;
		int countNull = 0;
		for (int i = 0; i < gridSize; i++) 
		{
			for (int j = 0; j < gridSize; j++) {
				
				if (board [i, j] == null) {
					countNull++;
				} else {
					//check khong cho tu null
					if (board [i, j - countNull] == null) 
					{
						board [i, j].Drop (i, j - countNull);
						//update y truoc khi gan vao board
						board [i, j].y -= countNull;
						board [i, j - countNull] = board [i, j];

						board [i, j - countNull].gameObject.name = i + "" + (j - countNull);
						board [i, j - countNull].gameObject.GetComponent<Renderer> ().sortingOrder = -(j - countNull);
						board [i, j] = null;
					}
				}
			}

			//kiem tra phan tu lon nhat
			foreach (BlockController blockCheck in board) 
			{	
				if(blockCheck!=null)
				if (blockCheck.value >= maxValue)
					maxValue = blockCheck.value;
			}




			for (int k = 0; k < countNull; k++) {
				var pos = ConvertBoardToPosition (i, gridSize+k);
				//Random gia tri dua vao max hien tai
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
				var block = Instantiate (blocks [randomValue], pos, blocks [randomValue].transform.rotation);
				var newY = gridSize - countNull + k;
				block.x = i;
				block.y = newY;

				board [i,  newY] = block.GetComponent<BlockController> ();
				board[i,newY].Drop (i,newY);
				board [i, newY].gameObject.name = i +""+ (newY);
				board [i,newY].gameObject.GetComponent<Renderer> ().sortingOrder = -(newY);
			}

			
			
			Debug.Log ("CHECK NULL "+i + " -  " + countNull);
			countNull = 0;
		}
		doneMove = true;
		if(doneMove==true)
		CheckGameOver();

		




//		Vector3 offset;
//		Vector3 offsetNgang;
//		Vector3 offsetDoc;
//		int randomValue = 0;
//
//		nullCounter = new int[gridSize];
//		for (int j = 0; j < gridSize; j++) 
//		{
//			nullCounter [j] = 0;
//		}
//
//		for (int j = 0; j < gridSize; j++) { 
//			
//			for (int i = 0; i < gridSize; i++)
//			{	
//				if (board [i, j] == null)
//					nullCounter [j]++;
//				if (board [i, j] != null) 
//				{	
//					//Vector3 current = board [i, j].gameObject.transform.position;
//					//Vector3 target = board [i, j].gameObject.transform.position - (someVector * nullCounter [j]);
//					//MoveTo (current, target);
//					//board [i, j].gameObject.transform.position -= (someVector * nullCounter [j]);
//					board [i, j].Drop (j , i-nullCounter[j], false);
//
//
//				}
//
//					
//			}
//		}
	//	Debug.Log (nullCounter [1]);

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
