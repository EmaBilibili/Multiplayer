using System;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
   private Rigidbody bulletRb;

   [SerializeField] private float speed = 10f;

   private void Awake()
   {
      bulletRb = GetComponent<Rigidbody>();
   }

   private void Start()
   {
      bulletRb.linearVelocity = transform.forward * speed;
   }
}
