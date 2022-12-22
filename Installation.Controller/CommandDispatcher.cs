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
        public delegate void SubscribeMethod(Command objectToSend);

        public CommandDispatcher RegisterReceiver<T>(object receiver, CommandVerb commandVerb)
        {
            receivers.Add(new Receiver
            {
                ReceiverObject = receiver,
                ReceivingType = typeof(T),
                CommandVerb = commandVerb
            });
            return this;
        }
        public CommandDispatcher RegisterReceiver<T>(object receiver)
        {
            receivers.Add(new Receiver
            {
                ReceiverObject = receiver,
                ReceivingType = typeof(T)
            });
            return this;
        }
        public void Send<T>(Command command)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => (r.CommandVerb & command.CommandVerb) == command.CommandVerb);
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is ICommandReceiver)
                {
                    (receiverObject.ReceiverObject as ICommandReceiver).Receive(command);
                }
            }
        }
        public void Send<T>(object obj)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => r.CommandVerb == null);
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is IObjectReceiver)
                {
                    (receiverObject.ReceiverObject as IObjectReceiver).Receive(obj);
                }

            }
        }
        public void Send<T>(object obj, Command command)
        {
            var receiverObjects = receivers.Where(r => r.ReceivingType == typeof(T)).Where(r => (r.CommandVerb & command.CommandVerb) == command.CommandVerb);
            foreach (var receiverObject in receiverObjects)
            {
                if (receiverObject.ReceiverObject is ICommandAndObjectReceiver)
                {
                    (receiverObject.ReceiverObject as ICommandAndObjectReceiver).Receive(obj, command);
                }

            }
        }


    }
}
