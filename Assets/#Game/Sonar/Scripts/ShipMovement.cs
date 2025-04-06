using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ShipMovement : MonoBehaviour
{
    public Canvas directionsUI;
    public Image[] directions; // 0 - вниз, 1 - вверх, 2 - вправо, 3 - влево
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float maxVelocity = 10f;
    public float drag = 1f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private CanvasGroup[] canvasGroups;
    private float flashSpeed = 4f;
    private float flashAlphaMin = 0.3f;
    private float flashAlphaMax = 1f;

    void Start()
    {
        directions = directionsUI.GetComponentsInChildren<Image>();
        canvasGroups = new CanvasGroup[directions.Length];

        for (int i = 0; i < directions.Length; i++)
        {
            var cg = directions[i].GetComponent<CanvasGroup>();
            if (!cg) cg = directions[i].gameObject.AddComponent<CanvasGroup>();
            canvasGroups[i] = cg;
            directions[i].gameObject.SetActive(false);
        }

        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.z = 0;
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            rb.AddForce(moveInput * acceleration, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxVelocity)
                rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, drag * Time.fixedDeltaTime);
        }

        UpdateDirectionIndicators(rb.linearVelocity);
    }

    void UpdateDirectionIndicators(Vector3 velocity)
    {
        float threshold = 0.2f;
        bool down = velocity.y < -threshold;
        bool up = velocity.y > threshold;
        bool right = velocity.x > threshold;
        bool left = velocity.x < -threshold;

        float speed = velocity.magnitude;
        float flashFrequency = 5f;
        float appearSpeed = 5f; // скорость плавного появления
        float disappearThreshold = 0.05f;

        bool blink = speed < 1f && Mathf.Sin(Time.time * flashFrequency * Mathf.PI * 2f) > 0;

        for (int i = 0; i < directions.Length; i++)
        {
            bool shouldBeVisible = (i == 0 && down) || (i == 1 && up) || (i == 2 && right) || (i == 3 && left);
            Image img = directions[i];
            CanvasGroup cg = canvasGroups[i];

            if (shouldBeVisible)
            {
                if (!img.gameObject.activeSelf)
                    img.gameObject.SetActive(true);

                float targetAlpha = 1f;

                if (speed < 1f)
                    targetAlpha = blink ? 1f : 0f;

                // Плавно переходим к нужной альфе
                cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, appearSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // Плавно исчезает
                cg.alpha = Mathf.MoveTowards(cg.alpha, 0f, appearSpeed * Time.fixedDeltaTime);

                if (cg.alpha <= disappearThreshold)
                    img.gameObject.SetActive(false);
            }
        }
    }

}


