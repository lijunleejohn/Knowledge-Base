using System.Diagnostics;
const int process_cound = 10;

/*
 * This application launch 10 processes (10 instances of SynchronizedLock.exe) to log to one single log.txt file
 * if you use Lock statement which logs on a local object inside a Process won't work, the application hangs
 */

Parallel.For(0, process_cound,
             index => {
                 Process.Start("C:\\Repo\\Knowledge-Base\\.NET Core\\AdvancedTopic\\Concurrency\\SynchronizedLock\\SynchronizedLock\\bin\\Debug\\net6.0\\SynchronizeLock.exe");
             });