namespace KCoreKit
{
    
    public interface ISerializeData
    {

    }

    public interface ISerializable<T> where T : ISerializeData
    {
        public void Serialize(out T data);
        public void Deserialize(in T data);
    }

}