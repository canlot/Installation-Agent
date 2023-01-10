using Installation.Executors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    public class ExecutableUnitInstallable : ExecutableUnit, IInstallable
    {
        public bool Installed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task InstallAsync(CancellationToken cancellationToken)
        {
            var executor = Executor.GetExecutor(ExecutableFilePath, ExecutableArguments, ExecutableDirectory, cancellationToken);

            try
            {
                CurrentlyExecuting = true;

                checkExecutor(executor);
                await (executor as IApplicationExecutor).InstallAsync();

                setExecutionStateFromExecutor(executor, SuccessfullInstallReturnCodes);
                Installed = getStateFromResult();
            }
            catch
            {
                throw;
            }
            finally
            {
                executor?.Dispose();
                CurrentlyExecuting = false;
            }
        }
        public override async Task Execute(CancellationToken cancellationToken)
        {
            await InstallAsync(cancellationToken);
        }
    }
}
