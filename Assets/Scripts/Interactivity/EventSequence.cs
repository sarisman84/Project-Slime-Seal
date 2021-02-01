using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity
{
    public class EventSequence : MonoBehaviour
    {
        public List<SequenceInfo> events = new List<SequenceInfo>();

        public bool CanBeExecuted { get; set; } = true;
        private Coroutine m_EventSequencer;

        public void PlaySequence()
        {
            if (m_EventSequencer != null)
                StopCoroutine(m_EventSequencer);

            if (CanBeExecuted)
                m_EventSequencer = StartCoroutine(RunSequence());
        }

        private IEnumerator RunSequence()
        {
            foreach (SequenceInfo sequenceEvent in events)
            {
                sequenceEvent.onStartEvent?.Invoke();
                yield return new WaitForSeconds(sequenceEvent.eventDuration);
                sequenceEvent.onEndEvent?.Invoke();
            }

            yield return null;
        }
    }

    [Serializable]
    public struct SequenceInfo
    {
        public UnityEvent onStartEvent;
        public UnityEvent onEndEvent;
        public float eventDuration;
    }
}