using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    const float SPD = 15f;

    [SerializeField]
    TextMeshProUGUI _Label;

    public int Number;
    public BlockState State = BlockState.Move;

    public enum BlockState
    {
        Stop = 1,
        Move = 2,
    }

    // Start is called before the first frame update
    void Start()
    {
        this._Label.text = this.Number.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // --------------------
    // ブロックを動かす
    // 最終地点に到達したら true を返す
    // --------------------
    public bool Move(Vector3 endPos)
    {
        var pos = this.transform.position;
        pos.y += SPD;
        this.transform.position = pos;

        // 最終地点に到達
        if(this.transform.position.y >= endPos.y)
        {
            this.State = BlockState.Stop;
            this.transform.position = endPos;
            return true;
        }

        return false;
    }

    // --------------------
    // マージ実行
    // --------------------
    public void Merge(int mergeNum)
    {
        this.Number = this.Number * (int)Mathf.Pow(2, mergeNum);
        Debug.Log("マージ番号 = " + this.Number);
        this._Label.text = this.Number.ToString();
        this.State = BlockState.Move;
    }
}
