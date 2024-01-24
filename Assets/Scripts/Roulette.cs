using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class Roulette : MonoBehaviour
{
    [SerializeField]
    private Transform piecePrefab;  // 룰렛에 표시되는 정보 프리팹
    [SerializeField]
    private Transform linePrefab;   // 정보들을 구분하는 선
    [SerializeField]
    private Transform pieceParent;  // 정보 프리팹들의 부모
    [SerializeField]
    private Transform lineParent;   // 선들의 부모
    [SerializeField]
    private RoulettePieceData[] roulettePieceData;  // 룰렛에 표시될 정보들

    [SerializeField]
    private int spinDuration;   // 회전시간
    [SerializeField]
    private Transform spinningRoulette; // 실제 회전하는 회전판 Transform
    [SerializeField]
    private AnimationCurve spinningCurve;   // 회전속도제어 그래프
    
    private float pieceAngle;   // 정보하나가 배치되는 각도
    private float halfPieceAngle;   // 정보하나가 배치되는 각도의 절반크기
    private float halfPieceAngleWithPaddings; // 선의 굵기를 고려한 Padding이 포함된 절반크기

    private int accumulatedWeight;  // 가중치 누적값
    private bool isSpinning = false;    // 현재 회전중인지
    private int selectedIndex = 0;  // 룰렛에서 선택된 아이템

    private void Awake()
    {
        this.pieceAngle = 360 / roulettePieceData.Length;
        this.halfPieceAngle = this.pieceAngle * 0.5f;
        this.halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle * 0.25f);

        this.SpawnPiecesAndLines();
        this.CalculateWeightsAndIndices();
        
        // Debug.Log($"Index : {GetRandomIndex()}");
    }

    private void SpawnPiecesAndLines()
    {
        Vector3 pieceParentPos = pieceParent.position;
        Vector3 lineParentPos = lineParent.position;
        
        for (int i = 0; i < roulettePieceData.Length; i++)
        {
            Transform piece = Instantiate(piecePrefab, pieceParentPos, Quaternion.identity, pieceParent);
            piece.GetComponent<RoulettePiece>().Setup(roulettePieceData[i]);
            piece.RotateAround(pieceParentPos, Vector3.back, (pieceAngle * i));
            
            Transform line = Instantiate(linePrefab, lineParentPos, Quaternion.identity, lineParent);
            line.RotateAround(lineParentPos, Vector3.back, (pieceAngle * i) + halfPieceAngle);
        }
    }

    // 가중치와 인덱스 계산
    private void CalculateWeightsAndIndices()
    {
        for (int i = 0; i < roulettePieceData.Length; i++)
        {
            roulettePieceData[i].index = i;

            if (roulettePieceData[i].chance <= 0)
            {
                roulettePieceData[i].chance = 1;
            }

            accumulatedWeight += roulettePieceData[i].chance;
            roulettePieceData[i].weight = accumulatedWeight;
            
            Debug.Log($"({roulettePieceData[i].index}){roulettePieceData[i].description}:{roulettePieceData[i].weight}");
        }
    }

    private int GetRandomIndex()
    {
        int weight = UnityEngine.Random.Range(0, accumulatedWeight);
        for (int i = 0; i < roulettePieceData.Length; i++)
        {
            if (roulettePieceData[i].weight > weight)
            {
                return i;
            }
        }

        return 0;
    }

    public void Spin(UnityAction<RoulettePieceData> action = null)
    {
        if (isSpinning)
            return;

        this.selectedIndex = GetRandomIndex();

        // 선택된 결과의 중심각도
        float angle = pieceAngle * selectedIndex;
        
        // 정확하게 중심이 아니라 결과값 범위 안의 임의의 각도 선택
        float leftOffset = (angle - halfPieceAngleWithPaddings) % 360;
        float rightOffset = (angle + halfPieceAngleWithPaddings) % 360;
        float randomAngle = Random.Range(leftOffset, rightOffset);

        // 목표 각도 = 결과각도 + 360 * 회전시간 * 회전속도
        int rotateSpeed = 2;
        float targetAngle = (randomAngle + 360 * spinDuration * rotateSpeed);
        
        Debug.Log($"SelectedIndex:{selectedIndex}, Angle:{angle}");
        Debug.Log($"left/right/random: {leftOffset}/{rightOffset}/{randomAngle}");
        Debug.Log($"targetAngle: {targetAngle}");

        this.isSpinning = true;
        StartCoroutine(OnSpin(targetAngle, action));
    }
    
    private IEnumerator OnSpin(float end, UnityAction<RoulettePieceData> action)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / spinDuration;

            float z = Mathf.Lerp(0, end, spinningCurve.Evaluate(percent));
            spinningRoulette.rotation = Quaternion.Euler(0,0,z);

            yield return null;
        }

        isSpinning = false;
        
        action?.Invoke(roulettePieceData[selectedIndex]);
    }
}
