# Installation-Agent

**In development**

### V2 - in development ###
 
#### Features ( additional to version 1) ####
 - multiple languages will be supported
 - an executable (software/script) can be split to multiple execution steps that can have different behaviour like execute in system or user context and many more
 - will be supporting multiple UIs that runs on different user session like terminal server


### V1 ###

#### Features ####

 - installs software and run scripts from share
 - shows status
 - users do not need admin privilege to install software/run scripts
 - executes in system context
 - multiple software can be set to execute (install/run) to service from user
 - MSI installer with arguments, can be provisioned silently to clients
 
#### Limitations ####
 - do not work with multiple RDP sessions on terminal server
 - no translation
 - no execution steps
 - cannot execute as user

![image](https://user-images.githubusercontent.com/7008555/210965639-68735607-8c10-4627-8f42-073936e81429.png)

Simple Installation Agent with UI for user and service running in background and installating the software that user choses.


Features

 - can install/reinstall/uninstall applications and execute scripts
 - background service ensures that the user doesn't need any admin privileges on the computer


Limitations

 - works only with one session, i.e. doesn't work with terminal server, because only one NamedPipeServerStream communicate with the client ( will be improved in future )


 

