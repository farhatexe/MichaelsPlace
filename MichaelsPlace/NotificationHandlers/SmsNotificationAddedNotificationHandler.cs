﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using MichaelsPlace.Models.Persistence;
using MichaelsPlace.Services.Messaging;
using MichaelsPlace.Utilities;
using Ninject;

namespace MichaelsPlace.Listeners
{
    [UsedImplicitly]
    public class SmsNotificationAddedNotificationHandler : NotificationHandlerBase<EntityAdded<SmsNotification>>
    {
        private Injected<ISmsService> _smsSender;

        [Inject]
        public ISmsService SmsService
        {
            get { return _smsSender.Value; }
            set { _smsSender.Value = value; }
        }


        public override void Handle(EntityAdded<SmsNotification> message)
        {
            SmsService.Send(message.Entity);
        }
    }
}