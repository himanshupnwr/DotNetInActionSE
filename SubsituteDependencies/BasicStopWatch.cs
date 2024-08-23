using Microsoft.Extensions.Internal;

namespace SubsituteDependencies
{
    //DateTimeOffset is a time-zone-aware version of DateTime.
    //DateTime has a concept of only local versus Coordinated Universal Time (UTC).
    //Handling local time is tricky and can easily create confusion.
    //DateTimeOffset identifies a single point in time and should be the default in your .NET code going forward.
    public class BasicStopwatch
    {
        private static readonly ISystemClock DefaultClock = new SystemClock();
        private DateTimeOffset? _startTime;
        private DateTimeOffset? _stopTime;
        private readonly ISystemClock _clock;

        public BasicStopwatch(ISystemClock? systemClock = null)
        {
            _clock = systemClock ?? DefaultClock;
        }

        public void Start()
        {
            _startTime = DateTime.UtcNow;
            _stopTime = null;
        }

        public void Stop()
        {
            if (_startTime != null)
            {
                _stopTime = DateTime.UtcNow;
            }
        }

        public TimeSpan? ElapsedTime
        {
            get
            {
                if (_startTime != null && _stopTime != null)
                {
                    return _stopTime - _startTime;
                }

                return null;
            }
        }
    }
}
