using System.Collections;
using UnityEngine;

namespace Assets.FSM.FSMActivity
{
    public class RangedAttackCooldown : MonoBehaviour
    {
        public float fireCooldown = 10f;
        public float fireCounter = 0f;

        private bool _canFire = true;
        public bool CanFire 
        {
            get
            {
                _canFire = fireCounter > fireCooldown;
                return _canFire;
            }
            set
            {
                _canFire = value;
            }
        }

        private void FixedUpdate()
        {
            fireCounter += Time.deltaTime;
        }
    }
}