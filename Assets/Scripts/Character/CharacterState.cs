using UnityEngine;

namespace Character
{
    /**
     * <summary>Base class for all character motion states.</summary>
     */
    public abstract class CharacterState
    {
        protected Character Character;

        /**
         * <summary>Called when the state is initially set.</summary>
         * <param name="character">Reference to the character this is acting on.</param>
         */
        public virtual void StateBegin(Character character)
        {
            Character = character;
        }

        /**
         * <summary>Called when the state exits.</summary>
         */
        public virtual void StateEnd()
        {
            Character = null;
        }

        /**
         * <summary>Called when the character motion vector changes.</summary>
         * <param name="direction">New direction for the character. Passed from the input stick or AI controller.</param>
         */
        public virtual void MoveChange(Vector2 direction)
        {
            Character.MotionStatus.CurrentMotion = direction;
        }

        /**
         * <summary>Called when the run state changes.</summary>
         * <param name="runValue">Value of the run state, either from the player button or AI controller.</param>
         */
        public virtual void RunChange(bool runValue)
        {
            Character.MotionStatus.Run = runValue;
        }

        /**
         * <summary>Internal function to see if the direction of the character changed and trigger
         * the callback in the character class.</summary>
         */
        protected void TestDirection()
        {
            var xMotion = Character.MotionStatus.CurrentMotion.x;
            if (!(Mathf.Abs(xMotion) > 0.001)) return;
            
            var dir = xMotion > 0;
            if (dir == Character.MotionStatus.DirectionIsForward) return;
            
            Character.ChangeDirection(dir);
            Character.MotionStatus.DirectionIsForward = dir;
        }

        /**
         * <summary>Internal function to set the velocity of the motion status structure.</summary>
         * <param name="speed">The speed modifier to use. This is multiplied with the current
         * motion in the status structure.</param>
         */
        protected void ApplySpeed(Vector2 speed)
        {
            Character.MotionStatus.MotionVelocity = speed * Character.MotionStatus.CurrentMotion;
        }
    }
    
    /**
     * <summary>State for the idle (no motion) state and animations.</summary>
     */
    public class IdleState : CharacterState
    {
        /**
         * <summary>Start point for the state. Sets the velocity to 0.</summary>
         */
        public override void StateBegin(Character character)
        {
            base.StateBegin(character);
            ApplySpeed(Vector2.zero);
            character.AnimationStatus.Animator.SetInteger(AnimationStatus.MotionHash, (int)AnimationStatus.MotionStates.Idle);
        }

        /**
         * <summary>Motion input updated. Tests for motion and run status and switches the state to walk or run.</summary>
         */
        public override void MoveChange(Vector2 direction)
        {
            base.MoveChange(direction);
            var val = direction.magnitude;
            if (Mathf.Abs(val) > 0.0001f)
            {
                Character.ChangeMotionState(Character.MotionStatus.Run 
                    ? Character.MotionStates.Run 
                    : Character.MotionStates.Walk);
            }
        }
    }

    /**
     * <summary>Walk state for the character. Moves the character at walkSpeed.</summary>
     */
    public class WalkState : CharacterState
    {
        /**
         * <summary>Start of the state. Sets the characters direction and initial velocity.</summary>
         */
        public override void StateBegin(Character character)
        {
            base.StateBegin(character);
            TestDirection();
            ApplySpeed(Character.walkSpeed);
            character.AnimationStatus.Animator.SetInteger(AnimationStatus.MotionHash, (int)AnimationStatus.MotionStates.Walking);
        }

        /**
         * <summary>Change in motion input. Adjusts the velocity, or reverts to idle if motion is 0.</summary>
         */
        public override void MoveChange(Vector2 direction)
        {
            base.MoveChange(direction);
            var val = direction.magnitude;
            if (Mathf.Abs(val) < 0.001)
                Character.ChangeMotionState(Character.MotionStates.Idle);
            else
            {
                TestDirection();
                ApplySpeed(Character.walkSpeed);
            }
        }

        /**
         * <summary>Run input change. Sets state to running.</summary>
         */
        public override void RunChange(bool runValue)
        {
            base.RunChange(runValue);
            if (!runValue) return;
            Character.MotionStatus.Run = true;
            Character.ChangeMotionState(Character.MotionStates.Run);
        }
    }

    /**
     * <summary>Run motion state. Moves the character at runSpeed.</summary>
     */
    public class RunState : CharacterState
    {
        /**
         * <summary>Start of the run state. Sets direction and initial velocity.</summary>
         */
        public override void StateBegin(Character character)
        {
            base.StateBegin(character);
            TestDirection();
            ApplySpeed(Character.runSpeed);
            character.AnimationStatus.Animator.SetInteger(AnimationStatus.MotionHash, (int)AnimationStatus.MotionStates.Running);
        }

        /**
         * <summary>Motion input changed. Sets the new velocity, or reverts to idle if motion is 0.</summary>
         */
        public override void MoveChange(Vector2 direction)
        {
            base.MoveChange(direction);
            var val = direction.magnitude;
            if(Mathf.Abs(val) < 0.001)
                Character.ChangeMotionState(Character.MotionStates.Idle);
            else
            {
                TestDirection();
                ApplySpeed(Character.runSpeed);
            }
        }

        /**
         * <summary>Run input changed. Reverts to the walk state.</summary>
         */
        public override void RunChange(bool runValue)
        {
            base.RunChange(runValue);
            if (runValue) return;
            Character.MotionStatus.Run = false;
            Character.ChangeMotionState(Character.MotionStates.Walk);
        }
    }

    public class ActionState : CharacterState
    {
        
    }
}
