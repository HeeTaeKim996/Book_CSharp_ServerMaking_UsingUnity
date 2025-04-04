using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;
using FreeNetUnity;

namespace FreeNetUnity
{
    public enum NETWORK_EVENT : byte
    {
        connected,

        disconnected,

        end
    }
}
public class CFreeNetEventManager 
{
    private object cs_queue;

    private Queue<NETWORK_EVENT> network_events;
    private Queue<CPacket> network_messages;


    public CFreeNetEventManager()
    {
        this.network_events = new Queue<NETWORK_EVENT>();
        this.network_messages = new Queue<CPacket>();
        this.cs_queue = new object();
    }



    public void enqueue_network_event(NETWORK_EVENT netEvent)
    {
        lock (this.cs_queue)
        {
            network_events.Enqueue(netEvent);
        }
    }
    public NETWORK_EVENT dequeue_network_event()
    {
        lock (this.cs_queue)
        {
            return network_events.Dequeue();
        }
    }
    public bool has_event()
    {
        lock (cs_queue)
        {
            return network_events.Count > 0;
        }
    }
    

    public void enqueue_net_message(CPacket msg)
    {
        lock (this.cs_queue)
        {
            network_messages.Enqueue(msg);
        }
    }
    public CPacket dequeue_net_message()
    {
        lock (this.cs_queue)
        {
            return network_messages.Dequeue();
        }
    }
    public bool has_message()
    {
        lock (this.cs_queue)
        {
            return network_messages.Count > 0;
        }
    }
}
