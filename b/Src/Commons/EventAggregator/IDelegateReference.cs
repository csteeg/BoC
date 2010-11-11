namespace BoC.EventAggregator
{
    using System;

    public interface IDelegateReference
    {
        Delegate Target
        {
            get;
        }
    }
}