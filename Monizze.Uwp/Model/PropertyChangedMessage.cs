using GalaSoft.MvvmLight.Messaging;

namespace Monizze.Model
{
    public class PropertyChangedMessage : PropertyChangedMessageBase
    {

        public PropertyChangedMessage(object sender, object target, object oldValue, object newValue, string propertyName)
            : base(sender, target, propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object NewValue { get; set; }

        public object OldValue { get; set; }
    }
}
