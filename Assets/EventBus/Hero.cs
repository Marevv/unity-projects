using UnityEngine;
using CustomAttributes;
using System.Collections.Generic;

namespace EventBus
{
    public class Hero : MonoBehaviour
    {
        int health = 100;
        int mana = 50;

        EventBinding<TestEvent> testEventBinding;
        EventBinding<PlayerEvent> playerEventBingind;

        private void OnEnable()
        {
            testEventBinding = new EventBinding<TestEvent>(HandleTestEvent);
            EventBus<TestEvent>.Register(testEventBinding);

            playerEventBingind = new EventBinding<PlayerEvent>(HandlePlayerEvent);
            EventBus<PlayerEvent>.Register(playerEventBingind);
        }


        private void OnDisable()
        {
            EventBus<TestEvent>.Deregister(testEventBinding);
            EventBus<PlayerEvent>.Deregister(playerEventBingind);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                EventBus<TestEvent>.Raise(new TestEvent());
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                EventBus<PlayerEvent>.Raise(new PlayerEvent
                {
                    health = health,
                    mana = mana
                });
            }
        }
        void HandleTestEvent()
        {
            Debug.Log("Test event recived");
        }

        void HandlePlayerEvent(PlayerEvent playerEvent)
        {
            Debug.Log($"Player event received! Health: {playerEvent.health}, Mana: {playerEvent.mana}");
        }
    }

}



