using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Rhino.ServiceBus;

namespace Bus
{
    public abstract class Handles<T> :ConsumerOf<T>
    {
        protected ILog Log = log4net.LogManager.GetLogger("CommandHandler");


        public abstract void SetupPolicies();
        public abstract void Handle(T msg);

        public Eventing.Store.IEventStore EventStore { get; set; }
        private bool _usedatabase = false;
        private bool _transactional = false;

        public void Policy_Database(bool isTransactional)
        {
            _usedatabase = true;
            _transactional = isTransactional;
        }

        ILog _policyExceptionLoggerL;
        Action<ILog, T, Exception> _policyExceptionLoggerFunc;
        public void Policy_ExceptionLog(ILog l, Action<ILog, T, Exception> func)
        {
            _policyExceptionLoggerL = l;
            _policyExceptionLoggerFunc = func;
        }


        ILog _policyLogBeforeL;
        Action<ILog, T> _policyLogBeforeFunc;
        public void Policy_LogBefore(ILog l, Action<ILog, T> func)
        {
            _policyLogBeforeL = l;
            _policyLogBeforeFunc = func;
        }

        ILog _policyLogAfterL;
        Action<ILog, T> _policyLogAfterFunc;
        public void Policy_LogAfter(ILog l, Action<ILog, T> func)
        {
            _policyLogAfterL = l;
            _policyLogAfterFunc = func;
        }




        //Typ lista av Before
        //Typ lista av After
        //Typ lista av Exceptions

        public void Consume(T message)
        {
            

            SetupPolicies();

            if (_policyLogBeforeFunc != null)
                _policyLogBeforeFunc(_policyLogBeforeL, message);

            //Do we need a database
            if (_usedatabase)
                EventStore.Open(_transactional);


            bool ok = true;
            Exception exRes = null;
            try
            {
                Handle(message);
            }
            catch (Exception ex)
            {
                ok = false;
                if (_policyExceptionLoggerFunc == null)
                    Log.Error("EXCEPTION CAUGHT: " + message.GetType() + " EX:" + ex.Message);
                else
                    _policyExceptionLoggerFunc(_policyExceptionLoggerL, message, ex);
                exRes = ex;
            }


            if (_usedatabase)
                EventStore.Close(ok);


            if (_policyLogAfterFunc != null)
                _policyLogAfterFunc(_policyLogAfterL, message);
            if (exRes != null)
                throw (exRes);
        }
    }

}
