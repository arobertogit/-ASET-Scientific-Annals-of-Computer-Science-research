using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAIP.Models.Design_Patterns
{
    public class Subscribe_Observer_
    {
    }

    public class Subscribe : ISubscribe
    {
        private List<Observer> observers = new List<Observer>();
        private int _int;
        public int Inventory
        {
            get
            {
                return _int;
            }
            set
            {
                // Just to make sure that if there is an increase in inventory then only we are notifying the observers.
                if (value > _int)
                    Notify();
                _int = value;
            }
        }
        public void SubscribeUser(Observer observer)
        {
            observers.Add(observer);
        }

        public void UnsubscribeUser(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            observers.ForEach(x => x.Update());
        }
    }


    public class Observer : IObserver
    {
        string ObserverEmail { get; private set; }
        public Observer(string name)
        {
            this.ObserverEmail = name;
        }
        public void Update()
        {
            Console.WriteLine("{0}: A paper new paper was published.", this.ObserverEmail);
        }
    }

    interface ISubscribe
    {
        void Subscribe(Observer observer);
        void Unsubscribe(Observer observer);
        void Notify();
    }
    interface IObserver
    {
        void Update();
    }
}