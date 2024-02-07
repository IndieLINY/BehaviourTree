using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace IndieLINY.AI.BehaviourTree
{
    public class BTServiceScheduler
    {
        private Dictionary<IBTServiceOwner, List<BTNService>> _owners = new();
        private Dictionary<IBTNSInterval, CancellationTokenSource> _intervalCanelToken = new();

        public void TryAddOwner(IBTServiceOwner owner)
        {
            if (!_owners.TryGetValue(owner, out var list))
            {
                list = new List<BTNService>(2);
                _owners.Add(owner, list);
                foreach (var service in owner.GetServices())
                {
                    list.Add(service);
                    Schedule(service);
                }
            }
        }

        public void TryRemoveOwner(IBTServiceOwner owner)
        {
            if (_owners.Remove(owner, out var list))
            {
                foreach (var service in list)
                {
                    UnSchedule(service);
                }
            }
        }

        private void UnSchedule(BTNService service)
        {
            switch (service)
            {
                case IBTNSInterval interval:
                    UnScheduleInterval(interval);
                    break;
            }
        }

        private void Schedule(BTNService service)
        {
            switch (service)
            {
                case IBTNSInterval interval:
                    ScheduleInverval(interval.GetInterval(), interval);
                    break;
            }
        }

        private void UnScheduleInterval(IBTNSInterval service)
        {
            if (_intervalCanelToken.Remove(service, out var token))
            {
                token.Cancel();
            }
        }

        private void ScheduleInverval(float interval, IBTNSInterval service)
        {
            var token = new CancellationTokenSource();
            _intervalCanelToken.Add(service, token);

            UniTask.Create(async () =>
            {
                while (true)
                {
                    service.Update();
                    await UniTask.Delay((int)(interval * 1000f), cancellationToken: token.Token);

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            });
        }
    }
}