using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Lớp này định nghĩa cấu trúc cho một thẻ bài lính.
[System.Serializable]
public class SoldierCard
{
    [Tooltip("ID này BẮT BUỘC phải khớp với ID của lính trong file BuildingData.")]
    public int id;
    public string soldierName;
    public Sprite cardSprite;
}

// Dòng này giúp bạn tạo asset SoldierData từ menu Assets trong Unity.
[CreateAssetMenu(fileName = "SoldierData", menuName = "CityBuilder/Soldier Data")]
public class SoldierData : ScriptableObject
{
    // Danh sách chứa tất cả các thẻ lính.
    public List<SoldierCard> soldierCards = new List<SoldierCard>();

    /// <summary>
    /// Tìm và trả về sprite của thẻ bài dựa vào ID của lính.
    /// </summary>
    /// <param name="id">ID của lính cần tìm.</param>
    /// <returns>Trả về Sprite nếu tìm thấy, ngược lại trả về null.</returns>
    public Sprite GetSpriteById(int id)
    {
        var card = soldierCards.FirstOrDefault(c => c.id == id);
        return card?.cardSprite;
    }
}