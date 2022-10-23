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
                
            this._SetBlock(0);
        };

        this._ViewModel.Lane02.OnClick = () => 
        {
            if(this._Moving)
                return;

            this._SetBlock(1);
        };

        this._ViewModel.Lane03.OnClick = () => 
        {
            if(this._Moving)
                return;
                
            this._SetBlock(2);

        };

        this._ViewModel.Lane04.OnClick = () => 
        {
            if(this._Moving)
                return;
                
            this._SetBlock(3);
        };

        this._ViewModel.Lane05.OnClick = () => 
        {
            if(this._Moving)
                return;

            this._SetBlock(4);
        };
    }

    protected override void _OnUpdate()
    {
        this._Move();
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
    // ブロック配置
    // --------------------
    void _SetBlock(int laneIndex)
    {
        // 対象レーンの一番最後に配置
        var pos = this._BlockPoss[laneIndex, BLOCK_NUM - 1];
        this._Block.transform.position = pos.transform.position;
        this._LaneData[laneIndex,BLOCK_NUM - 1] = this._Block;
        
        // 次のブロックを作成
        this._InstantBlock();
    }

    // --------------------
    // ブロックを動かす
    // --------------------
    void _Move()
    {
        this._Moving = false;

        for(int i = 0; i < LANE_NUM; i++)
        {
            for(int c = 0; c < BLOCK_NUM; c++)
            {
                var block = this._LaneData[i,c];
                if(block == null)
                    continue;

                if(block.State == Block.BlockState.Stop)
                    continue;

                var endIndex = this._GetEndIndex(i);
                if(endIndex == -1)
                    continue;

                var endPos = this._BlockPoss[i, endIndex].transform.position;
                if(block.Move(endPos))
                {
                    // 到達したら情報更新
                    this._LaneData[i, c] = null;
                    this._LaneData[i, endIndex] = block;

                    // 上のブロックと比較
                    if(endIndex != 0)
                    {
                        var upBlock = this._LaneData[i, endIndex - 1];
                        if(upBlock.Number == block.Number)
                        {
                            Debug.Log("同じ数値");
                            block.Merge(1);

                            this._LaneData[i, endIndex - 1] = null;
                            GameObject.Destroy(upBlock.gameObject);
                        }
                    }
                }
                else
                {
                    this._Moving = true;
                }
            }
        }
        //this._ShowLaneData();
    }

    // --------------------
    // ブロック生成
    // --------------------
    void _InstantBlock()
    {
        var block = GameObject.Instantiate(this._ViewModel.Block2, this._BlockPrefab.transform.position, Quaternion.identity, this._Area.transform);
        block.SetActive(true);
        this._Block = block.GetComponent<Block>();
    }

    // --------------------
    // レーン上のブロック最終地点
    // --------------------
    int _GetEndIndex(int laneIndex)
    {
        // 空いている場所のインデックスを取得
        int index = -1;
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            if(this._LaneData[laneIndex,i] == null)
            {
                index = i;
                break;
            }   
        }

        if(index == -1)
            Debug.LogWarning("配置できる場所が無い");

        return index;
    }

    void _ShowLaneData()
    {
        string message = "";

        for(int i = 0; i < BLOCK_NUM; i++)
        {
            for(int c = 0; c < LANE_NUM; c++)
            {
                var block = this._LaneData[c,i];
                int exit = block != null? 1:0;

                message += $"{exit}";
            }

            message += "\n";
        }

        Debug.Log(message);
    }

}
