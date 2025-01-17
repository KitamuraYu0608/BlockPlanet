using UnityEngine;

/// <summary>
/// ブロックの三次元配列
/// </summary>
public class BlockMap
{
    /// <summary>
    /// ブロックの情報
    /// </summary>
    protected class BlockInfo
    {
        //囲まれているかどうか
        public bool isSurround = false;
        //存在するかどうか
        public bool isEnable = false;
        public MeshRenderer renderer = null;
        public MeshFilter meshFilter = null;
        public BoxCollider collider = null;
        public BlockNumber blockNumber = null;
        public CombineInstance cmesh = new CombineInstance();
        //マテリアルの番号
        public int materialNumber = 0;
    }
    //サイズ分のマップを用意する
    protected BlockInfo[,,] blockArray = new BlockInfo[BlockMapSize.LineN, BlockMapSize.RowN, BlockMapSize.HeightN];
    //初期化したかどうか
    bool isInit = false;

    /// <summary>
    /// 初期化
    /// </summary>
    void Initialize()
    {
        for (int i = 0; i < blockArray.GetLength(0); ++i)
        {
            for (int j = 0; j < blockArray.GetLength(1); ++j)
            {
                for (int k = 0; k < blockArray.GetLength(2); ++k)
                {
                    blockArray[i, j, k] = new BlockInfo();
                }
            }
        }
        isInit = true;
    }

    /// <summary>
    /// Rendererの更新
    /// </summary>
    public void BlockRendererUpdate()
    {
        for (int i = 1; i < blockArray.GetLength(0) - 1; ++i)
        {
            for (int j = 1; j < blockArray.GetLength(1) - 1; ++j)
            {
                for (int k = 1; k < blockArray.GetLength(2) - 1; ++k)
                {
                    //囲まれていたらRendererをOffにする
                    if (IsSurround(i, j, k))
                    {
                        blockArray[i, j, k].renderer.enabled = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// PhysicsをOffにする
    /// </summary>
    public void BlockPhysicsOff()
    {
        foreach (var block in blockArray)
        {
            if (!block.isEnable) continue;
            block.collider.enabled = false;
        }
    }

    /// <summary>
    /// rendererをOffにする
    /// </summary>
    public void BlockRendererOff()
    {
        foreach (var block in blockArray)
        {
            if (!block.isEnable) continue;
            block.renderer.enabled = false;
        }
    }

    /// <summary>
    /// ブロックのセット
    /// </summary>
    /// <param name="line">縦</param>
    /// <param name="row">横</param>
    /// <param name="height">高さ</param>
    /// <param name="block">ブロックのオブジェクト</param>
    public void SetBlock(int line, int row, int height, GameObject block)
    {
        if (!isInit) Initialize();
        //範囲外かどうかチェックする
        if (!RangeCheck(line, row, height))
        {
            Debug.LogError("範囲外");
            return;
        }
        if (block == null) return;
        //各情報をクラスに格納
        blockArray[line, row, height].isEnable = true;
        blockArray[line, row, height].renderer = block.GetComponent<MeshRenderer>();
        blockArray[line, row, height].meshFilter = block.GetComponent<MeshFilter>();
        blockArray[line, row, height].collider = block.GetComponent<BoxCollider>();
        blockArray[line, row, height].blockNumber = block.GetComponent<BlockNumber>();
        blockArray[line, row, height].cmesh.transform = block.transform.localToWorldMatrix;
        //最適化したキューブをセット
        blockArray[line, row, height].cmesh.mesh = MakeOptimizeCube(blockArray[line, row, height].meshFilter, row);
        blockArray[line, row, height].materialNumber =
            BlockCreater.GetInstance().GetMaterialNumber(blockArray[line, row, height].renderer.sharedMaterial);
        //ブロックの番号をセット
        blockArray[line, row, height].blockNumber.SetNum(line, row, height);
    }

    /// <summary>
    /// ブロックが壊れたときに実行する
    /// </summary>
    /// <param name="blockNum">ブロックの番号</param>
    public virtual void BreakBlock(BlockNumber blockNum)
    {
        //壊れた場所はisEnableをfalse
        blockArray[blockNum.line, blockNum.row, blockNum.height].isEnable = false;

        //上下左右前後のブロックのrendererをOnにする

        if (blockNum.line < blockArray.GetLength(0) - 1 &&
            blockArray[blockNum.line + 1, blockNum.row, blockNum.height].renderer)
        {
            blockArray[blockNum.line + 1, blockNum.row, blockNum.height].renderer.enabled = true;
        }

        if (blockNum.line > 0 &&
            blockArray[blockNum.line - 1, blockNum.row, blockNum.height].renderer)
        {
            blockArray[blockNum.line - 1, blockNum.row, blockNum.height].renderer.enabled = true;
        }

        if (blockNum.row < blockArray.GetLength(1) - 1 &&
            blockArray[blockNum.line, blockNum.row + 1, blockNum.height].renderer)
        {
            blockArray[blockNum.line, blockNum.row + 1, blockNum.height].renderer.enabled = true;
        }

        if (blockNum.row > 0 &&
            blockArray[blockNum.line, blockNum.row - 1, blockNum.height].renderer)
        {
            blockArray[blockNum.line, blockNum.row - 1, blockNum.height].renderer.enabled = true;
        }

        if (blockNum.height < blockArray.GetLength(2) - 1 &&
            blockArray[blockNum.line, blockNum.row, blockNum.height + 1].renderer)
        {
            blockArray[blockNum.line, blockNum.row, blockNum.height + 1].renderer.enabled = true;
        }

        if (blockNum.height > 0 &&
            blockArray[blockNum.line, blockNum.row, blockNum.height - 1].renderer)
        {
            blockArray[blockNum.line, blockNum.row, blockNum.height - 1].renderer.enabled = true;
        }
    }

    /// <summary>
    /// 囲み判定
    /// </summary>
    /// <param name="line">縦</param>
    /// <param name="row">横</param>
    /// <param name="height">高さ</param>
    /// <returns>囲まれているかどうか</returns>
    bool IsSurround(int line, int row, int height)
    {
        //範囲外チェック
        if (!RangeCheck(line, row, height))
        {
            return false;
        }

        return blockArray[line + 1, row, height].isEnable &&
        blockArray[line - 1, row, height].isEnable &&
        blockArray[line, row + 1, height].isEnable &&
        blockArray[line, row - 1, height].isEnable &&
        blockArray[line, row, height + 1].isEnable &&
        blockArray[line, row, height - 1].isEnable;
    }

    /// <summary>
    /// 範囲チェック
    /// </summary>
    /// <param name="line">縦</param>
    /// <param name="row">横</param>
    /// <param name="height">高さ</param>
    /// <returns>範囲内ならtrue,範囲外ならfalse</returns>
    bool RangeCheck(int line, int row, int height)
    {
        return line >= 0 && line < blockArray.GetLength(0) &&
        row >= 0 && row < blockArray.GetLength(1) &&
        height >= 0 && height < blockArray.GetLength(2);
    }

    /// <summary>
    /// 最適化したキューブを生成し返す
    /// </summary>
    /// <param name="filter">メッシュを取り出すためのfilter</param>
    /// <param name="row">横の位置によって返すキューブを返す</param>
    /// <returns>最適化したキューブ</returns>
    protected virtual Mesh MakeOptimizeCube(MeshFilter filter, int row)
    {
        return filter.sharedMesh;
    }
}
