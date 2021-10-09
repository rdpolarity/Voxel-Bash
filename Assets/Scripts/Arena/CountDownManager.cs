using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RDPolarity.Arena
{
    /// <summary>
    /// Visual count down and start logic
    /// </summary>
    public class CountDownManager : MonoBehaviour
    {
        [SerializeField] private int countFrom = 3;
        [Serializable] public class OnFinish : UnityEvent { }
        public OnFinish onFinish = new OnFinish();
        [Serializable] public class OnTick : UnityEvent<int> { }
        public OnTick onTick = new OnTick();
        
        void Start()
        {
            StartCoroutine(TickFor(countFrom));
        }

        private IEnumerator TickFor(int seconds)
        {
            while (seconds >= 0)
            {
                onTick.Invoke(seconds);
                yield return new WaitForSeconds (1);
                seconds--;
            }
            onFinish.Invoke();
        }
    }
}