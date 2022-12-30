using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Executables
{
	public enum ExecuteContext
	{
		Machine = 1,
		User = 2,
	}
    public class ExecutableUnit
    {
		private Guid id;

		public Guid ID
		{
			get { return id; }
			set { id = value; }
		}

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


		public void Execute()
		{

		}


	}
}
