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

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Replaces itself with a prefab. </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [AddComponentMenu( "Miscellaneous/Prefab Reference" )]

    public class PrefabReference : MonoBehaviour
    {
        //-------------------------------------------------------------        
        [SerializeField, Tooltip
        ( "A link to the prefab that will replace this object."      )]
        //----------------------------------
        private GameObject _prefab = null;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Replaces this object 
        ///            with a prefab at runtime.             </summary>
        /// <remarks>  
        ///     This will get called if the object was instantiated
        ///     at runtime. All objects that exist in the scene at
        ///     build time will be replaced by the editor.   </remarks>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected void Awake()
        {
            Bake();
        }

#if UNITY_EDITOR
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Replaces this object 
        ///            with a prefab at build time.          </summary>
        /// <remarks>  
        ///     This will get called at the time of building 
        ///     the player and later at building each scene. </remarks>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [PostProcessBuild( -2 )]
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
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Creates a prefab instances, sets it up
        ///            and destroys this object.             </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void Bake()
        {
            GameObject instance = InstantiatePrefab();
            SetPrefabParent( instance );
            TransferChildren( instance );

            instance.gameObject.name = gameObject.name;
            instance.gameObject.tag = gameObject.tag;
            instance.gameObject.layer = gameObject.layer;
            DestroyImmediate( gameObject );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Moves all child transfroms from this object
        ///            to another object.                    </summary>
        /// <param name="instance"> 
        ///     The object children will be moved to.          </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void TransferChildren( GameObject instance )
        {
            foreach( Transform child in transform )
            {
                child.SetParent( instance.transform, true );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Sets the parent of this object to also be
        ///            the parent of another.                </summary>
        /// <param name="instance"> 
        ///     The object whose parent will be set.           </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void SetPrefabParent( GameObject instance )
        {
            if( transform.parent )
            {
                instance.transform.SetParent( transform.parent, true );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Instantiates a prefab instance.       </summary>
        /// <returns>  The new instance from the prefab.     </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private GameObject InstantiatePrefab()
        {
            return Entity.Spawn( _prefab,
                                 transform.position,
                                 transform.rotation );
        }
    }
}