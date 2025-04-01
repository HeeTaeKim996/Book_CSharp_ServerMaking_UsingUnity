using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;

namespace FreeNetUnity
{
    public enum NETWORK_EVENT : byte
    {
        connected,
        disconnected,
        end
    }

    public class CFreeNetEventManager
    {
        object cs_event;
        Queue<NETWORK_EVENT> network_events;

        Queue<CPacket> network_message_events;


        public CFreeNetEventManager()
        {
            this.cs_event = new object();
            this.network_events = new Queue<NETWORK_EVENT>();
            this.network_message_events = new Queue<CPacket>();
        }



        public void enqueue_network_event(NETWORK_EVENT event_type)
        {
            lock (this.cs_event)
            {
                this.network_events.Enqueue(event_type);
            }
        }
        public NETWORK_EVENT dequeue_network_event()
        {
            lock (cs_event)
            {
                return this.network_events.Dequeue();
            }
        }
        public bool has_event()
        {
            lock (this.cs_event)
            {
                return this.network_events.Count > 0;
            }
        }


        public void enqueue_network_message(CPacket buffer)
        {
            lock (cs_event)
            {
                this.network_message_events.Enqueue(buffer);
            }
        }
        public CPacket dequeue_network_message()
        {
            lock (cs_event)
            {
                return this.network_message_events.Dequeue();
            }
        }
        public bool has_message()
        {
            lock (this.cs_event)
            {
                return this.network_message_events.Count > 0;
            }
        }
    }
}


