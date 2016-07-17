namespace Izzo.Starball
{
    public interface ITargetOfInterestObserver
    {
        void AddTargetOfInterest( ITargetOfInterest item );
        bool RemoveTargetOfInterest( ITargetOfInterest item );
    }
}