using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Monizze.Model;

namespace Monizze.ViewModel
{
    public class SuperViewModelBase : ViewModelBase
    {
        protected virtual void Broadcast(object oldValue, object newValue, string propertyName)
        {
            var message = new PropertyChangedMessage(this, this, oldValue, newValue, propertyName);
            if (MessengerInstance != null)
            {
                MessengerInstance.Send(message);
            }
            else
            {
                Messenger.Default.Send(message);
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName, object oldValue, object newValue)
        {
            RaisePropertyChanged(propertyName);
            Broadcast(oldValue, newValue, propertyName);
        }
    }
}
