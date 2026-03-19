namespace KCoreKit
{
    public interface IView<TModel>
    {
        public TModel GetModel();
        public abstract void Setup(TModel model);
        public abstract void OnChange();
    }
}