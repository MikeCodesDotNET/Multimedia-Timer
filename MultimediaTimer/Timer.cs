using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MultimediaTimer
{
    //Used https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke as a guide 

    public sealed class Timer : IComponent
	{
        [DllImport("winmm.dll")]
        private static extern int timeGetDevCaps(ref TimerCaps caps, int sizeOfTimerCaps);


        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimeProc proc, int user, int mode);


        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);

        public event EventHandler Started;		
		public event EventHandler Stopped;

		public event EventHandler Tick;

		static Timer()
		{
			Timer.timeGetDevCaps(ref Timer.caps, Marshal.SizeOf<TimerCaps>(Timer.caps));
		}


		public Timer(System.ComponentModel.IContainer container)
		{
			container.Add(this);
			this.Initialize();
		}


		public Timer()
		{
			this.Initialize();
		}


		~Timer()
		{
			if (this.IsRunning)
			{
				Timer.timeKillEvent(this.timerID);
			}
		}


		private void Initialize()
		{
			this.mode = TimerMode.Periodic;
			this.period = Timer.Capabilities.PeriodMin;
			this.resolution = 1;
			this.running = false;
			this.timeProcPeriodic = new Timer.TimeProc(this.TimerPeriodicEventCallback);
			this.timeProcOneShot = new Timer.TimeProc(this.TimerOneShotEventCallback);
			this.tickRaiser = new Timer.EventRaiser(this.OnTick);
		}


		public void Start()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("Timer");
			}
			if (this.IsRunning)
			{
				return;
			}
			if (this.Mode == TimerMode.Periodic)
			{
				this.timerID = Timer.timeSetEvent(this.Period, this.Resolution, this.timeProcPeriodic, 0, (int)this.Mode);
			}
			else
			{
				this.timerID = Timer.timeSetEvent(this.Period, this.Resolution, this.timeProcOneShot, 0, (int)this.Mode);
			}
			if (this.timerID == 0)
			{
				throw new Exception("Unable to start multimedia Timer.");
			}
			this.running = true;
			if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
			{
				this.SynchronizingObject.BeginInvoke(new Timer.EventRaiser(this.OnStarted), new object[]
				{
					EventArgs.Empty
				});
				return;
			}
			this.OnStarted(EventArgs.Empty);
		}

		public void Stop()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("Timer");
			}
			if (!this.running)
			{
				return;
			}
			Timer.timeKillEvent(this.timerID);
			this.running = false;
			if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
			{
				this.SynchronizingObject.BeginInvoke(new Timer.EventRaiser(this.OnStopped), new object[]
				{
					EventArgs.Empty
				});
				return;
			}
			this.OnStopped(EventArgs.Empty);
		}


		private void TimerPeriodicEventCallback(int id, int msg, int user, int param1, int param2)
		{
			if (this.synchronizingObject != null)
			{
				this.synchronizingObject.BeginInvoke(this.tickRaiser, new object[]
				{
					EventArgs.Empty
				});
				return;
			}
			this.OnTick(EventArgs.Empty);
		}


		private void TimerOneShotEventCallback(int id, int msg, int user, int param1, int param2)
		{
			if (this.synchronizingObject != null)
			{
				this.synchronizingObject.BeginInvoke(this.tickRaiser, new object[]
				{
					EventArgs.Empty
				});
				this.Stop();
				return;
			}
			this.OnTick(EventArgs.Empty);
			this.Stop();
		}

		private void OnDisposed(EventArgs e)
		{
			EventHandler eventHandler = this.Disposed;
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		private void OnStarted(EventArgs e)
		{
			EventHandler started = this.Started;
			if (started != null)
			{
				started(this, e);
			}
		}

		private void OnStopped(EventArgs e)
		{
			EventHandler stopped = this.Stopped;
			if (stopped != null)
			{
				stopped(this, e);
			}
		}

		private void OnTick(EventArgs e)
		{
			EventHandler tick = this.Tick;
			if (tick != null)
			{
				tick(this, e);
			}
		}

		public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
		{
			get
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				return this.synchronizingObject;
			}
			set
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				this.synchronizingObject = value;
			}
		}

		public int Period
		{
			get
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				return this.period;
			}
			set
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				if (value <Timer.Capabilities.PeriodMin || value > Timer.Capabilities.PeriodMax)
				{
					throw new ArgumentOutOfRangeException("Period", value, "Multimedia Timer period out of range.");
				}
				this.period = value;
				if (this.IsRunning)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		public int MinPeriod
		{
			get
			{
				return Timer.Capabilities.PeriodMin;
			}
		}

		public int MaxPeriod
		{
			get
			{
				return Timer.Capabilities.PeriodMax;
			}
		}

		public int Resolution
		{
			get
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				return this.resolution;
			}
			set
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Resolution", value, "Multimedia timer resolution out of range.");
				}
				this.resolution = value;
				if (this.IsRunning)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		public TimerMode Mode
		{
			get
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				return this.mode;
			}
			set
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("Timer");
				}
				this.mode = value;
				if (this.IsRunning)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		public bool IsRunning
		{
			get
			{
				return this.running;
			}
		}

		public static TimerCaps Capabilities
		{
			get
			{
				return Timer.caps;
			}
		}

		public event EventHandler Disposed;


		public ISite Site
		{
			get
			{
				return this.site;
			}
			set
			{
				this.site = value;
			}
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			if (this.IsRunning)
			{
				this.Stop();
			}
			this.disposed = true;
			this.OnDisposed(EventArgs.Empty);
		}

		private const int TIMERR_NOERROR = 0;

		private int timerID;

		private volatile TimerMode mode;
		
		private volatile int period;

		private volatile int resolution;

		private Timer.TimeProc timeProcPeriodic;

		private Timer.TimeProc timeProcOneShot;

		private Timer.EventRaiser tickRaiser;

		private bool running;

		private volatile bool disposed;

		private System.ComponentModel.ISynchronizeInvoke synchronizingObject;

		private System.ComponentModel.ISite site;

		private static TimerCaps caps;

		private delegate void TimeProc(int id, int msg, int user, int param1, int param2);

		private delegate void EventRaiser(EventArgs e);
	}
}
