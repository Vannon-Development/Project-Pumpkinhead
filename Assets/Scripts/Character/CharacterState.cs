using UnityEngine;

namespace Character
{
    public enum StateName
    {
        Idle, Walk, Run, Action, Jump, Fall, Stun
    }
    
    public class Context
    {
        public BaseCharacter character;
        public State currentState;
        public Rigidbody2D body;
        public Animator ani;

        public Vector2 motion;
        public bool run;

        public Context(BaseCharacter character)
        {
            this.character = character;
        }

        public void StartDefault()
        {
            currentState = new IdleState();
            currentState.BeginState(this);
        }

        public readonly State[] statList =
        {
            new IdleState(),
            new WalkState(),
            new RunState(),
            new ActionState(),
            new JumpState(),
            new FallState(),
            new StunState()
        };
        
        public static readonly int MotionHash = Animator.StringToHash("motion");
        public static readonly int ActionHash = Animator.StringToHash("action");
    }
    
    public abstract class State
    {
        protected Context context;
        
        internal virtual void BeginState(Context ctx)
        {
            context = ctx;
        }

        protected virtual void EndState()
        {
            
        }

        public virtual void MoveChanged(Vector2 move)
        {
            context.motion = move;
        }

        protected void SetState(StateName state)
        {
            context.currentState.EndState();
            context.currentState = context.statList[(int)state];
            context.currentState.BeginState(context);
        }
    }

    public class IdleState : State
    {
        internal override void BeginState(Context ctx)
        {
            base.BeginState(ctx);
            context.ani.SetInteger(Context.MotionHash, 0);
        }

        public override void MoveChanged(Vector2 move)
        {
            base.MoveChanged(move);
            if (!context.motion.magnitude.NearZero())
                SetState(context.run ? StateName.Run : StateName.Walk);
        }
    }

    public class WalkState : State
    {
        
    }

    public class RunState : State
    {
        
    }

    public class ActionState : State
    {
        
    }

    public class JumpState : State
    {
        
    }

    public class FallState : State
    {
        
    }

    public class StunState : State
    {
        
    }
}
