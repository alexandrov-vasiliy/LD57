using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DepthController : MonoBehaviour
{
    // Ссылка на игрока, чей Y-координата определяет глубину
    public Transform player;

    public float depth;

    // Фоновый элемент (например, Image), у которого будем менять цвет
    public SpriteRenderer background;

    // Цвет на поверхности и на максимальной глубине
    public Color surfaceColor = Color.white;
    public Color deepColor = Color.black;

    // Максимальная глубина, при которой фон становится полностью темным
    public float maxDepth = 1000f;

    void Update()
    {
        // Предположим, что глубина — это абсолютное значение отрицательной координаты Y
         depth = Mathf.Abs(player.position.y);

        // Вычисляем коэффициент интерполяции от 0 до 1
        float t = Mathf.Clamp01(depth / maxDepth);

        // Интерполируем цвет фона: при глубине 0 цвет равен surfaceColor, при глубине maxDepth — deepColor
        background.color = Color.Lerp(surfaceColor, deepColor, t);
    }
}