﻿using UnityEngine;

namespace LiteFramework.Game.Sfx
{
    public class ParticleSfx : BaseSfx
    {
        private readonly ParticleSystem Particle_;
        private bool IsLoop_;
        private float Time_;
        private LiteAction Finished_;

        public ParticleSfx(string Name, Transform Trans)
            : base(Name, Trans)
        {
            Particle_ = GetComponent<ParticleSystem>();
            Finished_ = null;
            IsLoop_ = false;
            Time_ = 0;
        }

        public override void Tick(float DeltaTime)
        {
            if (!IsAlive)
            {
                return;
            }

            Time_ += DeltaTime;
            IsAlive = !IsEnd();
            if (!IsAlive)
            {
                Finished_?.Invoke();
            }
        }

        public override bool IsEnd()
        {
            if (IsLoop_)
            {
                return false;
            }

            if (Particle_ == null)
            {
                return true;
            }

            return Time_ > Particle_.main.duration;
        }

        protected override float GetZOrder(Vector2 Value)
        {
            return 0;
        }
		
        public override void Play(string AnimationName, bool IsLoop = false, LiteAction Finished = null)
        {
            Particle_.Clear(true);
            Particle_.Play(true);
            IsLoop_ = IsLoop;
            Time_ = 0;

            if (!IsLoop)
            {
                Finished_ = Finished;
            }
        }

        public override void Pause()
        {
            Particle_.Pause(true);
        }

        public override void Resume()
        {
            Particle_.Play(true);
        }
    }
}