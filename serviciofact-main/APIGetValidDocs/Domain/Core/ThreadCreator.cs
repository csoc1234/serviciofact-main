using APIGetValidDocs.Domain.Interface;
using APIGetValidDocs.Infraestructure.Database;
using APIGetValidDocs.Infraestructure.Interface;
using TFHKA.Storage.Fileshare.Client.Interface;

namespace APIGetValidDocs.Domain.Core
{
    public class ThreadCreator : IThreadCreator
    {
        private readonly IConfiguration _configuration;
        private IAccumulatorsMetric _accumulators;

        private readonly IBaseHttpClient _baseHttpClient;

        private readonly IFileShareClass _fileShareClass;

        public ThreadCreator(IConfiguration configuration, IAccumulatorsMetric accumulators, IBaseHttpClient baseHttpClient, IFileShareClass fileShareClass)
        {
            _configuration = configuration;
            _accumulators = accumulators;
            _baseHttpClient = baseHttpClient;
            _fileShareClass = fileShareClass;
        }

        public void GetFreeThread(List<TInvoiceFactoring> listInvoice)
        {
            int hilo_libre = 0;
            bool libre = false;
            int numThreads = Int32.Parse(_configuration["ThreadsNumber"]);
            int cant_reg_st_intermedio = listInvoice.Count;
            Thread[] hilos = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                hilos[i] = null;
            }
            int j = 0;
            while (j < cant_reg_st_intermedio)
            {

                #region conseguir_un_hilo_libre
                while (true)
                {
                    libre = false;
                    for (int i = 0; i < numThreads; i++)
                    {
                        if (hilos[i] == null || !hilos[i].IsAlive)
                        {
                            hilo_libre = i;
                            libre = true;
                            break;
                        }
                    }
                    if (libre)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                #endregion

                // darle trabajo al hilo libre. y asigarlo en el vector.

                ValidateInvoice proc = new ValidateInvoice(_configuration, listInvoice[j], hilo_libre, _baseHttpClient, _fileShareClass, _accumulators);

                /* ProcesoCorreccion proc = new ProcesoCorreccion(ls_estado_intermedio[j], hilo_libre, locklog, log_general_detail,
                       lock_filestorage_read, lock_filestorage_write, lock_filestorage_ubl, lock_filestorage_event, lock_filestorage_attached);*/

                hilos[hilo_libre] = new Thread(proc.Validate);
                hilos[hilo_libre].Start();

                j++;

                Thread.Sleep(1); //truco para no hacer que en el mismo mili segundo todos los hilos al inicio pidan recursos
            }

            //truco para no cerrar el hilo principal hasta que todos los hilos acaben.
            for (int i = 0; i < numThreads; i++)
            {
                if (hilos[i] != null)
                    hilos[i].Join();
            }

        }
    }
}
