using Installation.Models;
using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller
{
    public class EventDispatcher
    {
        private List<Receiver> receivers = new List<Receiver>();

        public EventDispatcher RegisterReceiver<T>(object receiver)
        {
            if (!checkIfReceiverAlreadyRegisteredForClass<T>(receiver))
            {
                receivers.Add(new Receiver
                {
                    ReceiverObject = receiver,
                    ReceivingType = typeof(T)
                });
            }
            return this;
        }
        private bool checkIfReceiverAlreadyRegisteredForClass<T>(object receiver)
        {
            var alreadyRegisteredObjects = receivers.Where(r => r.ReceivingType == typeof(T));
            foreach (Receiver r in alreadyRegisteredObjects)
            {
                if (r.ReceiverObject == receiver)
                    return true;
            }
            return false;
        }
        public void Send<T>(T obj)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T));
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is IObjectReceiver<T>)
                {
                    (receiverObject.ReceiverObject as IObjectReceiver<T>).Receive(obj);
                }

            }
        }


    }
}
