namespace Izzo.Starball
{
    using UnityEngine;
    using System.Collections;

    public class GenericTargetOfInterest : MonoBehaviour, ITargetOfInterest
    {
        [SerializeField]
        private float _importance = 0.5f;

        public virtual Vector3 position
        {
            get
            {
                return transform.position;
            }
        }

        public virtual float weight
        {
            get
            {
                return _importance;
            }
        }

        private IEnumerable targetObservers
        {
            get
            {
                Camera[] cameras = FindObjectsOfType<Camera>();
                foreach( Camera camera in cameras )
                {
                    var observer = 
                        camera.GetComponent<ITargetOfInterestObserver>();

                    if( observer != null )
                    {
                        yield return observer;
                    }
                }
            }
        }

        void Start()
        {
            foreach( ITargetOfInterestObserver observer in targetObservers )
            {
                observer.AddTargetOfInterest( this );
            }
        }

        void OnDestroy()
        {
            foreach( ITargetOfInterestObserver observer in targetObservers )
            {
                observer.RemoveTargetOfInterest( this );
            }
        }
    }
}