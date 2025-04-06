using UnityEngine;
using NaughtyAttributes;

public class LeverInputControl : MonoBehaviour
{
    [BoxGroup("Input Settings")]
    [Tooltip("Имя входной оси, например 'Horizontal' или 'Vertical'")]
    [Dropdown("GetInputAxisOptions")]
    public string inputAxis = "Horizontal";

    [BoxGroup("Movement Settings")]
    [Tooltip("Направление движения рычага (например, Vector3.forward для движения вперед-назад)")]
    public Vector3 movementAxis = Vector3.forward;

    [BoxGroup("Movement Settings")]
    [Tooltip("Максимальное смещение рычага от исходной позиции")]
    [Range(0.1f, 5.0f)]
    public float maxOffset = 0.5f;

    [BoxGroup("Movement Settings")]
    [Tooltip("Скорость перемещения рычага")]
    [Range(0.1f, 5.0f)]
    public float movementSpeed = 1.0f;

    [BoxGroup("Output")]
    [ReadOnly]
    [Tooltip("Нормализованное значение рычага (от -1 до 1)")]
    public float leverValue;

    // Исходная позиция рычага
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Получаем значение осевого ввода (от -1 до 1)
        float inputValue = Input.GetAxis(inputAxis);
        // Рассчитываем смещение за кадр с учетом скорости и времени
        float offset = inputValue * movementSpeed * Time.deltaTime;

        // Текущее смещение вдоль заданной оси
        float currentOffset = Vector3.Dot((transform.position - startPosition), movementAxis.normalized);
        // Вычисляем новое смещение, ограниченное maxOffset
        float newOffset = Mathf.Clamp(currentOffset + offset, -maxOffset, maxOffset);

        // Обновляем позицию рычага
        transform.position = startPosition + movementAxis.normalized * newOffset;

        // Вычисляем нормализованное значение рычага
        leverValue = newOffset / maxOffset;
        // Если нужен диапазон от 0 до 1, используйте:
        // leverValue = (newOffset + maxOffset) / (2 * maxOffset);
    }

    [ Button("Reset Lever Position")]
    private void ResetLever()
    {
        transform.position = startPosition;
        leverValue = 0f;
    }

    // Метод для заполнения выпадающего списка для inputAxis
    private string[] GetInputAxisOptions()
    {
        return new string[] { "Horizontal", "Vertical", "Mouse X", "Mouse Y" };
    }
}
