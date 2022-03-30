# Installation-Agent

Simple Installation Agent with UI for user and service running in background and installating the software that user choses.

In development.


Features

 - can install/reinstall/uninstall applications and execute scripts
 - background service ensures that the user doesn't need any admin privileges on the computer


Limitations

 - works only with one session, i.e. doesn't work with terminal server, because only one NamedPipeServerStream communicate with the client ( will be improved in future )