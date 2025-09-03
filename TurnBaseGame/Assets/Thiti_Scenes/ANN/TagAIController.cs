using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TagAIController : MonoBehaviour
{
    public enum States
    {
        MoveTowards,
        MoveAway,
        StandStill
    }

    public States currentState;
    public Transform opponent;
    public bool isChaser = true;

    [Header("Input Settings")]
    public InputAction toggleRoleAction;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 10f;
    public float staminaRegenRate = 15f;
    public float currentStamina;
    public float lowStaminaThreshold = 30f;

    private ANN net;
    private Rigidbody rb;
    public float moveSpeed = 5f;

    void Start()
    {
        net = new ANN(3, 4, 3); // Keep original 3-input architecture
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;

        toggleRoleAction.performed += ctx => ToggleRole();
        toggleRoleAction.Enable();

        Debug.Log($"Initialized as {(isChaser ? "CHASER" : "RUNNER")}. Press configured key to toggle role.");
    }

    void OnEnable()
    {
        toggleRoleAction?.Enable();
    }

    void OnDisable()
    {
        toggleRoleAction?.Disable();
    }

    void FixedUpdate()
    {
        if (opponent == null) return;

        // Inputs (keep original 3 inputs for ANN)
        Vector3 toOpponent = opponent.position - transform.position;
        toOpponent.y = 0f;

        float distance = Mathf.Clamp01(toOpponent.magnitude / 20f);
        float angle = Vector3.Dot(transform.forward.normalized, toOpponent.normalized);
        float role = isChaser ? 1f : 0f;

        List<float> inputs = new List<float> { distance, angle, role };

        // ANN Decision (unchanged architecture)
        List<float> outputs = net.Forward(inputs);
        int decision = GetBestAction(outputs);

        Debug.Log($"Output count: {outputs.Count}");

        // Rule-based fallback layer
        if (isChaser)
        {
            if (distance < 0.4f) decision = (int)States.MoveTowards;
        }
        else
        {
            if (distance < 0.4f && decision == (int)States.StandStill)
                decision = (int)States.MoveAway;
        }

        // STAMINA-BASED OVERRIDE (NEW)
        // If stamina is too low, force StandStill to recover
        if (currentStamina < lowStaminaThreshold &&
           (decision == (int)States.MoveTowards || decision == (int)States.MoveAway))
        {
            decision = (int)States.StandStill;
            Debug.Log("Low stamina! Forcing rest.");
        }

        currentState = (States)decision;

        Debug.Log($"Outputs: {string.Join(", ", outputs.Select(p => p.ToString("F2")))} Decision: {decision}");
        Debug.Log($"Final Decision: {currentState}, Stamina: {currentStamina:F1}");

        // Update stamina based on current state
        UpdateStamina();

        // Apply stamina factor to movement speed
        float staminaFactor = Mathf.Lerp(0.4f, 1f, currentStamina / maxStamina);

        Vector3 move = Vector3.zero;
        switch (currentState)
        {
            case States.MoveTowards:
                move = toOpponent.normalized;
                Debug.Log("MoveTowards");
                break;
            case States.MoveAway:
                move = -toOpponent.normalized;
                Debug.Log("MoveAway");
                break;
            case States.StandStill:
                move = Vector3.zero;
                Debug.Log("StandStill");
                break;
        }

        rb.linearVelocity = new Vector3(
            move.x * moveSpeed * staminaFactor,
            rb.linearVelocity.y,
            move.z * moveSpeed * staminaFactor
        );
    }

    private void UpdateStamina()
    {
        // Update stamina based on current action
        if (currentState == States.MoveTowards || currentState == States.MoveAway)
        {
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        }
        else
        {
            currentStamina += staminaRegenRate * Time.fixedDeltaTime;
        }

        // Clamp stamina between 0 and max
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    private int GetBestAction(List<float> outputs)
    {
        int bestIndex = 0;
        float bestValue = outputs[0];
        for (int i = 1; i < outputs.Count; i++)
        {
            if (outputs[i] > bestValue)
            {
                bestValue = outputs[i];
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    private void ToggleRole()
    {
        isChaser = !isChaser;
        Debug.Log($"Role changed to {(isChaser ? "CHASER" : "RUNNER")}");
    }

    // Optional: Visual feedback in scene view
    void OnDrawGizmos()
    {
        // Draw stamina bar above agent
        if (Application.isPlaying)
        {
            Vector3 barPosition = transform.position + Vector3.up * 2f;
            float barWidth = 2f;
            float barHeight = 0.2f;

            // Background bar
            Gizmos.color = Color.gray;
            Gizmos.DrawCube(barPosition, new Vector3(barWidth, barHeight, 0.1f));

            // Stamina fill
            Gizmos.color = Color.Lerp(Color.red, Color.green, currentStamina / maxStamina);
            float fillWidth = barWidth * (currentStamina / maxStamina);
            Gizmos.DrawCube(
                barPosition - new Vector3((barWidth - fillWidth) / 2f, 0f, 0f),
                new Vector3(fillWidth, barHeight, 0.1f)
            );
        }
    }
}