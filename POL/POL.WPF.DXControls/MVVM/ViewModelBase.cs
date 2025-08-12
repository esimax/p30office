namespace POL.WPF.DXControls.MVVM
{
    public abstract class ViewModelBase : NotifyObjectBase
    {
        public ViewCommandsBase Commands { get; protected set; }
    }
}
