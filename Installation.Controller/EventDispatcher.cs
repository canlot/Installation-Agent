using Installation.Models;
using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Controller
{
    public class EventDispatcher
    {
        private List<Receiver> receivers = new List<Receiver>();

        public EventDispatcher RegisterReceiver<T>(object receiver)
        {
            if (!checkIfReceiverAlreadyRegisteredForClass<T>(receiver)) // check if class already registered, if not methods can be called multiple times, as many as registered
            {
                receivers.Add(new Receiver
                {
                    ReceiverObject = receiver,
                    ReceivingType = typeof(T)
                });
            }
            else
                throw new EventAlreadyRegisteredException();
            return this;
        }
        public EventDispatcher RegisterReceiver<T, TReturn>(object receiver)
        {
            if (!checkIfReceiverAlreadyRegisteredForClass<T, TReturn>(receiver)) // check if class already registered, if not methods can be called multiple times, as many as registered
            {
                receivers.Add(new Receiver
                {
                    ReceiverObject = receiver,
                    ReceivingType = typeof(T),
                    ReturningType = typeof(TReturn),
                });
            }
            else
                throw new EventAlreadyRegisteredException();
            return this;
        }
        private bool checkIfReceiverAlreadyRegisteredForClass<T>(object receiver)
        {
            var alreadyRegisteredObjects = receivers.Where(r => r.ReceivingType == typeof(T))
                                                    .Where(r => r.ReturningType == null);
                                                    //check if only 1 generic argument registered the other has to be null
            foreach (Receiver r in alreadyRegisteredObjects)
            {
                if (r.ReceiverObject == receiver)
                    return true;
            }
            return false;
        }
        private bool checkIfReceiverAlreadyRegisteredForClass<T, TReturn>(object receiver)
        {
            var alreadyRegisteredObjects = receivers.Where(r => r.ReceivingType == typeof(T))
                                                    .Where(r => r.ReturningType == typeof(TReturn));
            foreach (Receiver r in alreadyRegisteredObjects)
            {
                if (r.ReceiverObject == receiver)
                    return true;
            }
            return false;
        }
        public void Send<T>(T obj)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => r.ReturningType == null);
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is IObjectReceiver<T>)
                {
                    (receiverObject.ReceiverObject as IObjectReceiver<T>).Receive(obj);
                }

            }
            throw new EventNotRegisteredException();
        }
        public TReturn Send<T, TReturn>(T obj)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => r.ReturningType == typeof(TReturn));
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is IObjectReceiver<T, TReturn>)
                {
                     return (receiverObject.ReceiverObject as IObjectReceiver<T, TReturn>).Receive(obj);
                }
            }
            throw new EventNotRegisteredException();
        }

    }
}
