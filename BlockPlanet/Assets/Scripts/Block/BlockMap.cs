using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �u���b�N�̎O�����z��
/// </summary>
public class BlockMap
{
    /// <summary>
    /// �u���b�N�̏��
    /// </summary>
    protected class BlockInfo
    {
        public bool isSurround = false;
        public bool isEnable = false;
        public MeshRenderer renderer = null;
        public MeshFilter meshFilter = null;
        public BoxCollider collider = null;
        public BlockNumber blockNumber = null;
        public CombineInstance cmesh = new CombineInstance();
        public int materialNumber = 0;
    }
    //�T�C�Y���̃}�b�v��p�ӂ���
    protected BlockInfo[,,] blockArray = new BlockInfo[BlockMapSize.LineN, BlockMapSize.RowN, BlockMapSize.HeightN];
    bool isInit = false;
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
    /// Renderer�̍X�V
    /// </summary>
    public void BlockRendererUpdate()
    {
        for (int i = 1; i < blockArray.GetLength(0) - 1; ++i)
        {
            for (int j = 1; j < blockArray.GetLength(1) - 1; ++j)
            {
                for (int k = 1; k < blockArray.GetLength(2) - 1; ++k)
                {
                    //�͂܂�Ă�����Renderer��Off�ɂ���
                    if (IsSurround(i, j, k))
                    {
                        blockArray[i, j, k].renderer.enabled = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Physics��Off�ɂ���
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
    /// renderer��Off�ɂ���
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
    /// �u���b�N�̃Z�b�g
    /// </summary>
    /// <param name="line">�c</param>
    /// <param name="row">��</param>
    /// <param name="height">����</param>
    /// <param name="block">�u���b�N�̃I�u�W�F�N�g</param>
    public void SetBlock(int line, int row, int height, GameObject block)
    {
        if (!isInit) Initialize();
        //�͈͊O���ǂ����`�F�b�N����
        if (!RangeCheck(line, row, height))
        {
            Debug.LogError("�͈͊O");
            return;
        }
        if (block == null) return;
        //�e�����N���X�Ɋi�[
        blockArray[line, row, height].isEnable = true;
        blockArray[line, row, height].renderer = block.GetComponent<MeshRenderer>();
        blockArray[line, row, height].meshFilter = block.GetComponent<MeshFilter>();
        blockArray[line, row, height].collider = block.GetComponent<BoxCollider>();
        blockArray[line, row, height].blockNumber = block.GetComponent<BlockNumber>();
        blockArray[line, row, height].cmesh.transform = block.transform.localToWorldMatrix;
        blockArray[line, row, height].cmesh.mesh = MakeOptimizeCube(blockArray[line, row, height].meshFilter, row);
        blockArray[line, row, height].materialNumber =
            BlockCreater.GetInstance().GetMaterialNumber(blockArray[line, row, height].renderer.sharedMaterial);
        blockArray[line, row, height].blockNumber.SetNum(line, row, height);
    }

    /// <summary>
    /// �u���b�N����ꂽ�Ƃ��Ɏ��s����
    /// </summary>
    /// <param name="blockNum">�u���b�N�̔ԍ�</param>
    public virtual void BreakBlock(BlockNumber blockNum)
    {
        blockArray[blockNum.line, blockNum.row, blockNum.height].isEnable = false;

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
    /// �͂ݔ���
    /// </summary>
    /// <param name="line">�c</param>
    /// <param name="row">��</param>
    /// <param name="height">����</param>
    /// <returns>�͂܂�Ă��邩�ǂ���</returns>
    bool IsSurround(int line, int row, int height)
    {
        //�͈͊O�`�F�b�N
        if (line == 0 || row == 0 || height == 0 ||
        line == blockArray.GetLength(0) - 1 ||
        row == blockArray.GetLength(1) - 1 ||
        height == blockArray.GetLength(2) - 1)
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

    bool RangeCheck(int line, int row, int height)
    {
        return line >= 0 && line < blockArray.GetLength(0) &&
        row >= 0 && row < blockArray.GetLength(1) &&
        height >= 0 && height < blockArray.GetLength(2);
    }

    protected virtual Mesh MakeOptimizeCube(MeshFilter filter, int row)
    {
        return filter.sharedMesh;
    }
}
