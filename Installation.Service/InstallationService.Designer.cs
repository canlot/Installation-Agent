namespace Installation.Service
{
    partial class InstallationService
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.Service = new System.Diagnostics.Process();
            // 
            // Service
            // 
            this.Service.StartInfo.Domain = "";
            this.Service.StartInfo.LoadUserProfile = false;
            this.Service.StartInfo.Password = null;
            this.Service.StartInfo.StandardErrorEncoding = null;
            this.Service.StartInfo.StandardOutputEncoding = null;
            this.Service.StartInfo.UserName = "";
            this.Service.Exited += new System.EventHandler(this.process1_Exited);
            // 
            // InstallationService
            // 
            this.ServiceName = "Installation Service";

        }

        #endregion

        private System.Diagnostics.Process Service;
    }
}
