using Installation.Executors;
using Installation.Models.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
	
    public class ExecutableUnit
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

		public string Name { get => Names.ReturnValueOrSubstringOrFirstValue(""); }

		public ExecuteAction ExecuteAction { get; set; }

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

		private int index;

		public int Index
		{
			get { return index; }
			set { index = value; }
		}



		public async Task Execute(CancellationToken cancellationToken)
		{
            if (string.IsNullOrWhiteSpace(ExecutableFilePath))
                throw new ArgumentNullException(nameof(ExecutableFilePath));

            Log.Information("Reinstalling application: {name}, file: {file}, dir {dir}", ExecutableFilePath, ExecutableDirectory);


            var executor = Executor.GetExecutor(ExecutableFilePath, ExecutableArguments, ExecutableDirectory, cancellationToken);
            try
            {

                checkExecutor(executor);

				switch(this.ExecuteAction)
				{
					case ExecuteAction.Install:
						await (executor as IInstallableExecutor).InstallAsync();
						break;
					case ExecuteAction.Reinstall:
						await (executor as IReinstallableExecutor).ReinstallAsync();
						break;
					case ExecuteAction.Uninstall:
						await (executor as IUninstallableExecutor).UninstallAsync();
                        break;
					case ExecuteAction.Run:
						await (executor as IRunnableExecutor).RunAsync();
                        break;
					default: throw new InvalidOperationException();
				}

                setExecutionStateFromExecutor(executor);

            }
            catch
            {
                throw;
            }
            finally
            {
                executor?.Dispose();
            }
        }

        protected void checkExecutor(Executor executor)
        {
            if (executor == null)
                throw new NullReferenceException("No executor for this file type found");
        }

        private void setExecutionStateFromExecutor(Executor executor) // if user defined return codes are present they will be used, otherwise standard return codes will be used
		{

			if(trueIfContainsSuccessfullReturnCode(executor)) //if return code match any codes that are defined as successfull in the ExecutableUnit or Executor, the State will be Success
			{
				StatusState = StatusState.Success;
			}
			else
			{
				StatusState = StatusState.Error;
				StatusMessage = executor.LastReturnMessage;
			}

		}

		private bool trueIfContainsSuccessfullReturnCode(Executor executor)
		{
			bool codeContains = false;
			if (executor is IInstallableExecutor)
			{
				codeContains = succesfullCodeContainsInCollection((executor as IInstallableExecutor).SuccessfullInstallationReturnCodes, SuccessfullReturnCodes);
			}
			else if (executor is IReinstallableExecutor)
			{
				codeContains = succesfullCodeContainsInCollection((executor as IReinstallableExecutor).SuccessfullReinstallationReturnCodes, SuccessfullReturnCodes);
			}
			else if (executor is IUninstallableExecutor)
			{
				codeContains = succesfullCodeContainsInCollection((executor as IUninstallableExecutor).SuccessfullUninstallationReturnCodes, SuccessfullReturnCodes);
			}
			else if (executor is IRunnableExecutor)
			{
				codeContains = succesfullCodeContainsInCollection((executor as IRunnableExecutor).SuccessfullRunReturnCodes, SuccessfullReturnCodes);
			}

			return codeContains;
		}

		private bool succesfullCodeContainsInCollection(params List<int>[] codeCollections)
		{
			foreach (var codeCollection in codeCollections)
			{
				if (codeCollection != null)
				{
					foreach(var code in codeCollection)
					{
						if(code == ReturnCode)
							return true;
					}
				}
			}
			return false;
		}



    }
}
