# Multimedia Timer for .NET Core
 .NET Core Wrapper for Windows Multimedia Timer. 
 

## Multimedia Timer 
>Multimedia timer services allow applications to schedule timer events with the greatest resolution (or accuracy) possible for the hardware platform. These multimedia timer services allow you to schedule timer events at a higher resolution than other timer services.
These timer services are useful for applications that demand high-resolution timing. For example, a MIDI sequencer requires a high-resolution timer because it must maintain the pace of MIDI events within a resolution of 1 millisecond.

[Learn more](https://cda.ms/1dr)

## Installing the Package 
``` 
Install-Package MultimediaTimer -Version 1.0.0
``` 

## How to use

``` 
var timer = new Timer();
timer.Tick += Timer_Tick;
timer.Delay = TimeSpan.FromMilliseconds(5);
timer.Resolution = TimeSpan.FromMilliseconds(2);
timer.Start();
```

## Bugs?!
If you find a bug then please create an Issue, or better yet a pull request with a fix! 
