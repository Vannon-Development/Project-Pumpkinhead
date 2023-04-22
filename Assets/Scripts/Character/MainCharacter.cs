using UnityEngine;

namespace Character
{
    public class MainCharacter : BaseCharacter
    {
        protected override void Start()
        {
            base.Start();
            GameScene.RegisterPlayer(gameObject);
        }
    }
}
