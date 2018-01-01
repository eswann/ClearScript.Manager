//-----------------------------------------------------------------------
// <copyright file="EventHanders.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Tabris.Winform.Control
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EventBus
    {
        public static EventAggregator _eventAggregator = new EventAggregator();
        public static Publisher _publisher = new Publisher(_eventAggregator);
    }


    /// <summary>
    /// 订阅者
    /// </summary>
    public class Subscriber
    {
        readonly EventAggregator eventAggregator;

        public Subscriber(EventAggregator eve)
        {
            eventAggregator = eve;
        }

        public Subscription<T> SubscriberMessage<T>(Action<T> action)
        {
            return eventAggregator.Subscribe(action);
        }
        public void UnSubcriber<T>(Subscription<T> subscription)
        {
            eventAggregator.UnSbscribe(subscription);
        }

    }
    /// <summary>
    /// 发布者
    /// </summary>
    public class Publisher
    {
        readonly EventAggregator EventAggregator;
        public Publisher(EventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        public void PublishMessage<T>(T obj)
        {
            EventAggregator.Publish(obj);
        }
    }

    /// <summary>
    /// 注册对象
    /// </summary>
    /// <typeparam name="Tmessage"></typeparam>
    public class Subscription<Tmessage> : IDisposable
    {
        public Action<Tmessage> Action { get; private set; }
        private readonly EventAggregator EventAggregator;
        private bool isDisposed;
        public Subscription(Action<Tmessage> action, EventAggregator eventAggregator)
        {
            Action = action;
            EventAggregator = eventAggregator;
        }

        ~Subscription()
        {
            if (!isDisposed)
                Dispose();
        }

        public void Dispose()
        {
            EventAggregator.UnSbscribe(this);
            isDisposed = true;
        }
    }


    /// <summary>
    /// Hub
    /// </summary>
    public class EventAggregator
    {
        private readonly Dictionary<Type, IList> subscriber;

        public EventAggregator()
        {
            subscriber = new Dictionary<Type, IList>();
        }

        public void Publish<TMessageType>(TMessageType message)
        {
            Type t = typeof(TMessageType);
            if (subscriber.ContainsKey(t))
            {
                IList actionlst = new List<Subscription<TMessageType>>(subscriber[t].Cast<Subscription<TMessageType>>());

                foreach (Subscription<TMessageType> a in actionlst)
                {
                    a.Action(message);
                }
            }
        }

        public Subscription<TMessageType> Subscribe<TMessageType>(Action<TMessageType> action)
        {
            Type t = typeof(TMessageType);
            IList actionlst;
            var actiondetail = new Subscription<TMessageType>(action, this);

            if (!subscriber.TryGetValue(t, out actionlst))
            {
                actionlst = new List<Subscription<TMessageType>> { actiondetail };
                subscriber.Add(t, actionlst);
            }
            else
            {
                actionlst.Add(actiondetail);
            }

            return actiondetail;
        }

        public void UnSbscribe<TMessageType>(Subscription<TMessageType> subscription)
        {
            Type t = typeof(TMessageType);
            if (subscriber.ContainsKey(t))
            {
                subscriber[t].Remove(subscription);
            }
        }

    }
}