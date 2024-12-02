using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DummyDialogue : MonoBehaviour
{
    public int health = 100;

    public PlayableDirector playableDirector;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        MovementControl player = FindObjectOfType<MovementControl>();
        if (player != null)
        {
            player.ToggleMovement(false);
        }

        // Play the timeline
        if (playableDirector != null)
        {
            playableDirector.Play();
            playableDirector.stopped += OnTimelineEnd;
        }

        Destroy(gameObject);
    }

    void OnTimelineEnd(PlayableDirector director)
    {
        MovementControl player = FindObjectOfType<MovementControl>();
        if (player != null)
        {
            player.ToggleMovement(true);
        }

        director.stopped -= OnTimelineEnd;
    }
}
