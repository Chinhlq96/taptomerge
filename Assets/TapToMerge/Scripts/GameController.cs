using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManager;
using DG.Tweening;
using UnityEngine.UI;

public class GameController : SingletonMonoBehaviour<GameController>
{
    private int gridSize = 5;

    private int score;
    //
    private int maxValue = 0;

    public bool isMerging;
    //-------
    // Kiem tra game over = 10 hoac het block ket noi
    private bool isGameOver;

    private BlockController[,] board;
    //-------

    [SerializeField]
    private Transform startPos;

    [SerializeField]
    private float offsetY;
    [SerializeField]
    private float offsetX;
    [SerializeField]
    Text messageText;
	[SerializeField]
	Text scoreText;

    const string messageGame = "Combine The Titles To Get 10";
    const string messageGameOver = "No Matched !";
    const string messageShare = "Share The Result";


    void OnEnable()
    {
        this.RegisterListener(EventID.BlockTap, (sender, param) => CheckTap((BlockController)param));

    }

    void OnDisable()
    {
        this.RemoveListener(EventID.BlockTap, (sender, param) => CheckTap((BlockController)param));
    }
    public void StartGame()
    {
        isMerging = false;
        score = 0;
        InstanceBlocks();
        messageText.text = messageGame;
    }

    void Update()
    {	//CheckMaxValue ();

        //  if (isGameOver)
        //      Debug.Log("Game Over!");
    }

    void InstanceBlocks()
    {
        board = new BlockController[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var pos = ConvertBoardToPosition(x, y + 2 * gridSize);
                var value = (int)Random.Range(1f, 5f);
                var block = ContentMgr.Instance.GetItem<BlockController>("block" + value, startPos, pos);

                block.Init(x, y, value);
                board[x, y] = block;
                board[x, y].Drop(x, y, true);

            }
        }
    }


    public Vector3 ConvertBoardToPosition(int x, int y)
    {
        Vector3 offset = new Vector3(offsetX * x, offsetY * y);
        return startPos.position + offset;
    }


    public void CheckTap(BlockController blockTap)
    {

        if (blocksActivated.Find(x => x == blockTap) == null)
        {
            foreach (BlockController block in blocksActivated)
            {
                if (block != null)
                {
                    block.isActivated = false;
                }
            }
			blockTap.isActivated = true;
            blocksActivated = BFSCheckBlock(blockTap);
            if (blocksActivated.Count == 0)
                blockTap.isActivated = false;
            else
            {
                blocksActivated.Add(blockTap);
            }
        }
        else
        {
			isMerging = true;

			Debug.Log ("CT1");
			//So Block pha duoc
			int count = 0, saveCount = blocksActivated.Count;
			for (int i = 0; i < blocksActivated.Count;) {
				//foreach (BlockController block in blocksActivated) {
				//Move ve theo path
				BlockController block = blocksActivated [0];
				if (block != blockTap) {
					Vector3[] path = BFSFindPath (block, blockTap).ToArray ();
					Debug.Log ("  " + block.gameObject.name);
					block.Move (path, () => { 
						board [block.x, block.y] = null; 
					});
				}
				blocksActivated.Remove (block);
				count++;
				Debug.Log ("BC" + blocksActivated.Count);
			}
			Debug.Log ("CT2");
			// Tinh diem
			int pointPerBlock = blockTap.value + 2;
			if (blockTap.value < 3)
				score += pointPerBlock * (blockTap.value * 2 + count - 2);
			else
				score += pointPerBlock * (blockTap.value * 2 + count - 2) - (blockTap.value - 2);
			scoreText.text = "" + score;
			StartCoroutine (WaitMerge (.2f, blockTap));
        }
    }

    IEnumerator WaitMerge(float duration, BlockController blockTap)
    {
        yield return new WaitForSeconds(duration);   //Wait
        ContentMgr.Instance.Despaw(blockTap.gameObject);
        //board[blockTap.x, blockTap.y] = null;
        var newValue = blockTap.value + 1;
        var pos = ConvertBoardToPosition(blockTap.x, blockTap.y);
        var newBlock = ContentMgr.Instance.GetItem<BlockController>("block" + newValue, startPos, pos);
        newBlock.Init(blockTap.x, blockTap.y, newValue);

        newBlock.isActivated = false;

        board[newBlock.x, newBlock.y] = newBlock;

        if (newBlock.value == 10)
        {
            isGameOver = true;
            Debug.Log("Game Over!");
        }

        Move();
        if (CheckGameOver())
        {
            messageText.text = messageGameOver;
        }
		isMerging = false;
    }
    List<BlockController> blocksActivated = new List<BlockController>();

    List<BlockController> BFSCheckBlock(BlockController block)
    {
        List<BlockController> result = new List<BlockController>();
        Queue<BlockController> queue = new Queue<BlockController>();
        BlockController currentBlock;

        queue.Enqueue(block);
        while (queue.Count > 0)
        {
			Debug.Log (result.Count);
            currentBlock = queue.Dequeue();
            if (currentBlock.value == -1)
            {
                Debug.Log("--------------");
                continue;
            }
            //Kiem tra tim dc 4 huong.
            if ((currentBlock.x + 1 < gridSize)
                && (board[currentBlock.x + 1, currentBlock.y].value == currentBlock.value) && (!board[currentBlock.x + 1, currentBlock.y].isActivated))
            {
                board[currentBlock.x + 1, currentBlock.y].isActivated = true;
                queue.Enqueue(board[currentBlock.x + 1, currentBlock.y]);
                result.Add(board[currentBlock.x + 1, currentBlock.y]);
            }
            if ((currentBlock.y + 1 < gridSize)
                && (board[currentBlock.x, currentBlock.y + 1].value == currentBlock.value) && (!board[currentBlock.x, currentBlock.y + 1].isActivated))
            {
                board[currentBlock.x, currentBlock.y + 1].isActivated = true;
                queue.Enqueue(board[currentBlock.x, currentBlock.y + 1]);
                result.Add(board[currentBlock.x, currentBlock.y + 1]);
            }
            if ((currentBlock.x - 1 >= 0)
                && (board[currentBlock.x - 1, currentBlock.y].value == currentBlock.value) && (!board[currentBlock.x - 1, currentBlock.y].isActivated))
            {
                board[currentBlock.x - 1, currentBlock.y].isActivated = true;
                queue.Enqueue(board[currentBlock.x - 1, currentBlock.y]);
                result.Add(board[currentBlock.x - 1, currentBlock.y]);
            }
            if ((currentBlock.y - 1 >= 0)
                && (board[currentBlock.x, currentBlock.y - 1].value == currentBlock.value) && (!board[currentBlock.x, currentBlock.y - 1].isActivated))
            {
                board[currentBlock.x, currentBlock.y - 1].isActivated = true;
                queue.Enqueue(board[currentBlock.x, currentBlock.y - 1]);
                result.Add(board[currentBlock.x, currentBlock.y - 1]);
            }
        }
        return result;
    }
    //Tim duong di ngan nhat
    List<Vector3> BFSFindPath(BlockController block, BlockController targetBlock)
    {
        List<Vector3> result = new List<Vector3>();
        Queue<BlockController> queue = new Queue<BlockController>();
        BlockController currentBlock;
        BlockController[,] parentBlock = new BlockController[gridSize, gridSize];

        int[,] visit = new int[gridSize, gridSize];
        //Khoi tao mang visit dnah dau da tham va hoan thanh tham
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                visit[i, j] = 0; // = 0 la chua tham
            }
        }

        queue.Enqueue(block);
        while (queue.Count > 0)
        {
//			Debug.Log("L--------------");
            currentBlock = queue.Dequeue();
            if (currentBlock.value == -1)
            {
                Debug.Log("--------------");
                continue;
            }
            visit[currentBlock.x, currentBlock.y] = 1; // = 1 la da tham
            // Neu bang target thi Add vi tri va thoat
            if ((currentBlock.x == targetBlock.x) && (currentBlock.y == targetBlock.y))
            {
                break;
            }
//			Debug.Log ("null?"+parentBlock[currentBlock.x, currentBlock.y]);

            //Kiem tra tim 4 huong.
            if ((currentBlock.x + 1 < gridSize) && (board[currentBlock.x + 1, currentBlock.y].isActivated)
                && (visit[currentBlock.x + 1, currentBlock.y] == 0))
            {
                queue.Enqueue(board[currentBlock.x + 1, currentBlock.y]);
                parentBlock[currentBlock.x + 1, currentBlock.y] = currentBlock;
            }
            if ((currentBlock.y + 1 < gridSize) && (board[currentBlock.x, currentBlock.y + 1].isActivated)
                && (visit[currentBlock.x, currentBlock.y + 1] == 0))
            {
                queue.Enqueue(board[currentBlock.x, currentBlock.y + 1]);
                parentBlock[currentBlock.x, currentBlock.y + 1] = currentBlock;
            }
            if ((currentBlock.x - 1 >= 0) && (board[currentBlock.x - 1, currentBlock.y].isActivated)
                && (visit[currentBlock.x - 1, currentBlock.y] == 0))
            {
                queue.Enqueue(board[currentBlock.x - 1, currentBlock.y]);
                parentBlock[currentBlock.x - 1, currentBlock.y] = currentBlock;
            }
            if ((currentBlock.y - 1 >= 0) && (board[currentBlock.x, currentBlock.y - 1].isActivated)
                && (visit[currentBlock.x, currentBlock.y - 1] == 0))
            {
                queue.Enqueue(board[currentBlock.x, currentBlock.y - 1]);
                parentBlock[currentBlock.x, currentBlock.y - 1] = currentBlock;
            }
        }
//		for (int i = 0; i < gridSize; i++)
//		{
//			for (int j = 0; j < gridSize; j++)
//			{
//				Debug.Log (parentBlock [i, j]);
//			}
//		}
//		Debug.Log ("  "+targetBlock.gameObject.name);

        currentBlock = parentBlock[targetBlock.x, targetBlock.y];

		result.Add (targetBlock.transform.position);
		while ((currentBlock.x != block.x) || (currentBlock.y != block.y)) {
			result.Add (currentBlock.transform.position);
			currentBlock = parentBlock [currentBlock.x, currentBlock.y];
		}
		result.Reverse ();
		return result;
    }

    bool CheckGameOver()
    {
        bool checkGameOver = true;
        //foreach (BlockController block in board)
        //{
        for (int x = 0; x < gridSize; x += 1)
            for (int y = 0; y < gridSize; y += 1)
            {
                var currentBlock = board[x, y];
                if (currentBlock == null)
                {
                    Debug.Log("--------------");
                    continue;
                }
                //Kiem tra 4 huong.
                if ((currentBlock.x + 1 < gridSize)
                    && (board[currentBlock.x + 1, currentBlock.y].value == currentBlock.value))
                    checkGameOver = false;
                if ((currentBlock.y + 1 < gridSize)
                    && (board[currentBlock.x, currentBlock.y + 1].value == currentBlock.value))
                    checkGameOver = false;
                if ((currentBlock.x - 1 >= 0)
                    && (board[currentBlock.x - 1, currentBlock.y].value == currentBlock.value))
                    checkGameOver = false;
                if ((currentBlock.y - 1 >= 0)
                    && (board[currentBlock.x, currentBlock.y - 1].value == currentBlock.value))
                    checkGameOver = false;
            }

        return checkGameOver;
    }


    void Move()
    {
        int randomValue = 0;
        int countNull = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {

                if (board[i, j] == null)
                {
                    countNull++;
                }
                else
                {
                    //check khong cho tu null
                    if (countNull != 0)
                    {
                        board[i, j].Drop(i, j - countNull);

                        board[i, j - countNull] = board[i, j];
                        board[i, j - countNull].Init(i, j - countNull, board[i, j].value);

                        board[i, j] = null;
                    }
                }
            }

            //kiem tra phan tu lon nhat
            foreach (BlockController blockCheck in board)
            {
                if (blockCheck != null)
                    if (blockCheck.value >= maxValue)
                        maxValue = blockCheck.value;
            }

            for (int k = 0; k < countNull; k++)
            {
                var pos = ConvertBoardToPosition(i, gridSize + k);
                //Random gia tri dua vao max hien tai
                switch (maxValue)
                {
                    case 4:
                        randomValue = (int)Random.Range(0f, 3f);
                        break;
                    case 5:
                    case 6:
                        randomValue = (int)Random.Range(0f, 4f);
                        break;
                    case 7:
                        randomValue = (int)Random.Range(0f, 5f);
                        break;
                    case 8:
                        randomValue = (int)Random.Range(0f, 5f);
                        break;
                    case 9:
                        randomValue = (int)Random.Range(0f, 6f);
                        break;
                    default:
                        randomValue = (int)Random.Range(0f, 7f);
                        break;
                }

                var newY = gridSize - countNull + k;
                board[i, newY] = ContentMgr.Instance.GetItem<BlockController>("block" + (randomValue + 1), startPos, pos);
                board[i, newY].Init(i, newY, randomValue + 1);
                board[i, newY].Drop(i, newY);

            }
            countNull = 0;
        }
    }
}
