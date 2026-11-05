[1mdiff --git a/Assets/Code/Weapons/Shotgun.cs b/Assets/Code/Weapons/Shotgun.cs[m
[1mindex 9c69af4..5447f35 100644[m
[1m--- a/Assets/Code/Weapons/Shotgun.cs[m
[1m+++ b/Assets/Code/Weapons/Shotgun.cs[m
[36m@@ -19,7 +19,7 @@[m [mpublic class Shotgun : Weapon[m
     [SerializeField] private bool animateOnShoot;[m
     [SerializeField] private bool animateOnReload;[m
     [SerializeField] private string shootAnimationTrigger = "Shoot";[m
[31m-    [SerializeField] private string reloadAnimationTrigger = "Reload";[m
[32m+[m[32m    [SerializeField] private string reloadAnimationBool = "isReloading";[m
     [SerializeField] private string idleAnimationState = "Idle";[m
 [m
     private Animator _weaponAnimator;[m
[36m@@ -31,6 +31,9 @@[m [mpublic class Shotgun : Weapon[m
     [SerializeField] private float fireRate = 0.8f;[m
     [SerializeField] private int damage = 20;[m
 [m
[32m+[m[32m    [Header("Transform Offset")][m
[32m+[m[32m    [SerializeField] private Vector3 rotationOffset = Vector3.zero;[m
[32m+[m
     [Header("Ammo")][m
     [SerializeField] private int magazineCapacity = 8;[m
     [SerializeField] private int currentAmmo = 8;[m
[36m@@ -85,7 +88,7 @@[m [mpublic class Shotgun : Weapon[m
         _pickedUp = true;[m
 [m
         transform.SetParent(weaponHolder);[m
[31m-        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);[m
[32m+[m[32m        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(rotationOffset));[m
 [m
         if (_rigidbody != null)[m
         {[m
[36m@@ -118,7 +121,6 @@[m [mpublic class Shotgun : Weapon[m
             {[m
                 audioSource.PlayOneShot(emptyClip);[m
             }[m
[31m-            // Agregamos un pequeño delay para que no se spammee el sonido de vacío si el jugador aprieta muy rápido[m
             nextFireTime = Time.time + 0.2f;[m
             return;[m
         }[m
[36m@@ -132,7 +134,6 @@[m [mpublic class Shotgun : Weapon[m
         nextFireTime = Time.time + fireRate;[m
         currentAmmo--;[m
 [m
[31m-        // REPRODUCCIÓN DEL DISPARO[m
         if (audioSource != null && shootClip != null)[m
         {[m
             audioSource.PlayOneShot(shootClip);[m
[36m@@ -182,9 +183,7 @@[m [mpublic class Shotgun : Weapon[m
 [m
         if (_weaponAnimator != null && animateOnReload)[m
         {[m
[31m-            _weaponAnimator.ResetTrigger(reloadAnimationTrigger);[m
[31m-            _weaponAnimator.Play(idleAnimationState, 0, 0f);[m
[31m-            _weaponAnimator.Update(0f);[m
[32m+[m[32m            _weaponAnimator.SetBool(reloadAnimationBool, false);[m
         }[m
 [m
         Debug.Log("Reload interrupted");[m
[36m@@ -209,12 +208,6 @@[m [mpublic class Shotgun : Weapon[m
             return;[m
         }[m
 [m
[31m-        if (animateOnReload && _weaponAnimator != null)[m
[31m-        {[m
[31m-            _weaponAnimator.ResetTrigger(reloadAnimationTrigger);[m
[31m-            _weaponAnimator.SetTrigger(reloadAnimationTrigger);[m
[31m-        }[m
[31m-[m
         _reloadCoroutine = StartCoroutine(ReloadOneByOne(ammoInventory, onReload));[m
     }[m
 [m
[36m@@ -222,6 +215,11 @@[m [mpublic class Shotgun : Weapon[m
     {[m
         _isReloading = true;[m
 [m
[32m+[m[32m        if (animateOnReload && _weaponAnimator != null)[m
[32m+[m[32m        {[m
[32m+[m[32m            _weaponAnimator.SetBool(reloadAnimationBool, true);[m
[32m+[m[32m        }[m
[32m+[m
         yield return new WaitForSeconds(reloadStartDelay);[m
 [m
         while (currentAmmo < magazineCapacity && ammoInventory.HasAmmo())[m
[36m@@ -229,7 +227,6 @@[m [mpublic class Shotgun : Weapon[m
             currentAmmo++;[m
             ammoInventory.TryUseAmmo();[m
 [m
[31m-            // REPRODUCCIÓN DE LA RECARGA (Suena por cada cartucho insertado)[m
             if (audioSource != null && reloadClip != null)[m
             {[m
                 audioSource.PlayOneShot(reloadClip);[m
[36m@@ -245,6 +242,12 @@[m [mpublic class Shotgun : Weapon[m
         }[m
 [m
         _isReloading = false;[m
[32m+[m
[32m+[m[32m        if (animateOnReload && _weaponAnimator != null)[m
[32m+[m[32m        {[m
[32m+[m[32m            _weaponAnimator.SetBool(reloadAnimationBool, false);[m
[32m+[m[32m        }[m
[32m+[m
         _reloadCoroutine = null;[m
 [m
         Debug.Log("Reload complete");[m
