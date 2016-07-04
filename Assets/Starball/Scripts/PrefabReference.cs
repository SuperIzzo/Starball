/*+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + *\
+                                                                              +
+     Starball - multiplayer ball sports game with spinning star drones.       +
+     Copyright (C) 2016  SuperIzzo                                            +
+                                                                              +
+     This file is part of Starball.                                           +
+                                                                              +
+     Mulsh is free software: you can redistribute it and/or modify            +
+     it under the terms of the GNU General Public License as published by     +
+     the Free Software Foundation, either version 3 of the License, or        +
+     ( at your option) any later version.                                     +
+                                                                              +
+     Mulsh is distributed in the hope that it will be useful,                 +
+     but WITHOUT ANY WARRANTY; without even the implied warranty of           +
+     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            +
+     GNU General Public License for more details.                             +
+                                                                              +
+     You should have received a copy of the GNU General Public License        +
+     along with Mulsh.  If not, see<http://www.gnu.org/licenses/>.            +
+                                                                              +
\*+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + */
namespace Izzo.Utility
{
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor.Callbacks;
#endif

    [AddComponentMenu( "Miscellaneous/Prefab Reference" )]
    public class PrefabReference : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        
        private void Awake()
        {
            Bake();
        }

#if UNITY_EDITOR
        [PostProcessScene( -2 )]
        public static void OnPostprocessScene()
        {
            foreach( var prefab in FindObjectsOfType<PrefabReference>() )
            {
                if( prefab.isActiveAndEnabled )
                {
                    prefab.Bake();
                }
            }
        }
#endif

        private void Bake()
        {
            GameObject instance = InstantiatePrefab();
            SetPrefabParent( instance );
            TransferChildren( instance );

            instance.gameObject.name = gameObject.name;
            instance.gameObject.tag = gameObject.tag;
            instance.gameObject.layer = gameObject.layer;
            Destroy( gameObject );
        }

        private void TransferChildren( GameObject instance )
        {
            foreach( Transform child in transform )
            {
                child.SetParent( instance.transform, true );
            }
        }

        private void SetPrefabParent( GameObject instance )
        {
            if( transform.parent )
            {
                instance.transform.SetParent( transform.parent, true );
            }
        }

        private GameObject InstantiatePrefab()
        {
            return Entity.Spawn( _prefab, 
                                 transform.position, 
                                 transform.rotation );
        }
    }
}