using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // 初期ブロック
        this._BlockPrefab.SetActive(false);
        this._InstantBlock();
        
        // 各レーンごとの設定
        ButtonBase[] laneButtons = new ButtonBase[LANE_NUM]
        {
            this._ViewModel.Lane01,
            this._ViewModel.Lane02,
            this._ViewModel.Lane03,
            this._ViewModel.Lane04,
            this._ViewModel.Lane05,
        };
        for(int i = 0; i < laneButtons.Length; i++)
        {
            int index = i;
            var button = laneButtons[index];

            // アクション設定
            button.OnClick = () => 
            {
                if(this._Moving)
                return;
                
                this._SetBlock(index);
            };

            // 配下のブロックポジションを取得
            var blockPosParent = button.transform.Find("Blocks");
            for(int c = 0; c < BLOCK_NUM; c++)
            {
                this._BlockPoss[i,c] = blockPosParent.GetChild(c);
            }
        }
    }

    protected override void _OnUpdate()
    {
        this._Move();
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

                // 今の位置より下には行かない
                if(endIndex >= c)
                    endIndex = c;

                var endPos = this._BlockPoss[i, endIndex].transform.position;
                if(block.Move(endPos))
                {
                    // 到達したら情報更新
                    this._LaneData[i, c] = null;
                    this._LaneData[i, endIndex] = block;

                    // マージチェック
                    int mergeNum = this._CheckMerge(i, endIndex);

                    // 同じ数値の分だけマージ
                    if(mergeNum >= 1)
                        block.Merge(mergeNum);
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

    // --------------------
    // 結合可能かをチェックと対象オブジェクトの破棄
    // 結合可能数を返す
    // --------------------
    int _CheckMerge(int laneIndex, int blockIndex)
    {
        int mergeNum = 0;
        var block = this._LaneData[laneIndex, blockIndex];

        // 上のブロックと比較
        if(blockIndex != 0)
        {
            var upBlock = this._LaneData[laneIndex, blockIndex - 1];
            if(upBlock != null && upBlock.Number == block.Number)
            {
                mergeNum++;
                this._LaneData[laneIndex, blockIndex - 1] = null;
                this._LaneMove(laneIndex);
                GameObject.Destroy(upBlock.gameObject);
            }
        }

        // 右のブロックと比較
        if(laneIndex != LANE_NUM - 1)
        {
            var rightBlock = this._LaneData[laneIndex + 1, blockIndex];
            if(rightBlock != null && rightBlock.Number == block.Number)
            {
                mergeNum++;
                this._LaneData[laneIndex + 1, blockIndex] = null;
                this._LaneMove(laneIndex + 1);
                GameObject.Destroy(rightBlock.gameObject);
            }
        }

        // 左のブロックと比較
        if(laneIndex != 0)
        {
            var leftBlock = this._LaneData[laneIndex -1, blockIndex];
            if(leftBlock != null && leftBlock.Number == block.Number)
            {
                mergeNum++;
                this._LaneData[laneIndex - 1, blockIndex] = null;
                this._LaneMove(laneIndex - 1);
                GameObject.Destroy(leftBlock.gameObject);
            }
        }

        return mergeNum;
    }

    // --------------------
    // 対象レーンのブロックにMoveステータスを付与
    // --------------------
    void _LaneMove(int laneIndex)
    {
        for(int i = 0; i < BLOCK_NUM; i++)
        {
            var block = this._LaneData[laneIndex, i];
            if(block != null)
                block.State = Block.BlockState.Move;
        }
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
