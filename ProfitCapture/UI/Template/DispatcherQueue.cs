

namespace ProfitCapture.UI.Template
{
    public class DispatcherQueue
    {
        private void Running()
        {
            Aguarde = new ManualResetEvent(false);
            Aguarde.Reset();

            Rodando = true;
            bool wait = false;

            while (Rodando)
            {
                try
                {
                    Aguarde.WaitOne();
                    Aguarde.Reset();

                    while (Fila.Count > 0)
                    {
                        lock (EnqueueLock)
                        {
                            Atual = Fila[0];
                            Fila.RemoveAt(0);
                        }

                        var instance = Atual.Method.Target;

                        Atual.Method.Method.Invoke(instance, new object[] { Atual.Data });
                    }
                }
                catch (ThreadAbortException eAbort)
                {
                    Rodando = false;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1);
                    //Logger.AppendError(ex);
                }
            }
        }

        public void Access(Action<List<ClientEventItem>> act)
        {
            lock (EnqueueLock)
            {
                act(Fila);
            }
        }



        public void Enqueue<T>(Action<T> method, T data)
        {
            ClientEventItem action = new ClientEventItem() { Method = method, Data = data };

            lock(EnqueueLock)
            {
                Fila.Add(action);
            }

            if(Aguarde != null) Aguarde.Set();
        }


        private void Start()
        {
            if (!Rodando)
            {
                thr = new Thread(Running);
                thr.Start();
            }
        }
        public void Stop()
        {
            Rodando = false;
            if (Aguarde != null) Aguarde.Set();
        }

        public int GetCount()
        {
            return Fila.Count;
        }
        public void Forward()
        {
            if (Aguarde != null) Aguarde.Set();
        }


        private Thread thr;
        private ManualResetEvent Aguarde;
        private List<ClientEventItem> Fila;
        private ClientEventItem Atual;
        private bool Rodando;
        private ClientEventItem EnqueueLock;


        public DispatcherQueue()
        {
            EnqueueLock = new ClientEventItem();

            Fila = new List<ClientEventItem>();
            Rodando = false;
            Start();
        }
    }

    public class ClientEventItem
    {
        public Delegate Method;
        public object Data;
    }

}
