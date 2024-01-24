// 룰렛 조각하나의 데이터 정의 스크립트
using System;
using UnityEngine;

[Serializable]
public class RoulettePieceData
{
    public Sprite icon; // 아이콘 이미지 파일
    public string description;  // 이름

    // 3개의 아이템 등장 확률이 100, 60, 40이면
    // 등장확률의 합은 200. 100/200 = 50%, 60/200 = 30%, 40/200 = 20%
    [Range(1,100)]
    public int chance = 100;    // 등장확률

    [HideInInspector]
    public int index;   // 아이템 순번
    
    [HideInInspector]
    public int weight;  // 가중치
}