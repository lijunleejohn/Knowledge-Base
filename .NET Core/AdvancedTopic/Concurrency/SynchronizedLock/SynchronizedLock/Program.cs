using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

/*
 * Task doesn't necesarilly to work Asynchroous
 * But Asnc and Await work with Task
 * */
const int threads_dimension = 200;
const int pool_dimension = 1; // in theory if thread pool number is bigger than 1 it might introduce conflict and break, set it to 100 it will break;
string logFile = @"log.txt";
Object logObject = new Object();
AutoResetEvent myResetEvent = new AutoResetEvent(true); // true - signaled, false - nonsigned which will wait forever
ManualResetEvent mre = new ManualResetEvent(false);  // same as above, need to call mre.Set() somewhere to signal for the 1st thread to start
EventWaitHandle mwh = new EventWaitHandle(false, EventResetMode.ManualReset);
Mutex mut = new Mutex(false, "Global\\osMutext"); // false here means the calling thread doesn not have ownership of the mutext, that is crucial otherwise it will be block forever
Semaphore smp = new Semaphore(initialCount: 0, maximumCount: pool_dimension); //Semaphore is not Mutual Exclusive One, it just limits the maximum thread number to access a resource, if maximumCount bigger than 1, conflict could happend

var tasks = new Task[threads_dimension];
for (int i = 0; i < threads_dimension; i++)
{
    // on the statement below if you use i.ToStri]ng() i will be always 10, which means the task does not run immediately, it will start running at Task.WaitAll()
    tasks[i] = Task.Run(() => { log($"ProcessId: {System.Diagnostics.Process.GetCurrentProcess().Id}, ThreadId: {Thread.CurrentThread.ManagedThreadId},  Task {DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff tt")}: This is a demo of multiple thread synchroization.\r\n"); });
}
mre.Set(); // signal the 1st thread to stsart, if you put this line under the next line it won't work and will be blocked forver
mwh.Set();
smp.Release(pool_dimension);
Task.WaitAll(tasks);

#region without synchronization lock
/*
this will throw System.IO.IOException: 
'The process cannot access the file ...
because it is being used by another process.'
*/
/*
void log(string message)
{
    File.AppendAllText(logFile, message);
}
*/
#endregion

#region use lock stament: 
/*
The lock statement acquires the mutual-exclusion lock for a given object, 
executes a statement block, and then releases the lock. 
While a lock is held, the thread that holds the lock can again acquire and release the lock. 
Any other thread is blocked from acquiring the lock and waits until the lock is released. 
The lock statement ensures that at maximum only one thread executes its body at any time moment.
    */
/*
void log(string message)
{
    lock (logObject)
    {
        File.AppendAllText(logFile, message);
    }
}
*/
#endregion

#region AutoResetEvent - inherited from EventWaitHandle
/*
Represents a thread synchronization event that, when signaled, 
resets automatically after releasing a single waiting thread. 
This class cannot be inherited. 
 */
/*
void log(string message)
{
    myResetEvent.WaitOne();
    File.AppendAllText(logFile, message);
    myResetEvent.Set();
}
*/
#endregion

#region ManualResetEvent - inherited from EventWaitHandle
/*
Represents a thread synchronization event that, when signaled, 
resets automatically after releasing a single waiting thread. 
This class cannot be inherited. 
 */
/*
void log(string message)
{
    mre.WaitOne();
    mre.Reset(); // unsignaled to block the other threads

    File.AppendAllText(logFile, message);
    mre.Set();  // signaled to allow the next thread to proceed
}
*/
#endregion

#region EventWaitHandle - inherited from WaitHandler
/*
WaitHandler - with protected constructor, encapsulates operating system-specific objects that wait for exclusive access to shared resources.
 */
/*
void log(string message)
{
    mwh.WaitOne();

    File.AppendAllText(logFile, message);
    mwh.Set();  // signaled to allow the next thread to proceed
}
*/
#endregion

#region Mutext - inherited from WaitHandler
/*
WaitHandler - with protected constructor, encapsulates operating system-specific objects that wait for exclusive access to shared resources.
Mutext - a synchronization primitive that can also be used for interprocess synchronization. It has the interprocess capability while log only works in the same AppDoamin
 */

void log(string message)
{
    mut.WaitOne();

    // no need for the following block of code since File.AppendAllText() will automatically create a new file if no exists. If enalbe the block it will hand if the file not exists
    //if (!File.Exists(logFile))
    //{
    //    File.Create(logFile);
    //}
    
    File.AppendAllText(logFile, message);
    mut.ReleaseMutex();  // signaled to allow the next thread to proceed
}

#endregion

#region Semaphore - inherited from WaitHandler
/*
WaitHandler - with protected constructor, encapsulates operating system-specific objects that wait for exclusive access to shared resources.
Semaphore - Limits the number of threads that can access a resource or pool of resources concurrently.
 */
/*
void log(string message)
{
    smp.WaitOne();
    File.AppendAllText(logFile, message);
    smp.Release(); // signaled to allow the next thread to proceed
}
*/
#endregion