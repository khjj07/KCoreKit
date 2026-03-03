namespace KCoreKit
{
    public interface IView<TModel>
    {
        public TModel GetModel();
        public abstract void Construct(TModel model);
        public abstract void OnUpdate();
    }
}