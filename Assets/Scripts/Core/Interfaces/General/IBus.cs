namespace Endsley
{
    public interface IBus<TEventType, TEventData>
    {
        void Subscribe(TEventType eventType, System.Action<TEventData> handler);
        void Unsubscribe(TEventType eventType, System.Action<TEventData> handler);
        void Emit(TEventType eventType, TEventData eventData);
    }
}