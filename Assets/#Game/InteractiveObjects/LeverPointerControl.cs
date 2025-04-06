using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.InteractiveObjects
{
    public class LeverPointerControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        // Максимальное смещение рычага от центра по оси движения
        public float maxOffset = 0.5f;
        // Ось движения рычага (например, вперед-назад)
        public Vector3 movementAxis = Vector3.forward;

        // Нормализованное значение рычага: от -1 до 1
       [field: ShowNonSerializedField] public float leverValue { get; private set; }

        private Vector3 startPosition;
        private Camera cam;
    
        void Start()
        {
            // Запоминаем исходное положение
            startPosition = transform.position;
            // Получаем основную камеру (убедитесь, что она установлена как Main Camera)
            leverValue = 0.5f;

            cam = Camera.main;
        }

        // Обработка нажатия
        public void OnPointerDown(PointerEventData eventData)
        {
            // Можно добавить дополнительную логику при начале взаимодействия
        }

        // Обработка перетаскивания
        public void OnDrag(PointerEventData eventData)
        {
            // Преобразуем позицию указателя в мировые координаты.
            // Для этого задаём корректное значение z (расстояние от камеры до рычага)
            Vector3 pointerPos = eventData.position;
            pointerPos.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
            Vector3 worldPos = cam.ScreenToWorldPoint(pointerPos);
            // Вычисляем смещение относительно исходной позиции
            Vector3 displacement = worldPos - startPosition;
            // Проецируем смещение на выбранную ось движения
            float moveAmount = Vector3.Dot(displacement, movementAxis.normalized);
            
            Debug.Log("ON Drag move Amount: " + moveAmount);

            
            // Ограничиваем смещение в пределах допустимого диапазона
            moveAmount = Mathf.Clamp(moveAmount, -maxOffset, maxOffset);
            Debug.Log("ON Drag move Amount Clamped: " + moveAmount);

            // Обновляем позицию рычага
            transform.position = startPosition + movementAxis.normalized * moveAmount;

            // Вычисляем нормализованное значение:
            // Для диапазона от -1 до 1:
            leverValue = (moveAmount + maxOffset) / (2 * maxOffset);
            // Если нужен диапазон от 0 до 1, то можно использовать:
            // leverValue = (moveAmount + maxOffset) / (2 * maxOffset);
        }

        // Обработка отпускания указателя
        public void OnPointerUp(PointerEventData eventData)
        {
            // Если требуется вернуть рычаг в исходное положение, можно добавить анимацию возврата
        }
    }
}
