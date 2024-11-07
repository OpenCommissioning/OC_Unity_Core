namespace OC.MaterialFlow
{
    public interface IPayloadBuffer
    {
        public void Add(Payload payload);
        public void Remove(Payload payload);
    }
}