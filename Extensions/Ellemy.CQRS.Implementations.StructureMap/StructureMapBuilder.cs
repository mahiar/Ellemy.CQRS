﻿using System.Collections.Generic;
using Ellemy.CQRS.Command;
using Ellemy.CQRS.Event;
using StructureMap;

namespace Ellemy.CQRS.Implementations.StructureMap
{
    public class StructureMapBuilder : IHandlerFactory, ICommandHandlerFactory
    {
        private readonly IContainer _container;

        public StructureMapBuilder(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<IDomainEventHandler<TEvent>> GetHandlersFor<TEvent>() where TEvent : IDomainEvent
        {
            return _container.GetAllInstances<IDomainEventHandler<TEvent>>();
        }

        public ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : ICommand
        {
            return _container.GetInstance<ICommandHandler<TCommand>>();
        }
        
    }
}