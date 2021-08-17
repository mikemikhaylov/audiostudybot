﻿using System.Threading;
using System.Threading.Tasks;

namespace AudioStudy.Bot.Domain.Services.Abstractions
{
    public interface IUpdatesQueueProducer<in T>
    {
        Task ProduceAsync(T update, CancellationToken cancellationToken);
    }
}