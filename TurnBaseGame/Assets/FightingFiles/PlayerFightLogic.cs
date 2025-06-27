using UnityEngine;
using System.Collections;

public class PlayerFightLogic : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {

    }

    public void PlayThrowAnimation()
    {
        animator.SetTrigger("Throw");
    }

    public void PlayKickAnimation()
    {
        animator.SetTrigger("Kick");
       

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
