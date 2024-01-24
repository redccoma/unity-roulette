// 룰렛 조각 제어 스크립트

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoulettePiece : MonoBehaviour
{
    [SerializeField]
    private Image imageIcon;
    [SerializeField]
    private TextMeshProUGUI textDescription;
    
    public void Setup(RoulettePieceData data)
    {
        this.imageIcon.sprite = data.icon;
        this.textDescription.text = data.description;
    }
}