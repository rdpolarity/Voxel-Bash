// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;

namespace LDG.Core
{
    using LDG.Core.Timers;
    using UnityEngine;

    /// <summary>
    /// A Particle can be emitted and will expire (disabled) after a certain amount of time.
    /// </summary>
    public class MonoBehaviourParticle : MonoBehaviourEx
    {
        public event Action<MonoBehaviourParticle> OnEmit;
        public event Action<MonoBehaviourParticle> OnExpire;

        private IParticleCollection owner;

        private Timer timer = new Timer();

        protected void Awake()
        {
            timer.Elapsed += TimeElapsed;
        }

        protected void Update()
        {
            timer.Update();
        }

        private void TimeElapsed(object source, ElapsedEventArgs e)
        {
            if (OnExpire != null) OnExpire.Invoke(this);

            Return();
        }

        public void SetOwner(IParticleCollection particleCollection)
        {
            owner = particleCollection;
        }

        public void Emit(float lifetime)
        {
            timer.Interval = lifetime;
            timer.Start();

            if(OnEmit != null)
            {
                OnEmit.Invoke(this);
            }
        }

        public void Return()
        {
            timer.TerminateEarly();
            owner.ReturnParticle(this);
        }

        public virtual void Initialize(object parameters)
        {
            
        }
    }
}