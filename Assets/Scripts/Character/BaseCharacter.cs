using System;
using UnityEngine;

namespace Character
{
    public class BaseCharacter : MonoBehaviour
    {
        public Vector2 walkSpeed;
        public Vector2 runSpeed;
        
        protected Context context;

        protected virtual void Awake()
        {
            context = new Context(this)
            {
                body = GetComponent<Rigidbody2D>(),
                ani = GetComponent<Animator>()
            };
        }

        protected virtual void Start()
        {
            context.StartDefault();
        }

        protected virtual void Update()
        {
        
        }

        protected void FixedUpdate()
        {
            
        }
    }
}
