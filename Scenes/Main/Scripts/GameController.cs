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

    [SerializeField]
    GameObject _BlockPrefab;

    [SerializeField]
    GameObject _Area;

    [SerializeField]
    Transform[] _BlockPoss;
    int _PosIndex;

    int[,] _LaneData = new int[5,6];

    // 生成ブロック
    GameObject _Block;

    protected override void _OnStart()
    {
        this._LaneData = new int[,]{
            {0,0,0,0,0,0},
            {0,0,0,0,0,0},
            {0,0,0,0,0,0},
            {0,0,0,0,0,0},
            {0,0,0,0,0,0},
            };

        this._BlockPrefab.SetActive(false);
        this._Block = GameObject.Instantiate(this._BlockPrefab, this._BlockPrefab.transform.position, Quaternion.identity, this._Area.transform);
        this._Block.SetActive(true);


        this._ViewModel.Lane01.OnClick = () => 
        {
            Debug.Log("クリック");
            this._MoveBlock();
        };
    }

    protected override void _OnUpdate()
    {

    }

    // --------------------
    // ブロックを動かす
    // --------------------
    async Task _MoveBlock()
    {
        Debug.Log("Move開始");

        int index = -1;
        for(int i = 0; i < LANE_NUM; i++)
        {
            if(this._LaneData[0,i] == 0)
             {
                index = i;
                break;
             }   
        }

        // レーンが埋まっていた場合
        if(index == -1)
            return;

        var endPos = this._BlockPoss[index].position.y;
        await this._Block.transform.DOMoveY(endPos, 0.5f).AsyncWaitForCompletion();

        this._EndMoveBlock(index);
        Debug.Log("Move終了");
    }

    // --------------------
    // ブロック動作後の処理
    // --------------------
    void _EndMoveBlock(int index)
    {
        this._LaneData[0,index] = 1;

        this._Block = GameObject.Instantiate(this._BlockPrefab, this._BlockPrefab.transform.position, Quaternion.identity, this._Area.transform);
        this._Block.SetActive(true);
    }
}
