using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EventBus
{
    public static class EventBusUtil
    {
        public static IReadOnlyList<Type> EventTypes { get; set; }
        public static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR
        public static PlayModeStateChange PlayModeState { get; set; }

        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            PlayModeState = state;
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ClearAllBuses();
            }
        }

#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
            EventBusTypes = InitializeAllBusses();
        }

        private static List<Type> InitializeAllBusses()
        {
            List<Type> eventBusTypes = new List<Type>();

            var typedef = typeof(EventBus<>);

            foreach (var eventType in EventTypes)
            {
                var busType = typedef.MakeGenericType(eventType);
                eventBusTypes.Add(busType);

                Debug.Log($"Initialized EventBus<{eventType.Name}>");
            }

            return eventBusTypes;
        }


        public static void ClearAllBuses()
        {
            Debug.Log("Cealring all buses...");
            for (int i = 0; i < EventBusTypes.Count; i++)
            {
                var busType = EventBusTypes[i];
                var clearMethod = busType.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                clearMethod.Invoke(null, null);
            }
        }

    }
}



