// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;
using UnityEngine;

namespace LDG.Core.Timers
{
    public struct ElapsedEventArgs
    {
        public int Iteration;
        public float Time;
    }

    /// <summary>
    /// Timer class that keeps time with Unity's time.
    /// </summary>
    public class Timer
    {
        public int Iteration { get; protected set; }
        public event Action<object, ElapsedEventArgs> Elapsed;

        public bool Enabled = false;
        public int Repetitions = 1;
        public bool AutoReset = false;
        public float Interval = 1.0f;

        private float counter;

        public Timer()
        {
            counter = 0;
            Iteration = 0;
        }

        public void Start()
        {
            Start(false);
        }

        /// <summary>
        /// Starts the timer with the option to trigger the Elapsed event handler 
        /// </summary>
        /// <param name="triggerElapsed"></param>
        public void Start(bool triggerElapsed)
        {
            counter = 0;
            Iteration = 0;

            Enabled = true;

            if (triggerElapsed)
            {
                ElapsedEventArgs e = new ElapsedEventArgs
                {
                    Iteration = 0,
                    Time = 0.0f
                };

                Elapsed.Invoke(this, e);
            }
        }

        public void Stop()
        {
            Enabled = false;
        }

        /// <summary>
        /// Terminates before the timer ends forcing the Elapsed event to be called in the next Update.
        /// </summary>
        public void TerminateEarly()
        {
            counter = Interval;
        }

        /// <summary>
        /// Count down by specified delta time
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (!Enabled || Iteration >= Repetitions) return;

            counter += deltaTime;

            if(counter >= Interval && Elapsed != null)
            {
                Iteration++;

                ElapsedEventArgs e = new ElapsedEventArgs
                {
                    Iteration = Iteration,
                    Time = (float)(Iteration - 1) * Interval + counter
                };

                Elapsed.Invoke(this, e);

                if (Iteration < Repetitions)
                {
                    counter -= Interval;
                }
                else
                {
                    Enabled = false;
                }
            }
        }

        /// <summary>
        /// Count down using Unity's Time.deltaTime
        /// </summary>
        public void Update()
        {
            Update(Time.deltaTime);
        }
    }
}