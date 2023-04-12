using Installation.Executors;
using Installation.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
	
    abstract public class ExecutableUnit
    {
		private Guid id;

		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		private Guid executableId;

		public Guid ExecutableId
		{
			get { return executableId; }
			set { executableId = value; }
		}

		private Dictionary<string, string> names;

		public Dictionary<string, string> Names
		{
			get { return names; }
			set { names = value; }
		}

		public string Name { get => Names.ReturnValueOrSubstringOrFirstValue(); }



		private string executableFilePath;

		public string ExecutableFilePath
		{
			get { return executableFilePath; }
			set { executableFilePath = value; }
		}
		private string executableArguments;

		public string ExecutableArguments
		{
			get { return executableArguments; }
			set { executableArguments = value; }
		}

		private string executableDirectory;

		public string ExecutableDirectory
		{
			get { return executableDirectory; }
			set { executableDirectory = value; }
		}


		private List<int> successfullReturnCodes;

		public List<int> SuccessfullReturnCodes
		{
			get { return successfullReturnCodes; }
			set { successfullReturnCodes = value; }
		}

		private ExecuteContext executeContext;

		public ExecuteContext ExecuteContext
		{
			get { return executeContext; }
			set { executeContext = value; }
		}

		private bool required;

		public bool Required
		{
			get { return required; }
			set { required = value; }
		}

		private int abortAfterSeconds;

		public int AbortAfterSeconds
		{
			get { return abortAfterSeconds; }
			set { abortAfterSeconds = value; }
		}

		private int retrys;

		public int Retrys
		{
			get { return retrys; }
			set { retrys = value; }
		}
        public string statusMessage;
        public string StatusMessage { get => statusMessage; set { statusMessage = value; } }

		private int returnCode;

		public int ReturnCode
		{
			get { return returnCode; }
			set { returnCode = value; }
		}

		private StatusState statusState;

		public StatusState StatusState
        {
			get { return statusState; }
			set { statusState = value; }
		}


		public abstract Task Execute(CancellationToken cancellationToken);

        protected void checkExecutor(Executor executor)
        {
            if (executor == null)
                throw new NullReferenceException("No executor for this file type found");
            if (!(executor is IApplicationExecutor))
                throw new InvalidOperationException("This operation is not supported for this file type");
        }




    }
}
