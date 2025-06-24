using UnityEngine;
using System.Collections;

public class PlayerFightLogic : MonoBehaviour
{
    public Animator animator;
    public void PlayThrowAnimation()
    {
        animator.SetTrigger("Throw");
        StartCoroutine(TempNotHatProjectile());
    }

    public void PlayKickAnimation()
    {
        float kickLength = GetAnimationClipLength("Kick");
        StartCoroutine(RotateKickRoutine(kickLength));
    }

    private IEnumerator RotateKickRoutine(float duration)
    {
        transform.Rotate(0, -90, 0);

        animator.SetTrigger("Kick");
        Debug.Log("Player Kick Animation");

        yield return new WaitForSeconds(duration);

        transform.Rotate(0, 90, 0);
    }

    private float GetAnimationClipLength(string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        Debug.LogWarning($"Animation clip '{clipName}' not found!");
        return 1f; // fallback duration
    }

    public IEnumerator TempNotHatProjectile()
    {
        yield return new WaitForSeconds(0.8f);
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.parent = transform;
        projectile.transform.localScale = new Vector3(0.5f, 0.25f,0.5f);
        projectile.transform.position = transform.position + new Vector3(1, 0.800000012f, 0);

        projectile.AddComponent<HatProjectile>();
        projectile.GetComponent<HatProjectile>().BasicAction(FindEnemyInSlot(1) + new Vector3(0,1.8f,0));

    }

    Vector3 FindEnemyInSlot(int slot)
    {
        // Find the GameObject named "Enemy Positions" in the scene
        GameObject enemyPositions = GameObject.Find("Enemy Positions");

        if (enemyPositions == null)
        {
            Debug.LogWarning("Enemy Positions GameObject not found!");
            return Vector3.zero;
        }

        // Check if the requested slot index is valid
        if (slot < 0 || slot >= enemyPositions.transform.childCount)
        {
            Debug.LogWarning("Invalid enemy slot index: " + slot);
            return Vector3.zero;
        }

        // Return the child at the given slot
        return enemyPositions.transform.GetChild(slot-1).position;
    }

}
