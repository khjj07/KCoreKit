namespace KCoreKit
{
    public abstract class PanelModelWidget<TModel> : PanelWidget
    {
        
        protected TModel model;

        public TModel GetModel()
        {
            return model;
        }
        
        public virtual void Construct(TModel model)
        {
            this.model = model;
        }

        public virtual void UpdateWidget(TModel model)
        {
            this.model = model;
        }
    }
}