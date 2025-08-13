using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;

namespace ManagedShell.Common.Common;

public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
{
    private readonly Dispatcher _dispatcher;
    private readonly ReaderWriterLock _lock;

    public ThreadSafeObservableCollection()
    {
        _dispatcher = Dispatcher.CurrentDispatcher;
        _lock = new ReaderWriterLock();
    }

    protected override void ClearItems()
    {
        if (_dispatcher.CheckAccess())
        {
            LockCookie c = _lock.UpgradeToWriterLock(-1);
            base.ClearItems();
            _lock.DowngradeFromWriterLock(ref c);
        }
        else
        {
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { Clear(); }, null);
        }
    }
    protected override void InsertItem(int index, T item)
    {
        if (_dispatcher.CheckAccess())
        {
            if (index > this.Count)
                return;
            LockCookie c = _lock.UpgradeToWriterLock(-1);
            base.InsertItem(index, item);
            _lock.DowngradeFromWriterLock(ref c);
        }
        else
        {
            object[] e = [index, item];
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { InsertItemImpl(e); }, e);
        }
    }
    private void InsertItemImpl(object[] e)
    {
        if (_dispatcher.CheckAccess())
        {
            InsertItem((int)e[0], (T)e[1]);
        }
        else
        {
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { InsertItemImpl(e); });
        }
    }
    protected override void MoveItem(int oldIndex, int newIndex)
    {
        if (_dispatcher.CheckAccess())
        {
            if (oldIndex >= this.Count || newIndex >= this.Count || oldIndex == newIndex)
                return;
            LockCookie c = _lock.UpgradeToWriterLock(-1);
            base.MoveItem(oldIndex, newIndex);
            _lock.DowngradeFromWriterLock(ref c);
        }
        else
        {
            object[] e = [oldIndex, newIndex];
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { MoveItemImpl(e); }, e);
        }
    }
    private void MoveItemImpl(object[] e)
    {
        if (_dispatcher.CheckAccess())
        {
            MoveItem((int)e[0], (int)e[1]);
        }
        else
        {
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { MoveItemImpl(e); });
        }
    }
    protected override void RemoveItem(int index)
    {
        if (_dispatcher.CheckAccess())
        {
            if (index >= this.Count)
                return;
            LockCookie c = _lock.UpgradeToWriterLock(-1);
            base.RemoveItem(index);
            _lock.DowngradeFromWriterLock(ref c);
        }
        else
        {
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { RemoveItem(index); }, index);
        }
    }
    protected override void SetItem(int index, T item)
    {
        if (_dispatcher.CheckAccess())
        {
            LockCookie c = _lock.UpgradeToWriterLock(-1);
            base.SetItem(index, item);
            _lock.DowngradeFromWriterLock(ref c);
        }
        else
        {
            object[] e = [index, item];
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetItemImpl(e); }, e);
        }
    }
    private void SetItemImpl(object[] e)
    {
        if (_dispatcher.CheckAccess())
        {
            SetItem((int)e[0], (T)e[1]);
        }
        else
        {
            _dispatcher.Invoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { SetItemImpl(e); });
        }
    }
}
