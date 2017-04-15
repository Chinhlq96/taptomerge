using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManager;

public class GameController : SingletonMonoBehaviour<GameController>
{
	private int gridSize = 5;
	private int x;
	private int y;
	private int[,] grid;
	private bool isGameOver;
	private bool checkGameOver;
	private BlockTest[,] board;
	private BlockTest[] listDestroy; 

	public Transform startPos;

	[SerializeField]
	private BlockTest[] blocks; 
	[SerializeField]
	private float offsetX;
	[SerializeField]
	private float offsetY;

	void OnEnable()
	{
		this.RegisterListener (EventID.BlockTap, (sender, param) => { CheckDestroy (); CheckTap (); });
		//this.RegisterListener (EventID.BlockTap, (sender, param) => CheckDestroy ());

	}

	void OnDisable()
	{
		this.RemoveListener (EventID.BlockTap, (sender, param) => { CheckDestroy (); CheckTap (); });
		//this.RemoveListener (EventID.BlockTap, (sender, param) => );
	}
	void Start () 
	{
		GenerateGrid ();
		InstanceBlocks ();
	}

	void GenerateGrid() {
		var test = 0;
		grid = new int[gridSize, gridSize];
		board = new BlockTest[gridSize,gridSize];
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
		Vector3 offset;
		for (x = 0; x < gridSize; x++)
		{
			for (y = 0; y < gridSize; y++) 
				for (int i = 0; i<blocks.Length; i++) 
					if (blocks[i].value == grid [x, y]) 
					{
						offset = new Vector3 (offsetX*y, offsetY*x);
						//var block = Instantiate (blocks[i], parent);
						//block.transform.localPosition = startPos.position + offset;
						var block = Instantiate (blocks [i], startPos.position + offset, blocks [i].transform.rotation);
						block.x = x;
						block.y = y;
						board [x, y] = block.GetComponent<BlockTest> ();
						//go.transform.SetAsFirstSibling ();
					}

			for (int i = 0; i<blocks.Length; i++) 
				blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder--;
		}
		//Set lai order
		for (int i = 0; i<blocks.Length; i++) 
			blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder = 0;
	}

	public void CheckTap () 
	{
		foreach (BlockTest block in board) 
		{
			if (block.isTapped) 
			{
				block.isActivated = true;	
				//Debug.Log (CheckInGroup (block, block));
				//Check xem co block nao active chua
				foreach (BlockTest activatedBlock in board)
					if (activatedBlock.isActivated && !CheckInGroup (block, activatedBlock)) {

						foreach (BlockTest sameBlock in board)
							if (CheckInGroup (activatedBlock, sameBlock))
								sameBlock.isActivated = false;
						activatedBlock.isActivated = false;
						//block.isTapped = false;
					}

				//Chua co block nao active
				if (BFSCheckBlock (block) == 0)
					block.isActivated = false;	
				block.isTapped = false;

				//Kiem tra GameOver neu khong noi duoc block nao
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
			//Check GameOver co block = 10
			if (block.value == 10)
				isGameOver = true;
			//Debug.Log (isGameOver);
		}

	}

	int BFSCheckBlock(BlockTest block) 
	{
		Queue<BlockTest> queue = new Queue<BlockTest> ();
		BlockTest currentBlock;
		int count = 0;
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
				count++;
			}
			if ((currentBlock.y + 1 < gridSize) 
				&& (board [currentBlock.x, currentBlock.y + 1].value == currentBlock.value) && (!board [currentBlock.x, currentBlock.y + 1].isActivated))
			{
				board [currentBlock.x, currentBlock.y + 1].isActivated = true;
				queue.Enqueue (board [currentBlock.x, currentBlock.y + 1]);
				count++;
			}
			if ((currentBlock.x - 1 >= 0) 
				&& (board [currentBlock.x - 1, currentBlock.y].value == currentBlock.value) && (!board [currentBlock.x - 1, currentBlock.y].isActivated)) 
			{
				board [currentBlock.x - 1, currentBlock.y].isActivated = true;
				queue.Enqueue (board [currentBlock.x - 1, currentBlock.y]);
				count++;
			}
			if ((currentBlock.y - 1 >= 0) 
				&& (board [currentBlock.x, currentBlock.y - 1].value == currentBlock.value) && (!board [currentBlock.x, currentBlock.y - 1].isActivated)) 
			{
				board [currentBlock.x, currentBlock.y - 1].isActivated = true;
				queue.Enqueue (board [currentBlock.x, currentBlock.y - 1]);
				count++;
			}
		}
		return count;
	}

	bool CheckInGroup (BlockTest block, BlockTest otherBlock) {
		if (!block.isActivated)
			return false;
		/*if ((otherBlock.x == block.x) && (otherBlock.y == block.y))
			return true;*/
		Queue<BlockTest> queue = new Queue<BlockTest> ();
		BlockTest currentBlock;
		queue.Enqueue (block);
		while (queue.Count > 0) 
		{
			currentBlock = queue.Dequeue ();
			//Kiem tra tim dc 4 huong.
			if ((currentBlock.x + 1 < gridSize)
				&& (otherBlock.x == currentBlock.x + 1) && (otherBlock.y == currentBlock.y))
				return true;
			if ((currentBlock.y + 1 < gridSize)
				&& (otherBlock.x == currentBlock.x) && (otherBlock.y == currentBlock.y + 1))
				return true;
			if ((currentBlock.x - 1 >= 0)
				&& (otherBlock.x == currentBlock.x - 1) && (otherBlock.y == currentBlock.y))
				return true;
			if ((currentBlock.y - 1 >= 0)
				&& (otherBlock.x == currentBlock.x) && (otherBlock.y == currentBlock.y - 1))
				return true;
		}
		return false;
	}

	void CheckDestroy() {
		foreach (BlockTest block in board) 
		{
			if (block.isTapped && block.isActivated) {
				//Destroy chinh no va block trong group cua no
				Debug.Log ("Explosion!!");
				/*var i = 0;
				foreach (BlockTest otherBlock in board)
					if (CheckInGroup(block, otherBlock)) {
						listDestroy[i] = otherBlock;
						i++;
					}
				foreach (BlockTest destroyedBlock in listDestroy) {
					Destroy(destroyedBlock.gameObject);
				}*/
				Destroy (block.gameObject);
			} 
		}

	}
}
