using System.Collections.Concurrent;


namespace ProfitCapture
{
    public class TypedActionQueue
    {
        private void Running()
        {
            Rodando = true;
            while (Rodando)
            {
                try
                {
                    Aguarde.WaitOne();

                    if (Fila.Count > 0)
                    {
                        while (Fila.TryDequeue(out Atual))
                        {
                            if(Max > 0 && Fila.Count >= Max)
                            {
                                continue;
                            }

                            Atual.Method.Method.Invoke(Atual.Method.Target, Atual.Data != null ? new object[] { Atual.Data } : new object[0]);
                        }
                    }

                    Aguarde.Reset();
                }
                catch (ThreadAbortException eAbort)
                {
                    Rodando = false;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Enqueue(Action method, bool important = false)
        {
            ClientEventItem action = new ClientEventItem() { Method = method, Important = important };
            Fila.Enqueue(action);

            lock (Lock)
            {
                Aguarde.Set();
            }
        }


        public void Enqueue<T>(Action<T> method, T data, bool important = false)
        {
            ClientEventItem action = new ClientEventItem() { Method = method, Data = data, Important = important };
            Fila.Enqueue(action);

            lock (Lock)
            {
                Aguarde.Set();
            }
        }

        private void Start()
        {
            if (!Rodando)
            {
                Aguarde = new ManualResetEvent(false);
                Aguarde.Reset();

                thr = new Thread(Running);
                thr.Start();
            }
        }
        public void Stop()
        {
            Rodando = false;
            if (Aguarde != null) Aguarde.Set();
        }


        private ClientEventItem Lock;
        private Thread thr;
        private ManualResetEvent Aguarde;
        private ConcurrentQueue<ClientEventItem> Fila;
        private ClientEventItem Atual;
        private bool Rodando;
        private int Max;


        public TypedActionQueue(int max = 0)
        {
            Max = max;

            Fila = new ConcurrentQueue<ClientEventItem>();
            Rodando = false;

            Lock = new ClientEventItem();

            Start();
        }


        class ClientEventItem
        {
            public Delegate Method { get; set; }
            public object Data { get; set; }
            public bool Important { get; set; }
        }

    }

}
