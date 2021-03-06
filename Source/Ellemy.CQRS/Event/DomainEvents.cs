using System;
using System.Collections.Generic;
using Ellemy.CQRS.Config;

namespace Ellemy.CQRS.Event
{
    public static class DomainEvents
    {
        [ThreadStatic] //so that each thread has its own callbacks
        private static List<Delegate> actions;

        [ThreadStatic] 
        private static List<Action> handlerActions;

        private static List<IDomainEvent> unpublishedEvents;

        public static IHandlerFactory Container { get { return Configure.CurrentConfig.HandlerFactory; } } 
        
        public static void Publish()
        {
            if (handlerActions != null)
            {
                handlerActions.ForEach(a => a());
            }
            //TODO We wanna start publishing to remote systems here 
            var publisher = Configure.CurrentConfig.EventPublisher;
            if(unpublishedEvents != null){
            foreach (var unpublishedEvent in unpublishedEvents)
            {
                publisher.Publish(unpublishedEvent);
            }
            unpublishedEvents.Clear();

            }

        }
        //Registers a callback for the given domain event
        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (actions == null)
                actions = new List<Delegate>();

            actions.Add(callback);
        }

        //Clears callbacks passed to Register on the current thread
        public static void ClearCallbacks()
        {
            actions = null;
        }

        //Raises the given domain event
        public static void Raise<T>(T args) where T : IDomainEvent
        {
            if (handlerActions == null)
                handlerActions = new List<Action>();
            if(unpublishedEvents == null)
            {
                unpublishedEvents = new List<IDomainEvent>();
            }
            unpublishedEvents.Add(args);
            if (Container != null)
            {
               
                foreach (var handler in Container.GetHandlersFor<T>())
                {
                    var handler1 = handler;
                    handlerActions.Add(() => handler1.Handle(args));
                }
            }

            if (actions != null)
                foreach (Delegate action in actions)
                    if (action is Action<T>)
                        ((Action<T>) action)(args);
        }
    }
}