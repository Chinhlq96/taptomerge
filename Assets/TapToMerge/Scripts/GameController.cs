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
	private bool isGameOver;
	private bool checkGameOver;
	private BlockController[,] board;
	private int[,] listDestroy; 

	public Transform startPos;

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
		Vector3 offset;
		for (x = 0; x < gridSize; x++)
		{
			for (y = 0; y < gridSize; y++) 
				for (int i = 0; i<blocks.Length; i++) 
					if (blocks[i].value == grid [x, y]) 
					{
						offset = new Vector3 (offsetX*y, offsetY*x);
						var block = Instantiate (blocks [i], startPos.position + offset, blocks [i].transform.rotation);
						block.x = x;
						block.y = y;
						board [x, y] = block.GetComponent<BlockController> ();
					}

			for (int i = 0; i<blocks.Length; i++) 
				blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder--;
		}
		//Set lai order
		for (int i = 0; i<blocks.Length; i++) 
			blocks [i].gameObject.GetComponent<Renderer> ().sortingOrder = 0;
	}

	public void CheckTap (BlockController blockTap) 
	{
		//Transform savePos = blockTap.transform;

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
					/*Vector3[] path = new Vector3[25];
					int k = 0;
					path [k] = blockTap.transform.position;
					for (int j = 0; j < path.Length; j++) 
					{*/
					//block.gameObject.transform.DOMove (blockTap.transform.position,0.2f);
					//}

					//BlockController.Instance.Move (path);
					Destroy (block.gameObject);
				}
			}
			Destroy (blockTap.gameObject);
			board [newBlock.x, newBlock.y] = newBlock.GetComponent<BlockController> ();
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

			if (checkGameOver || (block.value == 10))
				isGameOver = true;
		}
	}
}
