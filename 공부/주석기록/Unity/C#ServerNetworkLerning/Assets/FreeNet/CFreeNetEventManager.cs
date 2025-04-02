using FreeNet;
using System.Collections.Generic;

namespace FreeNetUnity
{
    public class CFreeNetEventManager
    {
        private Queue<NETWORK_EVENT> network_events;
        private Queue<CPacket> network_message;
        private object cs_networkQueue;

        public CFreeNetEventManager()
        {
            network_events = new Queue<NETWORK_EVENT>();
            network_message = new Queue<CPacket>();
            cs_networkQueue = new object();
        }


        public bool has_networkEvent()
        {
            lock (cs_networkQueue)
            {
                return network_events.Count > 0;
            }
        }
        public void enqueue_networkEvent(NETWORK_EVENT networkEvent)
        {
            lock (cs_networkQueue)
            {
                network_events.Enqueue(networkEvent);
            }
        }
        public NETWORK_EVENT dequeue_networkEvent()
        {
            lock (cs_networkQueue)
            {
                return network_events.Dequeue();
            }
        }



        public bool has_networkMessage()
        {
            lock (cs_networkQueue)
            {
                return network_message.Count > 0;
            }
        }
        public void enqueue_networkMessage(CPacket msg)
        {
            lock (cs_networkQueue)
            {
                network_message.Enqueue(msg);
            }
        }
        public CPacket dequeue_networkMessage()
        {
            lock (cs_networkQueue)
            {
                return network_message.Dequeue();
            }
        }
    }
}