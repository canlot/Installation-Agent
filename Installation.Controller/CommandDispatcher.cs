using Installation.Models;
using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller
{
    public class CommandDispatcher
    {
        private List<Receiver> receivers = new List<Receiver>();

        public CommandDispatcher RegisterReceiver<T>(object receiver, CommandAction commandVerb)
        {
            if (checkIfReceiverAlreadyRegisteredForCommand<T>(receiver, commandVerb))
                throw new Exception("Object already registered");
            receivers.Add(new Receiver
            {
                ReceiverObject = receiver,
                ReceivingType = typeof(T),
                CommandVerb = commandVerb
            });
            return this;
        }
        private bool checkIfReceiverAlreadyRegisteredForCommand<T>(object receiver, CommandAction commandVerb)
        {
            var alreadyRegisteredObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => (r.CommandVerb & commandVerb) == commandVerb);
            foreach (Receiver r in alreadyRegisteredObjects)
            {

                if (r.ReceiverObject == receiver)
                    return true;
            }
            return false;
        }
        public CommandDispatcher RegisterReceiver<T>(object receiver)
        {
            if (checkIfReceiverAlreadyRegisteredForCommand<T>(receiver))
                throw new Exception("Object already registered");
            receivers.Add(new Receiver
            {
                ReceiverObject = receiver,
                ReceivingType = typeof(T)
            });
            return this;
        }
        private bool checkIfReceiverAlreadyRegisteredForCommand<T>(object receiver)
        {
            var alreadyRegisteredObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => r.CommandVerb == null);
            foreach (Receiver r in alreadyRegisteredObjects)
            {
                if (r.ReceiverObject == receiver)
                    return true;
            }
            return false;
        }
        public void Send<T>(Command<T> command)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => (r.CommandVerb & command.CommandAction) == command.CommandAction);
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is ICommandReceiver<T>)
                {
                    (receiverObject.ReceiverObject as ICommandReceiver<T>).Receive(command);
                }
            }
        }
        public void Send<T>(T obj)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => r.CommandVerb == null);
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is IObjectReceiver<T>)
                {
                    (receiverObject.ReceiverObject as IObjectReceiver<T>).Receive(obj);
                }

            }
        }
        public void Send<T>(T obj, Command<T> command)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => (r.CommandVerb & command.CommandAction) == command.CommandAction);
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is ICommandAndObjectReceiver<T>)
                {
                    (receiverObject.ReceiverObject as ICommandAndObjectReceiver<T>).Receive(obj, command);
                }

            }
        }


    }
}
