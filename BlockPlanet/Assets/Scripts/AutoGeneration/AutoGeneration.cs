﻿using UnityEngine;

/// <summary>
/// 自動でマップを生成する
/// </summary>
static public class AutoGeneration
{
    static int[,] blockArray = new int[BlockCreater.line_n, BlockCreater.row_n];
    static int[,] oneQuaterBlockArray = new int[BlockCreater.line_n / 2, BlockCreater.row_n / 2];
    static bool isInitialize = false;
    static public int[,] Generate(int maxHeightDiff, float sameHeightPercent)
    {
        int randomPlayerPositionRandomHeight = Random.Range(1, 8);
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                oneQuaterBlockArray[i, j] = randomPlayerPositionRandomHeight;
            }
        }
        //プレイヤーの位置
        oneQuaterBlockArray[1, 1] += 100;
        //ランダムにブロックを配置する
        for (int i = 0; i < oneQuaterBlockArray.GetLength(0); ++i)
        {
            for (int j = 0; j < oneQuaterBlockArray.GetLength(1); ++j)
            {
                //初期に設定したところは何もしない
                if (i == 0 && (j == 0 || j == 1 || j == 2) ||
                i == 1 && (j == 0 || j == 1 || j == 2) ||
                i == 2 && (j == 0 || j == 1 || j == 2)) continue;
                //高さの合計
                int heightSum = 0;
                //壊れないブロックの合計
                int strongNum = 0;
                int blockCount = 0;
                if (i != 0)
                {
                    heightSum += oneQuaterBlockArray[i - 1, j] % 10;
                    if (oneQuaterBlockArray[i - 1, j] / 10 > 0)
                    {
                        ++strongNum;
                    }
                    ++blockCount;
                }
                if (j != 0)
                {
                    heightSum += oneQuaterBlockArray[i, j - 1] % 10;
                    if (oneQuaterBlockArray[i, j - 1] / 10 > 0)
                    {
                        ++strongNum;
                    }
                    ++blockCount;
                }
                if (i != 0 && j != 0)
                {
                    heightSum += oneQuaterBlockArray[i - 1, j - 1] % 10;
                    if (oneQuaterBlockArray[i - 1, j - 1] / 10 > 0)
                    {
                        ++strongNum;
                    }
                    ++blockCount;
                }
                //平均の高さにする
                if (Random.Range(0.0f, 1.0f) <= sameHeightPercent)
                {
                    oneQuaterBlockArray[i, j] = Mathf.RoundToInt(1.0f * heightSum / blockCount);
                }
                //平均の値から指定の範囲内に収まるように高さを設定する
                else
                {
                    oneQuaterBlockArray[i, j] = Mathf.Clamp(Random.Range(-maxHeightDiff, maxHeightDiff + 1) + heightSum / blockCount, 0, 7);
                }
                if (oneQuaterBlockArray[i, j] == 0) continue;
                //壊れないブロック
                if (Random.Range(0.0f, 1.0f) < 0.2f + 0.5f * strongNum / blockCount)
                {
                    oneQuaterBlockArray[i, j] += 10;
                }
            }
        }
        //四分の一をコピー
        for (int i = 0; i < oneQuaterBlockArray.GetLength(0); ++i)
        {
            for (int j = 0; j < oneQuaterBlockArray.GetLength(1); ++j)
            {
                blockArray[i, j] = oneQuaterBlockArray[i, j];
                blockArray[i, BlockCreater.row_n - j - 1] =
                    oneQuaterBlockArray[i, j] + ((oneQuaterBlockArray[i, j] > 100) ? 100 : 0);
                blockArray[BlockCreater.line_n - i - 1, j] =
                    oneQuaterBlockArray[i, j] + ((oneQuaterBlockArray[i, j] > 100) ? 200 : 0);
                blockArray[BlockCreater.line_n - i - 1, BlockCreater.row_n - j - 1] =
                    oneQuaterBlockArray[i, j] + ((oneQuaterBlockArray[i, j] > 100) ? 300 : 0);
            }
        }

        return blockArray;
    }
}
