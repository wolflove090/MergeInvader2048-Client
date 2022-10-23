using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;

public class GameController : ControllerBase<GameViewModel>
{
    const int LANE_NUM = 5;
    const int BLOCK_NUM = 6;

    // TODO ViewModel側に持つ
    [SerializeField]
    GameObject _BlockPrefab;

    [SerializeField]
    GameObject _Area;

    [SerializeField]
    Transform[,] _BlockPoss = new Transform[LANE_NUM, BLOCK_NUM];
    int _PosIndex;

    Block[,] _LaneData = new Block[5,6];

    // 生成ブロック
    Block _Block;

    bool _Moving;

    protected override void _OnStart()
    {
        // レーン情報の初期化
        this._LaneData = new Block[,]{
            {null,null,null,null,null,null},
            {null,null,null,null,null,null},
            {null,null,null,null,null,null},
            {null,null,null,null,null,null},
            {null,null,null,null,null,null},
            };

        // 各レーンからブロックのポジション情報を抽出
        this._BlockPoss = this._CreateBlockPos();

        // 初期ブロック
        this._BlockPrefab.SetActive(false);
        this._InstantBlock();
        

        // レーンタップアクション
        this._ViewModel.Lane01.OnClick = () => 
        {
            if(this._Moving)
                return;
                
            this._MoveBlock(0);
        };

        this._ViewModel.Lane02.OnClick = () => 
        {
            if(this._Moving)
                return;
                
            this._MoveBlock(1);
        };

        this._ViewModel.Lane03.OnClick = () => 
        {
            if(this._Moving)
                return;
                
            this._MoveBlock(2);
        };

        this._ViewModel.Lane04.OnClick = () => 
        {
            if(this._Moving)
                return;
                
            this._MoveBlock(3);
        };

        this._ViewModel.Lane05.OnClick = () => 
        {
            if(this._Moving)
                return;
                
            this._MoveBlock(4);
        };
    }

    protected override void _OnUpdate()
    {

    }

    // --------------------
    // ブロックPos情報生成
    // --------------------
    Transform[,] _CreateBlockPos()
    {
        Transform blockPosParent;
        Transform[,] blockPoss = new Transform[LANE_NUM, BLOCK_NUM];

        blockPosParent = this._ViewModel.Lane01.transform.Find("Blocks");
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            blockPoss[0,i] = blockPosParent.GetChild(i);
        }

        blockPosParent = this._ViewModel.Lane02.transform.Find("Blocks");
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            blockPoss[1,i] = blockPosParent.GetChild(i);
        }

        blockPosParent = this._ViewModel.Lane03.transform.Find("Blocks");
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            blockPoss[2,i] = blockPosParent.GetChild(i);
        }

        blockPosParent = this._ViewModel.Lane04.transform.Find("Blocks");
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            blockPoss[3,i] = blockPosParent.GetChild(i);
        }

        blockPosParent = this._ViewModel.Lane05.transform.Find("Blocks");
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            blockPoss[4,i] = blockPosParent.GetChild(i);
        }

        return blockPoss;
    }

    // --------------------
    // ブロックを動かす
    // --------------------
    async Task _MoveBlock(int laneIndex)
    {
        Debug.Log("Move開始");

        int index = -1;
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            if(this._LaneData[laneIndex,i] == null)
             {
                index = i;
                break;
             }   
        }

        // レーンが埋まっていた場合
        if(index == -1)
            return;

        this._Moving = true;

        var endPos = this._BlockPoss[laneIndex, index].position;
        var startPos = this._Block.transform.position;
        startPos.x = endPos.x;
        this._Block.transform.position = startPos;
        await this._Block.transform.DOMoveY(endPos.y, 0.5f).AsyncWaitForCompletion();

        this._EndMoveBlock(laneIndex, index);
        this._Moving = false;
        Debug.Log("Move終了");
    }

    // --------------------
    // ブロック動作後の処理
    // --------------------
    void _EndMoveBlock(int laneIndex, int index)
    {
        this._LaneData[laneIndex,index] = this._Block;
        this._InstantBlock();
    }

    void _InstantBlock()
    {

        var block = GameObject.Instantiate(this._ViewModel.Block2, this._BlockPrefab.transform.position, Quaternion.identity, this._Area.transform);
        block.SetActive(true);
        this._Block = block.GetComponent<Block>();
    }
}
