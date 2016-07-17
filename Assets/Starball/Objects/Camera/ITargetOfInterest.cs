namespace Izzo.Starball
{
    using UnityEngine;

    public interface ITargetOfInterest
    {
        Vector3 position { get; }
        float weight { get; }
    }
}