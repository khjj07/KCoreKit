namespace KCoreKit
{
    
    public interface ISaveData
    {

    }

    public interface ISerializable<T> where T : ISaveData
    {
        public void Serialize(out T data);
        public void Deserialize(in T data);
    }

}