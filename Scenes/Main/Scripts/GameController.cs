using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameController : ControllerBase<GameViewModel>
{
    [SerializeField]
    GameObject _Block;

    [SerializeField]
    Transform[] _BlockPoss;
    int _PosIndex;

    protected override void _OnStart()
    {
        this._ViewModel.Lane01.OnClick = () => 
        {
            Debug.Log("クリック");

            var endPos = this._BlockPoss[this._PosIndex].position.y;
            this._Block.transform.DOMoveY(endPos, 0.5f);

            this._PosIndex = (this._PosIndex + 1) % this._BlockPoss.Length;
        };
    }

    protected override void _OnUpdate()
    {

    }
}
