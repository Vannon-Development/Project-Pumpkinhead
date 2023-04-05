using UnityEngine;

namespace Character
{
    /**
     * <summary>Base class for all character motion states.</summary>
     */
    public abstract class MotionState
    {
        /**
         * <summary>Called when the state is initially set.</summary>
         * <param name="character">Reference to the character this is acting on.</param>
         */
        public abstract void StateBegin(Character character);
        /**
         * <summary>Called when the character motion vector changes.</summary>
         * <param name="character">Reference to the character this is acting on.</param>
         * <param name="direction">New direction for the character. Passed from the input stick or AI controller.</param>
         */
        public abstract void MoveChange(Character character, Vector2 direction);
        /**
         * <summary>Called when the run state changes.</summary>
         * <param name="character">Reference to the character this is acting on.</param>
         * <param name="runValue">Value of the run state, either from the player button or AI controller.</param>
         */
        public abstract void RunChange(Character character, bool runValue);
        
        /**
         * <summary>Internal function to see if the direction of the character changed and trigger
         * the callback in the character class.</summary>
         * <param name="character">Reference to the character this is acting on.</param>
         */
        protected static void TestDirection(Character character)
        {
            var xMotion = character.MotionStatus.CurrentMotion.x;
            if (!(Mathf.Abs(xMotion) > 0.001)) return;
            
            var dir = xMotion > 0;
            if (dir == character.MotionStatus.DirectionIsForward) return;
            
            character.ChangeDirection(dir);
            character.MotionStatus.DirectionIsForward = dir;
        }

        /**
         * <summary>Internal function to set the velocity of the motion status structure.</summary>
         * <param name="character">Reference to the character this is acting on.</param>
         * <param name="speed">The speed modifier to use. This is multiplied with the current
         * motion in the status structure.</param>
         */
        protected static void ApplySpeed(Character character, Vector2 speed)
        {
            character.MotionStatus.MotionVelocity = speed * character.MotionStatus.CurrentMotion;
        }
    }

    /**
     * <summary>Data structure that contains many MotionState specific variables.</summary>
     */
    public struct MotionStatus
    {
        /**
         * <summary>Flag for whether run is currently activated.</summary>
         */
        public bool Run;
        
        /**
         * <summary>Flag for whether the character is currently facing forward (right).</summary> 
         */
        public bool DirectionIsForward;
        
        /**
         * <summary>The last recorded motion vector, either from player input or AI controller.</summary>
         */
        public Vector2 CurrentMotion;
        
        /**
         * <summary>The current velocity caused by the motion controller. This is passed back to the character's
         * Fixed Update loop to get applied to the body.</summary>
         */
        public Vector2 MotionVelocity;
    }

    /**
     * <summary>State for the idle (no motion) state and animations.</summary>
     */
    public class IdleState : MotionState
    {
        /**
         * <summary>Start point for the state. Sets the velocity to 0.</summary>
         */
        public override void StateBegin(Character character)
        {
            ApplySpeed(character, Vector2.zero);
        }

        /**
         * <summary>Motion input updated. Tests for motion and run status and switches the state to walk or run.</summary>
         */
        public override void MoveChange(Character character, Vector2 direction)
        {
            var val = direction.magnitude;
            if (Mathf.Abs(val) > 0.0001f)
            {
                character.MotionStatus.CurrentMotion = direction;
                character.ChangeMotionState(character.MotionStatus.Run 
                    ? Character.MotionStates.Run 
                    : Character.MotionStates.Walk);
            }
        }

        /**
         * <summary>Run input updated. Records the run value for future use.</summary>
         */
        public override void RunChange(Character character, bool runValue)
        {
            character.MotionStatus.Run = runValue;
        }
    }

    /**
     * <summary>Walk state for the character. Moves the character at walkSpeed.</summary>
     */
    public class WalkState : MotionState
    {
        /**
         * <summary>Start of the state. Sets the characters direction and initial velocity.</summary>
         */
        public override void StateBegin(Character character)
        {
            TestDirection(character);
            ApplySpeed(character, character.walkSpeed);
        }

        /**
         * <summary>Change in motion input. Adjusts the velocity, or reverts to idle if motion is 0.</summary>
         */
        public override void MoveChange(Character character, Vector2 direction)
        {
            var val = direction.magnitude;
            character.MotionStatus.CurrentMotion = direction;
            if (Mathf.Abs(val) < 0.001)
                character.ChangeMotionState(Character.MotionStates.Idle);
            else
            {
                TestDirection(character);
                ApplySpeed(character, character.walkSpeed);
            }
        }

        /**
         * <summary>Run input change. Sets state to running.</summary>
         */
        public override void RunChange(Character character, bool runValue)
        {
            if (!runValue) return;
            character.MotionStatus.Run = true;
            character.ChangeMotionState(Character.MotionStates.Run);
        }
    }

    /**
     * <summary>Run motion state. Moves the character at runSpeed.</summary>
     */
    public class RunState : MotionState
    {
        /**
         * <summary>Start of the run state. Sets direction and initial velocity.</summary>
         */
        public override void StateBegin(Character character)
        {
            TestDirection(character);
            ApplySpeed(character, character.runSpeed);
        }

        /**
         * <summary>Motion input changed. Sets the new velocity, or reverts to idle if motion is 0.</summary>
         */
        public override void MoveChange(Character character, Vector2 direction)
        {
            var val = direction.magnitude;
            character.MotionStatus.CurrentMotion = direction;
            if(Mathf.Abs(val) < 0.001)
                character.ChangeMotionState(Character.MotionStates.Idle);
            else
            {
                TestDirection(character);
                ApplySpeed(character, character.runSpeed);
            }
        }

        /**
         * <summary>Run input changed. Reverts to the walk state.</summary>
         */
        public override void RunChange(Character character, bool runValue)
        {
            if (runValue) return;
            character.MotionStatus.Run = false;
            character.ChangeMotionState(Character.MotionStates.Walk);
        }
    }
}
